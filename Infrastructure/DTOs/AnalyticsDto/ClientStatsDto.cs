namespace Infrastructure.DTOs.AnalyticsDto;

public class ClientStatsDto
{
    public Guid ClientId { get; set; }
    public int AccountsCount { get; set; }
    public decimal TotalBalance { get; set; }
}