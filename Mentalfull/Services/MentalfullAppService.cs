using Volo.Abp.Application.Services;
using Mentalfull.Localization;

namespace Mentalfull.Services;

/* Inherit your application services from this class. */
public abstract class MentalfullAppService : ApplicationService
{
    protected MentalfullAppService()
    {
        LocalizationResource = typeof(MentalfullResource);
    }
}