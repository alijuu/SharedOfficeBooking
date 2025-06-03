namespace SharedOfficeBooking.Application.Dtos;

public class DeskBookingDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// True if that desk has a “WholeDay” booking for the given date;
    /// false if it only has “Hour” or “HalfDay” bookings on that date.
    /// </summary>
    public bool IsFullDay { get; set; }
}