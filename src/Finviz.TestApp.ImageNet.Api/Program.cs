using Finviz.TestApp.ImageNet.Domain;
using Finviz.TestApp.ImageNet.Domain.Entries;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDomainServices();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/test", async (ImageNetXmlParser parser) =>
    {
        var results = await parser
            .ParseAsync("tzutalin/ImageNet_Utils/refs/heads/master/detection_eval_tools/structure_released.xml");
        
        var withSizes = parser.ComputeSizes(results);
        
        return withSizes;
    })
    .WithName("Test");

app.Run();