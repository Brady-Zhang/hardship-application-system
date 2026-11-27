using System.Net;
using System.Net.Http.Json;
using HardshipApp.Api.Contracts;
using HardshipApp.IntegrationTests.Support;
using Microsoft.AspNetCore.Mvc;

namespace HardshipApp.IntegrationTests.Api;

public class ListAndUpdateApplicationsTests : IClassFixture<HardshipApiFactory>
{
    private readonly HttpClient _client;

    public ListAndUpdateApplicationsTests(HardshipApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Get_ReturnsEveryApplicationNewestFirst()
    {
        HardshipApplicationResponse first = await Create(ARequest() with { CustomerName = "First in" });
        HardshipApplicationResponse second = await Create(ARequest() with { CustomerName = "Second in" });

        List<HardshipApplicationResponse>? all = await _client
            .GetFromJsonAsync<List<HardshipApplicationResponse>>("/api/applications");

        Assert.NotNull(all);
        Assert.Contains(all, application => application.Id == first.Id);
        Assert.Contains(all, application => application.Id == second.Id);

        // Asserted as a property of the whole list rather than as the position of these two:
        // the clock is real here, so two records created back to back can share an instant.
        // The deterministic ordering test runs against the repository with a fake clock.
        List<DateTimeOffset> timestamps = all.ConvertAll(application => application.CreatedAt);
        Assert.Equal(timestamps.OrderByDescending(timestamp => timestamp), timestamps);
    }

    [Fact]
    public async Task Put_UpdatesTheRecordAndReturnsIt()
    {
        HardshipApplicationResponse created = await Create(ARequest());

        HardshipApplicationRequest change = ARequest() with
        {
            CustomerName = "Alex Taylor-Smith",
            MonthlyExpenses = 6000m,
            HardshipReason = "Medical costs",
        };

        HttpResponseMessage response =
            await _client.PutAsJsonAsync($"/api/applications/{created.Id}", change);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        HardshipApplicationResponse? updated =
            await response.Content.ReadFromJsonAsync<HardshipApplicationResponse>();

        Assert.NotNull(updated);
        Assert.Equal("Alex Taylor-Smith", updated.CustomerName);
        Assert.Equal(6000m, updated.MonthlyExpenses);
        Assert.Equal(-1000m, updated.MonthlySurplus);
        Assert.True(updated.IsInDeficit);

        HardshipApplicationResponse? fetched = await _client
            .GetFromJsonAsync<HardshipApplicationResponse>($"/api/applications/{created.Id}");

        Assert.Equal("Alex Taylor-Smith", fetched!.CustomerName);
    }

    [Fact]
    public async Task Put_SetsUpdatedAtAndLeavesCreatedAtAlone()
    {
        HardshipApplicationResponse created = await Create(ARequest());
        Assert.Null(created.UpdatedAt);

        HttpResponseMessage response = await _client.PutAsJsonAsync(
            $"/api/applications/{created.Id}",
            ARequest() with { MonthlyIncome = 5500m });

        HardshipApplicationResponse? updated =
            await response.Content.ReadFromJsonAsync<HardshipApplicationResponse>();

        Assert.NotNull(updated);
        Assert.NotNull(updated.UpdatedAt);
        Assert.Equal(created.CreatedAt, updated.CreatedAt);
    }

    [Fact]
    public async Task Put_WithAnUnknownId_ReturnsProblemDetails()
    {
        HttpResponseMessage response =
            await _client.PutAsJsonAsync($"/api/applications/{Guid.NewGuid()}", ARequest());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        ProblemDetails? problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal("Application not found", problem!.Title);
    }

    [Fact]
    public async Task Put_WithInvalidFields_ReturnsTheSameValidationShapeAsCreate()
    {
        HardshipApplicationResponse created = await Create(ARequest());

        HttpResponseMessage response = await _client.PutAsJsonAsync(
            $"/api/applications/{created.Id}",
            ARequest() with { CustomerName = "", MonthlyIncome = -1m });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        ValidationProblemDetails? problem =
            await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.Contains("CustomerName", problem!.Errors.Keys);
        Assert.Contains("MonthlyIncome", problem.Errors.Keys);
    }

    [Fact]
    public async Task Put_DoesNotChangeTheRecordWhenValidationFails()
    {
        HardshipApplicationResponse created = await Create(ARequest());

        await _client.PutAsJsonAsync(
            $"/api/applications/{created.Id}",
            ARequest() with { CustomerName = "" });

        HardshipApplicationResponse? fetched = await _client
            .GetFromJsonAsync<HardshipApplicationResponse>($"/api/applications/{created.Id}");

        Assert.Equal(created.CustomerName, fetched!.CustomerName);
        Assert.Null(fetched.UpdatedAt);
    }

    private async Task<HardshipApplicationResponse> Create(HardshipApplicationRequest request)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/applications", request);
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<HardshipApplicationResponse>())!;
    }

    private static HardshipApplicationRequest ARequest() => new(
        CustomerName: "Alex Taylor",
        DateOfBirth: new DateOnly(1990, 4, 17),
        MonthlyIncome: 5000m,
        MonthlyExpenses: 3200m,
        HardshipReason: "Reduced hours at work");
}
