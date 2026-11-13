using InvoiceMakerAPI.Controllers;
using InvoiceMakerAPI.DTOs;
using InvoiceMakerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace InvoiceMakerAPI.Tests.ControllersTests
{
    public class ReportControllerTests
    {
        private readonly Mock<IReportService> _reportServiceMock = new();

        [Fact]
        public async Task GetSummary_ReturnsOkWithSummary()
        {
            var summary = new DashboardSummaryDTO
            {
                TotalRevenue = 2500,
                PaidRevenue = 1500,
                OutstandingRevenue = 1000,
                TotalInvoices = 8,
                PaidInvoices = 5
            };
            _reportServiceMock.Setup(service => service.GetDashboardSummary()).ReturnsAsync(summary);

            var response = await CreateController().GetSummary();

            var okResult = Assert.IsType<OkObjectResult>(response);
            Assert.Same(summary, okResult.Value);
            Assert.Equal(2500, summary.TotalRevenue);
            Assert.Equal(5, summary.PaidInvoices);
            _reportServiceMock.Verify(service => service.GetDashboardSummary(), Times.Once);
        }

        [Fact]
        public async Task GetSummary_PropagatesServiceFailure()
        {
            var exception = new InvalidOperationException("Summary service failed");
            _reportServiceMock.Setup(service => service.GetDashboardSummary()).ThrowsAsync(exception);

            var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(
                () => CreateController().GetSummary());

            Assert.Same(exception, thrownException);
            _reportServiceMock.Verify(service => service.GetDashboardSummary(), Times.Once);
        }

        [Fact]
        public async Task GetMonthly_ReturnsOkWithRevenueForRequestedYear()
        {
            const int year = 2026;
            var monthlyRevenue = new List<MonthlyRevenueDTO>
            {
                new() { Year = year, Month = 1, Revenue = 1200 },
                new() { Year = year, Month = 2, Revenue = 1850 }
            };
            _reportServiceMock.Setup(service => service.GetMonthlyRevenue(year)).ReturnsAsync(monthlyRevenue);

            var response = await CreateController().GetMonthly(year);

            var okResult = Assert.IsType<OkObjectResult>(response);
            Assert.Same(monthlyRevenue, okResult.Value);
            Assert.Equal(2, monthlyRevenue.Count);
            Assert.Equal(1850, monthlyRevenue[1].Revenue);
            _reportServiceMock.Verify(service => service.GetMonthlyRevenue(year), Times.Once);
        }

        [Fact]
        public async Task GetMonthly_PropagatesServiceFailure()
        {
            const int year = 2026;
            var exception = new InvalidOperationException("Monthly revenue service failed");
            _reportServiceMock.Setup(service => service.GetMonthlyRevenue(year)).ThrowsAsync(exception);

            var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(
                () => CreateController().GetMonthly(year));

            Assert.Same(exception, thrownException);
            _reportServiceMock.Verify(service => service.GetMonthlyRevenue(year), Times.Once);
        }

        [Fact]
        public async Task GetStatusBreakdown_ReturnsOkWithStatusCounts()
        {
            var statusBreakdown = new List<StatusBreakdownDTO>
            {
                new() { Status = "Paid", Count = 5 },
                new() { Status = "Pending", Count = 3 }
            };
            _reportServiceMock
                .Setup(service => service.GetInvoiceStatusBreakdown())
                .ReturnsAsync(statusBreakdown);

            var response = await CreateController().GetStatusBreakdown();

            var okResult = Assert.IsType<OkObjectResult>(response);
            Assert.Same(statusBreakdown, okResult.Value);
            Assert.Equal(5, statusBreakdown[0].Count);
            Assert.Equal("Pending", statusBreakdown[1].Status);
            _reportServiceMock.Verify(service => service.GetInvoiceStatusBreakdown(), Times.Once);
        }

        [Fact]
        public async Task GetStatusBreakdown_PropagatesServiceFailure()
        {
            var exception = new InvalidOperationException("Status breakdown service failed");
            _reportServiceMock
                .Setup(service => service.GetInvoiceStatusBreakdown())
                .ThrowsAsync(exception);

            var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(
                () => CreateController().GetStatusBreakdown());

            Assert.Same(exception, thrownException);
            _reportServiceMock.Verify(service => service.GetInvoiceStatusBreakdown(), Times.Once);
        }

        [Fact]
        public async Task GetTopClients_ReturnsOkWithRequestedNumberOfClients()
        {
            const int top = 2;
            var topClients = new List<TopClientDTO>
            {
                new() { ClientName = "Example Corp", TotalSpent = 4200 },
                new() { ClientName = "Sample LLC", TotalSpent = 3100 }
            };
            _reportServiceMock.Setup(service => service.GetTopClients(top)).ReturnsAsync(topClients);

            var response = await CreateController().GetTopClients(top);

            var okResult = Assert.IsType<OkObjectResult>(response);
            Assert.Same(topClients, okResult.Value);
            Assert.Equal("Example Corp", topClients[0].ClientName);
            Assert.Equal(3100, topClients[1].TotalSpent);
            _reportServiceMock.Verify(service => service.GetTopClients(top), Times.Once);
        }

        [Fact]
        public async Task GetTopClients_PropagatesServiceFailure()
        {
            const int top = 2;
            var exception = new InvalidOperationException("Top clients service failed");
            _reportServiceMock.Setup(service => service.GetTopClients(top)).ThrowsAsync(exception);

            var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(
                () => CreateController().GetTopClients(top));

            Assert.Same(exception, thrownException);
            _reportServiceMock.Verify(service => service.GetTopClients(top), Times.Once);
        }

        private ReportController CreateController() => new(_reportServiceMock.Object);
    }
}
