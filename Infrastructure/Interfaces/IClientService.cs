using Infrastructure.DTOs.ClientDto;
using Infrastructure.Results;

namespace Infrastructure.Interfaces;

public interface IClientService
{
Task<PagedResult<ClinetGetDto>> GetAllAsync(ClientFilterDto dto);    Task<Result<ClinetGetDto>> CreateAsync(ClientCreateDto dto);
    Task<Result<string>> UpdateAsync(Guid id, ClientUpdateDto dto);
    Task<Result<string>> DeleteAsync(Guid id);
    Task<Result<ClinetGetDto>> GetByIdAsync(Guid id);
}