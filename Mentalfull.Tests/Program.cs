using Microsoft.AspNetCore.Builder;
using Mentalfull;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("Mentalfull.csproj");
await builder.RunAbpModuleAsync<MentalfullTestModule>(applicationName: "Mentalfull");
namespace Mentalfull
{
    public partial class Program
    {
    }
}
