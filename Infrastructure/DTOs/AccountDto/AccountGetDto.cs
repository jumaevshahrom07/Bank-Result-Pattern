using Domain.Entities;

namespace Infrastructure.DTOs.AccountDto;

public class AccountGetDto
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}