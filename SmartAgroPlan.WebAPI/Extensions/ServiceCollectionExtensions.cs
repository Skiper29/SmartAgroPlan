using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartAgroPlan.BLL.Interfaces.Recommendations;
using SmartAgroPlan.BLL.PipelineBehaviour;
using SmartAgroPlan.BLL.Services.Recommendations;
using SmartAgroPlan.BLL.Validators.Crops;
using SmartAgroPlan.DAL.Persistence;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;
using SmartAgroPlan.DAL.Repositories.Repositories.Realizations.Base;
using SmartAgroPlan.WebAPI.Utils;

namespace SmartAgroPlan.WebAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();

        var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        services.AddAutoMapper(cfg => cfg.AddMaps(currentAssemblies));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(currentAssemblies));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddValidatorsFromAssemblyContaining<BaseCropVarietyValidator>();

        services.AddScoped<IGrowthStageService, GrowthStageService>();
        services.AddScoped<IRecommendationService, RecommendationService>();
    }

    public static void AddApplicationServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<SmartAgroPlanDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.UseNetTopologySuite();
                npgsqlOptions.MigrationsAssembly(typeof(SmartAgroPlanDbContext).Assembly.GetName().Name);
            }));

        var corsSettings = SettingsExtractor.GetCorsSettings(configuration);
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(corsSettings.AllowedOrigins)
                    .WithHeaders(corsSettings.AllowedHeaders)
                    .WithMethods(corsSettings.AllowedMethods)
                    .WithExposedHeaders(corsSettings.ExposedHeaders)
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(corsSettings.PreflightMaxAgeInSeconds));
            });
        });

        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(30);
        });

        services.AddLogging();

        services.AddControllers(options =>
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
    }
}
