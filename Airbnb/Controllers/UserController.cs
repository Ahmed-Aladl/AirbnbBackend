using System.Net.Mail;
using Airbnb.Middleware;
using Airbnb.Services;
using Application.DTOs.UserDto;
using Application.Interfaces;
using Domain.Models;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly AirbnbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IHubContext<NotificationHub> _hub;

    public UserController(
        UserManager<User> userManager,
        AirbnbContext context,
        ITokenService tokenService,
        IEmailService emailService,
        IHubContext<NotificationHub> hub
    )
    {
        _userManager = userManager;
        _context = context;
        _tokenService = tokenService;
        _emailService = emailService;
        _hub = hub;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = new User
        {
            Email = dto.Email,
            UserName = dto.Email,
            CreateAt = DateTime.UtcNow,
            Roles = new List<IdentityRole> { new IdentityRole { Name = "user" } },
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        var otp = GenerateOtp();
        await _emailService.SendEmailAsync(dto.Email, "OTP Verification", $"Your OTP is: {otp}");

        await _context.UsersOtp.AddAsync(
            new UserOtp
            {
                UserId = user.Id,
                Code = otp,
                IsUsed = false,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            }
        );

        await _context.SaveChangesAsync();
        return Ok("OTP sent successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return Unauthorized("Invalid credentials");

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user);
        var userId = user.Id;
        var role = user.Roles;

        await _hub.Clients.User(user.Id).SendAsync("ReceiveNotification", "Welcome");

        return Ok(
            new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = userId,
                Roles = role.ToList(),
            }
        );
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(OtpDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return NotFound("User not found");

        var otp = await _context
            .UsersOtp.Where(x =>
                x.UserId == user.Id
                && x.Code == dto.Code
                && !x.IsUsed
                && x.ExpiresAt > DateTime.UtcNow
            )
            .FirstOrDefaultAsync();

        if (otp == null)
            return BadRequest("Invalid or expired OTP");

        otp.IsUsed = true;
        await _context.SaveChangesAsync();

        return Ok("OTP verified");
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("access_token");
        return Ok("Logged out successfully");
    }

    [HttpPost("send-reset-otp")]
    public async Task<IActionResult> SendResetOtp([FromBody] string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return NotFound("Email not found");

        var otp = GenerateOtp();
        await _emailService.SendEmailAsync(email, "Reset Password", $"Your reset code is: {otp}");

        await _context.UsersOtp.AddAsync(
            new UserOtp
            {
                UserId = user.Id,
                Code = otp,
                IsUsed = false,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            }
        );

        await _context.SaveChangesAsync();
        return Ok("OTP sent");
    }

    [Authorize]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return NotFound("User not found");

        var otp = await _context
            .UsersOtp.Where(x =>
                x.UserId == user.Id
                && x.Code == dto.Code
                && !x.IsUsed
                && x.ExpiresAt > DateTime.UtcNow
            )
            .FirstOrDefaultAsync();

        if (otp == null)
            return BadRequest("Invalid OTP");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        otp.IsUsed = true;
        await _context.SaveChangesAsync();

        return Ok("Password reset successfully");
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            return BadRequest("Invalid or expired refresh token");

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken(user);
        var expiry = _tokenService.GetRefreshTokenExpiryDate();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = expiry;

        await _context.SaveChangesAsync();

        return Ok(new TokenDto { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
    }

    private string GenerateOtp() => new Random().Next(100000, 999999).ToString();

    [Authorize(Roles = "admin")]
    [HttpGet("get-all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        return Ok(users);
    }

    [HttpGet("me")]
    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound("User not found");
        return Ok(user);
    }

    [HttpPost("profile")]
    [Authorize]
    public async Task<IActionResult> UpdatedProfile(string id, [FromBody] UpdateUserProfileDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound("User not found");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.PhoneNumber = dto.PhoneNumber;
        user.Bio = dto.Bio;
        user.BirthDate = dto.BirthDate;
        user.Country = dto.Country;

        await _userManager.UpdateAsync(user);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("profile/image")]
    [Authorize]
    public async Task<IActionResult> UpdateProfileImage(string id, [FromForm] IFormFile file)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound("User not found");

        var fileName = Guid.NewGuid().ToString();
        var path = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "images",
            "profile",
            fileName
        );
        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        user.ProfilePictureURL = fileName;
        await _userManager.UpdateAsync(user);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("profile/role")]
    [Authorize]
    public async Task<IActionResult> UpdateRole(string id, [FromBody] string role)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound("User not found");

        user.Roles = new List<IdentityRole> { new IdentityRole { Name = role } };
        await _userManager.UpdateAsync(user);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("delete-account")]
    [Authorize]
    public async Task<IActionResult> DeleteAccount(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound("User not found");

        await _userManager.DeleteAsync(user);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
