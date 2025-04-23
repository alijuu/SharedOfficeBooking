using System.Linq.Dynamic.Core;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SharedOfficeBooking.Domain.Entities;
using SharedOfficeBooking.Domain.Helpers;

namespace SharedOfficeBooking.Infrastructure.Repositories.Workspace;

public class WorkspaceRepository : IWorkspaceRepository
{
    private readonly SharedOfficeBookingDbContext _db;
    public WorkspaceRepository(SharedOfficeBookingDbContext db) => _db = db;

    public async Task<Domain.Entities.Workspace> AddAsync(Domain.Entities.Workspace ws)
    {
        _db.Workspaces.Add(ws);
        await _db.SaveChangesAsync();
        return ws;
    }

    public async Task DeleteAsync(int id)
    {
        var ws = await _db.Workspaces.FindAsync(id);
        if (ws == null) throw new KeyNotFoundException();
        _db.Workspaces.Remove(ws);
        await _db.SaveChangesAsync();
    }

    public async Task<Domain.Entities.Workspace?> GetByIdAsync(int id)
        => await _db.Workspaces
            .Include(w => w.Desks)
            .FirstOrDefaultAsync(w => w.Id == id);

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


    public async Task<Domain.Entities.Workspace> UpdateAsync(Domain.Entities.Workspace ws)
    {
        _db.Workspaces.Update(ws);
        await _db.SaveChangesAsync();
        return ws;
    }

    public async Task GenerateDesksFromFloorPlanAsync(int workspaceId)
    {
        var ws = await _db.Workspaces.FindAsync(workspaceId);
        if (ws == null) throw new KeyNotFoundException();

        var matrix = JsonSerializer.Deserialize<List<List<int>>>(ws.FloorPlan)
                     ?? new List<List<int>>();

        // clear existing desks
        var existing = _db.Desks.Where(d => d.WorkspaceId == workspaceId);
        _db.Desks.RemoveRange(existing);

        // create new
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
    }
}