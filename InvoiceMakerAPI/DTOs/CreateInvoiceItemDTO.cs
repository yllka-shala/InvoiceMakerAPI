namespace InvoiceMakerAPI.DTOs
{
    public class CreateInvoiceItemDTO
    {
        public string Description { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
