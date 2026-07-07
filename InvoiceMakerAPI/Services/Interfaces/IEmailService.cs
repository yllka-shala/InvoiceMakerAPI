namespace InvoiceMakerAPI.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, byte[] pdfBytes);
    }
}
