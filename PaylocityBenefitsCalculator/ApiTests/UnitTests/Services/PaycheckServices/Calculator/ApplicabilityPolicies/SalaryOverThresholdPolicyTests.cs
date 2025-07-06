using System;
using Api.Models;
using Api.Services.PaycheckServices.Calculator.ApplicabilityPolicies;
using Xunit;

namespace ApiTests.UnitTests.Services.PaycheckServices.Calculator.ApplicabilityPolicies;

public class SalaryOverThresholdPolicyTests
{
    [Theory]
    [InlineData(50000, 40000, true)]
    [InlineData(50000, 50000, false)]
    [InlineData(50000, 60000, false)]
    [InlineData(0, 0, false)]
    [InlineData(0, 1, false)]
    [InlineData(100000, 99999.99, true)]
    public void IsApplicable_ReturnsExpectedResult(decimal salary, decimal threshold, bool expected)
    {
        // Arrange
        var employee = new Employee { Salary = salary };
        var policy = new SalaryOverThresholdPolicy(threshold);
        var periodStart = new DateTime(2024, 1, 1);
        var periodEnd = new DateTime(2024, 1, 14);

        // Act
        var result = policy.IsApplicable(employee, periodStart, periodEnd);

        // Assert
        Assert.Equal(expected, result);
    }
}