namespace Infrastructure.DTOs.AnalyticsDto;

public class ClientBalanceDto
{
    public string Email { get; set; } = string.Empty;
    public decimal TotalBalance { get; set; }
}