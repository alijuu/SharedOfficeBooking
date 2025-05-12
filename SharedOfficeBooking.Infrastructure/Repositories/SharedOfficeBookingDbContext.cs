using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedOfficeBooking.Domain.Entities;

namespace SharedOfficeBooking.Infrastructure.Repositories;

public class SharedOfficeBookingDbContext : IdentityDbContext<ApplicationUser>
{
    public SharedOfficeBookingDbContext(DbContextOptions<SharedOfficeBookingDbContext> options) : base(options)
    {
    }
    
    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<Domain.Entities.Workspace> Workspaces => Set<Domain.Entities.Workspace>();
    public DbSet<Domain.Entities.Desk> Desks => Set<Domain.Entities.Desk>();
    public DbSet<Domain.Entities.Booking> Bookings => Set<Domain.Entities.Booking>();
}