namespace InvoiceMakerAPI.DTOs
{
    public class DashboardSummaryDTO
    {
        public decimal TotalRevenue { get; set; }
        public decimal PaidRevenue { get; set; }
        public decimal OutstandingRevenue { get; set; }
        public int TotalInvoices { get; set; }
        public int PaidInvoices { get; set; }

    }
}
