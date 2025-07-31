using System.Net;
using System.Security.Claims;
using Airbnb.Extensions;
using Application.DTOs.PropertyDTOS;
using Application.DTOs.PropertyImageDTOs;
using Application.Interfaces;
using Application.Result;
using Application.Services;
using Application.Shared;
using Azure;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
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
            //userId = config["userId"]??"1";
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
            var result = PropertyService.GetAll();
            return ToActionResult(result);

        }

        [EndpointSummary("Get All Properties")]
        [HttpGet("dashboard")]
        public async Task<IActionResult>GetAllForDashboard()
        {
            var result = await PropertyService.GetAllForDashboardAsync();
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
                page = 1; 

            if (pageSize < 1)
                pageSize = 10; 
            

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
            PropertyFilterDto filterDto= new()
            {
                Latitude = (decimal?) response.Lat ,
                Longitude= (decimal?) response.Lon,

                
            };
            var result = await PropertyService.GetNearestPageAsync(response, page, pageSize, maxDistanceKm);

            return ToActionResult(result);
        }
        [EndpointSummary("Search Properties Paginated 'Date Range, Location, etc...'")]
        [HttpGet("search")]
        //[Authorize]
        public async Task<IActionResult> GetNearestPagintedAsync([FromQuery]PropertyFilterDto filterDto)
        {
            var userId = User.GetUserId();
            var result = await PropertyService.GetFilteredPageAsync(filterDto,userId);

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
        [HttpPost]
        [Authorize(Roles =("Host"))]
        public IActionResult Add(PropertyDisplayDTO propertyDTO)
        {

            var hostId = User.GetUserId();
            if (hostId == null)
            {
                Console.WriteLine("*****\n\nUser.hostId is Null");
                return ToActionResult(Result<bool>.Fail("Unauthorized", (int)HttpStatusCode.Unauthorized));
            }
            propertyDTO.HostId = hostId;

            return ToActionResult(PropertyService.Add(propertyDTO));
                                  ;
        }


        [EndpointSummary("Update existing Property")]
        [HttpPut]
        [Authorize(Roles =("Host"))]
        public IActionResult Put(PropertyDisplayDTO propertyDTO)
        {
            var hostId = User.GetUserId() ?? userId;
            if (hostId == null || hostId != propertyDTO.HostId)
                return ToActionResult(Result<bool>.Fail("Unauthorized", (int)HttpStatusCode.Unauthorized));

            var result = PropertyService.Update(propertyDTO);

            return ToActionResult(result); ;
        }



        [HttpPut("accept/{propertyId}")]
        public async Task<IActionResult> Accept(int propertyId)
        {
            var result = await PropertyService.Accept(propertyId);
            return ToActionResult(result); ;
        }

        [HttpPut("reject/{propertyId}")]
        public async Task<IActionResult> Reject(int propertyId)
        {
            var result = await PropertyService.Reject(propertyId);
            return ToActionResult(result); ;
        }

        [EndpointSummary("Deletes existing Property")]
        [HttpDelete("{id}")]
        [Authorize(Roles ="Host")]
        public IActionResult Delete(int id)
        {
            var hostId= User.GetUserId();
            if (hostId == null)
                return ToActionResult(Result<bool>.Fail("Unauthorized", (int)HttpStatusCode.Unauthorized));

            return ToActionResult( PropertyService.Delete(id, hostId) );
        }


        [EndpointSummary("Upload images for a Property")]
        [Consumes("multipart/form-data")]
        [HttpPost("property-images/upload")]
        [Authorize(Roles ="Host")]
        public async Task<IActionResult> UploadPropertyImages([FromForm] PropertyImagesUploadContainerDTO dto)
        {
            var hostId = User.GetUserId();
            if(hostId == null || hostId != dto.HostId)
                return ToActionResult(Result<bool>.Fail("Unauthorized", (int)HttpStatusCode.Unauthorized));
            
            var property = await PropertyService.GetByIdWithCoverAsync(dto.PropertyId);
            if(hostId != property?.Data?.HostId)
                return ToActionResult(Result<bool>.Fail("Unauthorized", (int)HttpStatusCode.Unauthorized));


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
                //return CreatedAtAction(nameof(GetById), new { id = dto.PropertyId }, new {});
                return Ok(new
                {
                    success = true,
                    message = "Images uploaded successfully",
                    imageUrls = imageDtos.Select(img => img.ImageUrl).ToList()
                });

            }
            catch
            {

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

        }



        [EndpointSummary("Deletes images for a Property")]
        [HttpDelete("property-images/delete/{propertyId}")]
        [Authorize(Roles ="Host")]
        public async Task<IActionResult> DeletePropertyImages([FromForm] int[] imgIds,int propertyId)
        {
            var hostId = User.GetUserId();
            if (hostId == null)
                return ToActionResult(Result<bool>.Fail("Unauthorized", (int)HttpStatusCode.Unauthorized));

            var property = await PropertyService.GetByIdWithCoverAsync(propertyId);
            if (hostId != property?.Data?.HostId)
                return ToActionResult(Result<bool>.Fail("Unauthorized", (int)HttpStatusCode.Unauthorized));


            if (imgIds == null || imgIds.Length==0)
                return BadRequest("No files uploaded");
            try 
            {
                var result = await PropertyService.DeleteImages(imgIds, propertyId,hostId, _env.ContentRootPath, _env.WebRootPath);
                return ToActionResult(result);
            }
            catch
            {

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

        }



    }
}
