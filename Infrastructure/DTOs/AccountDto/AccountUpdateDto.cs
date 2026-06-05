using Domain.Entities;

namespace Infrastructure.DTOs.AccountDto;

public class AccountUpdateDto
{
    public string Currency { get; set; } = "USD";
    public AccountType Type { get; set; }
}