using Data.Models;
using InvoiceMakerAPI.Services.Interfaces;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace InvoiceMakerAPI.Services.Implementations
{
    public class PdfService : IPdfService
    {
        public byte[] GenerateInvoicePdf(Invoice invoice)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            using var stream = new MemoryStream();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.Header().Text($"Invoice: {invoice.InvoiceNumber}").FontSize(20).Bold();
                    page.Content().Column(column =>
                    {
                        column.Item().Text($"Client: {invoice.ClientName}");
                        column.Item().Text($"Email: {invoice.ClientEmail}");
                        column.Item().Text($"Address: {invoice.ClientAddress}");
                        column.Item().Text($"Issue Date: {invoice.IssueDate:d}");
                        column.Item().Text($"Due Date: {invoice.DueDate:d}");
                        column.Item().Text($"Total Amount: {invoice.TotalAmount:C}");

                        foreach (var item in invoice.Items)
                        {
                            column.Item().Text(item.Description);
                            column.Item().Text(item.Quantity.ToString());
                            column.Item().Text(item.UnitPrice.ToString("C"));
                            column.Item().Text((item.Quantity * item.UnitPrice).ToString("C"));
                        }
                    });

                });
            }).GeneratePdf(stream);

            return stream.ToArray();
        }
    }
}
