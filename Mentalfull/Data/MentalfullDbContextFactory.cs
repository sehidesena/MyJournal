using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Mentalfull.Data;

public class MentalfullDbContextFactory : IDesignTimeDbContextFactory<MentalfullDbContext>
{
    public MentalfullDbContext CreateDbContext(string[] args)
    {
        MentalfullGlobalFeatureConfigurator.Configure();
        MentalfullModuleExtensionConfigurator.Configure();

        // https://www.npgsql.org/efcore/release-notes/6.0.html#opting-out-of-the-new-timestamp-mapping-logic
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        MentalfullEfCoreEntityExtensionMappings.Configure();
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<MentalfullDbContext>()
            .UseNpgsql(configuration.GetConnectionString("Default"));

        return new MentalfullDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables();

        return builder.Build();
    }
}