using Airbnb.Extensions;
using Application.DTOs.PropertyDTOS;
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

        public PropertyController(PropertyService _propertyService, UserManager<User> user)
        {
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
            return PropertyService.Get(id)
                                  .ToActionResult();
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
        public IActionResult Put(PropertyDisplayDTO propertyDTO,int id)
        {
            return PropertyService.Update(propertyDTO)
                                  .ToActionResult();
        }

        // DELETE api/<PropertyController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {

            return PropertyService.Delete(id)
                                  .ToActionResult();
        }
    }
}
