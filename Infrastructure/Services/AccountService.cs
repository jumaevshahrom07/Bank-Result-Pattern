using Domain.Entities;
using Infrastructure.DTOs.AccountDto;
using Infrastructure.Interfaces;
using Infrastructure.Persistence.DataContext;
using Infrastructure.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class AccountService : IAccountService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AccountService> _logger;

    public AccountService(AppDbContext context, ILogger<AccountService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<AccountGetDto>>> GetAllAsync()
    {
        var data = await _context.Accounts
            .AsNoTracking()
            .Select(x => new AccountGetDto
            {
                Id = x.Id,
                ClientId = x.ClientId,
                AccountNumber = x.AccountNumber,
                Balance = x.Balance,
                Currency = x.Currency,
                Type = x.Type,
                IsActive = x.IsActive
            }).ToListAsync();

        return Result<List<AccountGetDto>>.Ok(data);
    }

    public async Task<Result<Guid>> CreateAsync(AccountCreateDto dto)
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            ClientId = dto.ClientId,
            AccountNumber = dto.AccountNumber,
            Balance = 0,
            Currency = "USD",
            Type = dto.Type,
            IsActive = true
        };

        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();

        return Result<Guid>.Ok(account.Id);
    }
    
    public async Task<Result<AccountGetDto>> GetByIdAsync(Guid id)
    {
        var acc = await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (acc == null)
            return Result<AccountGetDto>.Fail("Not found", ErrorType.NotFound);

        return Result<AccountGetDto>.Ok(new AccountGetDto
        {
            Id = acc.Id,
            ClientId = acc.ClientId,
            AccountNumber = acc.AccountNumber,
            Balance = acc.Balance,
            Currency = acc.Currency,
            Type = acc.Type,
            IsActive = acc.IsActive
        });
    }

    public async Task<Result<List<AccountGetDto>>> GetByClientIdAsync(Guid clientId)
    {
        var data = await _context.Accounts
            .Where(x => x.ClientId == clientId)
            .Select(x => new AccountGetDto
            {
                Id = x.Id,
                ClientId = x.ClientId,
                AccountNumber = x.AccountNumber,
                Balance = x.Balance,
                Currency = x.Currency,
                Type = x.Type,
                IsActive = x.IsActive
            }).ToListAsync();

        return Result<List<AccountGetDto>>.Ok(data);
    }


    public async Task<Result<string>> CloseAccountAsync(Guid id)
    {
        var acc = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == id);

        if (acc == null)
            return Result<string>.Fail("Not found", ErrorType.NotFound);

        if (acc.Balance > 0)
            return Result<string>.Fail("Balance must be 0", ErrorType.Validation);

        acc.IsActive = false;

        await _context.SaveChangesAsync();

        return Result<string>.Ok("Closed");
    }
}