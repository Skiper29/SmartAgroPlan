using AutoMapper;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Analysis;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Planning;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Nutrients;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Products;
using SmartAgroPlan.BLL.Models.FertilizerForecasting;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting;

namespace SmartAgroPlan.BLL.Mapping.FertilizerForecasting;

public class FertilizerForecastingProfile : Profile
{
    public FertilizerForecastingProfile()
    {
        CreateMap<NutrientRequirement, NutrientRequirementDto>().ReverseMap();
        CreateMap<FertilizerProduct
            , FertilizerProductDto>().ReverseMap();

        CreateMap<FertilizerApplication, FertilizerApplicationDto>()
            .ForMember(dest => dest.Products, opt => opt.Ignore());

        CreateMap<SeasonFertilizerPlan, SeasonFertilizerPlanDto>();
        CreateMap<CurrentRecommendation, CurrentRecommendationDto>();
        CreateMap<NutrientBalance, NutrientBalanceDto>();
        CreateMap<NutrientApplicationSummary, NutrientApplicationSummaryDto>()
            .ForMember(dest => dest.Applications, opt => opt.MapFrom(src => src.Applications));

        CreateMap<ApplicationSummaryItem, ApplicationSummaryItemDto>();
    }
}
