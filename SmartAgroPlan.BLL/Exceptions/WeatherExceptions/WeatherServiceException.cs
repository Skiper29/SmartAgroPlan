namespace SmartAgroPlan.BLL.Exceptions.WeatherExceptions;

public class WeatherServiceException : Exception
{
    public WeatherServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
