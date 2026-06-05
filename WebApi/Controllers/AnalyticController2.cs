using Infrastructure.AnaluticDto2;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/transfers")]
public class AnalyticController2 : ControllerBase
{
    private readonly IAnalyticService2 _service;

    public AnalyticController2(IAnalyticService2 service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TransferCreateDto dto)
    {
        var result = await _service.CreateTransferAsync(dto);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}