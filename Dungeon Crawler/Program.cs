using BlazorDungeon.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

try
{
    builder.Services.AddRazorPages();
    builder.Services.Configure<RazorPagesOptions>(options =>
    {
        options.RootDirectory = "/Components/Pages";
    });
    builder.Services.AddServerSideBlazor();
    builder.Services.AddScoped<IGameLogger, GameLogger>();
    builder.Services.AddScoped<IMonsterFactory, MonsterFactory>();
    builder.Services.AddScoped<IItemFactory, ItemFactory>();
    builder.Services.AddScoped<IGameService, GameService>();

    Console.WriteLine("Services registered successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"Error during service registration: {ex}");
    throw;
}

var app = builder.Build();

try
{
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    app.MapRazorPages();
    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");

    Console.WriteLine("App configured successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"Error during app configuration: {ex}");
    throw;
}

app.Run();