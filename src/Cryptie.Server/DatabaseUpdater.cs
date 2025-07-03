using Cryptie.Server.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace Cryptie.Server;

public class DatabaseUpdater
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseUpdater(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Applies any pending Entity Framework migrations to ensure the database
    /// schema is up to date.
    /// </summary>
    /// <returns>A task that completes when the migration process finishes.</returns>
    public async Task PerformDatabaseUpdate()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}