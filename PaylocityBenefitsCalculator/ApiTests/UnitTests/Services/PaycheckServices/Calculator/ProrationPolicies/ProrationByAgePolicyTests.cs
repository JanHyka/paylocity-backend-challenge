using System;
using Api.Models;
using Api.Services.PaycheckServices.Calculator.ProrationPolicies;
using Xunit;

namespace ApiTests.UnitTests.Services.PaycheckServices.Calculator.ProrationPolicies;

public class ProrationByAgePolicyTests
{
    [Theory]
    // applyDate > periodEnd: no proration
    [InlineData("2010-01-01", 18, "2027-01-01", "2027-01-14", 0)]
    // applyDate < periodStart: full proration
    [InlineData("2000-01-01", 18, "2020-01-01", "2020-01-14", 14)]
    // applyDate == periodStart: full proration
    [InlineData("2002-01-01", 18, "2020-01-01", "2020-01-14", 14)]
    // applyDate inside period: partial proration
    [InlineData("2002-01-10", 18, "2020-01-01", "2020-01-14", 5)] // turns 18 on 2020-01-10, so 10,11,12,13,14 = 5 days
    // applyDate == periodEnd: 1 day proration
    [InlineData("2002-01-14", 18, "2020-01-01", "2020-01-14", 1)]
    public void GetProrationFactorDays_ReturnsExpected(
        string dob, int ageLowerBound, string periodStart, string periodEnd, int expectedDays)
    {
        var dependent = new Dependent { DateOfBirth = DateTime.Parse(dob) };
        var policy = new ProrationByAgePolicy(ageLowerBound);
        var start = DateTime.Parse(periodStart);
        var end = DateTime.Parse(periodEnd);

        int result = policy.GetProrationFactorDays(dependent, start, end);

        Assert.Equal(expectedDays, result);
    }

    [Fact]
    public void GetProrationFactorDays_ReturnsFullPeriod_WhenApplyDateBeforePeriod()
    {
        var dependent = new Dependent { DateOfBirth = new DateTime(2000, 1, 1) };
        var policy = new ProrationByAgePolicy(10);
        var start = new DateTime(2015, 1, 1);
        var end = new DateTime(2015, 1, 31);

        int result = policy.GetProrationFactorDays(dependent, start, end);

        Assert.Equal(31, result);
    }

    [Fact]
    public void GetProrationFactorDays_ReturnsZero_WhenApplyDateAfterPeriod()
    {
        var dependent = new Dependent { DateOfBirth = new DateTime(2010, 1, 1) };
        var policy = new ProrationByAgePolicy(18);
        var start = new DateTime(2025, 1, 1);
        var end = new DateTime(2025, 1, 31);

        int result = policy.GetProrationFactorDays(dependent, start, end);

        Assert.Equal(0, result);
    }

    [Fact]
    public void GetProrationFactorDays_ReturnsCorrect_ForSingleDayPeriod()
    {
        var dependent = new Dependent { DateOfBirth = new DateTime(2002, 1, 14) };
        var policy = new ProrationByAgePolicy(18);
        var start = new DateTime(2020, 1, 14);
        var end = new DateTime(2020, 1, 14);

        int result = policy.GetProrationFactorDays(dependent, start, end);

        Assert.Equal(1, result);
    }
}