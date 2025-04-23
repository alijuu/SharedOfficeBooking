using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace SharedOfficeBooking.Domain.Entities;

public class Workspace
{
    public int Id { get; set; }
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(255)]
    public string Address { get; set; } = string.Empty;
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    [MaxLength(255)]
    public string Phone { get; set; } = string.Empty;
    [MaxLength(255)]
    public string ImageUrl { get; set; } = string.Empty;
    [MaxLength(5000)]
    public string Description { get; set; } = string.Empty;
    
    // JSON matrix: [[1,0],[1,1]]
    public string FloorPlan { get; set; } = string.Empty;
    
    [NotMapped]
    public List<List<int>> FloorPlanMatrix
    {
        get => string.IsNullOrEmpty(FloorPlan)
            ? new List<List<int>>()
            : JsonSerializer.Deserialize<List<List<int>>>(FloorPlan) ?? new();
        set => FloorPlan = JsonSerializer.Serialize(value);
    }

    public List<Desk> Desks { get; set; } = new();
}