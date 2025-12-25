using Volo.Abp.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Mentalfull.Data;

public class MentalfullDbSchemaMigrator : ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public MentalfullDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        
        /* We intentionally resolving the MentalfullDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<MentalfullDbContext>()
            .Database
            .MigrateAsync();

    }
}
