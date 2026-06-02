using VerificationService.Models;

namespace VerificationService.Application.Interfaces;

public interface IVerificationCodeRepository
{
    Task AddAsync(VerificationCode code);
    Task SaveChangesAsync();
    Task<VerificationCode?> GetLatestByEmailAsync(string email);
}