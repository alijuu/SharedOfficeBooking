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

        var desk = await _db.Desks.Include(d => d.Bookings)
            .FirstOrDefaultAsync(d => d.Id == dto.DeskId);
        if (desk == null)
        {
            response.Success = false;
            response.Message = "Desk not found.";
            return response;
        }

        DateTime startTime = dto.StartTime;
        DateTime endTime;

        switch (dto.Type)
        {
            case BookingType.Hour:
                if (!dto.EndTime.HasValue)
                {
                    response.Success = false;
                    response.Message = "EndTime must be provided for hourly bookings.";
                    return response;
                }

                if (dto.EndTime.Value <= startTime)
                {
                    response.Success = false;
                    response.Message = "EndTime must be after StartTime.";
                    return response;
                }

                endTime = dto.EndTime.Value;
                break;

            case BookingType.HalfDay:
                endTime = startTime.AddHours(4);

                // Must end by 5 PM
                var latestEnd = new DateTime(startTime.Year, startTime.Month, startTime.Day, 17, 0, 0);
                if (endTime > latestEnd)
                {
                    response.Success = false;
                    response.Message = "Half-day bookings must end by 5:00 PM.";
                    return response;
                }

                break;

            case BookingType.WholeDay:
                startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 9, 0, 0);
                endTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 17, 0, 0);
                break;

            default:
                response.Success = false;
                response.Message = "Unsupported booking type.";
                return response;
        }

        // Overlap check
        var overlaps = desk.Bookings.Any(b =>
            (startTime < b.EndTime) && (endTime > b.StartTime)
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
            StartTime = startTime,
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