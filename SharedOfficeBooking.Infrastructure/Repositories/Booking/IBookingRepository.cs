using SharedOfficeBooking.Application.Dtos;
using SharedOfficeBooking.Domain.Helpers;

namespace SharedOfficeBooking.Infrastructure.Repositories.Booking;

public interface IBookingRepository
{
    Task<ServiceResponse<Domain.Entities.Booking>> CreateBookingAsync(BookingCreateDto dto);
    Task<ServiceResponse<List<Domain.Entities.Booking>>> GetBookingsForDeskAsync(int deskId);
}