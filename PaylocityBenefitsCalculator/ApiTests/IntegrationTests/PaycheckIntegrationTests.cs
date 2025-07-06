using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Api.Dtos.Paycheck;
using Api.Models;
using Xunit;

namespace ApiTests.IntegrationTests;

public class PaycheckIntegrationTests : IntegrationTest
{
    [Fact]
    public async Task GetPaycheck_BasicEmployee_NoDependents_UnderThreshold()
    {
        // Act: call paycheck endpoint
        var startDate = new DateTime(2024, 6, 1);
        var response = await HttpClient.GetAsync(
            $"/api/v1/paychecks/user/1/from/{startDate:yyyy-MM-dd}/periodicity/BiWeekly");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<GetPaycheckDto>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse!.Success);
        Assert.Equal(1, apiResponse.Data!.EmployeeId);
        // 75420.99 * 14/366 (leap year) = 2884.96
        Assert.Equal(2884.96m, apiResponse.Data.GrossPay);
        // BaseCost: 1000/30*14 = 466.67
        Assert.Equal(466.67m, apiResponse.Data.BenefitsCost);
        // NetPay: 2884.96 - 466.67 = 2418.29
        Assert.Equal(2418.29m, apiResponse.Data.NetPay);
    }

    [Fact]
    public async Task GetPaycheck_ReturnsNotFound_ForNonexistentEmployee()
    {
        var startDate = new DateTime(2024, 6, 1);
        var response = await HttpClient.GetAsync(
            $"/api/v1/paychecks/user/99999/from/{startDate:yyyy-MM-dd}/periodicity/BiWeekly");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}