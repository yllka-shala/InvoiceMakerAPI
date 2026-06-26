using InvoiceMakerAPI.DTOs;

namespace InvoiceMakerAPI.Services.Interfaces
{
    public interface IReportService
    {
        Task<DashboardSummaryDTO> GetDashboardSummary();
        Task<List<MonthlyRevenueDTO>> GetMonthlyRevenue(int year);
        Task<List<StatusBreakdownDTO>> GetInvoiceStatusBreakdown();
        Task<List<TopClientDTO>> GetTopClients(int top = 5);
    }
}
