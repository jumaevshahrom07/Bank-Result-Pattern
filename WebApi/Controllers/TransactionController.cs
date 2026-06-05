using Infrastructure.DTOs.ClientDto;
using Infrastructure.DTOs.TransactionDto;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionController : BaseController
{
    private readonly ITransactionService _service;

    public TransactionController(ITransactionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return result.IsSuccess ? Ok(result.Data) : HandleError(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Data) : HandleError(result);
    }

    [HttpGet("account/{accountId:guid}")]
    public async Task<IActionResult> GetByAccount(Guid accountId)
    {
        var result = await _service.GetByAccountIdAsync(accountId);
        return result.IsSuccess ? Ok(result.Data) : HandleError(result);
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransactionCreateDto dto)
    {
        var result = await _service.TransferAsync(dto);
        return result.IsSuccess ? Ok(result.Data) : HandleError(result);
    }
}