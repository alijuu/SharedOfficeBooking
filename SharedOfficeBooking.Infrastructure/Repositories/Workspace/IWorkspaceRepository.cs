using SharedOfficeBooking.Domain.Helpers;

namespace SharedOfficeBooking.Infrastructure.Repositories.Workspace;

public interface IWorkspaceRepository
{
    Task<ServiceResponse<Domain.Entities.Workspace>> AddAsync(Domain.Entities.Workspace ws);
    Task<ServiceResponse<Domain.Entities.Workspace>?> GetByIdAsync(int id);
    Task<PaginatedResult<Domain.Entities.Workspace>> GetPagedAsync(int page, int pageSize);
    Task<ServiceResponse<Domain.Entities.Workspace>> UpdateAsync(int id, Domain.Entities.Workspace updatedWs);
    Task<ServiceResponse<string>> DeleteAsync(int id);

    /// <summary>
    /// Reads ws.FloorPlanJson, creates Desk rows×cols where cell==1
    /// </summary>
    Task<ServiceResponse<string>> GenerateDesksFromFloorPlanAsync(int workspaceId);
}