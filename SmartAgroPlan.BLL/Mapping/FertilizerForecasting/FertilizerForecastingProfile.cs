using AutoMapper;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Application;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Nutrients;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Planning;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Products;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.SoilConditions;
using SmartAgroPlan.BLL.Models.FertilizerForecasting;
using SmartAgroPlan.DAL.Entities.Fields;

namespace SmartAgroPlan.BLL.Mapping.FertilizerForecasting;

public class FertilizerForecastingProfile : Profile
{
    public FertilizerForecastingProfile()
    {
        // SeasonFertilizerPlan mappings
        CreateMap<SeasonFertilizerPlan, SeasonFertilizerPlanDto>()
            .ForMember(dest => dest.PlannedApplications, opt => opt.MapFrom(src => src.Applications))
            .ForMember(dest => dest.FieldName, opt => opt.Ignore())
            .ForMember(dest => dest.SowingDate, opt => opt.Ignore())
            .ForMember(dest => dest.ExpectedHarvestDate, opt => opt.Ignore())
            .ForMember(dest => dest.CostEstimate, opt => opt.Ignore());

        // NutrientRequirement mappings
        CreateMap<NutrientRequirement, NutrientRequirementDto>().ReverseMap();

        // FertilizerApplication mappings
        CreateMap<FertilizerApplication, FertilizerApplicationDto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsApplied, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.ActualApplicationDate, opt => opt.Ignore());

        // FertilizerProduct mappings
        CreateMap<FertilizerProduct, FertilizerProductDto>()
            .ForMember(dest => dest.EstimatedCostPerKg, opt => opt.Ignore())
            .ForMember(dest => dest.TotalCost, opt => opt.Ignore());

        // CurrentRecommendation mappings
        CreateMap<CurrentRecommendation, CurrentRecommendationDto>()
            .ForMember(dest => dest.GeneratedDate, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.CurrentGrowthStage, opt => opt.MapFrom(src => src.CurrentStage))
            .ForMember(dest => dest.ActionRequired, opt => opt.MapFrom(src => src.ShouldApplyNow))
            .ForMember(dest => dest.RecommendedProducts, opt => opt.MapFrom(src => src.Products))
            .ForMember(dest => dest.FieldId, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentSoilCondition, opt => opt.Ignore())
            .ForMember(dest => dest.WeatherImpact, opt => opt.Ignore());

        CreateMap<FieldCondition, SoilConditionSummaryDto>()
            .ForMember(dest => dest.LastRecordedAt, opt => opt.MapFrom(src => src.RecordedAt));
    }
}
