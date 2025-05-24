using System.Collections.Generic;

namespace PlanyApp.Service.Dto
{
    public class EmailMessageDto
    {
        public List<string> ToAddresses { get; set; } = new List<string>();
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; } = true; // Default to HTML emails

        public EmailMessageDto(string toAddress, string subject, string body, bool isHtml = true)
        {
            ToAddresses.Add(toAddress);
            Subject = subject;
            Body = body;
            IsHtml = isHtml;
        }

        public EmailMessageDto(List<string> toAddresses, string subject, string body, bool isHtml = true)
        {
            ToAddresses.AddRange(toAddresses);
            Subject = subject;
            Body = body;
            IsHtml = isHtml;
        }
    }
} 