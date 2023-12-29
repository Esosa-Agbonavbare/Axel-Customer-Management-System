using AxelCMS.Domain.Entities;

namespace AxelCMS.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendHtmlEmailAsync(MailRequest request);
    }
}
