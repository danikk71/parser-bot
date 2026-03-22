using System.Collections.Concurrent;
using HtmlAgilityPack;
using main.Interfaces;
using main.Models;
using main.Services.Fetcher;
using main.Services.Mappers;
using main.Services.Scrapers;
using main.Services.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

//var configurations = new ConfigurationBuilder()
//    .SetBasePath(Directory.GetCurrentDirectory())
//    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//    .Build();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IWebFetcher, HttpFetcher>();
builder.Services.AddSingleton<IMapper<HtmlNode, Product>, TelemartMapper>();
builder.Services.AddSingleton<IScraperService, TelemartScraper>();
builder.Services.AddScoped<IExporter<List<Product>>, DbStorageService>();
builder.Services.AddJsonService();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.MapControllers();

app.Run();

//var exporters = serviceProvider.GetServices<IExporter<List<Product>>>();
//var scraper = serviceProvider.GetRequiredService<IScraperService>();

//Console.WriteLine("Початок програми: ");
//await scraper.RunScraperAsync();

//var products = scraper.GetProductsList();
//foreach (var exporter in exporters)
//    await exporter.ExportAsync(products);

