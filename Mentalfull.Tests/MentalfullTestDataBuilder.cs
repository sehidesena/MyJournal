using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Mentalfull;

public class MentalfullTestDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly ICurrentTenant _currentTenant;

    public MentalfullTestDataSeedContributor(ICurrentTenant currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public Task SeedAsync(DataSeedContext context)
    {
        /* Seed additional test data... */

        using (_currentTenant.Change(context?.TenantId))
        {
            return Task.CompletedTask;
        }
    }
}
