using Data.APIContext;
using Data.Enums;
using Data.Models;
using InvoiceMakerAPI.DTOs;
using InvoiceMakerAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvoiceMakerAPI.Services.Implementations
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApiDbContext _context;
        private readonly IPdfService _pdfService;
        private readonly IEmailService _emailService;

        public InvoiceService(ApiDbContext context, IPdfService pdfService, IEmailService emailService)
        {
            _context = context;
            _pdfService = pdfService;
            _emailService = emailService;
        }

        public async Task<InvoiceResponseDTO> CreateInvoiceAsync(CreateInvoiceDTO invoiceDto)
        {
            var invoice = new Invoice
            {
                InvoiceNumber = $"INV-{DateTime.Now.Ticks}",
                ClientName = invoiceDto.ClientName,
                ClientEmail = invoiceDto.ClientEmail,
                ClientAddress = invoiceDto.ClientAddress,
                IssueDate = DateTime.Now,
                DueDate = invoiceDto.DueDate,
                Currency = invoiceDto.Currency,
                Items = invoiceDto.Items.Select(i => new InvoiceItem
                {
                    Description = i.Description,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            invoice.Subtotal = invoice.Items.Sum(i => i.TotalPrice);
            invoice.DiscountAmount = invoice.Subtotal * (invoiceDto.DiscountRate / 100);
            var afterDiscount = invoice.Subtotal - invoice.DiscountAmount;
            invoice.TaxAmount = afterDiscount * (invoiceDto.TaxRate / 100);
            invoice.TotalAmount = afterDiscount + invoice.TaxAmount;

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return new InvoiceResponseDTO
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                TotalAmount = invoice.TotalAmount
            };
        }

        public byte[] GenerateInvoicePdfAsync(Guid invoiceId)
        {
            var invoice = _context.Invoices
                                .AsNoTracking()
                                .Include(i => i.Items)
                                .FirstOrDefault(i => i.Id == invoiceId);

            var pdfBytes = _pdfService.GenerateInvoicePdf(invoice);

            if (invoice.ClientEmail is not null)
                _emailService.SendEmailAsync(invoice.ClientEmail, pdfBytes);
            
            return pdfBytes;
        }

        public async Task<bool> DeleteInvoiceAsync(Guid invoiceId)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceId);
            if (invoice is null)
                return false;

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<InvoiceResponseDTO>> GetAllInvoicesAsync()
        {
            var invoices = await _context.Invoices
                                    .AsNoTracking()
                                    .Include(i => i.Items)
                                    .ToListAsync();

            return invoices.Select(invoice => new InvoiceResponseDTO
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                TotalAmount = invoice.TotalAmount
            });
        }

        public async Task<InvoiceResponseDTO> GetInvoiceByIdAsync(Guid invoiceId)
        {
            var invoices = await _context.Invoices
                                    .AsNoTracking()
                                    .Include(i => i.Items)
                                    .FirstOrDefaultAsync(i => i.Id == invoiceId);


            if (invoices is null)
                return null;

            return new InvoiceResponseDTO
            {
                Id = invoices.Id,
                InvoiceNumber = invoices.InvoiceNumber,
                TotalAmount = invoices.TotalAmount
            };
        }

        public async Task<InvoiceResponseDTO> UpdateInvoiceAsync(UpdateInvoiceDTO invoice)
        {

            var existingInvoice = await _context.Invoices
                                    .Include(i => i.Items)
                                    .FirstOrDefaultAsync(i => i.Id == invoice.Id);

            if (existingInvoice is null)
                return null;

            existingInvoice.ClientName = invoice.ClientName;
            existingInvoice.ClientEmail = invoice.ClientEmail;
            existingInvoice.ClientAddress = invoice.ClientAddress;
            existingInvoice.DueDate = invoice.DueDate;

            _context.Invoices.Update(existingInvoice);
            await _context.SaveChangesAsync();

            return new InvoiceResponseDTO
            {
                Id = existingInvoice.Id,
                InvoiceNumber = existingInvoice.InvoiceNumber,
                TotalAmount = existingInvoice.TotalAmount
            };
        }
    }
}
