using AbcLettingAgency.Enums;

namespace AbcLettingAgency.Features.TenantManagement.Tenancies.helpers;

public static class RentSchedule
{
    /// <summary>
    /// Clamps the target day to the number of days in the given month, preserving DateTime.Kind.
    /// </summary>
    public static DateTime ClampDayInMonth(DateTime basis, int dueDay)
    {
        var max = DateTime.DaysInMonth(basis.Year, basis.Month);
        var d = Math.Clamp(dueDay, 1, max);
        return new DateTime(basis.Year, basis.Month, d, 0, 0, 0, basis.Kind);
    }

    /// <summary>
    /// Returns the first occurrence of the due day on or after the given date.
    /// </summary>
    public static DateTime FirstDueOnOrAfter(DateTime date, int dueDay)
    {
        var first = ClampDayInMonth(date, dueDay);
        return first < date ? ClampDayInMonth(date.AddMonths(1), dueDay) : first;
    }

    /// <summary>
    /// Derives the due date for a period that starts at <paramref name="start"/>.
    /// Monthly => first due day on/after start; Weekly/FourWeekly => due on start.
    /// </summary>
    public static DateTime DeriveDueDate(DateTime start, RentFrequency freq, int rentDueDay)
        => freq switch
        {
            RentFrequency.Monthly => FirstDueOnOrAfter(start, rentDueDay),
            RentFrequency.FourWeekly => start, // due on start of 4-week period
            RentFrequency.Weekly => start, // due on start of week period
            _ => FirstDueOnOrAfter(start, rentDueDay)
        };

    /// <summary>
    /// Derives the inclusive period end date for a period that starts at <paramref name="start"/>.
    /// Weekly = +6 days; FourWeekly = +27 days; Monthly = day before next due day.
    /// </summary>
    public static DateTime DerivePeriodEnd(DateTime start, RentFrequency freq, int rentDueDay)
    {
        if (freq == RentFrequency.Weekly)
            return start.AddDays(6);

        if (freq == RentFrequency.FourWeekly)
            return start.AddDays(27); // 28-day period inclusive

        // Monthly (and default): end is the day before the next due day on/after start
        var end = FirstDueOnOrAfter(start, rentDueDay).AddDays(-1);

        // If start is exactly the due day, the "day before" is < start; use next month instead.
        if (end < start)
            end = FirstDueOnOrAfter(start.AddMonths(1), rentDueDay).AddDays(-1);

        return end;
    }

    /// <summary>
    /// Gets the next period start after <paramref name="currentStart"/>.
    /// </summary>
    public static DateTime NextPeriodStart(DateTime currentStart, RentFrequency freq, int rentDueDay)
        => freq switch
        {
            RentFrequency.Monthly => ClampDayInMonth(currentStart.AddMonths(1), rentDueDay),
            RentFrequency.FourWeekly => currentStart.AddDays(28),
            RentFrequency.Weekly => currentStart.AddDays(7),
            _ => ClampDayInMonth(currentStart.AddMonths(1), rentDueDay)
        };
}
