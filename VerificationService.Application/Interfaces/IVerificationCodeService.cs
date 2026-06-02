using VerificationService.Models;

namespace VerificationService.Application.Interfaces;

public interface IVerificationCodeService
{
    Task<string> GenerateAndSaveCodeAsync(string email);
    Task<bool> ValidateCodeAsync(string email, int code);
}