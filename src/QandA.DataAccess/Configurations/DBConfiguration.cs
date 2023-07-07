using DbUp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace QandA.DataAccess.Configurations;

public static class DBConfiguration
{
    public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("ConnectionStrings:0:DefaultConenction").Value
            ?? throw new ArgumentNullException(nameof(configuration));

        // get connection string and create database if not exist
        EnsureDatabase.For.SqlDatabase(connectionString);

        var upgrader = DeployChanges
            .To.SqlDatabase(connectionString, null)
            .WithScriptsEmbeddedInAssembly(System.Reflection.Assembly.GetExecutingAssembly())
            .WithTransaction()
            .Build();

        if (upgrader.IsUpgradeRequired())
        {
            upgrader.PerformUpgrade();
        }
    }
}