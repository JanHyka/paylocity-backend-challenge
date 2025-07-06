using System;
using Api.Models;
using Api.Services.PaycheckServices.Calculator.ApplicabilityPolicies;
using Xunit;

namespace ApiTests.UnitTests.Services.PaycheckServices.Calculator.ApplicabilityPolicies;

public class DependentWasBornInStartMonthPolicyTests
{
    private readonly DependentWasBornInStartMonthPolicy _policy = new();

    [Theory]
    [InlineData(2024, 5, 1, 2024, 5, 1, true)]   // Born on first day of period month
    [InlineData(2024, 5, 15, 2024, 5, 1, true)]  // Born in the middle of period month
    [InlineData(2024, 5, 31, 2024, 5, 1, true)]  // Born on last day of period month
    [InlineData(2024, 6, 1, 2024, 5, 1, false)]  // Born after period month
    [InlineData(2024, 4, 30, 2024, 5, 1, true)] // Born before period month
    public void IsApplicable_ReturnsExpectedResult(
        int dobYear, int dobMonth, int dobDay,
        int periodStartYear, int periodStartMonth, int periodStartDay,
        bool expected)
    {
        var dependent = new Dependent
        {
            DateOfBirth = new DateTime(dobYear, dobMonth, dobDay)
        };
        var periodStart = new DateTime(periodStartYear, periodStartMonth, periodStartDay);
        var periodEnd = periodStart.AddMonths(1).AddDays(-1); // Not used by policy

        var result = _policy.IsApplicable(dependent, periodStart, periodEnd);

        Assert.Equal(expected, result);
    }
}