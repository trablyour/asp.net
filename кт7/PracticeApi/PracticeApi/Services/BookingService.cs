using Microsoft.EntityFrameworkCore;
using PracticeApi.Data;
using PracticeApi.Dtos;
using PracticeApi.Models;

namespace PracticeApi.Services
{
    // одна реализация для любого типа ресурса: HotelRoom, RestaurantTable и т.д.
    public class BookingService<TResource> : IBookingService<TResource> where TResource : Resource
    {
        private readonly AppDbContext _db;

        public BookingService(AppDbContext db)
        {
            _db = db;
        }

        // только брони ресурсов нужного типа
        private IQueryable<Booking> Bookings =>
            _db.Bookings.Where(b => _db.Set<TResource>().Any(r => r.Id == b.ResourceId));

        public async Task<List<TResource>> GetResourcesAsync()
        {
            return await _db.Set<TResource>().ToListAsync();
        }

        // ресурсы, у которых нет ни одной брони, пересекающейся с периодом
        public async Task<List<TResource>> GetAvailableAsync(DateTime from, DateTime to)
        {
            return await _db.Set<TResource>()
                .Where(r => !_db.Bookings.Any(b =>
                    b.ResourceId == r.Id &&
                    b.StartTime < to &&
                    from < b.EndTime))
                .ToListAsync();
        }

        public async Task<List<Booking>> GetBookingsAsync()
        {
            return await Bookings
                .Include(b => b.Resource)
                .OrderBy(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingAsync(int id)
        {
            return await Bookings
                .Include(b => b.Resource)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<OperationResult<Booking>> CreateAsync(CreateBookingDto dto)
        {
            if (dto.StartTime >= dto.EndTime)
                return OperationResult<Booking>.Fail("Время начала должно быть раньше времени окончания");

            var resource = await _db.Set<TResource>().FindAsync(dto.ResourceId);
            if (resource == null)
                return OperationResult<Booking>.Missing($"Ресурс с id {dto.ResourceId} не найден");

            if (await IsBusyAsync(dto.ResourceId, dto.StartTime, dto.EndTime, null))
                return OperationResult<Booking>.Fail("Ресурс уже забронирован на это время");

            var booking = new Booking
            {
                ResourceId = dto.ResourceId,
                CustomerName = dto.CustomerName,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            return OperationResult<Booking>.Ok(booking);
        }

        public async Task<OperationResult<Booking>> UpdateAsync(int id, UpdateBookingDto dto)
        {
            var booking = await Bookings.FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
                return OperationResult<Booking>.Missing("Бронирование не найдено");

            if (dto.StartTime >= dto.EndTime)
                return OperationResult<Booking>.Fail("Время начала должно быть раньше времени окончания");

            // при проверке пересечений не учитываем саму эту бронь
            if (await IsBusyAsync(booking.ResourceId, dto.StartTime, dto.EndTime, booking.Id))
                return OperationResult<Booking>.Fail("Ресурс уже забронирован на это время");

            booking.CustomerName = dto.CustomerName;
            booking.StartTime = dto.StartTime;
            booking.EndTime = dto.EndTime;

            await _db.SaveChangesAsync();

            return OperationResult<Booking>.Ok(booking);
        }

        public async Task<OperationResult<bool>> CancelAsync(int id)
        {
            var booking = await Bookings.FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
                return OperationResult<bool>.Missing("Бронирование не найдено");

            _db.Bookings.Remove(booking);
            await _db.SaveChangesAsync();

            return OperationResult<bool>.Ok(true);
        }

        // два интервала пересекаются, если начало одного раньше конца другого и наоборот
        private async Task<bool> IsBusyAsync(int resourceId, DateTime start, DateTime end, int? exceptBookingId)
        {
            return await _db.Bookings.AnyAsync(b =>
                b.ResourceId == resourceId &&
                (exceptBookingId == null || b.Id != exceptBookingId) &&
                b.StartTime < end &&
                start < b.EndTime);
        }
    }
}
