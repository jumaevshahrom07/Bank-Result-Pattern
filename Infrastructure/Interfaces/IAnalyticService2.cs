using Infrastructure.AnaluticDto2;
using Infrastructure.Results;

namespace Infrastructure.Interfaces;

public interface IAnalyticService2
{
    Task<Result<bool>> CreateTransferAsync(TransferCreateDto dto);
}