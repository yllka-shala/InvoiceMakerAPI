namespace InvoiceMakerAPI.DTOs
{
    public class CreateInvoiceDTO
    {
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string ClientAddress { get; set; }
        public DateTime DueDate { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal TaxRate { get; set; }
        public List<CreateInvoiceItemDTO> Items { get; set; } = new();
    }
}
