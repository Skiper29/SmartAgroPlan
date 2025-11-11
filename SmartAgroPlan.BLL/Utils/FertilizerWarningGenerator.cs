using SmartAgroPlan.BLL.Models.FertilizerForecasting;
using SmartAgroPlan.BLL.Models.FertilizerForecasting.Nutrients;
using SmartAgroPlan.DAL.Entities.Fields;

namespace SmartAgroPlan.BLL.Utils;

/// <summary>
/// Генератор попереджень для системи удобрення
/// </summary>
public static class FertilizerWarningGenerator
{
    /// <summary>
    /// Генерує попередження на основі поточного стану поля та стадії росту культури
    /// </summary>
    public static List<string> GenerateFieldWarnings(
        Field field,
        string currentStage,
        int daysToHarvest,
        double? soilMoisture = null,
        double? soilPh = null,
        double? temperature = null)
    {
        var warnings = new List<string>();

        // Перевірка на завершення вегетації
        if (currentStage == "Після збору врожаю" || daysToHarvest < 0)
        {
            warnings.Add("Культура вже зібрана. Внесення добрив не рекомендується для поточної культури.");
            return warnings;
        }

        // Попередження про близький збір врожаю
        if (daysToHarvest < 14)
            warnings.Add("Близько до збору врожаю. Уникайте внесення азоту для запобігання вилягання та погіршення якості зерна.");
        else if (daysToHarvest < 21)
            warnings.Add("До збору врожаю залишилось менше трьох тижнів. Рекомендується лише позакореневе підживлення мікроелементами.");

        // Попередження про вологість ґрунту
        var moisture = soilMoisture ?? field.Conditions?.FirstOrDefault()?.SoilMoisture ?? 0;
        if (moisture < 0.15)
            warnings.Add("Критично низька вологість ґрунту (<15%). Внесення добрив буде неефективним до поповнення вологи.");
        else if (moisture < 0.2)
            warnings.Add("Низька вологість ґрунту може значно знизити ефективність добрив. Рекомендується полив або очікування опадів.");
        else if (moisture > 0.9)
            warnings.Add("Надмірна вологість ґрунту (>90%). Відкладіть внесення добрив до покращення умов для уникнення змиву та денітрифікації.");
        else if (moisture > 0.8)
            warnings.Add("Висока вологість ґрунту може призвести до вимивання азоту. Розгляньте застосування інгібіторів нітрифікації.");

        // Попередження про кислотність ґрунту
        var ph = soilPh ?? field.Soil?.Acidity ?? 0;
        if (ph > 0)
        {
            if (ph < 5.0)
                warnings.Add($"Дуже кислий ґрунт (pH {ph:F1}). Внесення вапна обов'язкове для покращення засвоєння поживних речовин.");
            else if (ph < 5.5)
                warnings.Add($"Кислий ґрунт (pH {ph:F1}). Рекомендується вапнування та застосування фізіологічно лужних добрив.");
            else if (ph > 8.0)
                warnings.Add($"Лужний ґрунт (pH {ph:F1}). Обмежене засвоєння мікроелементів (Zn, Fe, Mn). Використовуйте хелатні форми.");
            else if (ph > 7.5)
                warnings.Add($"Слаболужний ґрунт (pH {ph:F1}). Можливі проблеми із засвоєнням фосфору та мікроелементів.");
        }

        // Попередження про температурні умови
        if (temperature.HasValue)
        {
            if (temperature < 5)
                warnings.Add($"Низька температура ґрунту ({temperature:F1}°C). Мінералізація добрив та їх засвоєння рослинами уповільнені.");
            else if (temperature < 10)
                warnings.Add($"Прохолодна температура ґрунту ({temperature:F1}°C). Ефективність азотних добрив знижена.");
            else if (temperature > 30)
                warnings.Add($"Висока температура ґрунту ({temperature:F1}°C). Ризик втрат азоту через випаровування аміаку.");
        }

        // Попередження залежно від стадії росту
        switch (currentStage)
        {
            case "Початкова":
                warnings.Add("Початкова стадія - коренева система розвивається. Забезпечте достатність фосфору для стимуляції росту коренів.");
                break;
            case "Розвиток":
                if (daysToHarvest < 45)
                    warnings.Add("Критична фаза розвитку - максимальна потреба у всіх поживних речовинах. Забезпечте своєчасне підживлення.");
                break;
            case "Середина сезону":
                warnings.Add("Період максимального споживання поживних речовин. Контролюйте забезпеченість азотом та калієм.");
                break;
            case "Пізній сезон":
                warnings.Add("Формування та налив врожаю. Калій та мікроелементи особливо важливі для якості продукції.");
                break;
        }

        return warnings;
    }

    /// <summary>
    /// Генерує попередження на основі балансу поживних речовин
    /// </summary>
    public static List<string> GenerateNutrientBalanceWarnings(
        NutrientRequirement deficit,
        NutrientRequirement surplus,
        double? soilOrganicMatter = null)
    {
        var warnings = new List<string>();

        // Попередження про основні елементи (NPK)
        AddNitrogenWarnings(warnings, deficit, surplus);
        AddPhosphorusWarnings(warnings, deficit, surplus);
        AddPotassiumWarnings(warnings, deficit, surplus);

        // Попередження про вторинні елементи
        AddSulfurWarnings(warnings, deficit, surplus);
        AddCalciumWarnings(warnings, deficit, surplus);
        AddMagnesiumWarnings(warnings, deficit, surplus);

        // Попередження про мікроелементи
        AddBoronWarnings(warnings, deficit, surplus);
        AddZincWarnings(warnings, deficit, surplus);
        AddManganeseWarnings(warnings, deficit, surplus);
        AddCopperWarnings(warnings, deficit, surplus);
        AddIronWarnings(warnings, deficit, surplus);
        AddMolybdenumWarnings(warnings, deficit, surplus);

        // Комплексні попередження про дисбаланси
        AddCombinedNutrientWarnings(warnings, deficit, surplus);

        // Попередження про органічну речовину
        if (soilOrganicMatter.HasValue)
        {
            if (soilOrganicMatter < 2.0)
                warnings.Add($"Низький вміст органічної речовини ({soilOrganicMatter:F1}%). Рекомендується внесення органічних добрив для покращення структури ґрунту.");
            else if (soilOrganicMatter > 6.0)
                warnings.Add($"Високий вміст органічної речовини ({soilOrganicMatter:F1}%). Контролюйте мінералізацію азоту для уникнення вилягання культур.");
        }

        return warnings;
    }

    #region Macronutrient Warnings

    private static void AddNitrogenWarnings(List<string> warnings, NutrientRequirement deficit, NutrientRequirement surplus)
    {
        if (surplus.Nitrogen > 50)
            warnings.Add("⚠️ КРИТИЧНО: Надлишок азоту (>50 кг/га) - високий ризик вилягання культури, затримки дозрівання та погіршення якості продукції.");
        else if (surplus.Nitrogen > 30)
            warnings.Add("⚠️ Надлишок азоту може призвести до вилягання культури, зниження якості зерна та підвищеної сприйнятливості до хвороб.");

        if (deficit.Nitrogen > 50)
            warnings.Add("🔴 КРИТИЧНО: Значний дефіцит азоту (>50 кг/га) - очікується зниження врожайності на 35-50% та сильний хлороз рослин.");
        else if (deficit.Nitrogen > 40)
            warnings.Add("🔴 Значний дефіцит азоту (>40 кг/га) - очікується зниження врожайності на 30-40%, пожовтіння нижніх листків.");
        else if (deficit.Nitrogen > 20)
            warnings.Add("🟡 Дефіцит азоту може призвести до хлорозу листя, уповільнення росту та зниження врожайності на 15-25%.");
    }

    private static void AddPhosphorusWarnings(List<string> warnings, NutrientRequirement deficit, NutrientRequirement surplus)
    {
        if (surplus.Phosphorus > 50)
            warnings.Add("⚠️ Надлишок фосфору (>50 кг/га) може блокувати засвоєння цинку та заліза, призвести до хлорозу та екологічних проблем.");
        else if (surplus.Phosphorus > 40)
            warnings.Add("⚠️ Надлишок фосфору може блокувати засвоєння цинку та заліза рослинами.");

        if (deficit.Phosphorus > 50)
            warnings.Add("🔴 КРИТИЧНО: Дефіцит фосфору (>50 кг/га) може знизити врожайність на 25-35%, значно уповільнити розвиток кореневої системи.");
        else if (deficit.Phosphorus > 30)
            warnings.Add("🟡 Дефіцит фосфору призведе до затримки розвитку, фіолетового забарвлення листя та погіршення якості врожаю.");
        else if (deficit.Phosphorus > 15)
            warnings.Add("🟡 Недостатній рівень фосфору може уповільнити розвиток кореневої системи та цвітіння.");
    }

    private static void AddPotassiumWarnings(List<string> warnings, NutrientRequirement deficit, NutrientRequirement surplus)
    {
        if (surplus.Potassium > 80)
            warnings.Add("⚠️ Надлишок калію (>80 кг/га) може перешкоджати засвоєнню магнію та кальцію, призвести до дисбалансу живлення.");
        else if (surplus.Potassium > 60)
            warnings.Add("⚠️ Надлишок калію може перешкоджати засвоєнню магнію та кальцію.");

        if (deficit.Potassium > 60)
            warnings.Add("🔴 КРИТИЧНО: Дефіцит калію (>60 кг/га) - знижується стійкість до посухи, хвороб та вилягання, погіршується якість плодів.");
        else if (deficit.Potassium > 40)
            warnings.Add("🟡 Дефіцит калію призведе до краєвого опіку листя, погіршення якості продукції та зниження стійкості до стресових умов.");
        else if (deficit.Potassium > 20)
            warnings.Add("🟡 Недостатній рівень калію може знизити стійкість до посухи та якість врожаю.");
    }

    #endregion

    #region Secondary Nutrient Warnings

    private static void AddSulfurWarnings(List<string> warnings, NutrientRequirement deficit, NutrientRequirement surplus)
    {
        if (deficit.Sulfur > 15)
            warnings.Add("🟡 Дефіцит сірки (>15 кг/га) - порушується синтез білка, знижується якість зерна, можливе пожовтіння молодих листків.");
        else if (deficit.Sulfur > 10)
            warnings.Add("🟡 Недостатньо сірки для оптимального синтезу амінокислот та білків.");

        if (surplus.Sulfur > 30)
            warnings.Add("⚠️ Надлишок сірки (>30 кг/га) може підкислювати ґрунт та погіршувати засвоєння інших елементів живлення.");
        else if (surplus.Sulfur > 20)
            warnings.Add("⚠️ Надлишок сірки може підкислювати ґрунт та погіршувати засвоєння інших елементів.");
    }

    private static void AddCalciumWarnings(List<string> warnings, NutrientRequirement deficit, NutrientRequirement surplus)
    {
        if (deficit.Calcium > 40)
            warnings.Add("🔴 Критичний дефіцит кальцію (>40 кг/га) - ризик верхівкової гнилі плодів, погіршення структури ґрунту та розвитку кореневої системи.");
        else if (deficit.Calcium > 30)
            warnings.Add("🟡 Значний дефіцит кальцію - ризик верхівкової гнилі плодів та погіршення структури ґрунту.");
        else if (deficit.Calcium > 20)
            warnings.Add("🟡 Дефіцит кальцію призведе до погіршення якості продукції, зниження лежкості та міцності клітинних стінок.");

        if (surplus.Calcium > 60)
            warnings.Add("⚠️ Надлишок кальцію (>60 кг/га) може блокувати засвоєння калію, магнію та мікроелементів.");
        else if (surplus.Calcium > 50)
            warnings.Add("⚠️ Надлишок кальцію може блокувати засвоєння калію, магнію та мікроелементів.");
    }

    private static void AddMagnesiumWarnings(List<string> warnings, NutrientRequirement deficit, NutrientRequirement surplus)
    {
        if (deficit.Magnesium > 20)
            warnings.Add("🟡 Дефіцит магнію (>20 кг/га) - порушується фотосинтез, з'являється міжжилковий хлороз нижніх листків.");
        else if (deficit.Magnesium > 12)
            warnings.Add("🟡 Недостатньо магнію для оптимального утворення хлорофілу та процесів фотосинтезу.");
        else if (deficit.Magnesium > 8)
            warnings.Add("🟡 Знижений рівень магнію може вплинути на інтенсивність фотосинтезу.");

        if (surplus.Magnesium > 40)
            warnings.Add("⚠️ Надлишок магнію (>40 кг/га) може перешкоджати засвоєнню кальцію та калію.");
        else if (surplus.Magnesium > 30)
            warnings.Add("⚠️ Надлишок магнію може перешкоджати засвоєнню кальцію та калію.");
    }

    #endregion

    #region Micronutrient Warnings

    private static void AddBoronWarnings(List<string> warnings, NutrientRequirement deficit, NutrientRequirement surplus)
    {
        if (deficit.Boron > 0.5)
            warnings.Add("🟡 Дефіцит бору (>0.5 кг/га) - порушується запліднення, ризик деформації плодів, некрозу точок росту та опадання квіток.");
        else if (deficit.Boron > 0.3)
            warnings.Add("🟡 Недостатньо бору може призвести до погіршення цвітіння, плодоутворення та розвитку насіння.");
        else if (deficit.Boron > 0.15)
            warnings.Add("🟡 Знижений рівень бору може вплинути на якість цвітіння.");

        if (surplus.Boron > 1.0)
            warnings.Add("⚠️ ТОКСИЧНІСТЬ: Надлишок бору (>1.0 кг/га) - некроз країв та кінчиків листя, значне зниження врожайності.");
        else if (surplus.Boron > 0.8)
            warnings.Add("⚠️ Надлишок бору токсичний для рослин - можливий некроз листя.");
    }

    private static void AddZincWarnings(List<string> warnings, NutrientRequirement deficit, NutrientRequirement surplus)
    {
        if (deficit.Zinc > 1.5)
            warnings.Add("🟡 Критичний дефіцит цинку (>1.5 кг/га) - сильне уповільнення росту, розеткоподібність, деформація листя.");
        else if (deficit.Zinc > 1.0)
            warnings.Add("🟡 Дефіцит цинку - уповільнення росту, деформація листя, зниження врожайності.");
        else if (deficit.Zinc > 0.5)
            warnings.Add("🟡 Недостатньо цинку для оптимального синтезу ауксинів та утворення білків.");

        if (surplus.Zinc > 3.0)
            warnings.Add("⚠️ ТОКСИЧНІСТЬ: Надлишок цинку (>3.0 кг/га) - хлороз, пригнічення росту.");
        else if (surplus.Zinc > 2.0)
            warnings.Add("⚠️ Надлишок цинку може бути токсичним та перешкоджати засвоєнню заліза.");
    }

    private static void AddManganeseWarnings(List<string> warnings, NutrientRequirement deficit, NutrientRequirement surplus)
    {
        if (deficit.Manganese > 2.0)
            warnings.Add("🟡 Критичний дефіцит марганцю (>2.0 кг/га) - міжжилковий хлороз, плямистість листя, значне зниження фотосинтезу.");
        else if (deficit.Manganese > 1.5)
            warnings.Add("🟡 Дефіцит марганцю - порушується фотосинтез, з'являється плямистість листя.");
        else if (deficit.Manganese > 0.8)
            warnings.Add("🟡 Недостатньо марганцю для активації ферментів фотосинтезу.");

        if (surplus.Manganese > 4.0)
            warnings.Add("⚠️ ТОКСИЧНІСТЬ: Надлишок марганцю (>4.0 кг/га) - бура плямистість листя, некрози.");
        else if (surplus.Manganese > 3.0)
            warnings.Add("⚠️ Надлишок марганцю може перешкоджати засвоєнню заліза та магнію.");
    }

    private static void AddCopperWarnings(List<string> warnings, NutrientRequirement deficit, NutrientRequirement surplus)
    {
        if (deficit.Copper > 0.4)
            warnings.Add("🟡 Критичний дефіцит міді (>0.4 кг/га) - хлороз, в'янення верхівок, погана стійкість стебла.");
        else if (deficit.Copper > 0.3)
            warnings.Add("🟡 Дефіцит міді - порушується фотосинтез, знижується стійкість до грибних хвороб.");
        else if (deficit.Copper > 0.15)
            warnings.Add("🟡 Недостатньо міді для оптимальної активності ферментів.");

        if (surplus.Copper > 0.8)
            warnings.Add("⚠️ ТОКСИЧНІСТЬ: Надлишок міді (>0.8 кг/га) - хлороз, пригнічення росту кореневої системи.");
        else if (surplus.Copper > 0.6)
            warnings.Add("⚠️ Надлишок міді токсичний для рослин та мікроорганізмів ґрунту.");
    }

    private static void AddIronWarnings(List<string> warnings, NutrientRequirement deficit, NutrientRequirement surplus)
    {
        if (deficit.Iron > 3.0)
            warnings.Add("🟡 Критичний дефіцит заліза (>3.0 кг/га) - сильний хлороз молодого листя, практично біле забарвлення.");
        else if (deficit.Iron > 2.0)
            warnings.Add("🟡 Дефіцит заліза - хлороз молодого листя, значне зниження фотосинтезу.");
        else if (deficit.Iron > 1.0)
            warnings.Add("🟡 Недостатньо заліза для синтезу хлорофілу та функціонування дихальних ферментів.");

        if (surplus.Iron > 5.0)
            warnings.Add("⚠️ Надлишок заліза (>5.0 кг/га) може перешкоджати засвоєнню фосфору, марганцю та цинку.");
        else if (surplus.Iron > 4.0)
            warnings.Add("⚠️ Надлишок заліза може перешкоджати засвоєнню фосфору та марганцю.");
    }

    private static void AddMolybdenumWarnings(List<string> warnings, NutrientRequirement deficit, NutrientRequirement surplus)
    {
        if (deficit.Molybdenum > 0.05)
            warnings.Add("🟡 Дефіцит молібдену (>0.05 кг/га) - порушується азотний обмін, особливо критично для бобових культур.");
        else if (deficit.Molybdenum > 0.02)
            warnings.Add("🟡 Недостатньо молібдену для ефективної фіксації азоту та відновлення нітратів.");
        else if (deficit.Molybdenum > 0.01)
            warnings.Add("🟡 Знижений рівень молібдену може вплинути на азотний обмін.");

        if (surplus.Molybdenum > 0.15)
            warnings.Add("⚠️ Надлишок молібдену (>0.15 кг/га) може викликати токсичність у тварин при споживанні кормів.");
        else if (surplus.Molybdenum > 0.1)
            warnings.Add("⚠️ Надлишок молібдену може викликати токсичність у тварин при споживанні кормів.");
    }

    #endregion

    #region Combined Warnings

    private static void AddCombinedNutrientWarnings(List<string> warnings, NutrientRequirement deficit, NutrientRequirement surplus)
    {
        // Критичний комплексний дефіцит
        if (deficit.Nitrogen > 30 && deficit.Phosphorus > 30 && deficit.Potassium > 40)
            warnings.Add("🔴 КРИТИЧНО: Комплексний дефіцит NPK - необхідне термінове внесення повного мінерального добрива для запобігання значних втрат врожаю.");
        else if (deficit.Nitrogen > 20 && deficit.Phosphorus > 20 && deficit.Potassium > 30)
            warnings.Add("🟡 Комплексний дефіцит NPK - рекомендується внесення комплексного добрива.");

        // Дисбаланс азот/калій
        if (surplus.Nitrogen > 30 && deficit.Potassium > 40)
            warnings.Add("⚠️ КРИТИЧНИЙ ДИСБАЛАНС: Надлишок азоту при дефіциті калію збільшує ризик вилягання, хвороб та поганої якості врожаю.");
        else if (surplus.Nitrogen > 20 && deficit.Potassium > 30)
            warnings.Add("⚠️ Дисбаланс N/K - надлишок азоту при дефіциті калію збільшує ризик вилягання та хвороб.");

        // Дисбаланс азот/фосфор
        if (surplus.Nitrogen > 30 && deficit.Phosphorus > 30)
            warnings.Add("⚠️ Дисбаланс N/P - надлишок азоту при дефіциті фосфору призводить до затримки цвітіння та формування плодів.");

        // Дефіцит кальцію та магнію
        if (deficit.Calcium > 25 && deficit.Magnesium > 15)
            warnings.Add("🟡 Комплексний дефіцит Ca і Mg може призвести до погіршення структури ґрунту, його підкислення та проблем з фотосинтезом.");
        else if (deficit.Calcium > 20 && deficit.Magnesium > 12)
            warnings.Add("🟡 Дефіцит Ca і Mg може призвести до погіршення структури ґрунту та його підкислення.");

        // Дефіцит мікроелементів
        if (deficit.Boron > 0.3 && deficit.Zinc > 0.5 && deficit.Manganese > 0.8)
            warnings.Add("🟡 Комплексний дефіцит мікроелементів (B, Zn, Mn) - рекомендується позакореневе підживлення комплексними препаратами.");
        else if (deficit.Boron > 0.3 && deficit.Zinc > 0.5)
            warnings.Add("🟡 Комплексний дефіцит мікроелементів (B, Zn) - рекомендується позакореневе підживлення.");

        // Дефіцит заліза та марганцю (типово для лужних ґрунтів)
        if (deficit.Iron > 1.5 && deficit.Manganese > 1.0)
            warnings.Add("🟡 Дефіцит Fe і Mn - можливо, пов'язано з високим pH ґрунту. Використовуйте хелатні форми добрив.");

        // Дисбаланс N/S
        if (deficit.Nitrogen < 10 && deficit.Sulfur > 15)
            warnings.Add("⚠️ Дисбаланс N/S - недостатність сірки може обмежувати ефективність азотного живлення.");
    }

    #endregion
}
