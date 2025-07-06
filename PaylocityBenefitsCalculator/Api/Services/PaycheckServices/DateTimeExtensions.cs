namespace Api.Services.PaycheckServices;

public static class DateTimeExtensions
{
    /// <summary>
    /// Calculate the end date of a bi-weekly period (14 days later)
    /// </summary>
    public static DateTime GetBiWeekEnd(this DateTime startDate)
        => startDate.AddDays(14).AddTicks(-1);
}
