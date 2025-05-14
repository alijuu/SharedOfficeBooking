using Microsoft.AspNetCore.Authorization;
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
    
    //[Authorize]
    [HttpGet("workspace/{workspaceId}/booked-now")]
    public async Task<IActionResult> GetCurrentlyBookedDesks(int workspaceId)
    {
        var response = await _deskRepository.GetCurrentlyBookedDesksByWorkspaceId(workspaceId);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

}
