using System.Diagnostics;
using Finviz.TestApp.ImageNet.Api.Mappers;
using Finviz.TestApp.ImageNet.Domain;
using Finviz.TestApp.ImageNet.Domain.Entries;
using Finviz.TestApp.ImageNet.Infrastructure;
using Finviz.TestApp.ImageNet.Infrastructure.Parsers;
using Finviz.TestApp.ImageNet.Persistence;
using Finviz.TestApp.ImageNet.Persistence.Contexts;
using Finviz.TestApp.ImageNet.Persistence.Extensions;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting up the application");
    
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
    );
    
    var environment = builder.Environment;

    builder.Services
        .AddApplicationDbContext(builder.Configuration, environment.IsDevelopment())
        .AddRepositories()
        .AddInfrastructureServices()
        .AddDomainServices()
        .AddOpenApi();

    var app = builder.Build();

    await app.ApplyMigrationsAsync<ApplicationDbContext>();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();

    app.MapGet("/api/imagenet/import", async (ImageNetXmlParser parser, IImageNetRepository imageNetRepository) =>
        {
            var stopwatch = Stopwatch.StartNew();
            
            var parsed = await parser
                .ParseAsync("tzutalin/ImageNet_Utils/refs/heads/master/detection_eval_tools/structure_released.xml");
            
            var withSizes = parser.ComputeSizes(parsed);
            
            await imageNetRepository.BulkInsertAsync(withSizes.Select(ImageNetMapper.MapNewEntity));
            
            stopwatch.Stop();
            
            return new
            {
                totalParsed = parsed.Count,
                totalSaved = withSizes.Count,
                durationMs = stopwatch.ElapsedMilliseconds,
                status = "Success"
            };
        })
        .WithName("Test");

    app.MapGet("/api/imagenet/tree", async (IImageNetRepository imageNetRepository, ImageNetService imageNetService) =>
        {
            var entries = await imageNetRepository.GetAllAsync();
            var tree = imageNetService.BuildTree(entries);
            return tree.Select(ImageNetMapper.MapToTreeItemResponse);
        })
        .WithName("Tree");

    app.MapGet("/api/imagenet/tree/root", async (IImageNetRepository imageNetRepository) =>
        {
            var entries = await imageNetRepository.GetRootAsync();
            return entries.Select(ImageNetMapper.MapToResponse);
        })
        .WithName("TreeRoot");

    app.MapGet("/api/imagenet/tree/children", async (IImageNetRepository imageNetRepository, int parentId) =>
        {
            var entries = await imageNetRepository.GetChildrenAsync(parentId);
            return entries.Select(ImageNetMapper.MapToResponse);
        })
        .WithName("TreeChildren");

    app.MapGet("/api/imagenet/search", async (IImageNetRepository imageNetRepository, string q) =>
        {
            var entries = await imageNetRepository.SearchAsync(q);
            return entries.Select(ImageNetMapper.MapToResponse);
        })
        .WithName("Search");

    app.Run();
    
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}