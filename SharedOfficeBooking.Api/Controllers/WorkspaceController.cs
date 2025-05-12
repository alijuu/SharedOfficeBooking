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
        var createResponse = await _repo.AddAsync(workspace);

        if (!createResponse.Success)
            return BadRequest(createResponse);

        var generateDesksResponse = await _repo.GenerateDesksFromFloorPlanAsync(createResponse.Data!.Id);
        if (!generateDesksResponse.Success)
            return BadRequest(generateDesksResponse);
        
        var updatedWorkspace = await _repo.GetByIdAsync(createResponse.Data.Id);

        var resultDto = _mapper.Map<WorkspaceResponseDto>(updatedWorkspace.Data);
        return CreatedAtAction(nameof(GetById), new { id = resultDto.Id }, new ServiceResponse<WorkspaceResponseDto>
        {
            Data = resultDto,
            Message = "Workspace created successfully.",
            Success = true
        });
    }


    [HttpGet("get/{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _repo.GetByIdAsync(id);
        if (!response.Success)
            return NotFound(response);

        var dto = _mapper.Map<WorkspaceResponseDto>(response.Data);
        return Ok(new ServiceResponse<WorkspaceResponseDto>
        {
            Data = dto,
            Message = "Workspace retrieved.",
            Success = true
        });
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
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
        var workspace = _mapper.Map<Workspace>(dto);

        var response = await _repo.UpdateAsync(id, workspace);

        if (!response.Success)
        {
            return NotFound(response);
        }

        await _repo.GenerateDesksFromFloorPlanAsync(id);

        var resultDto = _mapper.Map<WorkspaceResponseDto>(response.Data);
        return Ok(new ServiceResponse<WorkspaceResponseDto>
        {
            Data = resultDto,
            Success = true,
            Message = "Workspace updated successfully"
        });
    }


    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _repo.DeleteAsync(id);
        if (!response.Success)
            return NotFound(response);
        return Ok(response);
    }
}
