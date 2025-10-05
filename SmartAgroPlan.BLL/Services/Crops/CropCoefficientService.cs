using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.Interfaces.Crops;
using SmartAgroPlan.DAL.Enums;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.Services.Crops;

public class CropCoefficientService : ICropCoefficientService
{
    private readonly ILogger<CropCoefficientService> _logger;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public CropCoefficientService(
        IRepositoryWrapper repositoryWrapper,
        ILogger<CropCoefficientService> logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public double GetKc(CropType cropType, DateTime plantingDate, DateTime targetDate)
    {
        var definition = _repositoryWrapper.CropCoefficientDefinitionRepository
            .GetFirstOrDefaultAsync(ccd => ccd.CropType == cropType).Result;

        if (definition == null)
        {
            var errorMsg = $"Не вдалося знайти визначення коефіцієнта культури для типу культури {cropType}";
            _logger.LogError(errorMsg);
            throw new ArgumentException(errorMsg);
        }

        var daysSincePlanting = (int)(targetDate - plantingDate).TotalDays;

        var lIni = definition.LIni;
        var lDev = definition.LDev;
        var lMid = definition.LMid;
        var lLate = definition.LLate;
        var totalGrowthPeriod = lIni + lDev + lMid + lLate;

        if (daysSincePlanting >= totalGrowthPeriod) return definition.KcEnd;

        if (daysSincePlanting <= lIni || daysSincePlanting <= 0)
            return definition.KcIni;

        // development: linear from KcIni -> KcMid
        daysSincePlanting -= lIni;
        if (daysSincePlanting <= lDev)
        {
            var frac = (double)daysSincePlanting / lDev;
            return definition.KcIni + frac * (definition.KcMid - definition.KcIni);
        }

        // mid season: constant KcMid
        daysSincePlanting -= lDev;
        if (daysSincePlanting <= lMid)
            return definition.KcMid;

        // late season: linear KcMid -> KcEnd
        var fracLate = (double)daysSincePlanting / lLate;
        return definition.KcMid + fracLate * (definition.KcEnd - definition.KcMid);
    }
}
