using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rockaway.WebApp.Data;

namespace Rockaway.WebApp.Tests;

public static class TestDatabase {
	public static string GetSqliteDbName(this RockawayDbContext context) {
		var builder = new DbConnectionStringBuilder {
			ConnectionString = context.Database.GetConnectionString()
		};
		return builder["Data Source"].ToString()!;
	}

	public static RockawayDbContext Create(string? dbName = null) {
		dbName ??= Guid.NewGuid().ToString();
		var dbContext = Connect(dbName);
		dbContext.Database.EnsureCreated();
		return dbContext;
	}

	public static RockawayDbContext Connect(string dbName) {
		var connectionString = $"Data Source={dbName};Mode=Memory;Cache=Shared";
		var sqliteConnection = new SqliteConnection(connectionString);
		sqliteConnection.Open();
		var cmd = new SqliteCommand("PRAGMA case_sensitive_like = false", sqliteConnection);
		cmd.ExecuteNonQuery();
		var options = new DbContextOptionsBuilder<RockawayDbContext>().UseSqlite(sqliteConnection).Options;
		return new(options, unitTestMode: true);
	}

	public static WebApplicationFactory<T> WithTestDatabase<T>(this WebApplicationFactory<T> factory) where T : class {
		return factory.WithWebHostBuilder(builder => builder.ConfigureServices(services => {
			services.RemoveAll<RockawayDbContext>();
			services.AddScoped(_ => Create());
		}));
	}
}
