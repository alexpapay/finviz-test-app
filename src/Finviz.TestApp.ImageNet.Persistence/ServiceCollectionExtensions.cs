using System.Text.RegularExpressions;
using Finviz.TestApp.ImageNet.Persistence.Contexts;
using Finviz.TestApp.ImageNet.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Finviz.TestApp.ImageNet.Persistence;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<ImageNetRepository>()
            .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Repository")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
    
    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services,
        ConfigurationManager configuration,
        bool sensitiveDataLogging)
    {
        var defaultConnectionString = configuration.GetConnectionString("Default");

        ValidateConnectionString(defaultConnectionString);

        services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseNpgsql(defaultConnectionString)
                .EnableSensitiveDataLogging(sensitiveDataLogging));

        return services;
    }

    private static void ValidateConnectionString(string? defaultConnectionString)
    {
        if (string.IsNullOrWhiteSpace(defaultConnectionString))
        {
            throw new ArgumentException("Missing connection string to database");
        }

        var regex = ConnectionStringRegex();
        var match = regex.Match(defaultConnectionString);

        if (!match.Success)
        {
            throw new ArgumentException(
                "Please add a connection string to database into a .NET User Secrets file for this project");
        }
    }

    [GeneratedRegex("(?<Key>[^=;]+)=(?<Val>[^;]+)")]
    private static partial Regex ConnectionStringRegex();
}