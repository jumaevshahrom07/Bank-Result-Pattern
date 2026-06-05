namespace Infrastructure.DTOs.AnalyticsDto;

public class TopOutgoingAccountDto
{
    public Guid AccountId { get; set; }
    public int TransfersCount { get; set; }
}