using Microsoft.Extensions.DependencyInjection;

namespace HardshipApp.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IHardshipApplicationService, HardshipApplicationService>();

        return services;
    }
}
