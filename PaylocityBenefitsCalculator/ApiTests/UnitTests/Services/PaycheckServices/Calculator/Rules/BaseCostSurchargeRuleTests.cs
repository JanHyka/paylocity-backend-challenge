using System;
using Api.Models;
using Api.Services.PaycheckServices.Calculator.Rules;
using Xunit;

namespace ApiTests.UnitTests.Services.PaycheckServices.Calculator.Rules
{
    public class BaseCostSurchargeRuleTests
    {
        [Fact]
        public void Apply_AddsProportionalBaseCost_WhenPeriodWithinSingleMonth()
        {
            // Arrange
            var baseCost = 310m;
            var rule = new BaseCostSurchargeRule(baseCost);
            var paycheck = new Paycheck
            {
                GrossPay = 1000m,
                BenefitsCost = 0m,
                PayPeriodStart = new DateTime(2024, 6, 10),
                PayPeriodEnd = new DateTime(2024, 6, 19) // 10 days in June (30 days in June)
            };
            var employee = new Employee();

            // Act
            rule.Apply(paycheck, employee);

            // Intention: BenefitsCost should be baseCost * (number of days in period) / days in month
            // 310 * 10 / 30 = 103.33
            Assert.Equal(1000m, paycheck.GrossPay);
            Assert.Equal(103.33m, Math.Round(paycheck.BenefitsCost, 2));
        }

        [Fact]
        public void Apply_SplitsBaseCostBetweenMonths_WhenPeriodSpansTwoMonths()
        {
            // Arrange
            var baseCost = 310m;
            var rule = new BaseCostSurchargeRule(baseCost);
            var paycheck = new Paycheck
            {
                GrossPay = 1000m,
                BenefitsCost = 0m,
                PayPeriodStart = new DateTime(2024, 6, 28),
                PayPeriodEnd = new DateTime(2024, 7, 4)
            };
            var employee = new Employee();

            // Act
            rule.Apply(paycheck, employee);

            // June: 28,29,30 (3 days of 30) => 310 * 3 / 30 = 31
            // July: 1,2,3,4 (4 days of 31) => 310 * 4 / 31 = 40
            // Total BenefitsCost = 31 + 40 = 71
            Assert.Equal(1000m, paycheck.GrossPay);
            Assert.Equal(71m, Math.Round(paycheck.BenefitsCost));
        }

        [Fact]
        public void Apply_SplitsBaseCostCorrectly_WhenPeriodSpansFebruaryAndMarch_LeapYear()
        {
            // Arrange
            var baseCost = 290m;
            var rule = new BaseCostSurchargeRule(baseCost);
            var paycheck = new Paycheck
            {
                GrossPay = 500m,
                BenefitsCost = 0m,
                PayPeriodStart = new DateTime(2024, 2, 27),
                PayPeriodEnd = new DateTime(2024, 3, 2)
            };
            var employee = new Employee();

            // Act
            rule.Apply(paycheck, employee);

            // Feb 2024: 27,28,29 (3 days of 29) => 290 * 3 / 29 = 30
            // Mar: 1,2 (2 days of 31) => 290 * 2 / 31 = 18.7
            // Total = 30 + 18.7 = 48.7
            Assert.Equal(500m, paycheck.GrossPay);
            Assert.Equal(49m, Math.Round(paycheck.BenefitsCost));
        }

        [Fact]
        public void Apply_DoesNotChangeGrossPay_WhenPeriodSpansMultipleMonths()
        {
            // Arrange
            var baseCost = 200m;
            var rule = new BaseCostSurchargeRule(baseCost);
            var paycheck = new Paycheck
            {
                GrossPay = 800m,
                BenefitsCost = 0m,
                PayPeriodStart = new DateTime(2024, 12, 30),
                PayPeriodEnd = new DateTime(2025, 1, 2)
            };
            var employee = new Employee();

            // Act
            rule.Apply(paycheck, employee);

            // Dec: 30,31 (2 days of 31) => 200 * 2 / 31 = 12.9
            // Jan: 1,2 (2 days of 31) => 200 * 2 / 31 = 12.9
            // Total = 12.9 + 12.9 = 25.8
            Assert.Equal(800m, paycheck.GrossPay);
            Assert.Equal(26m, Math.Round(paycheck.BenefitsCost));
        }

        [Fact]
        public void Apply_Handles_SingleDayPeriod()
        {
            // Arrange
            var baseCost = 50m;
            var rule = new BaseCostSurchargeRule(baseCost);
            var paycheck = new Paycheck
            {
                GrossPay = 300m,
                BenefitsCost = 0m,
                PayPeriodStart = new DateTime(2024, 8, 15),
                PayPeriodEnd = new DateTime(2024, 8, 15)
            };
            var employee = new Employee();

            // Act
            rule.Apply(paycheck, employee);

            // Single day in August, so 50 * 1 / 31 = 1.61
            Assert.Equal(300m, paycheck.GrossPay);
            Assert.Equal(1.61m, Math.Round(paycheck.BenefitsCost, 2));
        }
    }
}