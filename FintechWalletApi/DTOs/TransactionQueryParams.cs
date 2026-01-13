namespace FintechWalletApi.DTOs;

public class TransactionQueryParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? Type { get; set; }          // Credit, Debit, Transfer
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
}
