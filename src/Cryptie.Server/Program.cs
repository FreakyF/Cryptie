using System.Globalization;
using System.Threading.RateLimiting;
using Cryptie.Common.Features.Authentication.DTOs;
using Cryptie.Common.Features.Authentication.Services;
using Cryptie.Common.Features.Authentication.Validators;
using Cryptie.Server.Features.Authentication.Services;
using Cryptie.Server.Features.GroupManagement;
using Cryptie.Server.Features.Messages.Services;
using Cryptie.Server.Persistence.DatabaseContext;
using Cryptie.Server.Services;
using FluentValidation;
using Scalar.AspNetCore;

namespace Cryptie.Server;

public class Program
{
    public static async Task Main(string[] args)
    {
        var dbContainer = new DockerStarter();
        await dbContainer.StartPostgresAsync();

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHsts(options =>
        {
            options.MaxAge = TimeSpan.FromDays(365);
            options.IncludeSubDomains = true;
            options.Preload = true;
        });

        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, ct) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retry))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retry.TotalSeconds).ToString(CultureInfo.InvariantCulture);
                }

                await context.HttpContext.Response.WriteAsync(
                    "Too many requests—please try again later.", ct);
            };

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                string userKey;
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    userKey = context.User.Identity.Name!;
                }
                else
                {
                    userKey = context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
                }

                return RateLimitPartition.GetTokenBucketLimiter(
                    userKey,
                    _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 60,
                        TokensPerPeriod = 20,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                        AutoReplenishment = true,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 5
                    });
            });
        });


        // Add services to the container.
        builder.Services.AddDbContext<IAppDbContext, AppDbContext>();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<ILockoutService, LockoutService>();
        builder.Services.AddScoped<IDatabaseService, DatabaseService>();
        builder.Services.AddScoped<IDelayService, DelayService>();
        builder.Services.AddScoped<IGroupManagementService, GroupManagementService>();

        builder.Services.AddScoped<IValidator<LoginRequestDto>, LoginRequestValidator>();
        builder.Services.AddScoped<IValidator<LogoutRequestDto>, LogoutRequestValidator>();
        builder.Services.AddScoped<IValidator<RegisterRequestDto>, RegisterRequestValidator>();
        builder.Services.AddScoped<IValidator<TotpRequestDto>, TotpRequestValidator>();

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddSignalR();

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

        app.UseRateLimiter();

        app.UseHsts();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.MapHub<MessageHub>("/messages");

        await app.RunAsync();
    }
}