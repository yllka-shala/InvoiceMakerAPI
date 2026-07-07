using InvoiceMakerAPI.DTOs;

namespace InvoiceMakerAPI.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceResponseDTO> CreateInvoiceAsync(CreateInvoiceDTO invoice);
        Task<InvoiceResponseDTO> UpdateInvoiceAsync(UpdateInvoiceDTO invoice);
        Task<InvoiceResponseDTO> GetInvoiceByIdAsync(Guid invoiceId);
        Task<IEnumerable<InvoiceResponseDTO>> GetAllInvoicesAsync();
        Task<bool> DeleteInvoiceAsync(Guid invoiceId);
        byte[] GenerateInvoicePdfAsync(Guid invoiceId);
    }
}
