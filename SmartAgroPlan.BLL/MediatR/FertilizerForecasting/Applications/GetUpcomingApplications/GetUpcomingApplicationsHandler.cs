using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Nutrients;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Planning;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Products;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.FertilizerForecasting.Applications.GetUpcomingApplications;

public class
    GetUpcomingApplicationsHandler : IRequestHandler<GetUpcomingApplicationsQuery,
    Result<List<FertilizerApplicationDto>>>
{
    private readonly ILogger<GetUpcomingApplicationsHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;

    public GetUpcomingApplicationsHandler(
        IRepositoryWrapper repository,
        IMapper mapper,
        ILogger<GetUpcomingApplicationsHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<FertilizerApplicationDto>>> Handle(
        GetUpcomingApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Отримання майбутніх заявок на внесення добрив для поля {FieldId} на наступні {DaysAhead} днів",
                request.FieldId,
                request.DaysAhead);

            var startDate = DateTime.Now;
            var endDate = startDate.AddDays(request.DaysAhead);

            var plans = await _repository.FertilizerApplicationPlanRepository.FindAll(
                a => a.FieldId == request.FieldId
                     && !a.IsCompleted
                     && a.PlannedApplicationDate >= startDate
                     && a.PlannedApplicationDate <= endDate,
                q => q.Include(a => a.PlanStage!)
                    .ThenInclude(s => s.ApplicationMethod)
                    .Include(a => a.Products)
                    .ThenInclude(p => p.FertilizerProduct)!
            ).ToListAsync(cancellationToken);

            var applications = plans.Select(p => new FertilizerApplicationDto
            {
                Id = p.Id,
                RecommendedDate = p.PlannedApplicationDate,
                CropStage = p.PlanStage?.StageName ?? "N/A",
                DaysAfterPlanting = p.DaysAfterPlanting,
                NutrientsToApply = new NutrientRequirementDto
                {
                    Nitrogen = p.PlannedNitrogen,
                    Phosphorus = p.PlannedPhosphorus,
                    Potassium = p.PlannedPotassium,
                    Sulfur = p.PlannedSulfur,
                    Calcium = p.PlannedCalcium,
                    Magnesium = p.PlannedMagnesium,
                    Boron = p.PlannedBoron,
                    Zinc = p.PlannedZinc,
                    Manganese = p.PlannedManganese,
                    Copper = p.PlannedCopper,
                    Iron = p.PlannedIron,
                    Molybdenum = p.PlannedMolybdenum
                },
                Products = p.Products.Select(pr => _mapper.Map<FertilizerProductDto>(pr.FertilizerProduct)).ToList(),
                ApplicationMethod = p.PlanStage?.ApplicationMethod?.Name ?? "N/A",
                Rationale = p.PlanStage?.Rationale ?? "",
                IsCompleted = p.IsCompleted
            }).ToList();

            _logger.LogInformation(
                "Отримано {ApplicationCount} майбутніх заявок на внесення добрив для поля {FieldId}",
                applications.Count,
                request.FieldId);

            return Result.Ok(applications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Помилка при отриманні майбутніх заявок на внесення добрив для поля {FieldId} на наступні {DaysAhead} днів",
                request.FieldId,
                request.DaysAhead);
            return Result.Fail(new Error("Помилка при отриманні майбутніх заявок на внесення добрив."));
        }
    }
}
