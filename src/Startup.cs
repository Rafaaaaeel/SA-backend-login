using Sa.Core.Extensions;
using Sa.Login.Api.IoC;

namespace Sa.Login.Api;

public class Startup(IWebHostEnvironment env)
{
    private readonly StartupManager _startupManager = new(env);
    private AppSettings _appSettings;

    public virtual void ConfigureServices(IServiceCollection services)
    {
        _appSettings = _startupManager.GetSettings().Result;

        services.AddRepositoresDependencyInjection();   
        services.AddDefaultServices(_appSettings);
        services.AddContexts(_appSettings);
        services.AddDefaultDependencies(_appSettings);
        services.AddRedisCache(_appSettings);
        // services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
    }

    public void Configure(IApplicationBuilder app)
    {
        _startupManager.Configure(app, _appSettings);
    }

}