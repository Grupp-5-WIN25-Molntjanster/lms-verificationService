using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using VerificationService.Application.Interfaces;
using VerificationService.Application.Services;
using VerificationService.Infrastructure.Data;
using VerificationService.Infrastructure.Repositories;
using VerificationService.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure();
        }
    )
);

builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    var connectionString = config["Azure:ServiceBus:ConnectionString"];
    var queueName = config["Azure:ServiceBus:QueueName"];

    Console.WriteLine($"ConnectionString: {connectionString}");
    Console.WriteLine($"QueueName: {queueName}");

    return new ServiceBusClient(connectionString);
});

builder.Services.AddSingleton<IServiceBusPublisher, ServiceBusPublisher>();

builder.Services.AddScoped<IVerificationCodeService, VerificationCodeService>();

builder.Services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/*app.UseHttpsRedirection();
*/
app.UseAuthorization();

app.MapControllers();

app.Run();