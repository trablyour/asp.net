using Microsoft.AspNetCore.Mvc;
using PracticeApi.Dtos;
using PracticeApi.Models;
using PracticeApi.Services;

namespace PracticeApi.Controllers
{
    // Задание 3. Обобщенный контроллер бронирования.
    // Вся логика здесь, а для нового типа ресурса достаточно унаследоваться и указать маршрут.
    public abstract class BookingControllerBase<TResource> : ControllerBase where TResource : Resource
    {
        protected readonly IBookingService<TResource> _service;

        protected BookingControllerBase(IBookingService<TResource> service)
        {
            _service = service;
        }

        // все ресурсы этого типа
        [HttpGet]
        public async Task<ActionResult<List<TResource>>> GetResources()
        {
            return await _service.GetResourcesAsync();
        }

        // свободные ресурсы в период: available?from=2026-11-10T12:00&to=2026-11-11T12:00
        [HttpGet("available")]
        public async Task<ActionResult<List<TResource>>> GetAvailable([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Параметр from должен быть раньше to");

            return await _service.GetAvailableAsync(from, to);
        }

        [HttpGet("bookings")]
        public async Task<ActionResult<List<Booking>>> GetBookings()
        {
            return await _service.GetBookingsAsync();
        }

        [HttpGet("bookings/{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _service.GetBookingAsync(id);
            if (booking == null)
                return NotFound("Бронирование не найдено");

            return booking;
        }

        [HttpPost("bookings")]
        public async Task<IActionResult> Create(CreateBookingDto dto)
        {
            var result = await _service.CreateAsync(dto);

            if (result.NotFound)
                return NotFound(result.Error);
            if (!result.Success)
                return Conflict(result.Error);

            return CreatedAtAction(nameof(GetBooking), new { id = result.Value!.Id }, result.Value);
        }

        [HttpPut("bookings/{id}")]
        public async Task<IActionResult> Update(int id, UpdateBookingDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);

            if (result.NotFound)
                return NotFound(result.Error);
            if (!result.Success)
                return Conflict(result.Error);

            return Ok(result.Value);
        }

        [HttpDelete("bookings/{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _service.CancelAsync(id);

            if (result.NotFound)
                return NotFound(result.Error);

            return NoContent();
        }
    }
}
