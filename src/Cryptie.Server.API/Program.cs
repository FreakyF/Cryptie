using System.Threading.RateLimiting;
using Cryptie.Server.API.Features.Authentication.Services;
using Cryptie.Server.API.Features.Authentication.Validators;
using Cryptie.Server.Domain.Features.Authentication.DTOs;
using Cryptie.Server.Domain.Features.Authentication.Services;
using Cryptie.Server.Infrastructure.Persistence.DatabaseContext;
using FluentValidation;
using Scalar.AspNetCore;

namespace Cryptie.Server.API;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var dbContainer = new DockerStarter();
        await dbContainer.StartPostgresAsync();

        var builder = WebApplication.CreateBuilder(args);

        const string secureCorsPolicy = "SecureCorsPolicy";
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

        builder.Services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
        builder.Services.AddScoped<IValidator<LogoutRequest>, LogoutRequestValidator>();
        builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
        builder.Services.AddScoped<IValidator<TotpRequest>, TotpRequestValidator>();

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();


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