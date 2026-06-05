namespace Infrastructure.AnaluticDto2;

public class TransferCreateDto
{
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
    public decimal Amount { get; set; }
}