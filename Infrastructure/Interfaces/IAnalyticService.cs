using Infrastructure.DTOs.AccountDto;
using Infrastructure.DTOs.AnalyticsDto;
using Infrastructure.DTOs.ClientDto;
using Infrastructure.DTOs.TransactionDto;
using Infrastructure.Results;


namespace Infrastructure.Interfaces;

public interface IAnalyticService
{
    Task<Result<List<ActiveClientDto>>> GetActiveClientsAsync();
    Task<Result<List<ClientAccountDto>>> GetClientAccountsByEmailAsync(string email);
    Task<Result<List<TransactionGetDto>>> GetAccountTransfersAsync(Guid accountId);
    Task<Result<decimal>> GetClientBalanceAsync(string email);
    Task<Result<List<LowBalanceAccountDto>>> GetLowBalanceAccountsAsync(decimal threshold);
    Task<Result<List<AccountOwnerDto>>> GetAccountsWithOwnersAsync();
    Task<Result<RichestClientDto>> GetRichestClientAsync();
    Task<Result<List<TopOutgoingAccountDto>>> GetTopOutgoingAccountsAsync(int limit);
    Task<Result<List<ClientStatsDto>>> GetClientStatsAsync();
    Task<Result<List<HighOutgoingAccountDto>>> GetHighOutgoingAccountsAsync(decimal amount);
    Task<Result<decimal>> GetTotalFeesAsync();
    Task<Result<decimal>> GetAverageTransferAsync();
    Task<Result<List<SelfTransferClientDto>>> GetSelfTransferClientsAsync();
    Task<Result<BusiestDayDto>> GetBusiestDayAsync();
    Task<Result<List<DepositOnlyAccountDto>>> GetDepositOnlyAccountsAsync();
}