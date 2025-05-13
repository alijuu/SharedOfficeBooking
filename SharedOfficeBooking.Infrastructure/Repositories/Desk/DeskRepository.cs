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
}