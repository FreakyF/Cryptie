using Cryptie.Server.API.Features.Authentication;
using Cryptie.Server.Infrastructure.Persistence.DatabaseContext;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Cryptie.Server.API.Tests;

public class AuthenticationApiFactory:WebApplicationFactory<Program>,IAsyncLifetime
{
    private readonly PostgreSqlContainer _database;

    public AuthenticationApiFactory()
    {
        _database = new PostgreSqlBuilder()
            .WithImage("postgres:17-alpine")
            .WithDatabase("cryptie-test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("pg_isready -U postgres -d cryptie-test"))
            .WithCleanUp(true)
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var old = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (old is not null)
            {
                services.Remove(old);
            }
            
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseNpgsql(_database.GetConnectionString()));
            
            using var scope = services.BuildServiceProvider().CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            ctx.Database.Migrate();
        });
    }

    public async Task InitializeAsync() => await _database.StartAsync();
    public async Task DisposeAsync()    => await _database.DisposeAsync();
    
}