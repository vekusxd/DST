namespace DST.Bot.Features.Common;

public static class CommonExtension
{
    public static IServiceCollection AddHelper(this IServiceCollection services)
    {
        services.AddScoped<UserHelper>();
        services.AddScoped<MenuHelper>();
        return services;
    }
}