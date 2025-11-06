using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Finviz.TestApp.ImageNet.Persistence.Extensions;

public static class EntityFrameworkExtensions
{
    public static async Task ApplyMigrationsAsync<T>(this IHost app) where T : DbContext
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<T>();

        if (context.Database.IsRelational())
        {
            await context.Database.MigrateAsync();
        }
    }
}