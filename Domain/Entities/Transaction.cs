using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public decimal Fee { get; set; }
    public TransactionStatus Status { get; set; }
    public TransactionType Type { get; set; }
    public string? Description { get; set; }
    public DateTime Timestamp { get; set; }
    public Account FromAccount { get; set; } = null!;
    public Account ToAccount { get; set; } = null!;
}