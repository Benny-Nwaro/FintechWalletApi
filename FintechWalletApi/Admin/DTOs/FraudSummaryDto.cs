namespace FintechWalletApi.Admin.DTOs{

public class FraudSummaryDto
{
    public int TotalFraudEvents { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}
}