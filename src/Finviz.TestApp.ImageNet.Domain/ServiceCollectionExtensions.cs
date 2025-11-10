using Finviz.TestApp.ImageNet.Domain.Entries;
using Microsoft.Extensions.DependencyInjection;

namespace Finviz.TestApp.ImageNet.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<ImageNetService>();

        return services;
    }
}
