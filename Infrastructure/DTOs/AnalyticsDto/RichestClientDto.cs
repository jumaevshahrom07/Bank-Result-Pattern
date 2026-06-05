namespace Infrastructure.DTOs.AnalyticsDto;

public class RichestClientDto
{
    public Guid ClientId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public decimal TotalBalance { get; set; }
}