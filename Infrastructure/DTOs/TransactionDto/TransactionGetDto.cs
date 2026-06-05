using Domain.Entities;
namespace Infrastructure.DTOs.TransactionDto;

public class TransactionGetDto
{
    public Guid Id { get; set; }
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public decimal Fee { get; set; }
    public TransactionStatus Status { get; set; }
    public string? Description { get; set; }
    public DateTime Timestamp { get; set; }
}