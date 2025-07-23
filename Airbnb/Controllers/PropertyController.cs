using System.Net;
using Airbnb.Extensions;
using Application.DTOs.PropertyDTOS;
using Application.DTOs.PropertyImageDTOs;
using Application.Interfaces;
using Application.Result;
using Application.Services;
using Application.Shared;
using Azure;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Terminal;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Airbnb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : BaseController
    {
        
        public PropertyService PropertyService { get; }
        public UserManager<User> UserManager { get; }

        private readonly IWebHostEnvironment _env;
        private readonly IFileService _fileService;
        private readonly string userId = "1";

        public PropertyController(
                                    PropertyService _propertyService,
                                    UserManager<User> user,
                                    IWebHostEnvironment env, 
                                    IFileService fileService,
                                    IConfiguration config
                                )
        {
            _env = env;
            _fileService = fileService;
            PropertyService = _propertyService;
            UserManager = user;
            userId = config["userId"];
        }


        [EndpointSummary("Get images by property id")]
        [HttpGet("{id}/images")]
        public IActionResult GetPropertyImages(int id)
        {
            var result = PropertyService.GetImagesByPropertyId(id);
            return ToActionResult(result);
        }


        [EndpointSummary("Get All Properties")]
        [HttpGet]
        public async Task<IActionResult>GetAll()
        {
            //var user = new User
            //{
            //    Id = "1",
            //    UserName = "3ssam",
            //    Email = "3ssam@airbnb.com",
            //    FirstName = "Ahmed",
            //    LastName = "Essam",
            //    CreateAt = DateTime.Now.AddYears(-1),
            //    UpdatedAt = DateTime.Now,
            //    ProfilePictureURL = "https://example.com/admin-profile.jpg",
            //    Bio = "System Administrator",
            //    Country = "USA",
            //    BirthDate = new DateOnly(1990, 1, 1),
            //    EmailConfirmed = true,
            //};
            //await UserManager.CreateAsync(user,"3ssaM@asd");
            //Console.WriteLine("\n\n\n*******************************************user created *******************************************\n\n\n");
            var result = PropertyService.GetAll();
            return ToActionResult(result);

        }

        [EndpointSummary("Get Properties Page")]
        [HttpGet("page")]
        public async Task<IActionResult> GetPage(
                        [FromHeader] int page=1,
                        [FromHeader] int pageSize=10
            )
        {

            if (page< 0)
                page = 1; // Default

            if (pageSize < 1)
                pageSize = 10; // Default
            

            pageSize = Math.Min(pageSize, 100);


            var result = await PropertyService.GetPageAsync(page, pageSize);

            return ToActionResult(result);

        }


        [EndpointSummary("Gets nearest Properties Paginated")]
        [HttpGet("nearest")]
        public async Task<IActionResult> GetNearestPagintedAsync(
                        [FromHeader] int page = 1,
                        [FromHeader] int pageSize = 10,
                        [FromHeader] double maxDistanceKm = 10
            )
        {
            if (page < 0)
                page = 1; // Default

            if (pageSize < 1)
                pageSize = 10; // Default


            pageSize = Math.Min(pageSize, 100);

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (ip == null)
                return Fail("Internal Server Error");

            using var client = new HttpClient();
            var response = await client.GetFromJsonAsync<IpLocation>("http://ip-api.com/json/" + ip);
            if (response == null)
                return Fail("Internal Server Error");
            var result = await PropertyService.GetNearestPageAsync(response, page, pageSize, maxDistanceKm);

            return ToActionResult(result);
        }
        [EndpointSummary("Search Properties Paginated 'Date Range, Location, etc...'")]
        [HttpGet("search")]
        public async Task<IActionResult> GetNearestPagintedAsync([FromQuery]PropertyFilterDto filterDto)
        {
            var result = await PropertyService.GetFilteredPageAsync(filterDto);

            return ToActionResult(result);
        }


        [EndpointSummary("Gets Property Object only")]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = PropertyService.Get(id);
            return ToActionResult(result);
        }
        [EndpointSummary("Gets property with cover")]
        [HttpGet("cover/{id}")]
        public async Task<IActionResult> GetByWithCoverIdAsync(int id)
        {
            var result = await PropertyService.GetByIdWithCoverAsync(id);
            return ToActionResult(result);
        }

        [EndpointSummary("Get by host Id")]
        [HttpGet("host/{hostId}")]
        public async Task<IActionResult> GetByHostIdAsync(string hostId)
        {
            return ToActionResult(await PropertyService.GetByHostIdAsync(hostId));
        }
        [EndpointSummary("Get by host Id with cover")]
        [HttpGet("host/cover/{hostId}")]
        public async Task<IActionResult> GetByHostIdWithCoverAsync(string hostId)
        {
            return ToActionResult(await PropertyService.GetByHostIdWithCoverAsync(hostId));
        }

        [EndpointSummary("Add a new Property")]
        [HttpPost("{id}")]
        public IActionResult Add(PropertyDisplayDTO propertyDTO, int id)
        {
            return PropertyService.Add(propertyDTO)
                                  .ToActionResult();
        }

        [EndpointSummary("Update existing Property")]
        [HttpPut("{id}")]
        public IActionResult Put(PropertyDisplayDTO propertyDTO, int id)
        {
            var result = PropertyService.Update(propertyDTO);

            return ToActionResult(result); ;
        }

        [EndpointSummary("Deletes existing Property")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {

            return PropertyService.Delete(id)
                                  .ToActionResult();
        }


        [EndpointSummary("Upload images for a Property")]
        [Consumes("multipart/form-data")]
        [HttpPost("property-images/upload")]
        public async Task<IActionResult> UploadPropertyImages([FromForm] PropertyImagesUploadContainerDTO dto)
        {
            Console.WriteLine("\n\n\n\n\n\n******************************************************uploading photos******************************************************");
            if (dto.Files == null || !dto.Files.Any())
                return BadRequest("No files uploaded");

            var imageDtos = new List<PropertyImageCreateDTO>();

            for (int i = 0; i < dto.Files.Count; i++)
            {
                var file = dto.Files[i];
                string imageUrl = await _fileService.UploadFileAsync(file, _env.WebRootPath); // Save file, return URL

                imageDtos.Add(new PropertyImageCreateDTO
                {
                    GroupName = dto.GroupName,
                    ImageUrl = imageUrl,
                    IsCover = i == dto.CoverIndex
                });
            }

            var containerDto = new PropertyImagesCreateContainerDTO
            {
                PropertyId = dto.PropertyId,
                HostId = dto.HostId,
                Images = imageDtos
            };

            try
            {
                var result = PropertyService.AddImages(containerDto);
                if (!result.IsSuccess)
                    for (int i = 0; i < imageDtos.Count; i++)
                    {
                        await _fileService.DeleteFileAsync(imageDtos[i].ImageUrl, _env.WebRootPath);
                    }

                if (!result.IsSuccess)
                    return ToActionResult(result);
                return CreatedAtAction(nameof(GetById), new { id = dto.PropertyId }, new {});
            }
            catch
            {

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

        }



    }
}
