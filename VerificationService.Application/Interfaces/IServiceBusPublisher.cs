namespace VerificationService.Application.Interfaces;

public interface IServiceBusPublisher
{
    Task SendVerificationEmailAsync(string email, string code);
}