using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TurboVolt.Mappings;
using TurboVolt.Models;
using TurboVolt.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Initialisation de Mapster
MapsterConfig.RegisterMappings();

// 2. Enregistrement de HybridCache (.NET 9)
#pragma warning disable EXTEXP0018
builder.Services.AddHybridCache();
#pragma warning restore EXTEXP0018

// 3. Enregistrement automatique des validateurs FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// 4. Injections des services applicatifs
builder.Services.AddScoped<IBlivraisonService, BlivraisonService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<ILookupService, LookupService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
