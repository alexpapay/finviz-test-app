using Finviz.TestApp.ImageNet.Domain.Entries;
using Microsoft.Extensions.DependencyInjection;

namespace Finviz.TestApp.ImageNet.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddHttpClient<ImageNetXmlParser>(client =>
        {
            client.BaseAddress = new Uri("https://raw.githubusercontent.com/");
            client.Timeout = TimeSpan.FromSeconds(30); 
        });
        
        return services;
    }
}