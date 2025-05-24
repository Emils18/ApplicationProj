using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private static readonly List<Booking> bookings = new List<Booking>();

        [HttpPost]
        public IActionResult CreateBooking([FromBody] Booking booking)
        {
            bookings.Add(booking);
            return Ok(booking);
        }

        [HttpGet]
        public IActionResult GetBookings()
        {
            return Ok(bookings);
        }
    }
}
