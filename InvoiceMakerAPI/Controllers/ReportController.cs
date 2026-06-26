using InvoiceMakerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceMakerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var result = await _reportService.GetDashboardSummary();
            return Ok(result);
        }

        [HttpGet("monthly/{year}")]
        public async Task<IActionResult> GetMonthly(int year)
        {
            var result = await _reportService.GetMonthlyRevenue(year);
            return Ok(result);
        }

        [HttpGet("status-breakdown")]
        public async Task<IActionResult> GetStatusBreakdown()
        {
            var result = await _reportService.GetInvoiceStatusBreakdown();
            return Ok(result);
        }

        [HttpGet("top-clients/{top}")]
        public async Task<IActionResult> GetTopClients(int top = 5)
        {
            var result = await _reportService.GetTopClients(top);
            return Ok(result);
        }
    }
}
