using System.Net;
using Airbnb.DTOs.PropertyImageDTOs;
using Airbnb.Extensions;
using Application.DTOs.PropertyDTOS;
using Application.DTOs.PropertyImageDTOs;
using Application.Interfaces;
using Application.Result;
using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Airbnb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : BaseController
    {
        public PropertyService PropertyService { get; }
        private readonly IWebHostEnvironment _env;
        private readonly IFileService _fileService;

        public PropertyController(PropertyService _propertyService, UserManager<User> user, IWebHostEnvironment env, IFileService fileService)
        {
            _env = env;
            _fileService = fileService;
            PropertyService = _propertyService;
        }

        // GET: api/<PropertyController>
        [HttpGet]
        public IActionResult Get()
        {

            return PropertyService.GetAll()
                                  .ToActionResult();

        }

        // GET api/<PropertyController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var result = PropertyService.Get(id);
            return ToActionResult(result);
        }

        // POST api/<PropertyController>
        [HttpPost("{id}")]
        public IActionResult Add(PropertyDisplayDTO propertyDTO, int id)
        {
            return PropertyService.Add(propertyDTO)
                                  .ToActionResult();
        }

        // PUT api/<PropertyController>/5
        [HttpPut("{id}")]
        public IActionResult Put(PropertyDisplayDTO propertyDTO, int id)
        {
            var result = PropertyService.Update(propertyDTO);

            return ToActionResult(result); ;
        }

        // DELETE api/<PropertyController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {

            return PropertyService.Delete(id)
                                  .ToActionResult();
        }


        [Consumes("multipart/form-data")]
        [HttpPost("property-images/upload")]
        public async Task<IActionResult> UploadPropertyImages([FromForm] PropertyImagesUploadContainerDTO dto)
        {
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
                if(!result.IsSuccess)
                    for (int i = 0; i < imageDtos.Count; i++)
                    {
                        await _fileService.DeleteFileAsync(imageDtos[i].ImageUrl, _env.WebRootPath);
                    }

                if (!result.IsSuccess)
                    return ToActionResult(result);
                return CreatedAtAction(nameof(Get), new {id=dto.PropertyId});
            }
            catch
            {

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

        }


    }
}
