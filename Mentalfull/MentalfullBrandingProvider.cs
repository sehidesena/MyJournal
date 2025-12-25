using Microsoft.Extensions.Localization;
using Mentalfull.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Mentalfull;

[Dependency(ReplaceServices = true)]
public class MentalfullBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<MentalfullResource> _localizer;

    public MentalfullBrandingProvider(IStringLocalizer<MentalfullResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}