using System.Transactions;
using Domain.Entities;
using Infrastructure.DTOs.AnalyticsDto;
using Infrastructure.DTOs.TransactionDto;
using Infrastructure.Interfaces;
using Infrastructure.Persistence.DataContext;
using Infrastructure.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class AnalyticService : IAnalyticService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AnalyticService> _logger;

    public AnalyticService(AppDbContext context, ILogger<AnalyticService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // 1. Active Clients
    public async Task<Result<List<ActiveClientDto>>> GetActiveClientsAsync()
    {
        var data = await _context.Clients
            .Where(x => x.IsActive)
            .Select(x => new ActiveClientDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName
            })
            .ToListAsync();

        return Result<List<ActiveClientDto>>.Ok(data);
    }

    // 2. Client Accounts by Email
    public async Task<Result<List<ClientAccountDto>>> GetClientAccountsByEmailAsync(string email)
    {
        var data = await _context.Accounts
            .Where(a => a.Client.Email == email)
            .Select(a => new ClientAccountDto
            {
                AccountId = a.Id,
                AccountNumber = a.AccountNumber,
                Balance = a.Balance,
                Currency = a.Currency
            })
            .ToListAsync();

        return Result<List<ClientAccountDto>>.Ok(data);
    }

    // 3. Account Transfers (Completed only)
    public async Task<Result<List<TransactionGetDto>>> GetAccountTransfersAsync(Guid accountId)
    {
        var data = await _context.Transactions
            .Where(t =>
                t.ToAccountId == accountId &&
                t.Status == Domain.Entities.TransactionStatus.Completed)
            .Select(t => new TransactionGetDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Timestamp = t.Timestamp,
                Status = t.Status
            })
            .ToListAsync();

        return Result<List<TransactionGetDto>>.Ok(data);
    }

    // 4. Client Balance
    public async Task<Result<decimal>> GetClientBalanceAsync(string email)
    {
        var total = await _context.Accounts
            .Where(a => a.Client.Email == email)
            .SumAsync(a => a.Balance);

        return Result<decimal>.Ok(total);
    }

    // 5. Low balance accounts
    public async Task<Result<List<LowBalanceAccountDto>>> GetLowBalanceAccountsAsync(decimal threshold)
    {
        var data = await _context.Accounts
            .Where(a => a.Balance < threshold)
            .Select(a => new LowBalanceAccountDto
            {
                AccountId = a.Id,
                AccountNumber = a.AccountNumber,
                Balance = a.Balance
            })
            .ToListAsync();

        return Result<List<LowBalanceAccountDto>>.Ok(data);
    }

    // 6. Accounts with owners
    public async Task<Result<List<AccountOwnerDto>>> GetAccountsWithOwnersAsync()
    {
        var data = await _context.Accounts
            .Include(a => a.Client)
            .Select(a => new AccountOwnerDto
            {
                AccountNumber = a.AccountNumber,
                FirstName = a.Client.FirstName,
                LastName = a.Client.LastName,
                Balance = a.Balance
            })
            .ToListAsync();

        return Result<List<AccountOwnerDto>>.Ok(data);
    }

    // 7. Richest client
    public async Task<Result<RichestClientDto>> GetRichestClientAsync()
    {
        var data = await _context.Clients
            .Select(c => new RichestClientDto
            {
                ClientId = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                TotalBalance = c.Accounts.Sum(a => a.Balance)
            })
            .OrderByDescending(x => x.TotalBalance)
            .FirstOrDefaultAsync();

        if (data == null)
        {
            return Result<RichestClientDto>.Fail("Client not found", ErrorType.NotFound);
        }

        return Result<RichestClientDto>.Ok(data);
    }

    // 8. Top outgoing accounts
    public async Task<Result<List<TopOutgoingAccountDto>>> GetTopOutgoingAccountsAsync(int limit)
    {
        var data = await _context.Transactions
            .Where(t => t.Type == TransactionType.Outgoing)
            .GroupBy(t => t.FromAccountId)
            .Select(g => new TopOutgoingAccountDto
            {
                AccountId = g.Key,
                TransfersCount = g.Count()
            })
            .OrderByDescending(x => x.TransfersCount)
            .Take(limit)
            .ToListAsync();

        return Result<List<TopOutgoingAccountDto>>.Ok(data);
    }

    // 9. Client stats
    public async Task<Result<List<ClientStatsDto>>> GetClientStatsAsync()
    {
        var data = await _context.Clients
            .Select(c => new ClientStatsDto
            {
                ClientId = c.Id,
                AccountsCount = c.Accounts.Count,
                TotalBalance = c.Accounts.Sum(a => a.Balance)
            })
            .ToListAsync();

        return Result<List<ClientStatsDto>>.Ok(data);
    }

    // 10. High outgoing accounts
    public async Task<Result<List<HighOutgoingAccountDto>>> GetHighOutgoingAccountsAsync(decimal amount)
    {
        var data = await _context.Transactions
            .Where(t => t.Type == TransactionType.Outgoing)
            .GroupBy(t => t.FromAccountId)
            .Select(g => new HighOutgoingAccountDto
            {
                AccountId = g.Key,
                TotalOutgoing = g.Sum(x => x.Amount)
            })
            .Where(x => x.TotalOutgoing > amount)
            .ToListAsync();

        return Result<List<HighOutgoingAccountDto>>.Ok(data);
    }

    // 11. Total fees
    public async Task<Result<decimal>> GetTotalFeesAsync()
    {
        var total = await _context.Transactions
            .SumAsync(t => t.Fee);

        return Result<decimal>.Ok(total);
    }

    // 12. Average transfer
    public async Task<Result<decimal>> GetAverageTransferAsync()
    {
        var avg = await _context.Transactions
            .Where(t => t.Status == Domain.Entities.TransactionStatus.Completed)
            .AverageAsync(t => t.Amount);

        return Result<decimal>.Ok(avg);
    }

    // 13. Self transfer clients
    public async Task<Result<List<SelfTransferClientDto>>> GetSelfTransferClientsAsync()
    {
        var data = await _context.Transactions
            .Where(t => t.FromAccount.ClientId == t.ToAccount.ClientId)
            .Select(t => new SelfTransferClientDto
            {
                ClientId = t.FromAccount.ClientId,
                FirstName = t.FromAccount.Client.FirstName,
                LastName = t.FromAccount.Client.LastName
            })
            .Distinct()
            .ToListAsync();

        return Result<List<SelfTransferClientDto>>.Ok(data);
    }

    // 14. Busiest day
    public async Task<Result<BusiestDayDto>> GetBusiestDayAsync()
    {
        var data = await _context.Transactions
            .Where(t => t.Status == Domain.Entities.TransactionStatus.Completed)
            .GroupBy(t => t.Timestamp.Date)
            .Select(g => new BusiestDayDto
            {
                Date = g.Key,
                TotalAmount = g.Sum(x => x.Amount)
            })
            .OrderByDescending(x => x.TotalAmount)
            .FirstOrDefaultAsync();

        if (data == null)
        {
            return Result<BusiestDayDto>.Fail("No transactions found", ErrorType.NotFound);
        }

        return Result<BusiestDayDto>.Ok(data);
    }

    // 15. Deposit only accounts
    public async Task<Result<List<DepositOnlyAccountDto>>> GetDepositOnlyAccountsAsync()
    {
        var data = await _context.Accounts
            .Where(a =>
                a.IncomingTransactions.Any(t => t.Type == TransactionType.Deposit) &&
                !a.OutgoingTransactions.Any(t => t.Type == TransactionType.Outgoing))
            .Select(a => new DepositOnlyAccountDto
            {
                AccountId = a.Id,
                AccountNumber = a.AccountNumber
            })
            .ToListAsync();

        return Result<List<DepositOnlyAccountDto>>.Ok(data);
    }
}