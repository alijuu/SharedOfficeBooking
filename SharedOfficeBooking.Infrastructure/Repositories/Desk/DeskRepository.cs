using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SharedOfficeBooking.Application.Dtos;
using SharedOfficeBooking.Domain.Helpers;

namespace SharedOfficeBooking.Infrastructure.Repositories.Desk;

public class DeskRepository : IDeskRepository
{
    private readonly SharedOfficeBookingDbContext _context;
    private readonly IMapper _mapper;

    public DeskRepository(SharedOfficeBookingDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<List<DeskResponseDto>>> GetDesksByWorkspaceIdAsync(int workspaceId)
    {
        var response = new ServiceResponse<List<DeskResponseDto>>();

        var workspace = await _context.Workspaces
            .Include(w => w.Desks)
            .FirstOrDefaultAsync(w => w.Id == workspaceId);

        if (workspace == null)
        {
            response.Success = false;
            response.Message = "Workspace not found.";
            return response;
        }

        var deskDtos = workspace.Desks.Select(d => new DeskResponseDto
        {
            Id = d.Id,
            Code = d.Code,
            WorkspaceId = workspace.Id,
        }).ToList();

        response.Data = deskDtos;
        return response;
    }
    
    public async Task<ServiceResponse<List<DeskResponseDto>>> GetCurrentlyBookedDesksByWorkspaceId(int workspaceId)
    {
        var now = DateTime.UtcNow;

        var bookedDesks = await _context.Bookings
            .Where(b => b.Desk.WorkspaceId == workspaceId &&
                        b.StartTime <= now &&
                        b.EndTime >= now)
            .Select(b => new DeskResponseDto
            {
                Id = b.Desk.Id,
                Code = b.Desk.Code,
                WorkspaceId = b.Desk.WorkspaceId
            })
            .Distinct()
            .ToListAsync();

        return new ServiceResponse<List<DeskResponseDto>>
        {
            Data = bookedDesks
        };
    }

}