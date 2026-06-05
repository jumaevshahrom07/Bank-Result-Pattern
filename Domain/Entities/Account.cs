using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Account
{
    [Key]
    public Guid Id { get; set; }

    public Guid ClientId { get; set; }

    [Required, MaxLength(20)]
    public string AccountNumber { get; set; } = string.Empty;

    public decimal Balance { get; set; }

    [MaxLength(3)]
    public string Currency { get; set; } = "USD";

    public AccountType Type { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Client Client { get; set; } = null!;

    public List<Transaction> OutgoingTransactions { get; set; } = new();
    public List<Transaction> IncomingTransactions { get; set; } = new();
}