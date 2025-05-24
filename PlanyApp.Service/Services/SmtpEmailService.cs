using Microsoft.Extensions.Configuration;
using PlanyApp.Service.Dto;
using PlanyApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PlanyApp.Service.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string? _smtpHost;
        private readonly int _smtpPort;
        private readonly string? _smtpUser;
        private readonly string? _smtpPass;
        private readonly bool _enableSsl;
        private readonly string? _fromAddress;
        private readonly string? _fromName;

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

            // Enhanced validation: Throw if critical settings are missing
            if (string.IsNullOrEmpty(_smtpHost) || 
                _smtpPort <= 0 || 
                string.IsNullOrEmpty(_smtpUser) || 
                string.IsNullOrEmpty(_smtpPass) || 
                string.IsNullOrEmpty(_fromAddress))
            {
                // Log the error before throwing for better diagnostics
                var missingSettings = new List<string>();
                if (string.IsNullOrEmpty(_smtpHost)) missingSettings.Add(nameof(_smtpHost));
                if (_smtpPort <= 0) missingSettings.Add(nameof(_smtpPort));
                if (string.IsNullOrEmpty(_smtpUser)) missingSettings.Add(nameof(_smtpUser));
                if (string.IsNullOrEmpty(_smtpPass)) missingSettings.Add(nameof(_smtpPass));
                if (string.IsNullOrEmpty(_fromAddress)) missingSettings.Add(nameof(_fromAddress));
                
                string errorMessage = $"Critical Error: SMTP Email settings are missing or invalid. Missing: {string.Join(", ", missingSettings)}";
                Console.WriteLine(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
        }

        public async Task SendEmailAsync(EmailMessageDto emailMessage)
        {
            // Constructor now guarantees critical fields are non-null if it succeeds.
            // So, we can use the null-forgiving operator (!) here if needed, but direct use should be fine due to the constructor check.
            using (var client = new SmtpClient(_smtpHost, _smtpPort))
            {
                client.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                client.EnableSsl = _enableSsl;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromAddress!, _fromName ?? string.Empty), // Can use ! for _fromAddress due to constructor validation
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