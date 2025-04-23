using System.ComponentModel.DataAnnotations;

namespace SharedOfficeBooking.Domain.Entities;

public class Desk
{
    public int Id { get; set; }
    [MaxLength(255)] public string Code { get; set; } = string.Empty;
    public int WorkspaceId { get; set; }
    public Workspace Workspace { get; set; } = null!;

    // Navigation: bookings for this desk
    public List<Booking> Bookings { get; set; } = new();
}