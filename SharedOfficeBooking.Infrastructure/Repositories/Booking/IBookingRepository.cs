using SharedOfficeBooking.Application.Dtos;
using SharedOfficeBooking.Domain.Helpers;

namespace SharedOfficeBooking.Infrastructure.Repositories.Booking;

public interface IBookingRepository
{
    Task<ServiceResponse<Domain.Entities.Booking>> CreateBookingAsync(Guid userId, BookingCreateDto dto);
    Task<ServiceResponse<List<Domain.Entities.Booking>>> GetBookingsForDeskAsync(int deskId);
    Task<ServiceResponse<List<UserBookingsDto>>> GetBookingsForUserAsync(Guid userId);

}