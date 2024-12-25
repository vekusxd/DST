using DST.Bot.Database;
using DST.Bot.Features.Common;
using DST.Bot.Features.GenerateFrontPage;
using DST.Bot.Features.GetSources;
using DST.Bot.Features.SetupBot;
using DST.Bot.Features.StateManager;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

QuestPDF.Settings.License = LicenseType.Community;

builder.Services
    .AddTgBotServices(builder.Configuration)
    .AddStateManagement();

builder.Services.AddScoped<UserHelper>();
builder.Services.AddScoped<MenuHelper>();
builder.Services.AddHttpClient("cyberleninka",o =>
{
    o.BaseAddress = new Uri("https://cyberleninka.ru/");
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? throw new Exception("No connection string was found");

builder.Services.AddDbContext<AppDbContext>(
    opts => opts.UseSqlite(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseWebHook();

app.Run();

