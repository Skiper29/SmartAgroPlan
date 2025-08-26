namespace SmartAgroPlan.WebAPI.Utils.Settings
{
    public class CorsSettings
    {
        public string[] AllowedOrigins { get; set; } = null!;
        public string[] AllowedHeaders { get; set; } = null!;
        public string[] AllowedMethods { get; set; } = null!;
        public string[] ExposedHeaders { get; set; } = null!;
        public int PreflightMaxAgeInSeconds { get; set; }
    }
}
