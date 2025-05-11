using Microsoft.EntityFrameworkCore;
using SharedOfficeBooking.Application.Dtos;
using SharedOfficeBooking.Domain.Entities;
using SharedOfficeBooking.Domain.Helpers;

namespace SharedOfficeBooking.Infrastructure.Repositories.Booking;

public class BookingRepository : IBookingRepository
{
    private readonly SharedOfficeBookingDbContext _db;

    public BookingRepository(SharedOfficeBookingDbContext db)
    {
        _db = db;
    }

    public async Task<ServiceResponse<Domain.Entities.Booking>> CreateBookingAsync(BookingCreateDto dto)
    {
        var response = new ServiceResponse<Domain.Entities.Booking>();

        var desk = await _db.Desks.Include(d => d.Bookings).FirstOrDefaultAsync(d => d.Id == dto.DeskId);
        if (desk == null)
        {
            response.Success = false;
            response.Message = "Desk not found.";
            return response;
        }

        var duration = dto.Type switch
        {
            BookingType.Hour => TimeSpan.FromHours(1),
            BookingType.HalfDay => TimeSpan.FromHours(4),
            BookingType.WholeDay => TimeSpan.FromHours(8),
            _ => TimeSpan.FromHours(1)
        };

        var endTime = dto.StartTime.Add(duration);

        var overlaps = desk.Bookings.Any(b =>
            (dto.StartTime < b.EndTime) && (endTime > b.StartTime)
        );

        if (overlaps)
        {
            response.Success = false;
            response.Message = "Booking conflicts with an existing booking.";
            return response;
        }

        var booking = new Domain.Entities.Booking
        {
            UserId = dto.UserId,
            DeskId = dto.DeskId,
            StartTime = dto.StartTime,
            EndTime = endTime,
            Type = dto.Type,
            CreatedAt = DateTime.UtcNow,
            IsApproved = true
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        response.Data = booking;
        response.Message = "Booking created successfully.";
        return response;
    }

    public async Task<ServiceResponse<List<Domain.Entities.Booking>>> GetBookingsForDeskAsync(int deskId)
    {
        var response = new ServiceResponse<List<Domain.Entities.Booking>>();

        var desk = await _db.Desks.Include(d => d.Bookings).FirstOrDefaultAsync(d => d.Id == deskId);
        if (desk == null)
        {
            response.Success = false;
            response.Message = "Desk not found.";
            return response;
        }

        response.Data = desk.Bookings
            .Where(b => b.EndTime > DateTime.UtcNow)
            .OrderByDescending(b => b.StartTime)
            .ToList();
        
        response.Message = "Bookings retrieved successfully.";
        return response;
    }
}
