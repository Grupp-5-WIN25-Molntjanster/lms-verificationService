using Microsoft.EntityFrameworkCore;
using VerificationService.Application.Interfaces;
using VerificationService.Infrastructure.Data;
using VerificationService.Models;

namespace VerificationService.Infrastructure.Repositories;

public class VerificationCodeRepository : IVerificationCodeRepository
{
    private readonly ApplicationDbContext _dbContext;

    public VerificationCodeRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(VerificationCode code)
    {
        _dbContext.VerificationCodes.Add(code);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task<VerificationCode?> GetLatestByEmailAsync(string email)
    {
        return await _dbContext.VerificationCodes
            .Where(v => v.Email == email)
            .OrderByDescending(v => v.CreatedAt)
            .FirstOrDefaultAsync();
    }
}