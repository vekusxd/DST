using DST.Bot.Database;
using DST.Bot.Entities;
using Hangfire;
using Hangfire.Storage.SQLite;

namespace DST.Bot.Features.Hangfire;

public static class HangfireSetup
{
    public static IServiceCollection AddHangfire(this IServiceCollection services, IConfigurationManager manager)
    {
        var hangfireConnection = manager.GetConnectionString("Hangfire")
                                 ?? throw new Exception("Missing hangfire connection string");

        services.AddHangfire(configuration => configuration
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSQLiteStorage(hangfireConnection));

        services.AddHangfireServer();
        
        services.AddScoped<BackgroundJobs>();

        return services;
    }
}

public class BackgroundJobs
{
    private readonly AppDbContext _dbContext;

    public BackgroundJobs(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task DecrementCounter(User user)
    {
        Console.WriteLine($"Уменьшаем счетчик для {user.Name}");
        
        await _dbContext.Entry(user)
            .Reference(u => u.BugData)
            .LoadAsync();

        if (user.BugData.CountThisDay == 0)
            return;

        user.BugData.CountThisDay -= 1;
        _dbContext.Update(user);
        await _dbContext.SaveChangesAsync();

    }
}