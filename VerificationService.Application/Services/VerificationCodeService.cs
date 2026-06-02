using VerificationService.Application.Interfaces;
using VerificationService.Models;

namespace VerificationService.Application.Services;

public class VerificationCodeService : IVerificationCodeService
{
    private readonly IVerificationCodeRepository _repository;

    public VerificationCodeService(IVerificationCodeRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> GenerateAndSaveCodeAsync(string email)
    {
        var random = new Random();
        var code = random.Next(100000, 999999).ToString();

        var verificationCode = new VerificationCode
        {
            Email = email,
            Code = code,
            CreatedAt = DateTime.UtcNow,
            IsUsed = false
        };

        await _repository.AddAsync(verificationCode);
        await _repository.SaveChangesAsync();

        return code;
    }

    public async Task<bool> ValidateCodeAsync(string email, int code)
    {
        var verification = await _repository.GetLatestByEmailAsync(email);

        if (verification == null)
            return false;

        if (verification.IsUsed)
            return false;

        if (verification.Code != code.ToString())
            return false;

        verification.IsUsed = true;

        await _repository.SaveChangesAsync();

        return true;
    }
}