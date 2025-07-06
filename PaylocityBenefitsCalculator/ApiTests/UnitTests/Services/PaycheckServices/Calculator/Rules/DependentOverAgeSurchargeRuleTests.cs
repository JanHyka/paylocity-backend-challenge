using System;
using System.Collections.Generic;
using Api.Models;
using Api.Services.PaycheckServices.Calculator.ApplicabilityPolicies;
using Api.Services.PaycheckServices.Calculator.ProrationPolicies;
using Api.Services.PaycheckServices.Calculator.Rules;
using Moq;
using Xunit;

namespace ApiTests.UnitTests.Services.PaycheckServices.Calculator.Rules
{
    public class DependentOverAgeSurchargeRuleTests
    {
        [Fact]
        public void Apply_AddsSurcharge_ForApplicableDependent_SingleMonth()
        {
            // Arrange
            var monthlySurcharge = 300m;
            var periodStart = new DateTime(2024, 6, 1);
            var periodEnd = new DateTime(2024, 6, 14);
            var prorationDays = 14;

            var applicabilityMock = new Mock<IApplicabilityPolicy<Dependent>>();
            applicabilityMock.Setup(a => a.IsApplicable(It.IsAny<Dependent>(), periodStart, periodEnd)).Returns(true);

            var prorationMock = new Mock<IProrationPolicy<Dependent>>();
            prorationMock.Setup(p => p.GetProrationFactorDays(It.IsAny<Dependent>(), periodStart, periodEnd)).Returns(prorationDays);

            var rule = new DependentOverAgeSurchargeRule(monthlySurcharge, applicabilityMock.Object, prorationMock.Object);
            var dependent = new Dependent();
            var employee = new Employee { Dependents = [dependent] };
            var paycheck = new Paycheck
            {
                BenefitsCost = 0m,
                PayPeriodStart = periodStart,
                PayPeriodEnd = periodEnd
            };

            // Act
            rule.Apply(paycheck, employee);

            // Assert
            // Expected: 300 * 14 / 30 = 140
            Assert.Equal(140m, paycheck.BenefitsCost);
        }

        [Fact]
        public void Apply_AddsSurcharge_ForApplicableDependent_TwoMonths()
        {
            // Arrange
            var monthlySurcharge = 310m;
            var periodStart = new DateTime(2024, 6, 28);
            var periodEnd = new DateTime(2024, 7, 4);

            var applicabilityMock = new Mock<IApplicabilityPolicy<Dependent>>();
            applicabilityMock.Setup(a => a.IsApplicable(It.IsAny<Dependent>(), periodStart, periodEnd)).Returns(true);

            var prorationMock = new Mock<IProrationPolicy<Dependent>>();
            // June: 28,29,30 (3 days)
            prorationMock.Setup(p => p.GetProrationFactorDays(It.IsAny<Dependent>(), periodStart, new DateTime(2024, 6, 30))).Returns(3);
            // July: 1,2,3,4 (4 days)
            prorationMock.Setup(p => p.GetProrationFactorDays(It.IsAny<Dependent>(), new DateTime(2024, 7, 1), periodEnd)).Returns(4);

            var rule = new DependentOverAgeSurchargeRule(monthlySurcharge, applicabilityMock.Object, prorationMock.Object);
            var dependent = new Dependent();
            var employee = new Employee { Dependents = [dependent] };
            var paycheck = new Paycheck
            {
                BenefitsCost = 0m,
                PayPeriodStart = periodStart,
                PayPeriodEnd = periodEnd
            };

            // Act
            rule.Apply(paycheck, employee);

            // Assert
            // June: 310 * 3 / 30 = 31
            // July: 310 * 4 / 31 = 40
            // Total = 71
            Assert.Equal(71m, Math.Round(paycheck.BenefitsCost));
        }

        [Fact]
        public void Apply_DoesNotAddSurcharge_WhenNotApplicable()
        {
            // Arrange
            var monthlySurcharge = 200m;
            var periodStart = new DateTime(2024, 5, 1);
            var periodEnd = new DateTime(2024, 5, 14);

            var applicabilityMock = new Mock<IApplicabilityPolicy<Dependent>>();
            applicabilityMock.Setup(a => a.IsApplicable(It.IsAny<Dependent>(), periodStart, periodEnd)).Returns(false);

            var prorationMock = new Mock<IProrationPolicy<Dependent>>();
            prorationMock.Setup(p => p.GetProrationFactorDays(It.IsAny<Dependent>(), periodStart, periodEnd)).Returns(14);

            var rule = new DependentOverAgeSurchargeRule(monthlySurcharge, applicabilityMock.Object, prorationMock.Object);
            var dependent = new Dependent();
            var employee = new Employee { Dependents = [dependent] };
            var paycheck = new Paycheck
            {
                BenefitsCost = 0m,
                PayPeriodStart = periodStart,
                PayPeriodEnd = periodEnd
            };

            // Act
            rule.Apply(paycheck, employee);

            // Assert
            Assert.Equal(0m, paycheck.BenefitsCost);
        }

        [Fact]
        public void Apply_AddsSurcharge_ForMultipleDependents()
        {
            // Arrange
            var monthlySurcharge = 100m;
            var periodStart = new DateTime(2024, 8, 1);
            var periodEnd = new DateTime(2024, 8, 10);
            var prorationDays = 10;

            var applicabilityMock = new Mock<IApplicabilityPolicy<Dependent>>();
            applicabilityMock.Setup(a => a.IsApplicable(It.IsAny<Dependent>(), periodStart, periodEnd)).Returns(true);

            var prorationMock = new Mock<IProrationPolicy<Dependent>>();
            prorationMock.Setup(p => p.GetProrationFactorDays(It.IsAny<Dependent>(), periodStart, periodEnd)).Returns(prorationDays);

            var rule = new DependentOverAgeSurchargeRule(monthlySurcharge, applicabilityMock.Object, prorationMock.Object);
            var dependents = new List<Dependent> { new(), new() };
            var employee = new Employee { Dependents = dependents };
            var paycheck = new Paycheck
            {
                BenefitsCost = 0m,
                PayPeriodStart = periodStart,
                PayPeriodEnd = periodEnd
            };

            // Act
            rule.Apply(paycheck, employee);

            // Assert
            // Each: 100 * 10 / 31 = 32.26, total = 64.52
            Assert.Equal(64.52m, Math.Round(paycheck.BenefitsCost, 2));
        }

        [Fact]
        public void Apply_DoesNothing_WhenNoDependents()
        {
            // Arrange
            var monthlySurcharge = 100m;
            var periodStart = new DateTime(2024, 9, 1);
            var periodEnd = new DateTime(2024, 9, 14);

            var applicabilityMock = new Mock<IApplicabilityPolicy<Dependent>>();
            var prorationMock = new Mock<IProrationPolicy<Dependent>>();

            var rule = new DependentOverAgeSurchargeRule(monthlySurcharge, applicabilityMock.Object, prorationMock.Object);
            var employee = new Employee { Dependents = [] };
            var paycheck = new Paycheck
            {
                BenefitsCost = 0m,
                PayPeriodStart = periodStart,
                PayPeriodEnd = periodEnd
            };

            // Act
            rule.Apply(paycheck, employee);

            // Assert
            Assert.Equal(0m, paycheck.BenefitsCost);
        }

        [Fact]
        public void Apply_AddsSurcharge_OnlyForApplicableDependents()
        {
            // Arrange
            var monthlySurcharge = 200m;
            var periodStart = new DateTime(2024, 10, 1);
            var periodEnd = new DateTime(2024, 10, 10);
            var prorationDays = 10;

            var dependent1 = new Dependent { FirstName = "Applicable" };
            var dependent2 = new Dependent { FirstName = "NotApplicable" };
            var dependents = new List<Dependent> { dependent1, dependent2 };
            var employee = new Employee { Dependents = dependents };

            var applicabilityMock = new Mock<IApplicabilityPolicy<Dependent>>();
            applicabilityMock.Setup(a => a.IsApplicable(dependent1, periodStart, periodEnd)).Returns(true);
            applicabilityMock.Setup(a => a.IsApplicable(dependent2, periodStart, periodEnd)).Returns(false);

            var prorationMock = new Mock<IProrationPolicy<Dependent>>();
            prorationMock.Setup(p => p.GetProrationFactorDays(dependent1, periodStart, periodEnd)).Returns(prorationDays);
            prorationMock.Setup(p => p.GetProrationFactorDays(dependent2, periodStart, periodEnd)).Returns(prorationDays); // Should not be called, but safe

            var rule = new DependentOverAgeSurchargeRule(monthlySurcharge, applicabilityMock.Object, prorationMock.Object);
            var paycheck = new Paycheck
            {
                BenefitsCost = 0m,
                PayPeriodStart = periodStart,
                PayPeriodEnd = periodEnd
            };

            // Act
            rule.Apply(paycheck, employee);

            // Assert
            // Only dependent1 is applicable: 200 * 10 / 31 = 64.52
            Assert.Equal(64.52m, Math.Round(paycheck.BenefitsCost, 2));
        }
    }
}