using SmartAgroPlan.BLL.Models.FertilizerForecasting;
using SmartAgroPlan.BLL.Models.FertilizerForecasting.Nutrients;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.BLL.Utils;

/// <summary>
///     Генератор рекомендацій для системи удобрення
/// </summary>
public static class FertilizerRecommendationGenerator
{
    /// <summary>
    ///     Генерує рекомендації на основі балансу поживних речовин
    /// </summary>
    public static List<string> GenerateBalanceRecommendations(
        NutrientRequirement deficit,
        int daysToHarvest,
        CropType? cropType = null,
        double? fieldAreaHa = null,
        GrowthStage? currentStage = null,
        double? soilAcidity = null)
    {
        var recommendations = new List<string>();

        // Основні макроелементи
        AddNitrogenRecommendations(recommendations, deficit, daysToHarvest, cropType);
        AddPhosphorusRecommendations(recommendations, deficit, daysToHarvest, fieldAreaHa);
        AddPotassiumRecommendations(recommendations, deficit, daysToHarvest, currentStage);

        // Вторинні елементи
        AddSulfurRecommendations(recommendations, deficit, cropType);
        AddCalciumRecommendations(recommendations, deficit, soilAcidity);
        AddMagnesiumRecommendations(recommendations, deficit, soilAcidity);

        // Мікроелементи
        AddMicronutrientRecommendations(recommendations, deficit, soilAcidity, currentStage);

        // Комплексні рекомендації
        AddComplexRecommendations(recommendations, deficit, daysToHarvest, currentStage);

        // Рекомендації щодо термінів внесення
        AddTimingRecommendations(recommendations, daysToHarvest, currentStage);

        return recommendations;
    }

    /// <summary>
    ///     Генерує рекомендації щодо усунення дефіциту поживних речовин
    /// </summary>
    public static List<string> GenerateDeficitRecommendations(
        List<NutrientDeficit> deficits,
        CropType? cropType = null,
        int? daysToHarvest = null)
    {
        var recommendations = new List<string>();

        var highPriorityDeficits = deficits.Where(d => d.Urgency == "High").ToList();
        var mediumPriorityDeficits = deficits.Where(d => d.Urgency == "Medium").ToList();

        if (highPriorityDeficits.Any())
        {
            recommendations.Add("🔴 ТЕРМІНОВІ ДІЇ:");
            foreach (var deficit in highPriorityDeficits)
            {
                var productRecommendation = GetProductRecommendation(deficit.NutrientName, deficit.DeficitAmount);
                recommendations.Add(
                    $"  • Негайно внести {deficit.DeficitAmount:F1} кг/га {deficit.NutrientName}. {productRecommendation}");
            }
        }

        if (mediumPriorityDeficits.Any())
        {
            recommendations.Add("🟡 РЕКОМЕНДОВАНІ ДІЇ:");
            foreach (var deficit in mediumPriorityDeficits)
            {
                var productRecommendation = GetProductRecommendation(deficit.NutrientName, deficit.DeficitAmount);
                recommendations.Add(
                    $"  • Внести {deficit.DeficitAmount:F1} кг/га {deficit.NutrientName} при наступному підживленні. {productRecommendation}");
            }
        }

        // Додаткові рекомендації залежно від культури
        if (cropType.HasValue) AddCropSpecificRecommendations(recommendations, cropType.Value, deficits);

        return recommendations;
    }

    /// <summary>
    ///     Генерує загальні агрономічні рекомендації
    /// </summary>
    public static List<string> GenerateAgronomicRecommendations(
        NutrientRequirement deficit,
        NutrientRequirement surplus,
        CropType cropType,
        GrowthStage currentStage,
        double? soilAcidity = null,
        double? soilOrganicMatter = null)
    {
        var recommendations = new List<string>();

        // Рекомендації щодо pH
        if (soilAcidity.HasValue)
        {
            if (soilAcidity < 5.5)
                recommendations.Add(
                    $"💡 Ґрунт кислий (pH {soilAcidity:F1}). Проведіть вапнування доломітовим вапном: 3-5 т/га для підвищення pH до оптимального рівня 6.0-7.0.");
            else if (soilAcidity > 7.5)
                recommendations.Add(
                    $"💡 Ґрунт лужний (pH {soilAcidity:F1}). Використовуйте фізіологічно кислі добрива (сульфат амонію) або внесіть гіпс для покращення засвоєння поживних речовин.");
        }

        // Рекомендації щодо органічної речовини
        if (soilOrganicMatter.HasValue)
        {
            if (soilOrganicMatter < 2.5)
                recommendations.Add(
                    $"💡 Низький вміст органічної речовини ({soilOrganicMatter:F1}%). Внесіть 30-40 т/га гною або компосту для покращення родючості ґрунту.");
            else if (soilOrganicMatter < 3.5)
                recommendations.Add(
                    $"💡 Помірний вміст органічної речовини ({soilOrganicMatter:F1}%). Рекомендується внесення 20-30 т/га органіки під наступну культуру.");
        }

        // Рекомендації залежно від стадії росту
        AddStageSpecificRecommendations(recommendations, currentStage, cropType, deficit);

        // Рекомендації щодо способів внесення
        AddApplicationMethodRecommendations(recommendations, currentStage, deficit);

        return recommendations;
    }

    #region Micronutrient Recommendations

    private static void AddMicronutrientRecommendations(
        List<string> recommendations,
        NutrientRequirement deficit,
        double? soilAcidity,
        GrowthStage? currentStage)
    {
        var microDeficits = new List<string>();

        if (deficit.Boron > 0.3)
            microDeficits.Add($"бору ({deficit.Boron:F2} кг/га)");
        if (deficit.Zinc > 0.5)
            microDeficits.Add($"цинку ({deficit.Zinc:F2} кг/га)");
        if (deficit.Manganese > 0.8)
            microDeficits.Add($"марганцю ({deficit.Manganese:F2} кг/га)");
        if (deficit.Copper > 0.15)
            microDeficits.Add($"міді ({deficit.Copper:F2} кг/га)");
        if (deficit.Iron > 1.0)
            microDeficits.Add($"заліза ({deficit.Iron:F2} кг/га)");
        if (deficit.Molybdenum > 0.02)
            microDeficits.Add($"молібдену ({deficit.Molybdenum:F3} кг/га)");

        if (microDeficits.Any())
        {
            recommendations.Add($"✅ Виявлено дефіцит мікроелементів: {string.Join(", ", microDeficits)}.");

            if (currentStage.HasValue &&
                (currentStage == GrowthStage.Development || currentStage == GrowthStage.MidSeason))
                recommendations.Add(
                    "💡 Найбільш ефективним буде позакореневе підживлення комплексними мікродобривами у хелатній формі.");
            else
                recommendations.Add(
                    "💡 Рекомендується позакореневе підживлення мікроелементами у хелатній формі (2-3 обробки з інтервалом 10-14 днів).");

            if (soilAcidity.HasValue && soilAcidity > 7.5)
                recommendations.Add(
                    "⚠️ Лужний ґрунт обмежує доступність мікроелементів. Обов'язково використовуйте хелатні форми (EDTA, DTPA).");
        }

        // Специфічні рекомендації
        if (deficit.Boron > 0.3)
            recommendations.Add(
                "💡 Бор: Використовуйте борну кислоту (17% B) або буру для ґрунтового внесення 1-2 кг/га, або 0.05-0.1% розчин для обприскування.");

        if (deficit.Zinc > 0.5 && soilAcidity.HasValue && soilAcidity > 7.0)
            recommendations.Add(
                "💡 Цинк: При високому pH використовуйте цинк-хелат EDTA. Для позакореневого підживлення - сульфат цинку 0.3-0.5%.");

        if (deficit.Iron > 1.0)
            recommendations.Add(
                "💡 Залізо: Використовуйте хелат заліза (Fe-EDDHA для дуже лужних ґрунтів, Fe-EDTA для слаболужних). Позакореневе підживлення - 2-3 обробки.");
    }

    #endregion

    #region Macronutrient Recommendations

    private static void AddNitrogenRecommendations(
        List<string> recommendations,
        NutrientRequirement deficit,
        int daysToHarvest,
        CropType? cropType)
    {
        if (deficit.Nitrogen > 50 && daysToHarvest > 30)
            recommendations.Add(
                $"✅ Критичний дефіцит азоту ({deficit.Nitrogen:F1} кг/га). Негайно внесіть 2/3 норми карбамідом або аміачною селітрою, залишок - через 10-14 днів.");
        else if (deficit.Nitrogen > 40 && daysToHarvest > 30)
            recommendations.Add(
                $"✅ Значний дефіцит азоту ({deficit.Nitrogen:F1} кг/га). Рекомендується дробне внесення: 60% зараз, 40% через 2 тижні.");
        else if (deficit.Nitrogen > 20 && daysToHarvest > 30)
            recommendations.Add(
                $"✅ Дефіцит азоту ({deficit.Nitrogen:F1} кг/га). Внесіть аміачну селітру 34% (N) або карбамід 46% (N).");
        else if (deficit.Nitrogen > 10 && daysToHarvest > 14)
            recommendations.Add(
                $"✅ Помірний дефіцит азоту ({deficit.Nitrogen:F1} кг/га). Розгляньте позакореневе підживлення карбамідом 5% розчином.");

        if (deficit.Nitrogen > 20 && daysToHarvest <= 30)
            recommendations.Add(
                "⚠️ До збору врожаю менше місяця. Позакореневе підживлення карбамідом буде найбільш ефективним.");

        // Спеціальні рекомендації для бобових
        if (cropType.HasValue && cropType == CropType.Soy)
            if (deficit.Nitrogen > 10)
                recommendations.Add(
                    "💡 Для бобових культур: забезпечте достатність молібдену та кобальту для ефективної азотфіксації. Можна знизити норми азотних добрив.");
    }

    private static void AddPhosphorusRecommendations(
        List<string> recommendations,
        NutrientRequirement deficit,
        int daysToHarvest,
        double? fieldAreaHa)
    {
        if (deficit.Phosphorus > 50)
        {
            var dapAmount = deficit.Phosphorus / 0.46; // DAP містить 46% P2O5
            recommendations.Add(
                $"✅ Критичний дефіцит фосфору ({deficit.Phosphorus:F1} кг/га P₂O₅). Внесіть діамонійфосфат (DAP): {dapAmount:F0} кг/га під основний обробіток.");
        }
        else if (deficit.Phosphorus > 30)
        {
            recommendations.Add(
                $"✅ Значний дефіцит фосфору ({deficit.Phosphorus:F1} кг/га P₂O₅). Рекомендується внесення суперфосфату або DAP під основний обробіток.");
        }
        else if (deficit.Phosphorus > 15)
        {
            recommendations.Add(
                $"✅ Дефіцит фосфору ({deficit.Phosphorus:F1} кг/га P₂O₅). Внесіть суперфосфат простий 20% (P₂O₅) або подвійний 40% (P₂O₅).");
        }
        else if (deficit.Phosphorus > 5 && daysToHarvest < 60)
        {
            recommendations.Add(
                "✅ Помірний дефіцит фосфору. Ефективним буде позакореневе підживлення монокалійфосфатом 0.3-0.5% розчином.");
        }

        if (deficit.Phosphorus > 30)
            recommendations.Add(
                "💡 Фосфор малорухомий у ґрунті. Вносьте локально (стрічковим способом) або під загортання для кращої ефективності.");
    }

    private static void AddPotassiumRecommendations(
        List<string> recommendations,
        NutrientRequirement deficit,
        int daysToHarvest,
        GrowthStage? currentStage)
    {
        if (deficit.Potassium > 60)
            recommendations.Add(
                $"✅ Критичний дефіцит калію ({deficit.Potassium:F1} кг/га K₂O). Негайно внесіть калій хлористий або сульфат калію у 2 прийоми.");
        else if (deficit.Potassium > 40)
            recommendations.Add(
                $"✅ Значний дефіцит калію ({deficit.Potassium:F1} кг/га K₂O). Рекомендується калій хлористий 60% (K₂O) або сульфат калію 50% (K₂O).");
        else if (deficit.Potassium > 20)
            recommendations.Add(
                $"✅ Дефіцит калію ({deficit.Potassium:F1} кг/га K₂O). Внесіть калійні добрива під підживлення або з поливною водою.");

        if (currentStage.HasValue && currentStage == GrowthStage.MidSeason && deficit.Potassium > 30)
            recommendations.Add(
                "💡 Середина вегетації - критичний період для калію. Позакореневе підживлення монокалійфосфатом або калій сульфатом підвищить якість врожаю.");

        if (daysToHarvest <= 30 && deficit.Potassium > 20)
            recommendations.Add(
                "💡 Калій покращує якість та лежкість продукції. Позакореневе підживлення сульфатом калію 1-2% розчином буде ефективним.");
    }

    #endregion

    #region Secondary Nutrient Recommendations

    private static void AddSulfurRecommendations(
        List<string> recommendations,
        NutrientRequirement deficit,
        CropType? cropType)
    {
        if (deficit.Sulfur > 15)
            recommendations.Add(
                $"✅ Дефіцит сірки ({deficit.Sulfur:F1} кг/га S). Внесіть сульфат амонію (24% S) або елементарну сірку.");
        else if (deficit.Sulfur > 10)
            recommendations.Add(
                $"✅ Помірний дефіцит сірки ({deficit.Sulfur:F1} кг/га S). Використовуйте сірковмісні добрива: сульфат амонію або калію магнію сульфат.");

        // Спеціальні рекомендації для культур, що потребують багато сірки
        if (cropType.HasValue && (cropType == CropType.Rapeseed || cropType == CropType.Corn))
            if (deficit.Sulfur > 5)
                recommendations.Add(
                    $"💡 {GetCropName(cropType.Value)} має підвищену потребу в сірці. Забезпечте достатнє внесення сірковмісних добрив.");
    }

    private static void AddCalciumRecommendations(
        List<string> recommendations,
        NutrientRequirement deficit,
        double? soilAcidity)
    {
        if (deficit.Calcium > 40)
            recommendations.Add(
                $"✅ Значний дефіцит кальцію ({deficit.Calcium:F1} кг/га Ca). Внесіть вапно або гіпс: 2-4 т/га.");
        else if (deficit.Calcium > 20)
            recommendations.Add(
                $"✅ Дефіцит кальцію ({deficit.Calcium:F1} кг/га Ca). Рекомендується кальцієва селітра або позакореневе підживлення хлоридом кальцію 0.3-0.5%.");

        if (deficit.Calcium > 20 && soilAcidity.HasValue && soilAcidity < 6.0)
            recommendations.Add(
                "💡 Кислий ґрунт. Вапнування доломітовим вапном вирішить проблему дефіциту кальцію та магнію одночасно.");
    }

    private static void AddMagnesiumRecommendations(
        List<string> recommendations,
        NutrientRequirement deficit,
        double? soilAcidity)
    {
        if (deficit.Magnesium > 20)
            recommendations.Add(
                $"✅ Значний дефіцит магнію ({deficit.Magnesium:F1} кг/га Mg). Внесіть доломітове вапно або сульфат магнію (кізерит).");
        else if (deficit.Magnesium > 12)
            recommendations.Add(
                $"✅ Дефіцит магнію ({deficit.Magnesium:F1} кг/га Mg). Рекомендується позакореневе підживлення сульфатом магнію 1-2% розчином.");
        else if (deficit.Magnesium > 8)
            recommendations.Add(
                "✅ Помірний дефіцит магнію. Позакореневе підживлення сульфатом або нітратом магнію буде ефективним.");

        if (deficit.Magnesium > 12 && soilAcidity.HasValue && soilAcidity < 6.0)
            recommendations.Add(
                "💡 При кислому ґрунті використовуйте доломітове вапно - воно постачає і кальцій, і магній.");
    }

    #endregion

    #region Complex and Timing Recommendations

    private static void AddComplexRecommendations(
        List<string> recommendations,
        NutrientRequirement deficit,
        int daysToHarvest,
        GrowthStage? currentStage)
    {
        // Комплексне добриво при множинному дефіциті
        if (deficit.Nitrogen > 20 && deficit.Phosphorus > 15 && deficit.Potassium > 20)
            recommendations.Add(
                "💡 Комплексний дефіцит NPK. Рекомендується застосування повного мінерального добрива (наприклад, нітроамофоска 16:16:16 або аналоги).");

        // Рекомендації для стартових добрив
        if (currentStage.HasValue && currentStage == GrowthStage.Initial)
            if (deficit.Phosphorus > 10)
                recommendations.Add(
                    "💡 Початкова стадія: внесіть стартове добриво з високим вмістом фосфору (10:40:10 або DAP) для стимуляції росту кореневої системи.");

        // Рекомендації для критичних періодів
        if (currentStage.HasValue && currentStage == GrowthStage.MidSeason)
            recommendations.Add(
                "💡 Критична фаза: максимальна потреба у всіх елементах живлення. Забезпечте збалансоване постачання NPK та мікроелементів.");
    }

    private static void AddTimingRecommendations(
        List<string> recommendations,
        int daysToHarvest,
        GrowthStage? currentStage)
    {
        if (daysToHarvest > 60)
            recommendations.Add(
                "⏱️ Оптимальний час для коригувальних підживлень. Можна використовувати ґрунтове внесення добрив.");
        else if (daysToHarvest > 30)
            recommendations.Add(
                "⏱️ До збору врожаю залишилось 1-2 місяці. Пріоритет - позакореневим підживленням та швидкодіючим формам добрив.");
        else if (daysToHarvest > 14)
            recommendations.Add(
                "⏱️ До збору врожаю менше місяця. Використовуйте тільки позакореневі підживлення калієм та мікроелементами.");
        else
            recommendations.Add(
                "⏱️ Близько до збору врожаю. Уникайте внесення азоту. За потреби - тільки легке підживлення калієм для покращення якості.");

        // Найкращий час доби для обробок
        if (currentStage.HasValue && (currentStage == GrowthStage.Development || currentStage == GrowthStage.MidSeason))
            recommendations.Add(
                "🕐 Позакореневі обробки проводьте вранці (6-10 год) або ввечері (17-20 год) при температурі не вище 25°C та вологості повітря >60%.");
    }

    private static void AddStageSpecificRecommendations(
        List<string> recommendations,
        GrowthStage currentStage,
        CropType cropType,
        NutrientRequirement deficit)
    {
        switch (currentStage)
        {
            case GrowthStage.Initial:
                recommendations.Add(
                    "🌱 Початкова стадія: пріоритет - фосфорні добрива для розвитку кореневої системи та азот для росту.");
                if (deficit.Phosphorus > 10 || deficit.Nitrogen > 15)
                    recommendations.Add(
                        "💡 Використовуйте стартові добрива з підвищеним вмістом фосфору, можна вносити локально при сівбі.");
                break;

            case GrowthStage.Development:
                recommendations.Add(
                    "🌿 Стадія активного росту: максимальна потреба в азоті та калії для наростання вегетативної маси.");
                if (deficit.Nitrogen > 20)
                    recommendations.Add(
                        "💡 Проведіть основне азотне підживлення. Можливе дробне внесення для підвищення ефективності.");
                break;

            case GrowthStage.MidSeason:
                recommendations.Add(
                    "🌾 Критична фаза (цвітіння/формування врожаю): збалансоване живлення NPK + мікроелементи.");
                recommendations.Add("💡 Особливо важливі калій, сірка, бор та цинк для формування якісного врожаю.");
                break;

            case GrowthStage.LateSeason:
                recommendations.Add("🌾 Дозрівання: калій та мікроелементи для якості врожаю. Обмежте азот.");
                recommendations.Add(
                    "💡 Позакореневі підживлення калієм покращать якість, лежкість та товарний вигляд продукції.");
                break;
        }
    }

    private static void AddApplicationMethodRecommendations(
        List<string> recommendations,
        GrowthStage? currentStage,
        NutrientRequirement deficit)
    {
        if (!currentStage.HasValue) return;

        recommendations.Add("📋 СПОСОБИ ВНЕСЕННЯ:");

        if (currentStage == GrowthStage.PreSowing || currentStage == GrowthStage.Initial)
        {
            recommendations.Add(
                "  • Основне внесення: розкидання з загортанням на глибину 10-15 см (NPK, вапно, органіка)");
            recommendations.Add(
                "  • Припосівне: стрічковим способом на 3-5 см збоку та 3-5 см глибше від насіння (P-добрива)");
        }
        else if (currentStage == GrowthStage.Development || currentStage == GrowthStage.MidSeason)
        {
            if (deficit.Nitrogen > 20 || deficit.Potassium > 20)
                recommendations.Add(
                    "  • Підживлення: внесення в міжряддя з загортанням або фертигація через систему зрошення");

            recommendations.Add(
                "  • Позакореневе: обприскування 2-3% розчином для макроелементів, 0.01-0.1% для мікроелементів");
            recommendations.Add("  • Оптимальна витрата робочого розчину: 200-300 л/га");
        }
        else if (currentStage == GrowthStage.LateSeason)
        {
            recommendations.Add("  • Тільки позакореневе підживлення легкозасвоюваними формами добрив");
        }
    }

    private static void AddCropSpecificRecommendations(
        List<string> recommendations,
        CropType cropType,
        List<NutrientDeficit> deficits)
    {
        var cropName = GetCropName(cropType);

        switch (cropType)
        {
            case CropType.Wheat:
            case CropType.Barley:
                if (deficits.Any(d => d.NutrientName.Contains("Азот")))
                    recommendations.Add(
                        $"💡 {cropName}: Критичні періоди для азоту - кущення та вихід у трубку. Забезпечте достатність азоту в ці фази.");
                break;

            case CropType.Corn:
                if (deficits.Any(d => d.NutrientName.Contains("Цинк")))
                    recommendations.Add(
                        $"💡 {cropName}: Особливо чутлива до дефіциту цинку. Обробка насіння або позакореневе підживлення обов'язкові.");
                if (deficits.Any(d => d.NutrientName.Contains("Азот")))
                    recommendations.Add(
                        $"💡 {cropName}: Висока потреба в азоті. Дробне внесення (3-4 рази) підвищить ефективність.");
                break;

            case CropType.Sunflower:
                if (deficits.Any(d => d.NutrientName.Contains("Бор")))
                    recommendations.Add(
                        $"💡 {cropName}: Критична потреба в борі під час цвітіння для запліднення. Позакореневі обробки обов'язкові.");
                break;

            case CropType.Rapeseed:
                if (deficits.Any(d => d.NutrientName.Contains("Сірка")))
                    recommendations.Add(
                        $"💡 {cropName}: Дуже висока потреба в сірці (співвідношення N:S має бути 5-6:1).");
                if (deficits.Any(d => d.NutrientName.Contains("Бор")))
                    recommendations.Add($"💡 {cropName}: Бор критично важливий для цвітіння та формування стручків.");
                break;

            case CropType.Soy:
                if (deficits.Any(d => d.NutrientName.Contains("Молібден")))
                    recommendations.Add(
                        $"💡 {cropName}: Молібден необхідний для азотфіксації. Обробка насіння молібденом обов'язкова.");
                break;

            case CropType.Potato:
            case CropType.Tomato:
                if (deficits.Any(d => d.NutrientName.Contains("Кальцій")))
                    recommendations.Add(
                        $"💡 {cropName}: Дефіцит кальцію призводить до верхівкової гнилі. Позакореневі обробки кальцієм у період росту плодів.");
                break;
        }
    }

    #endregion

    #region Helper Methods

    private static string GetProductRecommendation(string nutrientName, double amount)
    {
        return nutrientName switch
        {
            var n when n.Contains("Азот") =>
                $"Варіанти: карбамід 46% (N) - {amount / 0.46:F0} кг/га, аміачна селітра 34% (N) - {amount / 0.34:F0} кг/га.",
            var p when p.Contains("Фосфор") =>
                $"Варіанти: суперфосфат подвійний 40% (P₂O₅) - {amount / 0.40:F0} кг/га, DAP 46% (P₂O₅) - {amount / 0.46:F0} кг/га.",
            var k when k.Contains("Калій") =>
                $"Варіанти: калій хлористий 60% (K₂O) - {amount / 0.60:F0} кг/га, сульфат калію 50% (K₂O) - {amount / 0.50:F0} кг/га.",
            _ => "Використовуйте відповідні мінеральні добрива."
        };
    }

    private static string GetCropName(CropType cropType)
    {
        return cropType switch
        {
            CropType.Wheat => "Пшениця",
            CropType.Corn => "Кукурудза",
            CropType.Barley => "Ячмінь",
            CropType.Sunflower => "Соняшник",
            CropType.Soy => "Соя",
            CropType.Rapeseed => "Ріпак",
            CropType.Potato => "Картопля",
            CropType.SugarBeet => "Цукровий буряк",
            CropType.Tomato => "Томат",
            _ => "Культура"
        };
    }

    #endregion
}
