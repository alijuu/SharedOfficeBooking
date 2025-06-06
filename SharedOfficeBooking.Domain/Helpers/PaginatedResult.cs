namespace SharedOfficeBooking.Domain.Helpers;

public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int Page { get; set; }   
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}