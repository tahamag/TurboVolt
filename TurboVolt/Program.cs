using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TurboVolt.Mappings;
using TurboVolt.Models;
using TurboVolt.Services;
using Scalar.AspNetCore;
using System.Reflection;
using TurboVolt.Validators;

var builder = WebApplication.CreateBuilder(args);
// 1. Déclaration de la politique CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
// 1. Initialisation de Mapster
MapsterConfig.RegisterMappings();

// 2. Enregistrement de HybridCache (.NET 9)
#pragma warning disable EXTEXP0018
builder.Services.AddHybridCache();
#pragma warning restore EXTEXP0018

// 3. Enregistrement automatique des validateurs FluentValidation
//builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddValidatorsFromAssemblyContaining<TurboVolt.Validators.ArticleFilterValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<BreceptionFilterValidator>();

// 4. Injections des services applicatifs
builder.Services.AddScoped<IBlivraisonService, BlivraisonService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<ILookupService, LookupService>();
builder.Services.AddScoped<IBreceptionService, BreceptionService>();
builder.Services.AddScoped<IAuthService, AuthService>();
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

try
{
    builder.Services.AddOpenApi();
    var app = builder.Build();

    app.UseCors("AllowAngular");
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (System.Reflection.ReflectionTypeLoadException ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("=== DÉTAILS DE L'ERREUR DE REFLEXION ===");
    foreach (var loaderEx in ex.LoaderExceptions)
    {
        Console.WriteLine($"-> {loaderEx?.Message}");
    }
    Console.ResetColor();
    throw;
}