using Serilog;
using SmartAgroPlan.BLL.Services.Weather;

namespace SmartAgroPlan.WebAPI.Extensions;

public static class ConfigureHostBuilderExtensions
{
    public static void ConfigureApplication(this ConfigureHostBuilder host, WebApplicationBuilder builder)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";

        builder.Configuration.ConfigureCustom(environment);
    }

    public static void ConfigureSerilog(this IServiceCollection services, WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration);
        });
    }

    public static void ConfigureOpenMeteo(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.Configure<OpenMeteoOptions>(builder.Configuration.GetSection("OpenMeteo"));
    }
}
