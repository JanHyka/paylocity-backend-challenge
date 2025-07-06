using System;
using Api.Models;
using Api.Services.PaycheckServices.Calculator.ApplicabilityPolicies;
using Xunit;

namespace ApiTests.UnitTests.Services.PaycheckServices.Calculator.ApplicabilityPolicies;

public class DependantOverAgeFromBirthdayPolicyTests
{
    [Theory]
    // Dependent turns 18 before the period ends
    [InlineData("2000-01-01", 18, "2018-12-31", true)]
    // Dependent turns 18 exactly on the period end
    [InlineData("2000-01-01", 18, "2018-01-01", true)]
    // Dependent turns 18 after the period end
    [InlineData("2000-01-01", 18, "2017-12-31", false)]
    // Dependent is much older than the threshold
    [InlineData("1980-05-15", 18, "2020-06-01", true)]
    // Dependent is much younger than the threshold
    [InlineData("2010-05-15", 18, "2020-06-01", false)]
    public void IsApplicable_ReturnsExpectedResult(
        string dob, int ageLowerBound, string periodEnd, bool expected)
    {
        var dependent = new Dependent
        {
            DateOfBirth = DateTime.Parse(dob)
        };
        var policy = new DependantOverAgeFromBirthdayPolicy(ageLowerBound);
        var periodStart = DateTime.Parse("2000-01-01"); // Not used by policy
        var periodEndDate = DateTime.Parse(periodEnd);

        var result = policy.IsApplicable(dependent, periodStart, periodEndDate);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsApplicable_True_WhenTurnsAgeOnPeriodEnd()
    {
        var dob = new DateTime(2010, 6, 1);
        int ageLowerBound = 10;
        var periodEnd = new DateTime(2020, 6, 1);
        var dependent = new Dependent { DateOfBirth = dob };
        var policy = new DependantOverAgeFromBirthdayPolicy(ageLowerBound);

        Assert.True(policy.IsApplicable(dependent, DateTime.MinValue, periodEnd));
    }

    [Fact]
    public void IsApplicable_False_WhenTurnsAgeDayAfterPeriodEnd()
    {
        var dob = new DateTime(2010, 6, 2);
        int ageLowerBound = 10;
        var periodEnd = new DateTime(2020, 6, 1);
        var dependent = new Dependent { DateOfBirth = dob };
        var policy = new DependantOverAgeFromBirthdayPolicy(ageLowerBound);

        Assert.False(policy.IsApplicable(dependent, DateTime.MinValue, periodEnd));
    }
}