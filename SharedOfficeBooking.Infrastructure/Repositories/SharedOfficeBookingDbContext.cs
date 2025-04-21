using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedOfficeBooking.Domain.Entities;

namespace SharedOfficeBooking.Infrastructure.Repositories;

public class SharedOfficeBookingDbContext : IdentityDbContext<ApplicationUser>
{
    public SharedOfficeBookingDbContext(DbContextOptions<SharedOfficeBookingDbContext> options) : base(options)
    {
    }
}