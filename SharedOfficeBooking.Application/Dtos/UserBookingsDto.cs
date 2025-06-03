namespace SharedOfficeBooking.Application.Dtos;

public class UserBookingsDto
{
        public string Id { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public double DurationHrs { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
    
}