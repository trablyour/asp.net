using Microsoft.AspNetCore.Mvc;
using PracticeApi.Models;
using PracticeApi.Services;

namespace PracticeApi.Controllers
{
    [ApiController]
    [Route("api/restaurant-tables")]
    public class RestaurantTablesController : BookingControllerBase<RestaurantTable>
    {
        public RestaurantTablesController(IBookingService<RestaurantTable> service) : base(service)
        {
        }
    }
}
