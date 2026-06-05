namespace Infrastructure.DTOs.AnalyticsDto;

public class DepositOnlyAccountDto
{
    public Guid AccountId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
}