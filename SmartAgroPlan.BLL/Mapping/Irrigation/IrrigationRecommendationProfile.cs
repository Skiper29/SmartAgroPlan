using AutoMapper;
using SmartAgroPlan.BLL.DTO.Irrigation;
using SmartAgroPlan.BLL.Models.Weather;
using SmartAgroPlan.DAL.Entities.Irrigation;

namespace SmartAgroPlan.BLL.Mapping.Irrigation;

public class IrrigationRecommendationProfile : Profile
{
    public IrrigationRecommendationProfile()
    {
        CreateMap<IrrigationRecommendation, IrrigationRecommendationDto>()
            .ForMember(dest => dest.WeatherConditions, opt => opt.Ignore())
            .ForMember(dest => dest.Forecast, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<WeatherData, WeatherConditionsDto>();
    }
}
