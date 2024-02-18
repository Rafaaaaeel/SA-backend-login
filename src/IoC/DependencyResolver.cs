namespace Sa.Login.Api.IoC;

public static class DependencyResolver
{
    public static IServiceCollection AddRepositoresDependencyInjection(this IServiceCollection services) 
    {
        services.AddSingleton<ISessionToken, SessionToken>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddTransient<IEmailSender, EmailSender>();
        return services;
    }

    public static IServiceCollection AddContexts<T>(this IServiceCollection services, T appSettings) where T : AppSettings
    {
        services.AddDbContext<UserContext>(opt => opt.UseSqlServer(appSettings.SqlConfiguration.ConnectionString));
        return services;
    }

}