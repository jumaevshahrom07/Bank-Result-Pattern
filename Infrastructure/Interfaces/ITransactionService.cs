using Infrastructure.DTOs.ClientDto;
using Infrastructure.DTOs.TransactionDto;
using Infrastructure.Results;

namespace Infrastructure.Interfaces;

public interface ITransactionService
{
    Task<Result<List<TransactionGetDto>>> GetAllAsync();
    Task<Result<Guid>> TransferAsync(TransactionCreateDto dto);
    Task<Result<TransactionGetDto>> GetByIdAsync(Guid id);
    Task<Result<List<TransactionGetDto>>> GetByAccountIdAsync(Guid accountId);
}