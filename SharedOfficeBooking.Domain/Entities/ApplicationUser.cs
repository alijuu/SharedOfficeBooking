using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SharedOfficeBooking.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    [MaxLength(255)]
    public string Phone { get; set; } = string.Empty;
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Role Role { get; set; } = Role.User;

    public List<Booking> Bookings { get; set; } = new();
}