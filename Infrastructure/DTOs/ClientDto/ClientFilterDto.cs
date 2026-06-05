using Infrastructure.Results;

namespace Infrastructure.DTOs.ClientDto;

public class ClientFilterDto : PagedRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
}