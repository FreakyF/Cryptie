// using System;
// using System.Threading.Tasks;
// using Cryptie.Server;
// using Cryptie.Server.Persistence.DatabaseContext;
// using Microsoft.Data.Sqlite;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using Moq;
// using Xunit;
//
// public class DatabaseUpdaterTests
// {
//     [Fact]
//     public async Task PerformDatabaseUpdate_CallsMigrateAsync_WithSqlite()
//     {
//         // Arrange
//         var connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=:memory:");
//         connection.Open();
//         var options = new DbContextOptionsBuilder<AppDbContext>()
//             .UseSqlite(connection)
//             .Options;
//         var dbContext = new AppDbContext(options);
//         await dbContext.Database.EnsureCreatedAsync();
//
//         var serviceProviderMock = new Mock<IServiceProvider>();
//         var scopeMock = new Mock<IServiceScope>();
//         var scopeFactoryMock = new Mock<IServiceScopeFactory>();
//         var serviceScopeProvider = new Mock<IServiceProvider>();
//
//         serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(scopeFactoryMock.Object);
//         scopeFactoryMock.Setup(x => x.CreateScope()).Returns(scopeMock.Object);
//         scopeMock.SetupGet(x => x.ServiceProvider).Returns(serviceScopeProvider.Object);
//         serviceScopeProvider.Setup(x => x.GetService(typeof(AppDbContext))).Returns(dbContext);
//
//         // Act & Assert
//         var updater = new DatabaseUpdater(serviceProviderMock.Object);
//         var exception = await Record.ExceptionAsync(() => updater.PerformDatabaseUpdate());
//         Assert.Null(exception); // Sprawdzamy, czy migracja się wykonała bez wyjątku
//     }
// }
