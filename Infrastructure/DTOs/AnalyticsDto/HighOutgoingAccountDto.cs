namespace Infrastructure.DTOs.AnalyticsDto;

public class HighOutgoingAccountDto
{
    public Guid AccountId { get; set; }
    public decimal TotalOutgoing { get; set; }
}