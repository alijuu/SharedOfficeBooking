using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SharedOfficeBooking.Domain.Entities;
using SharedOfficeBooking.Domain.Helpers;

namespace SharedOfficeBooking.Infrastructure.Repositories.Workspace;

public class WorkspaceRepository : IWorkspaceRepository
{
    private readonly SharedOfficeBookingDbContext _db;
    public WorkspaceRepository(SharedOfficeBookingDbContext db) => _db = db;

    public async Task<ServiceResponse<Domain.Entities.Workspace>> AddAsync(Domain.Entities.Workspace ws)
    {
        var response = new ServiceResponse<Domain.Entities.Workspace>();
        try
        {
            _db.Workspaces.Add(ws);
            await _db.SaveChangesAsync();
            response.Data = ws;
            response.Message = "Workspace created.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    public async Task<ServiceResponse<string>> DeleteAsync(int id)
    {
        var response = new ServiceResponse<string>();
        var ws = await _db.Workspaces.FindAsync(id);
        if (ws == null)
        {
            response.Success = false;
            response.Message = "Workspace not found.";
            return response;
        }

        _db.Workspaces.Remove(ws);
        await _db.SaveChangesAsync();
        response.Data = "Workspace deleted.";
        return response;
    }

    public async Task<ServiceResponse<Domain.Entities.Workspace?>> GetByIdAsync(int id)
    {
        var response = new ServiceResponse<Domain.Entities.Workspace?>();
        var ws = await _db.Workspaces
            .Include(w => w.Desks)
            .FirstOrDefaultAsync(w => w.Id == id);
        if (ws == null)
        {
            response.Success = false;
            response.Message = "Workspace not found.";
        }
        else
        {
            response.Data = ws;
        }
        return response;
    }
    public async Task<PaginatedResult<Domain.Entities.Workspace>> GetPagedAsync(int page, int pageSize)
    {
        var query = _db.Workspaces.AsQueryable();

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Domain.Entities.Workspace>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }


    public async Task<ServiceResponse<Domain.Entities.Workspace>> UpdateAsync(int id, Domain.Entities.Workspace updatedWs)
    {
        var serviceResponse = new ServiceResponse<Domain.Entities.Workspace>();

        var existingWs = await _db.Workspaces.FindAsync(id);
        if (existingWs == null)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = $"Workspace with ID {id} not found.";
            return serviceResponse;
        }

        // Update fields
        existingWs.Name = updatedWs.Name;
        existingWs.Address = updatedWs.Address;
        existingWs.Email = updatedWs.Email;
        existingWs.Phone = updatedWs.Phone;
        existingWs.ImageUrl = updatedWs.ImageUrl;
        existingWs.Description = updatedWs.Description;
        existingWs.FloorPlan = updatedWs.FloorPlan;

        _db.Workspaces.Update(existingWs);
        await _db.SaveChangesAsync();

        serviceResponse.Data = existingWs;
        serviceResponse.Message = "Workspace updated successfully";
        return serviceResponse;
    }

    public async Task<ServiceResponse<string>> GenerateDesksFromFloorPlanAsync(int workspaceId)
    {
        var response = new ServiceResponse<string>();
        var ws = await _db.Workspaces.FindAsync(workspaceId);
        if (ws == null)
        {
            response.Success = false;
            response.Message = "Workspace not found.";
            return response;
        }

        var matrix = JsonSerializer.Deserialize<List<List<int>>>(ws.FloorPlan)
                     ?? new List<List<int>>();

        var existing = _db.Desks.Where(d => d.WorkspaceId == workspaceId);
        _db.Desks.RemoveRange(existing);

        for (int r = 0; r < matrix.Count; r++)
        {
            for (int c = 0; c < matrix[r].Count; c++)
            {
                if (matrix[r][c] == 1)
                {
                    _db.Desks.Add(new Desk
                    {
                        WorkspaceId = workspaceId,
                        Code = $"R{r + 1}C{c + 1}"
                    });
                }
            }
        }

        await _db.SaveChangesAsync();
        response.Data = "Desks generated from floor plan.";
        return response;
    }
}