using PracticeApi.Dtos;
using PracticeApi.Models;

namespace PracticeApi.Services
{
    public interface IBookingService<TResource> where TResource : Resource
    {
        Task<List<TResource>> GetResourcesAsync();
        Task<List<TResource>> GetAvailableAsync(DateTime from, DateTime to);

        Task<List<Booking>> GetBookingsAsync();
        Task<Booking?> GetBookingAsync(int id);

        Task<OperationResult<Booking>> CreateAsync(CreateBookingDto dto);
        Task<OperationResult<Booking>> UpdateAsync(int id, UpdateBookingDto dto);
        Task<OperationResult<bool>> CancelAsync(int id);
    }
}
