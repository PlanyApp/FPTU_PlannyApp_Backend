using PlanyApp.Service.Dto;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessageDto emailMessage);
    }
} 