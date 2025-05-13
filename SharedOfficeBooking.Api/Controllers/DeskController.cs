using Microsoft.AspNetCore.Mvc;
using SharedOfficeBooking.Infrastructure.Repositories.Desk;

namespace SharedOfficeBooking.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DeskController : ControllerBase
{
    private readonly IDeskRepository _deskRepository;

    public DeskController(IDeskRepository deskRepository)
    {
        _deskRepository = deskRepository;
    }

    [HttpGet("desk/{workspaceId:int}")]
    public async Task<IActionResult> GetDesksByWorkspace(int workspaceId)
    {
        var result = await _deskRepository.GetDesksByWorkspaceIdAsync(workspaceId);
        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}
