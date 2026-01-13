namespace FintechWalletApi.Admin.DTOs{

public class DailyVolumeDto
{
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }
    public int TransactionCount { get; set; }
}
}