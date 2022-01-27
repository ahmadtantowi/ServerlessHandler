using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using ServerlessHandler.Extensions;
using ServerlessHandler.Sample;

[assembly: FunctionsStartup(typeof(Startup))]
namespace ServerlessHandler.Sample;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddServerlessHandler(typeof(Startup));
    }
}
