using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartAgroPlan.BLL.Interfaces.Crops;
using SmartAgroPlan.BLL.Interfaces.FertilizerForecasting;
using SmartAgroPlan.BLL.Interfaces.Irrigation;
using SmartAgroPlan.BLL.Interfaces.Weather;
using SmartAgroPlan.BLL.PipelineBehaviour;
using SmartAgroPlan.BLL.Services.Crops;
using SmartAgroPlan.BLL.Services.FertilizerForecasting;
using SmartAgroPlan.BLL.Services.Irrigation;
using SmartAgroPlan.BLL.Services.Weather;
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

        // Crop and Irrigation Services
        services.AddScoped<ICropCoefficientService, CropCoefficientService>();
        services.AddScoped<IFAO56Calculator, FAO56Calculator>();
        services.AddScoped<ISoilWaterService, SoilWaterService>();

        // Fertilizer Forecasting Services
        services.AddScoped<IFertilizerCalculationService, FertilizerCalculationService>();
        services.AddScoped<IFertilizerPlanManagementService, FertilizerPlanManagementService>();
        services.AddScoped<IFertilizerApplicationRecordService, FertilizerApplicationRecordService>();
        services.AddScoped<IFertilizerProductService, FertilizerProductService>();

        // Weather Service
        services.AddHttpClient<IWeatherService, OpenMeteoService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });
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
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        Console.OutputEncoding = Encoding.UTF8;
    }
}