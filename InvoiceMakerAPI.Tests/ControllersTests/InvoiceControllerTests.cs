using Data.Enums;
using InvoiceMakerAPI.Controllers;
using InvoiceMakerAPI.DTOs;
using InvoiceMakerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace InvoiceMakerAPI.Tests.ControllersTests
{
    public class InvoiceControllerTests
    {
        private readonly Mock<IInvoiceService> _invoiceServiceMock = new();

        [Fact]
        public async Task CreateInvoice_ReturnsOk_WhenInvoiceIsCreated()
        {
            var request = new CreateInvoiceDTO
            {
                ClientName = "Example Client",
                ClientEmail = "client@example.com",
                ClientAddress = "123 Main Street",
                DueDate = new DateTime(2026, 12, 1),
                DiscountRate = 5,
                TaxRate = 10,
                Currency = Currency.USD,
                Items = [new CreateInvoiceItemDTO { Description = "Consulting", Quantity = 2, UnitPrice = 100 }]
            };
            var invoice = new InvoiceResponseDTO
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = "INV-001",
                TotalAmount = 209
            };
            _invoiceServiceMock.Setup(service => service.CreateInvoiceAsync(request)).ReturnsAsync(invoice);

            var response = await CreateController().CreateInvoice(request);

            var okResult = Assert.IsType<OkObjectResult>(response);
            Assert.Same(invoice, okResult.Value);
            _invoiceServiceMock.Verify(service => service.CreateInvoiceAsync(request), Times.Once);
        }

        [Fact]
        public async Task CreateInvoice_ReturnsNotFound_WhenInvoiceIsNotCreated()
        {
            var request = new CreateInvoiceDTO
            {
                ClientName = "Example Client",
                ClientEmail = "client@example.com",
                ClientAddress = "123 Main Street",
                DueDate = new DateTime(2026, 12, 1),
                Currency = Currency.USD
            };
            _invoiceServiceMock.Setup(service => service.CreateInvoiceAsync(request)).ReturnsAsync((InvoiceResponseDTO)null!);

            var response = await CreateController().CreateInvoice(request);

            Assert.IsType<NotFoundResult>(response);
            _invoiceServiceMock.Verify(service => service.CreateInvoiceAsync(request), Times.Once);
        }

        [Fact]
        public async Task GenerateInvoicePdf_ReturnsPdfFile_WhenPdfIsGenerated()
        {
            var invoiceId = Guid.NewGuid();
            byte[] pdfBytes = [37, 80, 68, 70];
            _invoiceServiceMock.Setup(service => service.GenerateInvoicePdfAsync(invoiceId)).Returns(pdfBytes);

            var response = await CreateController().GenerateInvoicePdf(invoiceId);

            var fileResult = Assert.IsType<FileContentResult>(response);
            Assert.Equal("application/pdf", fileResult.ContentType);
            Assert.Equal($"Invoice_{invoiceId}.pdf", fileResult.FileDownloadName);
            Assert.Equal(pdfBytes, fileResult.FileContents);
            _invoiceServiceMock.Verify(service => service.GenerateInvoicePdfAsync(invoiceId), Times.Once);
        }

        [Fact]
        public async Task GenerateInvoicePdf_ReturnsNotFound_WhenPdfIsEmpty()
        {
            var invoiceId = Guid.NewGuid();
            _invoiceServiceMock.Setup(service => service.GenerateInvoicePdfAsync(invoiceId)).Returns([]);

            var response = await CreateController().GenerateInvoicePdf(invoiceId);

            Assert.IsType<NotFoundResult>(response);
            _invoiceServiceMock.Verify(service => service.GenerateInvoicePdfAsync(invoiceId), Times.Once);
        }

        [Fact]
        public async Task GetInvoiceById_ReturnsOk_WhenInvoiceExists()
        {
            var invoiceId = Guid.NewGuid();
            var invoice = new InvoiceResponseDTO
            {
                Id = invoiceId,
                InvoiceNumber = "INV-002",
                TotalAmount = 350
            };
            _invoiceServiceMock.Setup(service => service.GetInvoiceByIdAsync(invoiceId)).ReturnsAsync(invoice);

            var response = await CreateController().GetInvoiceById(invoiceId);

            var okResult = Assert.IsType<OkObjectResult>(response);
            Assert.Same(invoice, okResult.Value);
            _invoiceServiceMock.Verify(service => service.GetInvoiceByIdAsync(invoiceId), Times.Once);
        }

        [Fact]
        public async Task GetInvoiceById_ReturnsNotFound_WhenInvoiceDoesNotExist()
        {
            var invoiceId = Guid.NewGuid();
            _invoiceServiceMock
                .Setup(service => service.GetInvoiceByIdAsync(invoiceId))
                .ReturnsAsync((InvoiceResponseDTO)null!);

            var response = await CreateController().GetInvoiceById(invoiceId);

            Assert.IsType<NotFoundResult>(response);
            _invoiceServiceMock.Verify(service => service.GetInvoiceByIdAsync(invoiceId), Times.Once);
        }

        [Fact]
        public async Task GetAllInvoices_ReturnsOk_WhenInvoicesExist()
        {
            InvoiceResponseDTO[] invoices =
            [
                new InvoiceResponseDTO { Id = Guid.NewGuid(), InvoiceNumber = "INV-003", TotalAmount = 125 },
                new InvoiceResponseDTO { Id = Guid.NewGuid(), InvoiceNumber = "INV-004", TotalAmount = 275 }
            ];
            _invoiceServiceMock.Setup(service => service.GetAllInvoicesAsync()).ReturnsAsync(invoices);

            var response = await CreateController().GetAllInvoices();

            var okResult = Assert.IsType<OkObjectResult>(response);
            Assert.Same(invoices, okResult.Value);
            _invoiceServiceMock.Verify(service => service.GetAllInvoicesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllInvoices_ReturnsNotFound_WhenNoInvoicesAreReturned()
        {
            _invoiceServiceMock
                .Setup(service => service.GetAllInvoicesAsync())
                .ReturnsAsync((IEnumerable<InvoiceResponseDTO>)null!);

            var response = await CreateController().GetAllInvoices();

            Assert.IsType<NotFoundResult>(response);
            _invoiceServiceMock.Verify(service => service.GetAllInvoicesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateInvoice_ReturnsOk_WhenInvoiceIsUpdated()
        {
            var request = new UpdateInvoiceDTO
            {
                Id = Guid.NewGuid(),
                ClientName = "Updated Client",
                ClientEmail = "updated@example.com",
                ClientAddress = "456 Oak Avenue",
                DueDate = new DateTime(2026, 12, 15)
            };
            var invoice = new InvoiceResponseDTO
            {
                Id = request.Id,
                InvoiceNumber = "INV-005",
                TotalAmount = 500
            };
            _invoiceServiceMock.Setup(service => service.UpdateInvoiceAsync(request)).ReturnsAsync(invoice);

            var response = await CreateController().UpdateInvoice(request);

            var okResult = Assert.IsType<OkObjectResult>(response);
            Assert.Same(invoice, okResult.Value);
            _invoiceServiceMock.Verify(service => service.UpdateInvoiceAsync(request), Times.Once);
        }

        [Fact]
        public async Task UpdateInvoice_ReturnsNotFound_WhenInvoiceDoesNotExist()
        {
            var request = new UpdateInvoiceDTO
            {
                Id = Guid.NewGuid(),
                ClientName = "Updated Client",
                ClientEmail = "updated@example.com",
                ClientAddress = "456 Oak Avenue",
                DueDate = new DateTime(2026, 12, 15)
            };
            _invoiceServiceMock
                .Setup(service => service.UpdateInvoiceAsync(request))
                .ReturnsAsync((InvoiceResponseDTO)null!);

            var response = await CreateController().UpdateInvoice(request);

            Assert.IsType<NotFoundResult>(response);
            _invoiceServiceMock.Verify(service => service.UpdateInvoiceAsync(request), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenInvoiceIsDeleted()
        {
            var invoiceId = Guid.NewGuid();
            _invoiceServiceMock.Setup(service => service.DeleteInvoiceAsync(invoiceId)).ReturnsAsync(true);

            var response = await CreateController().Delete(invoiceId);

            var okResult = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(true, okResult.Value);
            _invoiceServiceMock.Verify(service => service.DeleteInvoiceAsync(invoiceId), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenInvoiceDoesNotExist()
        {
            var invoiceId = Guid.NewGuid();
            _invoiceServiceMock.Setup(service => service.DeleteInvoiceAsync(invoiceId)).ReturnsAsync(false);

            var response = await CreateController().Delete(invoiceId);

            Assert.IsType<NotFoundResult>(response);
            _invoiceServiceMock.Verify(service => service.DeleteInvoiceAsync(invoiceId), Times.Once);
        }

        private InvoiceController CreateController() => new(_invoiceServiceMock.Object);
    }
}
