using Infrastructure.DTOs.AnalyticsDto;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController : BaseController
{
    private readonly IAnalyticService _service;

    public AnalyticsController(IAnalyticService service)
    {
        _service = service;
    }

    [HttpGet("active-clients")]
    public async Task<IActionResult> GetActiveClients()
        => HandleError(await _service.GetActiveClientsAsync());

    [HttpGet("client-accounts")]
    public async Task<IActionResult> GetClientAccounts([FromQuery] string email)
        => HandleError(await _service.GetClientAccountsByEmailAsync(email));

    [HttpGet("account-transfers/{accountId}")]
    public async Task<IActionResult> GetAccountTransfers(Guid accountId)
        => HandleError(await _service.GetAccountTransfersAsync(accountId));

    [HttpGet("client-balance")]
    public async Task<IActionResult> GetClientBalance([FromQuery] string email)
        => HandleError(await _service.GetClientBalanceAsync(email));

    [HttpGet("low-balance-accounts")]
    public async Task<IActionResult> GetLowBalanceAccounts([FromQuery] decimal threshold)
        => HandleError(await _service.GetLowBalanceAccountsAsync(threshold));

    [HttpGet("accounts-with-owners")]
    public async Task<IActionResult> GetAccountsWithOwners()
        => HandleError(await _service.GetAccountsWithOwnersAsync());

    [HttpGet("richest-client")]
    public async Task<IActionResult> GetRichestClient()
        => HandleError(await _service.GetRichestClientAsync());

    [HttpGet("top-outgoing-accounts")]
    public async Task<IActionResult> GetTopOutgoingAccounts([FromQuery] int limit = 5)
        => HandleError(await _service.GetTopOutgoingAccountsAsync(limit));

    [HttpGet("client-stats")]
    public async Task<IActionResult> GetClientStats()
        => HandleError(await _service.GetClientStatsAsync());

    [HttpGet("high-outgoing-accounts")]
    public async Task<IActionResult> GetHighOutgoingAccounts([FromQuery] decimal amount)
        => HandleError(await _service.GetHighOutgoingAccountsAsync(amount));

    [HttpGet("total-fees")]
    public async Task<IActionResult> GetTotalFees()
        => HandleError(await _service.GetTotalFeesAsync());

    [HttpGet("average-transfer")]
    public async Task<IActionResult> GetAverageTransfer()
        => HandleError(await _service.GetAverageTransferAsync());

    [HttpGet("self-transfer-clients")]
    public async Task<IActionResult> GetSelfTransferClients()
        => HandleError(await _service.GetSelfTransferClientsAsync());

    [HttpGet("busiest-day")]
    public async Task<IActionResult> GetBusiestDay()
        => HandleError(await _service.GetBusiestDayAsync());

    [HttpGet("deposit-only-accounts")]
    public async Task<IActionResult> GetDepositOnlyAccounts()
        => HandleError(await _service.GetDepositOnlyAccountsAsync());

}