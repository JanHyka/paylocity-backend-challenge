using System;
using Api.Models;
using Api.Services.PaycheckServices.Calculator.ProrationPolicies;
using Xunit;

namespace ApiTests.UnitTests.Services.PaycheckServices.Calculator.ProrationPolicies
{
    public class FullProrationPolicyTests
    {
        private readonly FullProrationPolicy _policy = new();

        [Fact]
        public void GetProrationFactorDays_ReturnsOne_ForSameDay()
        {
            var dependent = new Dependent();
            var date = new DateTime(2024, 6, 1);

            int result = _policy.GetProrationFactorDays(dependent, date, date);

            Assert.Equal(1, result);
        }

        [Fact]
        public void GetProrationFactorDays_ReturnsCorrect_ForMultipleDays()
        {
            var dependent = new Dependent();
            var start = new DateTime(2024, 6, 1);
            var end = new DateTime(2024, 6, 14);

            int result = _policy.GetProrationFactorDays(dependent, start, end);

            Assert.Equal(14, result);
        }

        [Fact]
        public void GetProrationFactorDays_ReturnsCorrect_ForFullMonth()
        {
            var dependent = new Dependent();
            var start = new DateTime(2024, 7, 1);
            var end = new DateTime(2024, 7, 31);

            int result = _policy.GetProrationFactorDays(dependent, start, end);

            Assert.Equal(31, result);
        }

        [Fact]
        public void GetProrationFactorDays_Works_AcrossMonths()
        {
            var dependent = new Dependent();
            var start = new DateTime(2024, 6, 28);
            var end = new DateTime(2024, 7, 2);

            int result = _policy.GetProrationFactorDays(dependent, start, end);

            Assert.Equal(5, result);
        }

        [Fact]
        public void GetProrationFactorDays_Works_AcrossYears()
        {
            var dependent = new Dependent();
            var start = new DateTime(2023, 12, 30);
            var end = new DateTime(2024, 1, 2);

            int result = _policy.GetProrationFactorDays(dependent, start, end);

            Assert.Equal(4, result);
        }
    }
}