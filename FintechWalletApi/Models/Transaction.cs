namespace FintechWalletApi.Models;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid WalletId { get; set; }

    public decimal Amount { get; set; }   
    public string Type { get; set; } = null!; 
    public string Description { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
