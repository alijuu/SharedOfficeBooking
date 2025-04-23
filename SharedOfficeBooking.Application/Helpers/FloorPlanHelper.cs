using System.Text.Json;

namespace SharedOfficeBooking.Application.Helpers;

public static class FloorPlanHelper
{
    public static string SerializeMatrix(List<List<int>> matrix)
    {
        return JsonSerializer.Serialize(matrix, new JsonSerializerOptions());
    }

    public static List<List<int>> DeserializeMatrix(string json)
    {
        return string.IsNullOrEmpty(json)
            ? new List<List<int>>()
            : JsonSerializer.Deserialize<List<List<int>>>(json)!;
    }
}
    
