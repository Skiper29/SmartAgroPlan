using SmartAgroPlan.BLL.Interfaces.Crops;
using SmartAgroPlan.DAL.Entities.Crops;

namespace SmartAgroPlan.BLL.Services.Crops;

public class CropCoefficientService : ICropCoefficientService
{
    public double GetKc(CropCoefficientDefinition definition, DateTime plantingDate, DateTime targetDate)
    {
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
