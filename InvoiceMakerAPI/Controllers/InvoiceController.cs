using InvoiceMakerAPI.DTOs;
using InvoiceMakerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceMakerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpPost("CreateInvoice")]
        public async Task<IActionResult> CreateInvoice(CreateInvoiceDTO dto)
        {
            var invoice = await _invoiceService.CreateInvoiceAsync(dto);
            if (invoice == null)
            {
                return NotFound();
            }
            return Ok(invoice);
        }

        [HttpGet("GenerateInvoicePdf/{invoiceId}")]
        public async Task<IActionResult> GenerateInvoicePdf(Guid invoiceId)
        {
            var pdfBytes = _invoiceService.GenerateInvoicePdfAsync(invoiceId);
            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                return NotFound();
            }
            return File(pdfBytes, "application/pdf", $"Invoice_{invoiceId}.pdf");
        }

        [HttpGet("GetInvoiceById/{id}")]
        public async Task<IActionResult> GetInvoiceById(Guid id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            return Ok(invoice);
        }

        [HttpGet("GetAllInvoices")]
        public async Task<IActionResult> GetAllInvoices()
        {
            var invoice = await _invoiceService.GetAllInvoicesAsync();

            if (invoice == null)
            {
                return NotFound();
            }
            return Ok(invoice);
        }

        [HttpPut("UpdateInvoice")]
        public async Task<IActionResult> UpdateInvoice(UpdateInvoiceDTO dto)
        {
            var invoice = await _invoiceService.UpdateInvoiceAsync(dto);
            if (invoice == null)
            {
                return NotFound();
            }
            return Ok(invoice);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var invoice = await _invoiceService.DeleteInvoiceAsync(id);

            if (!invoice)
                return NotFound();

            return Ok(invoice);
        }
    }
}
