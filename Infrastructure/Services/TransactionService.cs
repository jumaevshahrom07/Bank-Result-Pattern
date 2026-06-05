using Domain.Entities;
using Infrastructure.DTOs.ClientDto;
using Infrastructure.DTOs.TransactionDto;
using Infrastructure.Interfaces;
using Infrastructure.Persistence.DataContext;
using Infrastructure.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(AppDbContext context, ILogger<TransactionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<Guid>> TransferAsync(TransactionCreateDto dto)
    {
        var from = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == dto.FromAccountId);
        var to = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == dto.ToAccountId);

        if (from == null || to == null)
        {
            return Result<Guid>.Fail("Account not found", ErrorType.NotFound);
        }

        if (dto.FromAccountId == dto.ToAccountId)
        {
            return Result<Guid>.Fail("Cannot transfer to same account", ErrorType.Validation);
        }

        decimal fee = from.ClientId == to.ClientId ? 0 : dto.Amount * 0.01m;

        var total = dto.Amount = fee;
        if (from.Balance < dto.Amount)
        {
            return Result<Guid>.Fail("Not enough balance", ErrorType.Validation);
        }

        from.Balance -= total;
        to.Balance += dto.Amount;

        var tx = new Transaction
        {
            Id = Guid.NewGuid(),
            FromAccountId = from.Id,
            ToAccountId = to.Id,
            Amount = dto.Amount,
            Fee = fee,
            Status = TransactionStatus.Completed,
            Description = dto.Description,
            Timestamp = DateTime.UtcNow
        };

        await _context.Transactions.AddAsync(tx);
        await _context.SaveChangesAsync();

        return Result<Guid>.Ok(tx.Id);
    }

    public async Task<Result<List<TransactionGetDto>>> GetAllAsync()
    {
        var data = await _context.Transactions
            .AsNoTracking()
            .Select(x => new TransactionGetDto
            {
                Id = x.Id,
                FromAccountId = x.FromAccountId,
                ToAccountId = x.ToAccountId,
                Amount = x.Amount,
                Fee = x.Fee,
                Status = x.Status,
                Description = x.Description,
                Timestamp = x.Timestamp
            }).ToListAsync();

        return Result<List<TransactionGetDto>>.Ok(data);
    }

    public async Task<Result<TransactionGetDto>> GetByIdAsync(Guid id)
    {
        var tx = await _context.Transactions.FirstOrDefaultAsync(x => x.Id == id);

        if (tx == null)
        {
            return Result<TransactionGetDto>.Fail("Not found", ErrorType.NotFound);
        }

        return Result<TransactionGetDto>.Ok(new TransactionGetDto
        {
            Id = tx.Id,
            FromAccountId = tx.FromAccountId,
            ToAccountId = tx.ToAccountId,
            Amount = tx.Amount,
            Fee = tx.Fee,
            Status = tx.Status,
            Description = tx.Description,
            Timestamp = tx.Timestamp
        });
    }

    public async Task<Result<List<TransactionGetDto>>> GetByAccountIdAsync(Guid accountId)
    {
        var data = await _context.Transactions
            .Where(x => x.FromAccountId == accountId || x.ToAccountId == accountId)
            .Select(x => new TransactionGetDto
            {
                Id = x.Id,
                FromAccountId = x.FromAccountId,
                ToAccountId = x.ToAccountId,
                Amount = x.Amount,
                Fee = x.Fee,
                Status = x.Status,
                Description = x.Description,
                Timestamp = x.Timestamp
            }).ToListAsync();

        return Result<List<TransactionGetDto>>.Ok(data);
    }
}