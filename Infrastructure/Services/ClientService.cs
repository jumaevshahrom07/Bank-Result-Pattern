using Domain.Entities;
using Infrastructure.DTOs.ClientDto;
using Infrastructure.Interfaces;
using Infrastructure.Persistence.DataContext;
using Infrastructure.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class ClientService : IClientService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ClientService> _logger;

    public ClientService(AppDbContext context, ILogger<ClientService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<ClinetGetDto>> GetAllAsync(ClientFilterDto dto)
    {
        var query = _context.Clients.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(dto.FirstName))
        {
            query = query.Where(x => x.FirstName.ToLower().Contains(dto.FirstName.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(dto.LastName))
        {
            query = query.Where(x => x.LastName.ToLower().Contains(dto.LastName.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            query = query.Where(x => x.Email.ToLower().Contains(dto.Email.ToLower()));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((dto.Page - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .Select(x => new ClinetGetDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                BirthDate = x.BirthDate,
                IsActive = x.IsActive
            })
            .ToListAsync();

        return PagedResult<ClinetGetDto>.Ok(items, totalCount, dto.Page, dto.PageSize);
    }


    public async Task<Result<ClinetGetDto>> CreateAsync(ClientCreateDto dto)
    {
        try
        {
            if (dto.FirstName is { Length: > 50 })
            {
                return Result<ClinetGetDto>.Fail("First name is required", ErrorType.Validation);
            }

            if (dto.LastName is { Length: > 50 })
            {
                return Result<ClinetGetDto>.Fail("Last name is required", ErrorType.Validation);
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return Result<ClinetGetDto>.Fail("Email is required", ErrorType.Validation);
            }

            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                return Result<ClinetGetDto>.Fail("PhoneNumber is required", ErrorType.Validation);
            }

            if (string.IsNullOrWhiteSpace(dto.PasswordHash))
            {
                return Result<ClinetGetDto>.Fail("Password is required", ErrorType.Validation);
            }

            var email = dto.Email.Trim().ToLower();
            var phone = dto.PhoneNumber.Trim();

            var exists = await _context.Clients
                .AnyAsync(x => x.Email.ToLower() == email || x.PhoneNumber == phone);

            if (exists)
            {
                return Result<ClinetGetDto>.Fail("Client already exists", ErrorType.Conflict);
            }

            var client = new Client
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = dto.PasswordHash,
                BirthDate = dto.BirthDate,
                IsActive = true
            };

            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();

            return Result<ClinetGetDto>.Ok(new ClinetGetDto
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber,
                BirthDate = client.BirthDate,
                IsActive = client.IsActive
            });
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Create Client error");
            return Result<ClinetGetDto>.Fail("Internal error");
        }
    }

    public async Task<Result<string>> UpdateAsync(Guid id, ClientUpdateDto dto)
    {
        if (dto.FirstName is { Length: > 50 })
        {
            return Result<string>.Fail("First name is required and length cannot be more than 50 characters", ErrorType.Validation);
        }

        if (dto.LastName is { Length: > 50 })
        {
            return Result<string>.Fail("Last name is required and length cannot be more than 50 characters", ErrorType.Validation);
        }

        if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            return Result<string>.Fail("PhoneNumber is required", ErrorType.Validation);
        }

        var client = await _context.Clients.FirstOrDefaultAsync(x => x.Id == id);

        if (client == null)
        {
            return Result<string>.Fail("Client not found", ErrorType.NotFound);
        }

        var noChange =
        client.FirstName.Trim().ToLower() == dto.FirstName.Trim().ToLower() &&
        client.LastName.Trim().ToLower() == dto.LastName.Trim().ToLower() &&
        client.PhoneNumber.Trim() == dto.PhoneNumber.Trim();

        if (noChange)
        {
            return Result<string>.Fail("Nothing changed", ErrorType.Validation);
        }

        var exist = await _context.Clients.AnyAsync(c =>
            c.Id != id &&
            c.FirstName.ToLower() == dto.FirstName.ToLower() &&
            c.LastName.ToLower() == dto.LastName.ToLower() &&
            c.PhoneNumber == dto.PhoneNumber);

        if (exist)
        {
            return Result<string>.Fail("Client wuth same data alradey exist", ErrorType.Conflict);
        }

        client.FirstName = dto.FirstName;
        client.LastName = dto.LastName;
        client.PhoneNumber = dto.PhoneNumber;

        await _context.SaveChangesAsync();
        return Result<string>.Ok("Updated");
    }

    public async Task<Result<string>> DeleteAsync(Guid id)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(x => x.Id == id);

        if (client == null)
        {
            return Result<string>.Fail("Client not found", ErrorType.NotFound);
        }

        client.IsActive = false;
        await _context.SaveChangesAsync();
        return Result<string>.Ok("Deactivated");
    }

    public async Task<Result<ClinetGetDto>> GetByIdAsync(Guid id)
    {
        var client = await _context.Clients
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new ClinetGetDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                PasswordHash = x.PasswordHash,
                BirthDate = x.BirthDate,
                IsActive = x.IsActive
            }).FirstOrDefaultAsync();

        if (client == null)
        {
            return Result<ClinetGetDto>.Fail("Client not found", ErrorType.NotFound);
        }

        return Result<ClinetGetDto>.Ok(client);
    }
}