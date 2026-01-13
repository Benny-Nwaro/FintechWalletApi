using FintechWalletApi.Admin.DTOs;
using FintechWalletApi.Data;

namespace FintechWalletApi.Admin;

public class AdminReportService : IAdminReportService
{
    private readonly AppDbContext _context;

    public AdminReportService(AppDbContext context)
    {
        _context = context;
    }

    public DailyVolumeDto GetDailyVolume(DateTime date)
    {
        var start = date.Date;
        var end = start.AddDays(1);

        var transactions = _context.Transactions
            .Where(t => t.CreatedAt >= start && t.CreatedAt < end);

        return new DailyVolumeDto
        {
            Date = start,
            TotalAmount = transactions.Sum(t => Math.Abs(t.Amount)),
            TransactionCount = transactions.Count()
        };
    }

    public List<TopWalletDto> GetTopWallets(int top)
    {
        return _context.Transactions
            .GroupBy(t => t.WalletId)
            .Select(g => new TopWalletDto
            {
                WalletId = g.Key,
                TotalVolume = g.Sum(x => Math.Abs(x.Amount)),
                TransactionCount = g.Count()
            })
            .OrderByDescending(x => x.TotalVolume)
            .Take(top)
            .ToList();
    }

    public FraudSummaryDto GetFraudSummary(DateTime from, DateTime to)
    {
        var fraudEvents = _context.AuditLogs
            .Where(a =>
                a.Action == "FraudRuleTriggered" &&
                a.CreatedAt >= from &&
                a.CreatedAt <= to);

        return new FraudSummaryDto
        {
            FromDate = from,
            ToDate = to,
            TotalFraudEvents = fraudEvents.Count()
        };
    }
}
