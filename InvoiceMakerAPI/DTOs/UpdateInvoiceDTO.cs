using Data.Enums;

namespace InvoiceMakerAPI.DTOs
{
    public class UpdateInvoiceDTO
    {
        public Guid Id { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string ClientAddress { get; set; }
        public DateTime DueDate { get; set; }
    }
}
