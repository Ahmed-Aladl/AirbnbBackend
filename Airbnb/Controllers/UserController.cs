using System.Net.Mail;
using Airbnb.Middleware;
using Airbnb.Services;
using Application.DTOs.UserDto;
using Application.Interfaces;
using Application.Interfaces.IRepositories;
using Azure.Core;
using Domain.Models;
using Infrastructure.Common;
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
    private readonly IUserRepository _userRepository;
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
        IHubContext<NotificationHub> hub,
        IUserRepository userRepository
    )
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _context = context;
        _tokenService = tokenService;
        _emailService = emailService;
        _hub = hub;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var userIsExist = await _userManager.FindByEmailAsync(dto.Email);
        if (userIsExist != null)
            return BadRequest(new { error = "User already exists" });

        var username = dto.Email.Split('@')[0];
        var user = new User
        {
            Email = dto.Email,
            UserName = username,
            CreateAt = DateTime.UtcNow,
            IsConfirmed = false,
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        await _userManager.AddToRoleAsync(user, "Guest");

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
        return Ok(new { message = "OTP sent successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return BadRequest(new { error = "Invalid credentials" });

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken(user);

        var notification = new Notification
        {
            UserId = user.Id,
            Message = "Welcome",
            CreatedAt = DateTime.UtcNow,
            isRead = false,
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        await _hub.Clients.User(user.Id).SendAsync("ReceiveNotification", "Welcome");

        var identityRoles = roles.Select(role => new IdentityRole { Name = role }).ToList();
        Response.Cookies.Append(
            "refreshToken",
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Path = "/api/user/refresh-token",
            }
        );
        Response.Cookies.Append(
            "accessToken",
            accessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                Path = "/",
            }
        );

        if (user.IsConfirmed == true)
        {
            return Ok(
                new TokenDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    UserId = user.Id,
                    Roles = identityRoles,
                }
            );
        }
        else
        {
            return BadRequest(new { error = "you should confirm email go to forget password" });
        }
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(OtpDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return BadRequest(new { error = "User not found" });

        var otp = await _context.UsersOtp.FirstOrDefaultAsync(x =>
            x.UserId == user.Id && x.Code == dto.Code && !x.IsUsed && x.ExpiresAt > DateTime.UtcNow
        );

        if (otp == null)
            return BadRequest(new { error = "Invalid or expired OTP" });

        otp.IsUsed = true;
        user.IsConfirmed = true;
        var allUserOtps = _context.UsersOtp.Where(x => x.UserId == user.Id);
        _context.UsersOtp.RemoveRange(allUserOtps);

        await _context.SaveChangesAsync();

        return Ok(new { message = "OTP verified" });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("access_token");
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpPost("send-reset-otp")]
    public async Task<IActionResult> SendResetOtp(SendResetOtpDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return NotFound(new { error = "User not Found" });

        var otp = GenerateOtp();
        await _emailService.SendEmailAsync(
            dto.Email,
            "Reset Password",
            $"Your reset code is: {otp}"
        );

        await _context.UsersOtp.AddAsync(
            new UserOtp
            {
                UserId = user.Id,
                Code = otp,
                IsUsed = false,
                ExpiresAt = DateTime.UtcNow.AddMinutes(1),
            }
        );

        await _context.SaveChangesAsync();
        return Ok(new { message = "OTP sent" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return NotFound(new { error = "User not found" });

        var otp = await _context.UsersOtp.FirstOrDefaultAsync(x =>
            x.UserId == user.Id && x.Code == dto.Code && !x.IsUsed && x.ExpiresAt > DateTime.UtcNow
        );

        if (otp == null)
            return BadRequest(new { error = "Invalid OTP" });

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        otp.IsUsed = true;
        user.IsConfirmed = true;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Password reset successfully" });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.RefreshToken == dto.RefreshToken
        );

        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            return BadRequest(new { error = "Invalid or expired refresh token" });

        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = _tokenService.GenerateAccessToken(user, roles);

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

    [HttpGet("profile/{id}")]
    public async Task<IActionResult> GetProfile(string id)
    {
        var user = _userRepository.GetById(id);
        if (user == null)
            return BadRequest(new { error = "User not found" });

        return Ok(
            new UserProfileDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Bio = user.Bio,
                BirthDate = user.BirthDate,
                Country = user.Country,
                ProfilePictureURL = user.ProfilePictureURL,
            }
        );
    }

    [HttpPut("profile/{id}")]
    public async Task<IActionResult> UpdatedProfile(string id, [FromBody] UpdateUserProfileDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return BadRequest(new { error = "User not found" });

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.PhoneNumber = dto.PhoneNumber;
        user.Bio = dto.Bio;
        user.BirthDate = dto.BirthDate;
        user.Country = dto.Country;

        await _userManager.UpdateAsync(user);
        await _context.SaveChangesAsync();
        return Ok(new { message = "User updated" });
    }

    [Consumes("multipart/form-data")]
    [HttpPost("profile/image")]
    public async Task<IActionResult> UpdateProfileImage([FromForm] ProfileImageUploadDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user == null)
            return BadRequest(new { error = "User not found" });

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.File.FileName);
        var path = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "images",
            "profile",
            fileName
        );

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        user.ProfilePictureURL = fileName;
        await _userManager.UpdateAsync(user);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Photo uploaded" });
    }

    [HttpPost("profile/{id}/role")]
    public async Task<IActionResult> UpdateRole(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return BadRequest(new { error = "User not found" });

        await _userManager.AddToRoleAsync(user, "host");
        var roles = await _userManager.GetRolesAsync(user);

        var identityRoles = roles.Select(role => new IdentityRole { Name = role }).ToList();
        await _userManager.UpdateAsync(user);
        await _context.SaveChangesAsync();

        return Ok(
            new
            {
                Roles = identityRoles,
            });
    }

    [HttpDelete("delete-account/{id}")]
    public async Task<IActionResult> DeleteAccount(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return BadRequest(new { error = "User not found" });

        await _userManager.DeleteAsync(user);
        return Ok(new { message = "User deleted" });
    }
}
