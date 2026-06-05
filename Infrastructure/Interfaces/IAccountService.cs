using Infrastructure.DTOs.AccountDto;
using Infrastructure.Results;

namespace Infrastructure.Interfaces;

public interface IAccountService
{
    Task<Result<List<AccountGetDto>>> GetAllAsync();
    Task<Result<Guid>> CreateAsync(AccountCreateDto dto);
    Task<Result<AccountGetDto>> GetByIdAsync(Guid id);
    Task<Result<List<AccountGetDto>>> GetByClientIdAsync(Guid clientId);
    Task<Result<string>> CloseAccountAsync(Guid id);
}