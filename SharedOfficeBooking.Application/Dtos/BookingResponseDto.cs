using SharedOfficeBooking.Domain.Entities;

namespace SharedOfficeBooking.Application.Dtos;

public class BookingResponseDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public int DeskId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsApproved { get; set; }
    public BookingType Type { get; set; }
}