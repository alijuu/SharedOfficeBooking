using SharedOfficeBooking.Domain.Entities;

namespace SharedOfficeBooking.Application.Dtos;

public class BookingCreateDto
{
    public int DeskId { get; set; }
    public BookingType Type { get; set; } = BookingType.Hour;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}