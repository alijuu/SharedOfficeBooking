using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedOfficeBooking.Application.Dtos;
using SharedOfficeBooking.Domain.Helpers;
using SharedOfficeBooking.Infrastructure.Repositories.Workspace;

namespace SharedOfficeBooking.Controllers;

[ApiController]
[Route("api/workspaces")]
// [Authorize(Roles = "Admin,SuperAdmin")]
public class WorkspaceController : ControllerBase
{
    private readonly IWorkspaceRepository _repo;
    private readonly IMapper _mapper;

    public WorkspaceController(IWorkspaceRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] WorkspaceCreateDto dto)
    {
        var workspace = _mapper.Map<Workspace>(dto);

        var created = await _repo.AddAsync(workspace);
        await _repo.GenerateDesksFromFloorPlanAsync(created.Id);

        var resultDto = _mapper.Map<WorkspaceResponseDto>(created);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, resultDto);
    }


    [HttpGet("get/{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var ws = await _repo.GetByIdAsync(id);
        if (ws == null) return NotFound();
        return Ok(ws);
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _repo.GetPagedAsync(page, pageSize);

        var mappedItems = _mapper.Map<IEnumerable<WorkspaceResponseDto>>(result.Items);

        var paginatedDto = new PaginatedResult<WorkspaceResponseDto>
        {
            Items = mappedItems,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };

        return Ok(paginatedDto);
    }


    [HttpPut("update/{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] WorkspaceUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest("Workspace ID mismatch.");

        // Map the DTO to the Workspace entity
        var workspace = _mapper.Map<Workspace>(dto);

        // Update the workspace in the repository
        var updated = await _repo.UpdateAsync(workspace);
        await _repo.GenerateDesksFromFloorPlanAsync(updated.Id);

        // Map the updated workspace to the response DTO
        var resultDto = _mapper.Map<WorkspaceResponseDto>(updated);
        return Ok(resultDto);
    }


    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        return NoContent();
    }
}
