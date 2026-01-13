namespace FintechWalletApi.Admin.DTOs;

public class TopWalletDto
{
    public Guid WalletId { get; set; }
    public decimal TotalVolume { get; set; }
    public int TransactionCount { get; set; }
}
