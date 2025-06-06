using System.Threading.RateLimiting;
using Cryptie.Common.Features.Authentication.DTOs;
using Cryptie.Common.Features.Authentication.Validators;
using Cryptie.Server.API.Features.Authentication.Services;
using Cryptie.Server.Domain.Features.Authentication.Services;
using Cryptie.Server.Infrastructure.Persistence.DatabaseContext;
using FluentValidation;
using Scalar.AspNetCore;

namespace Cryptie.Server.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var dbContainer = new DockerStarter();
        await dbContainer.StartPostgresAsync();

        var builder = WebApplication.CreateBuilder(args);

        const string secureCorsPolicy = "SecureCorsPolicy"; //TODO do usunięcia
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(secureCorsPolicy, policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                    .WithHeaders("Content-Type", "Authorization", "X-Totp-Token", "X-Secret")
                    .WithMethods("GET", "POST", "OPTIONS")
                    .SetPreflightMaxAge(TimeSpan.FromHours(2));
            });
        });

        builder.Services.AddHsts(options =>
        {
            options.MaxAge = TimeSpan.FromDays(365);
            options.IncludeSubDomains = true;
            options.Preload = true;
        });

        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    context.Request.Headers.Host.ToString(),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));
        });

        // Add services to the container.
        builder.Services.AddDbContext<IAppDbContext, AppDbContext>();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<ILockoutService, LockoutService>();
        builder.Services.AddScoped<IDatabaseService, DatabaseService>();
        builder.Services.AddScoped<IDelayService, DelayService>();

        builder.Services.AddScoped<IValidator<LoginRequestDto>, LoginRequestValidator>();
        builder.Services.AddScoped<IValidator<LogoutRequestDto>, LogoutRequestValidator>();
        builder.Services.AddScoped<IValidator<RegisterRequestDto>, RegisterRequestValidator>();
        builder.Services.AddScoped<IValidator<TotpRequestDto>, TotpRequestValidator>();

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        app.UseWebSockets(new WebSocketOptions
        {
            KeepAliveInterval = TimeSpan.FromSeconds(120)
        });

        var databaseUpdater = new DatabaseUpdater(app.Services);
        await databaseUpdater.PerformDatabaseUpdate();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseCors(secureCorsPolicy);

        app.UseRateLimiter();

        app.UseHsts();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}