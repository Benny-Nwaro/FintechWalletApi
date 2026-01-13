using FintechWalletApi.Admin.DTOs;

namespace FintechWalletApi.Admin;

public interface IAdminReportService
{
    DailyVolumeDto GetDailyVolume(DateTime date);
    List<TopWalletDto> GetTopWallets(int top);
    FraudSummaryDto GetFraudSummary(DateTime from, DateTime to);
}
