using Data.Models;

namespace InvoiceMakerAPI.Services.Interfaces
{
    public interface IPdfService
    {
        byte[] GenerateInvoicePdf(Invoice invoice);
    }
}
