namespace Infrastructure.DTOs.AnalyticsDto;

public class AccountTransferDto
{
    public Guid TransactionId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
    public string Status { get; set; } = string.Empty;
}