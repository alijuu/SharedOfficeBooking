namespace SharedOfficeBooking.Application.Dtos;

public class WorkspaceUpdateDto
{
    public int Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<List<int>> FloorPlan { get; set; } = new();
}
