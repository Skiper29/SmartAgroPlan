namespace SmartAgroPlan.DAL.Enums;

/// <summary>
///     Основні фенологічні стадії росту культури (за FAO-56).
/// </summary>
public enum GrowthStage
{
    /// <summary>
    ///     Період до сівби.
    /// </summary>
    PreSowing,

    /// <summary>
    ///     LIni: Початкова стадія (від сівби до 10% покриття).
    /// </summary>
    Initial,

    /// <summary>
    ///     LDev: Стадія розвитку (від 10% до повного покриття).
    /// </summary>
    Development,

    /// <summary>
    ///     LMid: Середня стадія (від повного покриття до початку дозрівання).
    /// </summary>
    MidSeason,

    /// <summary>
    ///     LLate: Пізня стадія (від дозрівання до збору врожаю).
    /// </summary>
    LateSeason
}
