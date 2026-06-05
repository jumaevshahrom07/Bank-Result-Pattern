namespace Infrastructure.DTOs.AnalyticsDto;

public class LowBalanceAccountDto
{
    public Guid AccountId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}