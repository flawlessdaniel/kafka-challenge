namespace Infrastructure.Models;

public class Transaction
{
    public Transaction()
    {
        
    }

    public Guid Id { get; set; }
    public Guid TargetAccountId { get; set; }
    public Guid SourceAccountId { get; set; }
    public int TransferTypeId { get; set; }
    public float Value { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}