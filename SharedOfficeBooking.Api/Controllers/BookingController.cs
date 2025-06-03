using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedOfficeBooking.Application.Dtos;
using SharedOfficeBooking.Domain.Helpers;
using SharedOfficeBooking.Infrastructure.Repositories.Booking;

namespace SharedOfficeBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingRepository _repo;
    private readonly IMapper _mapper;

    public BookingController(IBookingRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDto dto)
    {
        // 1) Get the user ID from the token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null 
            || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new ServiceResponse<string>
            {
                Success = false,
                Message = "Invalid token or user ID."
            });
        }

        // 2) Call CreateBookingAsync with the extracted userId + dto
        var result = await _repo.CreateBookingAsync(userId, dto);

        if (!result.Success)
            return BadRequest(result);

        var responseDto = _mapper.Map<BookingResponseDto>(result.Data);
        return Ok(new ServiceResponse<BookingResponseDto>
        {
            Data = responseDto,
            Message = result.Message
        });
    }

    [HttpGet("desk/{deskId:int}")]
    public async Task<IActionResult> GetBookingsForDesk(int deskId)
    {
        var result = await _repo.GetBookingsForDeskAsync(deskId);

        if (!result.Success)
            return NotFound(result);

        var list = _mapper.Map<List<BookingResponseDto>>(result.Data);
        return Ok(new ServiceResponse<List<BookingResponseDto>>
        {
            Data = list,
            Message = result.Message
        });
    }
    
    [Authorize]
    [HttpGet("user")]
    public async Task<IActionResult> GetBookingsForCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new ServiceResponse<string> { Success = false, Message = "Invalid token or user ID." });
        }

        var result = await _repo.GetBookingsForUserAsync(userId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

}
