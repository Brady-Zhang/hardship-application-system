using System.Net;
using System.Net.Http.Json;
using System.Text;
using HardshipApp.Api.Contracts;
using HardshipApp.IntegrationTests.Support;
using Microsoft.AspNetCore.Mvc;

namespace HardshipApp.IntegrationTests.Api;

public class CreateApplicationTests : IClassFixture<HardshipApiFactory>
{
    private readonly HttpClient _client;

    public CreateApplicationTests(HardshipApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Post_WithValidApplication_ReturnsCreatedAndPersistsRecord()
    {
        HardshipApplicationRequest request = AValidRequest();

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/applications", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        HardshipApplicationResponse? created =
            await response.Content.ReadFromJsonAsync<HardshipApplicationResponse>();

        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal(request.CustomerName, created.CustomerName);
        Assert.Equal(request.MonthlyIncome, created.MonthlyIncome);

        // Created, and still there on a fresh request — not just echoed back.
        HardshipApplicationResponse? fetched = await _client
            .GetFromJsonAsync<HardshipApplicationResponse>($"/api/applications/{created.Id}");

        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched.Id);
        Assert.Equal(request.CustomerName, fetched.CustomerName);
        Assert.Equal(request.HardshipReason, fetched.HardshipReason);
    }

    [Fact]
    public async Task Post_ReturnsTheSurplusComputedByTheServer()
    {
        HardshipApplicationRequest request = AValidRequest() with
        {
            MonthlyIncome = 4321.55m,
            MonthlyExpenses = 1234.45m,
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/applications", request);
        HardshipApplicationResponse? created =
            await response.Content.ReadFromJsonAsync<HardshipApplicationResponse>();

        Assert.Equal(3087.10m, created!.MonthlySurplus);
        Assert.False(created.IsInDeficit);
    }

    [Fact]
    public async Task Post_AcceptsAnApplicantSpendingMoreThanTheyEarn()
    {
        HardshipApplicationRequest request = AValidRequest() with
        {
            MonthlyIncome = 2500m,
            MonthlyExpenses = 4100m,
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/applications", request);
        HardshipApplicationResponse? created =
            await response.Content.ReadFromJsonAsync<HardshipApplicationResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(-1600m, created!.MonthlySurplus);
        Assert.True(created.IsInDeficit);
    }

    [Fact]
    public async Task Post_WithNoCustomerName_ReturnsFieldLevelValidationErrors()
    {
        HardshipApplicationRequest request = AValidRequest() with { CustomerName = "" };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/applications", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        ValidationProblemDetails? problem =
            await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // The front end needs to know which field, not just that something was wrong.
        Assert.NotNull(problem);
        Assert.Contains("CustomerName", problem.Errors.Keys);
        Assert.Contains("Customer name is required.", problem.Errors["CustomerName"]);
    }

    [Fact]
    public async Task Post_WithAnUnreadableField_SaysSoWithoutLeakingInternals()
    {
        StringContent body = new(
            """{"customerName":"Alex","dateOfBirth":"not-a-date","monthlyIncome":1,"monthlyExpenses":1}""",
            Encoding.UTF8,
            "application/json");

        HttpResponseMessage response = await _client.PostAsync("/api/applications", body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string raw = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("HardshipApp", raw, StringComparison.Ordinal);
        Assert.DoesNotContain("LineNumber", raw, StringComparison.Ordinal);

        ValidationProblemDetails? problem =
            await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Same shape and the same field naming as a business validation failure, so the
        // front end has one thing to handle.
        Assert.NotNull(problem);
        Assert.Contains("DateOfBirth", problem.Errors.Keys);
    }

    [Fact]
    public async Task GetById_WithAnUnknownId_ReturnsNotFound()
    {
        HttpResponseMessage response = await _client.GetAsync($"/api/applications/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static HardshipApplicationRequest AValidRequest() => new(
        CustomerName: "Alex Taylor",
        DateOfBirth: new DateOnly(1990, 4, 17),
        MonthlyIncome: 5000m,
        MonthlyExpenses: 3200m,
        HardshipReason: "Reduced hours at work");
}
