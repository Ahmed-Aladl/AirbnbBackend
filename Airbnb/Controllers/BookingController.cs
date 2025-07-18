using Airbnb.Extensions;
using Application.DTOs.BookingDTOS;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        public BookingService book { get; }

        public BookingController(BookingService _book) 
        
        {
            book = _book; 
        }

        [HttpGet]
        public IActionResult GetAll() 
        {

           return book.GetAllBookings().ToActionResult(); 
        
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id) 
        { 
            return book.GetBookingById(id).ToActionResult(); 
  
        }

        [HttpDelete("id")]
        public IActionResult delete(int id) 
        
        {
            return book.DeleteBooking(id).ToActionResult();
             
        }

        [HttpPost]
        public IActionResult Create(BookingDTO booking)
        {

            return book.CreateBooking(booking).ToActionResult();

        }     

       

        










    }
}
