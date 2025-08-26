using SmartAgroPlan.WebAPI.Utils;

namespace SmartAgroPlan.WebAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");


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

            services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
        }
    }
}
