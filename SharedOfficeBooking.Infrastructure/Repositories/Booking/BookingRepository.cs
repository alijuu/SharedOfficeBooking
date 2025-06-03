using System.Globalization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SharedOfficeBooking.Application.Dtos;
using SharedOfficeBooking.Domain.Entities;
using SharedOfficeBooking.Domain.Helpers;

namespace SharedOfficeBooking.Infrastructure.Repositories.Booking;

public class BookingRepository : IBookingRepository
{
    private readonly SharedOfficeBookingDbContext _db;
    private readonly IMapper _mapper;

    public BookingRepository(SharedOfficeBookingDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<Domain.Entities.Booking>> CreateBookingAsync(Guid userId, BookingCreateDto dto)
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
            UserId = userId,
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


    public async Task<ServiceResponse<List<UserBookingsDto>>> GetBookingsForUserAsync(Guid userId)
    {
        var response = new ServiceResponse<List<UserBookingsDto>>();

        // Eagerly load Desk → Workspace so we can read Name, ImageUrl, etc.
        var bookings = await _db.Bookings
            .Where(b => b.UserId == userId)
            .Include(b => b.Desk)
            .ThenInclude(d => d.Workspace)
            .OrderByDescending(b => b.StartTime)
            .ToListAsync();

        var nowUtc = DateTime.UtcNow;

        var bookingDtos = bookings.Select(b =>
        {
            var workspace = b.Desk.Workspace!; // not null
            return new UserBookingsDto()
            {
                Id = b.Id.ToString(),
                Date = b.StartTime.ToString("dd. MMMM yyyy", CultureInfo.InvariantCulture),
                Location = workspace.Name,
                ThumbnailUrl = workspace.ImageUrl,
                DurationHrs = (b.EndTime - b.StartTime).TotalHours,
                Price = 30M, // mock for now; later move price to Desk or Booking
                Status = (b.EndTime > nowUtc) ? "upcoming" : "past"
            };
        }).ToList();

        response.Data = bookingDtos;
        response.Message = "User bookings retrieved successfully.";
        return response;
    }
}