using Airbnb.Extensions;
using Application.DTOs.BookingDTOS;
using Application.Services;
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
        public async Task<IActionResult> GetAll()
        {
            var result = await book.GetAllBookingsAsync();
            return result.ToActionResult();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await book.GetBookingByIdAsync(id);
            return result.ToActionResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await book.DeleteBookingAsync(id);
            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookingDTO booking)
        {
            var result = await book.CreateBookingAsync(booking);
            return result.ToActionResult();
        }
    }
}
