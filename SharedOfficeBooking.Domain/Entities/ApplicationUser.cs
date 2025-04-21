using Microsoft.AspNetCore.Identity;

namespace SharedOfficeBooking.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string Role { get; set; } = "User";
}