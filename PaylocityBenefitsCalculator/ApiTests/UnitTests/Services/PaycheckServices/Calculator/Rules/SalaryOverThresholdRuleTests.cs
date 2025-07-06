using System;
using Api.Models;
using Api.Services.PaycheckServices.Calculator.ApplicabilityPolicies;
using Api.Services.PaycheckServices.Calculator.Rules;
using Moq;
using Xunit;

namespace ApiTests.UnitTests.Services.PaycheckServices.Calculator.Rules
{
    public class SalaryOverThresholdRuleTests
    {
        [Fact]
        public void Apply_DoesNothing_WhenNotApplicable()
        {
            // Arrange
            var applicabilityMock = new Mock<IApplicabilityPolicy<Employee>>();
            applicabilityMock.Setup(a => a.IsApplicable(It.IsAny<Employee>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(false);

            var rule = new SalaryOverThresholdRule(0.1m, applicabilityMock.Object);
            var paycheck = new Paycheck
            {
                BenefitsCost = 0m,
                PayPeriodStart = new DateTime(2024, 1, 1),
                PayPeriodEnd = new DateTime(2024, 1, 14)
            };
            var employee = new Employee { Salary = 100000m };

            // Act
            rule.Apply(paycheck, employee);

            // Assert
            Assert.Equal(0m, paycheck.BenefitsCost);
        }

        [Fact]
        public void Apply_AddsProratedCost_WhenApplicable_SameYear_NonLeap()
        {
            // Arrange
            var applicabilityMock = new Mock<IApplicabilityPolicy<Employee>>();
            applicabilityMock.Setup(a => a.IsApplicable(It.IsAny<Employee>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(true);

            var rule = new SalaryOverThresholdRule(0.1m, applicabilityMock.Object);
            var paycheck = new Paycheck
            {
                BenefitsCost = 0m,
                PayPeriodStart = new DateTime(2023, 1, 1),
                PayPeriodEnd = new DateTime(2023, 1, 14) // 14 days
            };
            var employee = new Employee { Salary = 100000m };

            // Act
            rule.Apply(paycheck, employee);

            // Assert
            // annualCost = 100000 * 0.1 = 10000
            // days in period = 14
            // days in year = 365
            // expected = 10000 * 14 / 365 = 383.56
            Assert.Equal(383.56m, Math.Round(paycheck.BenefitsCost, 2));
        }

        [Fact]
        public void Apply_AddsProratedCost_WhenApplicable_SameYear_Leap()
        {
            // Arrange
            var applicabilityMock = new Mock<IApplicabilityPolicy<Employee>>();
            applicabilityMock.Setup(a => a.IsApplicable(It.IsAny<Employee>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(true);

            var rule = new SalaryOverThresholdRule(0.1m, applicabilityMock.Object);
            var paycheck = new Paycheck
            {
                BenefitsCost = 0m,
                PayPeriodStart = new DateTime(2024, 2, 1),
                PayPeriodEnd = new DateTime(2024, 2, 14) // 14 days
            };
            var employee = new Employee { Salary = 100000m };

            // Act
            rule.Apply(paycheck, employee);

            // Assert
            // annualCost = 100000 * 0.1 = 10000
            // days in period = 14
            // days in year = 366
            // expected = 10000 * 14 / 366 = 382.51
            Assert.Equal(382.51m, Math.Round(paycheck.BenefitsCost, 2));
        }

        [Fact]
        public void Apply_AddsProratedCost_WhenPeriodSpansTwoYears()
        {
            // Arrange
            var applicabilityMock = new Mock<IApplicabilityPolicy<Employee>>();
            applicabilityMock.Setup(a => a.IsApplicable(It.IsAny<Employee>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(true);

            var rule = new SalaryOverThresholdRule(0.1m, applicabilityMock.Object);
            var paycheck = new Paycheck
            {
                BenefitsCost = 0m,
                PayPeriodStart = new DateTime(2023, 12, 28),
                PayPeriodEnd = new DateTime(2024, 1, 5) // Spans 2023 and 2024
            };
            var employee = new Employee { Salary = 100000m };

            // Act
            rule.Apply(paycheck, employee);

            // Assert
            // 2023: 28,29,30,31 = 4 days, 365 days in year
            // 2024: 1,2,3,4,5 = 5 days, 366 days in year (leap)
            // annualCost = 10000
            // 2023: 10000 * 4 / 365 = 109.59
            // 2024: 10000 * 5 / 366 = 136.61
            // total = 246.20
            Assert.Equal(246.20m, Math.Round(paycheck.BenefitsCost, 2));
        }

        [Fact]
        public void Apply_DoesNotAddCost_WhenSalaryIsBelowThreshold()
        {
            // Arrange
            var applicabilityMock = new Mock<IApplicabilityPolicy<Employee>>();
            applicabilityMock.Setup(a => a.IsApplicable(It.Is<Employee>(e => e.Salary > 50000m), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(false);
            applicabilityMock.Setup(a => a.IsApplicable(It.Is<Employee>(e => e.Salary <= 50000m), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(false);

            var rule = new SalaryOverThresholdRule(0.1m, applicabilityMock.Object);
            var paycheck = new Paycheck
            {
                BenefitsCost = 0m,
                PayPeriodStart = new DateTime(2024, 1, 1),
                PayPeriodEnd = new DateTime(2024, 1, 14)
            };
            var employee = new Employee { Salary = 40000m };

            // Act
            rule.Apply(paycheck, employee);

            // Assert
            Assert.Equal(0m, paycheck.BenefitsCost);
        }

        [Fact]
        public void Apply_Handles_SingleDayPeriod()
        {
            // Arrange
            var applicabilityMock = new Mock<IApplicabilityPolicy<Employee>>();
            applicabilityMock.Setup(a => a.IsApplicable(It.IsAny<Employee>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(true);

            var rule = new SalaryOverThresholdRule(0.1m, applicabilityMock.Object);
            var paycheck = new Paycheck
            {
                BenefitsCost = 0m,
                PayPeriodStart = new DateTime(2024, 3, 15),
                PayPeriodEnd = new DateTime(2024, 3, 15)
            };
            var employee = new Employee { Salary = 100000m };

            // Act
            rule.Apply(paycheck, employee);

            // Assert
            // annualCost = 10000
            // days in period = 1
            // days in year = 366
            // expected = 10000 * 1 / 366 = 27.32
            Assert.Equal(27.32m, Math.Round(paycheck.BenefitsCost, 2));
        }
    }
}