using SharedOfficeBooking.Application.Dtos;
using SharedOfficeBooking.Domain.Helpers;

namespace SharedOfficeBooking.Infrastructure.Repositories.Desk;

public interface IDeskRepository
{
    Task<ServiceResponse<List<DeskResponseDto>>> GetDesksByWorkspaceIdAsync(int workspaceId);

}