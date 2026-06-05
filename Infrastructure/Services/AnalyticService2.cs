using Domain.Entities;
using Infrastructure.AnaluticDto2;
using Infrastructure.Interfaces;
using Infrastructure.Persistence.DataContext;
using Infrastructure.Results;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class AnalyticService2 : IAnalyticService2
{
    private readonly AppDbContext _context;

    public AnalyticService2(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> CreateTransferAsync(TransferCreateDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        var from = await _context.Accounts
            .FirstOrDefaultAsync(x => x.Id == dto.FromAccountId);

        var to = await _context.Accounts
            .FirstOrDefaultAsync(x => x.Id == dto.ToAccountId);

        if (from == null || to == null)
        {
            return Result<bool>.Fail("Account not found", ErrorType.NotFound);
        }

        if (dto.FromAccountId == dto.ToAccountId)
        {
            return Result<bool>.Fail("Cannot transfcer to same account", ErrorType.Validation);
        }

        if (from.Balance < dto.Amount)
        {
            return Result<bool>.Fail("Insufficient funds", ErrorType.Validation);
        }

        decimal fee = from.ClientId == to.ClientId ? 0 : dto.Amount * 0.01m;

        decimal totalDebit = dto.Amount + fee;

        if (from.Balance < totalDebit)
        {
            return Result<bool>.Fail("Insufficient funds for fee", ErrorType.Validation);
        }

        from.Balance -= totalDebit;
        to.Balance += dto.Amount;

        var transfer = new Transaction
        {
            Id = Guid.NewGuid(),
            FromAccountId = from.Id,
            ToAccountId = to.Id,
            Amount = dto.Amount,
            Fee = fee,
            Status = TransactionStatus.Completed,
            Type = TransactionType.Outgoing,
            Timestamp = DateTime.UtcNow,
            Description = "Transfer"
        };

        _context.Transactions.Add(transfer);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return Result<bool>.Ok(true);
    }
}