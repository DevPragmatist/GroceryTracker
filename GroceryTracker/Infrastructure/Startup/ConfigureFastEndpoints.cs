using FastEndpoints;

namespace GroceryTracker.Infrastructure.Startup;

public static class ConfigureFastEndpointsExtensions
{
    public static void ConfigureFastEndpoints(this WebApplicationBuilder builder)
    {
        builder.Services.AddFastEndpoints();
    }
}