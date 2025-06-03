using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SharedOfficeBooking.Application.Dtos;
using SharedOfficeBooking.Domain.Entities;
using SharedOfficeBooking.Domain.Helpers;

namespace SharedOfficeBooking.Infrastructure.Repositories.Desk;

public class DeskRepository : IDeskRepository
{
    private readonly SharedOfficeBookingDbContext _context;
    private readonly IMapper _mapper;

    public DeskRepository(SharedOfficeBookingDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<List<DeskResponseDto>>> GetDesksByWorkspaceIdAsync(int workspaceId)
    {
        var response = new ServiceResponse<List<DeskResponseDto>>();

        var workspace = await _context.Workspaces
            .Include(w => w.Desks)
            .FirstOrDefaultAsync(w => w.Id == workspaceId);

        if (workspace == null)
        {
            response.Success = false;
            response.Message = "Workspace not found.";
            return response;
        }

        var deskDtos = workspace.Desks.Select(d => new DeskResponseDto
        {
            Id = d.Id,
            Code = d.Code,
            WorkspaceId = workspace.Id,
        }).ToList();

        response.Data = deskDtos;
        return response;
    }
    
    public async Task<ServiceResponse<List<DeskBookingDto>>> GetCurrentlyBookedDesksByWorkspaceId(int workspaceId)
{
    var response = new ServiceResponse<List<DeskBookingDto>>();

    // 1) Determine “today” (UTC date) and then build the day‐start/end times: 9 AM → 5 PM.
    var today       = DateTime.UtcNow.Date;
    var dayStart    = new DateTime(today.Year, today.Month, today.Day, 9, 0, 0, DateTimeKind.Utc);
    var dayEnd      = new DateTime(today.Year, today.Month, today.Day, 17, 0, 0, DateTimeKind.Utc);

    // 2) Grab any booking that overlaps [dayStart, dayEnd].
    //    A booking overlaps if booking.StartTime < dayEnd && booking.EndTime > dayStart.
    var bookingsToday = await _context.Bookings
        .Where(b =>
            b.Desk.WorkspaceId == workspaceId &&
            b.StartTime < dayEnd &&
            b.EndTime > dayStart)
        .Include(b => b.Desk)
        .ToListAsync();

    // 3) Group by desk, then for each desk merge its “clipped” intervals and check coverage.
    var deskDtos = bookingsToday
        .GroupBy(b => b.Desk)
        .Select(grouping =>
        {
            var desk = grouping.Key!;

            // Build a list of intervals clipped to [dayStart, dayEnd]
            var intervals = grouping
                .Select(b =>
                {
                    // Clip the booking window to within [dayStart, dayEnd]
                    var start = b.StartTime < dayStart ? dayStart : b.StartTime;
                    var end   = b.EndTime   > dayEnd   ? dayEnd   : b.EndTime;
                    return (Start: start, End: end);
                })
                // Only keep intervals that actually have positive length
                .Where(interval => interval.End > interval.Start)
                .OrderBy(interval => interval.Start)
                .ToList();

            // Merge overlapping/adjacent intervals
            var merged = new List<(DateTime Start, DateTime End)>();
            foreach (var interval in intervals)
            {
                if (merged.Count == 0)
                {
                    merged.Add(interval);
                }
                else
                {
                    var last = merged[merged.Count - 1];
                    if (interval.Start <= last.End) 
                    {
                        // They overlap or touch → extend the last interval’s end if needed
                        var newEnd = interval.End > last.End ? interval.End : last.End;
                        merged[merged.Count - 1] = (last.Start, newEnd);
                    }
                    else
                    {
                        merged.Add(interval);
                    }
                }
            }

            // Check if the merged intervals cover [dayStart, dayEnd] exactly.
            // Condition for “full day”:
            //   - The first merged interval must start exactly at dayStart
            //   - The last merged interval must end exactly at dayEnd
            //   - And there must be no gaps in between (since we merged, this is implied)
            bool isFullDay = false;
            if (merged.Count > 0)
            {
                var first = merged.First();
                var last  = merged.Last();
                if (first.Start <= dayStart && last.End >= dayEnd)
                {
                    // Since we merged overlapping/adjacent, the only way for coverage
                    // is that the merged chain starts at or before dayStart and ends at or after dayEnd.
                    isFullDay = true;
                }
            }

            return new DeskBookingDto
            {
                Id        = desk.Id,
                Code      = desk.Code,
                IsFullDay = isFullDay
            };
        })
        .ToList();

    response.Data    = deskDtos;
    response.Success = true;
    return response;
}


}