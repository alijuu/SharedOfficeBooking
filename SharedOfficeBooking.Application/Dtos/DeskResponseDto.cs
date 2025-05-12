namespace SharedOfficeBooking.Application.Dtos;

public class DeskResponseDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public int WorkspaceId { get; set; }
}