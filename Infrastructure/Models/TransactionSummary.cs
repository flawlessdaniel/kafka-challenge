namespace Infrastructure.Models;

public class TransactionSummary
{
    public Guid AccountId { get; set; }
    public float Accumulated { get; set; }
    public string Date { get; set; }
}