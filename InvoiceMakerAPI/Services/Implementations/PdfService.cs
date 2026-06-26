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
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    // Header
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text("YOUR COMPANY")
                                .FontSize(24)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            column.Item().Text("Company Address");
                            column.Item().Text("company@email.com");
                            column.Item().Text("+1 (555) 123-4567");
                        });

                        row.ConstantItem(220).AlignRight().Column(column =>
                        {
                            column.Item().AlignRight().Text("INVOICE")
                                .FontSize(28)
                                .Bold();

                            column.Item().AlignRight().Text($"Invoice #: {invoice.InvoiceNumber}");
                            column.Item().AlignRight().Text($"Issue Date: {invoice.IssueDate:d}");
                            column.Item().AlignRight().Text($"Due Date: {invoice.DueDate:d}");
                        });
                    });

                    page.Content().PaddingTop(25).Column(column =>
                    {
                        // Client information
                        column.Item()
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(12)
                            .Column(client =>
                            {
                                client.Item().Text("Bill To")
                                    .Bold()
                                    .FontSize(12);

                                client.Item().Text(invoice.ClientName);
                                client.Item().Text(invoice.ClientEmail);
                                client.Item().Text(invoice.ClientAddress);
                            });

                        column.Item().PaddingTop(25);

                        // Items
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4);
                                columns.ConstantColumn(60);
                                columns.ConstantColumn(90);
                                columns.ConstantColumn(90);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCell).Text("Description");
                                header.Cell().Element(HeaderCell).AlignCenter().Text("Qty");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Unit Price");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Total");
                            });

                            foreach (var item in invoice.Items)
                            {
                                table.Cell().Element(BodyCell).Text(item.Description);
                                table.Cell().Element(BodyCell).AlignCenter().Text(item.Quantity.ToString());
                                table.Cell().Element(BodyCell).AlignRight().Text(item.UnitPrice.ToString("C"));
                                table.Cell().Element(BodyCell).AlignRight()
                                    .Text((item.Quantity * item.UnitPrice).ToString("C"));
                            }
                        });

                        column.Item().PaddingTop(20);

                        // Total section (uses your existing value)
                        column.Item()
                            .AlignRight()
                            .Width(220)
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(10)
                            .Row(row =>
                            {
                                row.RelativeItem().Text("Total Amount").Bold();

                                row.ConstantItem(90)
                                    .AlignRight()
                                    .Text(invoice.TotalAmount.ToString("C"))
                                    .Bold()
                                    .FontSize(12);
                            });

                        column.Item().PaddingTop(35);

                        column.Item()
                            .BorderTop(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .PaddingTop(10)
                            .Text("Thank you for your business!")
                            .Italic()
                            .FontColor(Colors.Grey.Darken1);
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                            text.Span(" of ");
                            text.TotalPages();
                        });
                });

                static IContainer HeaderCell(IContainer container)
                {
                    return container
                        .Background(Colors.Blue.Darken2)
                        .PaddingVertical(8)
                        .PaddingHorizontal(6)
                        .DefaultTextStyle(x => x.FontColor(Colors.White).Bold());
                }

                static IContainer BodyCell(IContainer container)
                {
                    return container
                        .BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .PaddingVertical(8)
                        .PaddingHorizontal(6);
                }
            })
            .GeneratePdf(stream);

            return stream.ToArray();
        }
    }
}
