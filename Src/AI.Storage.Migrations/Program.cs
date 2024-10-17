using AI.Storage.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

try
{
    Log.Information("Starting migration application");

    var services = new ServiceCollection();
    services.AddLogging(loggingBuilder =>
        loggingBuilder.AddSerilog(dispose: true));

    services.AddDbContext<StorageDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

    var serviceProvider = services.BuildServiceProvider();

    // Apply migrations
    using (var scope = serviceProvider.CreateScope())
    {
        var dbContext = scope.ServiceProvider
            .GetRequiredService<StorageDbContext>();
        
        await dbContext.Database.MigrateAsync();
    }

    Log.Information("Migrations applied successfully");
    
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "An error occurred while applying migrations");
    
    return 1;
}
finally
{
    Log.CloseAndFlush();
}