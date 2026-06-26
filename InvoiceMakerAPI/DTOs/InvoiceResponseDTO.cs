namespace InvoiceMakerAPI.DTOs
{
    public class InvoiceResponseDTO
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = default!;
        public decimal TotalAmount { get; set; }
    }
}
