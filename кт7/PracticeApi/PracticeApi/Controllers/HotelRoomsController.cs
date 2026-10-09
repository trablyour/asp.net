using Microsoft.AspNetCore.Mvc;
using PracticeApi.Models;
using PracticeApi.Services;

namespace PracticeApi.Controllers
{
    [ApiController]
    [Route("api/hotel-rooms")]
    public class HotelRoomsController : BookingControllerBase<HotelRoom>
    {
        public HotelRoomsController(IBookingService<HotelRoom> service) : base(service)
        {
        }
    }
}
