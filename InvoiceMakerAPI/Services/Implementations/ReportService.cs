using Data.APIContext;
using Data.Enums;
using InvoiceMakerAPI.DTOs;
using InvoiceMakerAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvoiceMakerAPI.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly ApiDbContext _context;

        public ReportService(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDTO> GetDashboardSummary()
        {
            var invoices = await _context.Invoices.ToListAsync();

            var result = new DashboardSummaryDTO
                        {
                            TotalRevenue = invoices.Sum(i => i.TotalAmount),
                            PaidRevenue = invoices.Where(i => i.Status == InvoiceStatus.Paid)
                                                  .Sum(i => i.TotalAmount),
                            OutstandingRevenue = invoices.Where(i => i.Status != InvoiceStatus.Paid)
                                                         .Sum(i => i.TotalAmount),
                            TotalInvoices = invoices.Count,
                            PaidInvoices = invoices.Count(i => i.Status == InvoiceStatus.Paid)
                        };

            return result;
        }

        public async Task<List<StatusBreakdownDTO>> GetInvoiceStatusBreakdown()
        {
            var result = await _context.Invoices
                                .GroupBy(i => i.Status)
                                .Select(g => new StatusBreakdownDTO
                                {
                                    Status = g.Key.ToString(),
                                    Count = g.Count()
                                })
                                .ToListAsync();
            return result;
        }

        public async Task<List<MonthlyRevenueDTO>> GetMonthlyRevenue(int year)
        {
            var result = await _context.Invoices
                               .Where(i => i.IssueDate.Year == year && i.Status == InvoiceStatus.Paid)
                               .GroupBy(i => new {i.IssueDate.Year, i.IssueDate.Month})
                               .Select(g => new MonthlyRevenueDTO
                               {
                                   Year = g.Key.Year,
                                   Month = g.Key.Month,
                                   Revenue = g.Sum(i => i.TotalAmount)
                               })
                               .OrderBy(m => m.Month)
                               .ToListAsync();
            return result;
        }

        public async Task<List<TopClientDTO>> GetTopClients(int top = 5)
        {
            var result = await _context.Invoices
                                .Where(i => i.Status == InvoiceStatus.Paid)
                                .GroupBy(i => i.ClientName)
                                .Select(g => new TopClientDTO
                                {
                                    ClientName = g.Key,
                                    TotalSpent = g.Sum(i => i.TotalAmount)
                                })
                                .OrderByDescending(c => c.TotalSpent)
                                .Take(top)
                                .ToListAsync();
            return result;
        }
    }
}
