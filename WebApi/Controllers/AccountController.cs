using Infrastructure.DTOs.AccountDto;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountController : BaseController
{
    private readonly IAccountService _service;

    public AccountController(IAccountService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return result.IsSuccess ? Ok(result.Data) : HandleError(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AccountCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return result.IsSuccess ? Ok(result.Data) : HandleError(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Data) : HandleError(result);
    }

    [HttpGet("client/{clientId:guid}")]
    public async Task<IActionResult> GetByClient(Guid clientId)
    {
        var result = await _service.GetByClientIdAsync(clientId);
        return result.IsSuccess ? Ok(result.Data) : HandleError(result);
    }

    [HttpPut("{id:guid}/close")]
    public async Task<IActionResult> Close(Guid id)
    {
        var result = await _service.CloseAccountAsync(id);
        return result.IsSuccess ? Ok(result.Data) : HandleError(result);
    }
}