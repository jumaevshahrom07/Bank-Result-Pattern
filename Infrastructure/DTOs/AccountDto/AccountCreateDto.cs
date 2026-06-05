using Domain.Entities;

namespace Infrastructure.DTOs.AccountDto;

public class AccountCreateDto
{
    public Guid ClientId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "USD";
    public AccountType Type { get; set; }
}