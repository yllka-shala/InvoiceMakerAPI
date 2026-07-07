using System;
using System.Threading.Tasks;
using InvoiceMakerAPI.Services.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using InvoiceMakerAPI.DTOs;

namespace InvoiceMakerAPI.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings) 
        { 
            _emailSettings = emailSettings.Value;
        }
        public async Task SendEmailAsync(string email, byte[] pdfBytes)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Invoice Maker", _emailSettings.Username));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Your Invoice";
            var bodyBuilder = new BodyBuilder
            {
                TextBody = "Please find your invoice attached."
            };
            bodyBuilder.Attachments.Add("invoice.pdf", pdfBytes);
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, false);
            await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
