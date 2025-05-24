using Microsoft.Extensions.Configuration;
using PlanyApp.Service.Dto;
using PlanyApp.Service.Interfaces;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PlanyApp.Service.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass; // This should be an App Password if using Gmail
        private readonly bool _enableSsl;
        private readonly string _fromAddress;
        private readonly string _fromName;

        public SmtpEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpHost = _configuration["EmailSettings:SmtpHost"];
            _smtpPort = _configuration.GetValue<int>("EmailSettings:SmtpPort");
            _smtpUser = _configuration["EmailSettings:SmtpUser"];
            _smtpPass = _configuration["EmailSettings:SmtpPass"]; // App Password for Gmail
            _enableSsl = _configuration.GetValue<bool>("EmailSettings:EnableSsl");
            _fromAddress = _configuration["EmailSettings:FromAddress"];
            _fromName = _configuration["EmailSettings:FromName"];

            if (string.IsNullOrEmpty(_smtpHost) || _smtpPort <= 0 || string.IsNullOrEmpty(_smtpUser) || 
                string.IsNullOrEmpty(_smtpPass) || string.IsNullOrEmpty(_fromAddress))
            {
                Console.WriteLine("Critical Error: SMTP Email settings are missing or invalid in configuration.");
                // Consider throwing an exception to prevent app startup without email config if critical
                // throw new InvalidOperationException("SMTP Email settings are missing or invalid.");
            }
        }

        public async Task SendEmailAsync(EmailMessageDto emailMessage)
        {
            if (string.IsNullOrEmpty(_smtpHost)) // Check if config was invalid during construction
            {
                Console.WriteLine("Email not sent: SMTP settings are not configured.");
                // Optionally, throw an exception or return a failure status if this service is critical.
                return; 
            }

            using (var client = new SmtpClient(_smtpHost, _smtpPort))
            {
                client.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                client.EnableSsl = _enableSsl;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromAddress, _fromName ?? string.Empty),
                    Subject = emailMessage.Subject,
                    Body = emailMessage.Body,
                    IsBodyHtml = emailMessage.IsHtml,
                };

                foreach (var toAddress in emailMessage.ToAddresses)
                {
                    mailMessage.To.Add(toAddress);
                }
                
                try
                {
                    await client.SendMailAsync(mailMessage);
                    Console.WriteLine($"Email sent to {string.Join(",", emailMessage.ToAddresses)} with subject '{emailMessage.Subject}'");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send email. Error: {ex.Message}");
                    // Log the full exception details for debugging
                    // Consider re-throwing or handling specific exceptions as needed
                }
            }
        }
    }
} 