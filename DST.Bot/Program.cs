using DST.Bot.Database;
using DST.Bot.Features.Common;
using DST.Bot.Features.GenerateFrontPage;
using DST.Bot.Features.GetSources;
using DST.Bot.Features.GigaChat;
using DST.Bot.Features.SetupBot;
using DST.Bot.Features.StateManager;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

builder.Services
    .AddTg(builder.Configuration)
    .AddHelper()
    .AddStateManagement()
    .AddGigaChat(builder.Configuration);

builder.Services.AddHttpClient("cyberleninka",o =>
{
    o.BaseAddress = new Uri("https://cyberleninka.ru/");
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? throw new Exception("No connection string was found");

builder.Services.AddDbContext<AppDbContext>(
    opts => opts.UseSqlite(connectionString));

var app = builder.Build();


app.UseHttpsRedirection();

app.UseWebHook();

app.Run();

