using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SharedOfficeBooking.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    [MaxLength(255)]
    public string Phone { get; set; } = string.Empty;
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = "User";

    public List<Booking> Bookings { get; set; } = new();
}