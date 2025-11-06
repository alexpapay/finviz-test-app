using Finviz.TestApp.ImageNet.Infrastructure.Parsers;
using Microsoft.Extensions.DependencyInjection;

namespace Finviz.TestApp.ImageNet.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddHttpClient<ImageNetXmlParser>(client =>
        {
            client.BaseAddress = new Uri("https://raw.githubusercontent.com/");
            client.Timeout = TimeSpan.FromSeconds(30); 
        });
        
        return services;
    }
}