using SharedOfficeBooking.Domain.Helpers;

namespace SharedOfficeBooking.Infrastructure.Repositories.Workspace;

public interface IWorkspaceRepository
{
    Task<Domain.Entities.Workspace> AddAsync(Domain.Entities.Workspace ws);
    Task<Domain.Entities.Workspace?> GetByIdAsync(int id);
    Task<PaginatedResult<Domain.Entities.Workspace>> GetPagedAsync(int page, int pageSize);
    Task<Domain.Entities.Workspace> UpdateAsync(Domain.Entities.Workspace ws);
    Task DeleteAsync(int id);

    /// <summary>
    /// Reads ws.FloorPlanJson, creates Desk rows×cols where cell==1
    /// </summary>
    Task GenerateDesksFromFloorPlanAsync(int workspaceId);
}