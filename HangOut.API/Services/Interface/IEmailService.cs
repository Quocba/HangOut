using HangOut.Domain.Payload.Base;

namespace HangOut.API.Services.Interface;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage emailMessage);
}