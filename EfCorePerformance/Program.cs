using EfCorePerformance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(
        "Host=host.docker.internal;Port=5432;Database=postgres;User Id=postgres;Password=welcome",
        o => o.UseNetTopologySuite());
    options.EnableSensitiveDataLogging();
});

builder.Services.AddScoped<DataProvider, DataProvider>();

builder.Services.AddHostedService<DbInitializerHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/person", async (
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken,
    [FromServices] DataProvider provider) =>
{
    if (DbInitializerHostedService.DataIsReady)
    {
        return await provider.GetHumansWithLinqMethodSyntax(pageNumber, pageSize, cancellationToken);
    }

    return new ResponseModel()
    {
        Message = "Preparing data, try after sometime"
    };
});

app.MapGet("/personraw", async (
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken,
    [FromServices] DataProvider provider) =>
{
    if (DbInitializerHostedService.DataIsReady)
    {
        return await provider.GetHumansWithLinqRawQuery(pageNumber, pageSize, cancellationToken);
    }

    return new ResponseModel()
    {
        Message = "Preparing data, try after sometime"
    };
});

app.Run();