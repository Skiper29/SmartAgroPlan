using SmartAgroPlan.WebAPI.Utils.Settings;

namespace SmartAgroPlan.WebAPI.Utils
{
    public static class SettingsExtractor
    {
        public static CorsSettings GetCorsSettings(ConfigurationManager configuration)
        {
            return new CorsSettings
            {
                AllowedOrigins = GetAllowedCorsValues(configuration, "AllowedOrigins"),
                AllowedHeaders = GetAllowedCorsValues(configuration, "AllowedHeaders"),
                AllowedMethods = GetAllowedCorsValues(configuration, "AllowedMethods"),
                ExposedHeaders = GetAllowedCorsValues(configuration, "ExposedHeaders"),
                PreflightMaxAgeInSeconds = int.Parse(configuration.GetValue<string>("CORS:PreflightMaxAge") ?? "600")
            };
        }

        private static string[] GetAllowedCorsValues(IConfiguration configuration, string key)
        {
            string? allowedCorsValuesStringified = configuration.GetValue<string>($"CORS:{key}");
            return (allowedCorsValuesStringified ?? "*")
                .Split(',', StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
