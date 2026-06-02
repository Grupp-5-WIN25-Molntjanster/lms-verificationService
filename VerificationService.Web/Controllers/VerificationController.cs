using Microsoft.AspNetCore.Mvc;
using VerificationService.Application.Interfaces;
using VerificationService.Models;

namespace VerificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VerificationController : ControllerBase
{
    private readonly IVerificationCodeService _service;
    private readonly IServiceBusPublisher _publisher;
    private readonly IVerificationCodeRepository _repository;

    public VerificationController(
        IVerificationCodeService service,
        IServiceBusPublisher publisher,
        IVerificationCodeRepository repository)
    {
        _service = service;
        _publisher = publisher;
        _repository = repository;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] SendVerificationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("Email is required");

        var code = await _service.GenerateAndSaveCodeAsync(request.Email);

        await _publisher.SendVerificationEmailAsync(request.Email, code);

        return Ok(new { message = "Verification code queued" });
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate([FromBody] ValidateVerificationRequest request)
    {
        var verification = await _repository.GetLatestByEmailAsync(request.Email);

        if (verification == null)
        {
            return BadRequest(new { message = "No code found for this email" });
        }

        if (verification.Code != request.Code.ToString())
        {
            return BadRequest(new { message = "Wrong code" });
        }

        if (verification.IsUsed)
        {
            return BadRequest(new { message = "Code already used" });
        }

        verification.IsUsed = true;

        await _repository.SaveChangesAsync();

        return Ok(new
        {
            message = "Code is valid"
        });
    }
}