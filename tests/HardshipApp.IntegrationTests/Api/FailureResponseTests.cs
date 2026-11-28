using System.Net;
using System.Net.Http.Json;
using HardshipApp.IntegrationTests.Support;
using HardshipApp.Models;
using HardshipApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HardshipApp.IntegrationTests.Api;

/// <summary>
/// What the API says when something goes wrong that no validator anticipated.
/// </summary>
public class FailureResponseTests : IClassFixture<HardshipApiFactory>
{
    private readonly HardshipApiFactory _factory;

    public FailureResponseTests(HardshipApiFactory factory) => _factory = factory;

    [Fact]
    public async Task AnUnhandledFailure_ReturnsProblemDetailsAndSaysNothingAboutTheInternals()
    {
        HttpClient client = WithAFailingService();

        HttpResponseMessage response = await client.GetAsync("/api/applications");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        ProblemDetails? problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("An unexpected error occurred", problem!.Title);

        // The log gets the exception; the caller gets none of it.
        string body = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("The database is on fire", body);
        Assert.DoesNotContain("HardshipApp", body);
        Assert.DoesNotContain("at ", body);
    }

    [Fact]
    public async Task AnUnknownPath_ReturnsProblemDetailsRatherThanAnEmptyBody()
    {
        HttpClient client = _factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/api/nothing-here");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        ProblemDetails? problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal(404, problem!.Status);
    }

    [Fact]
    public async Task AnIdThatIsNotAGuid_ReturnsProblemDetails()
    {
        HttpClient client = _factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/api/applications/not-a-guid");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    private HttpClient WithAFailingService() =>
        _factory
            .WithWebHostBuilder(builder => builder.ConfigureServices(services =>
            {
                services.RemoveAll<IHardshipApplicationService>();
                services.AddScoped<IHardshipApplicationService, FailingService>();
            }))
            .CreateClient();

    private sealed class FailingService : IHardshipApplicationService
    {
        public Task<IReadOnlyList<HardshipApplication>> ListAsync(CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("The database is on fire");

        public Task<HardshipApplication> CreateAsync(
            HardshipApplication application,
            CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("The database is on fire");

        public Task<HardshipApplication?> FindAsync(Guid id, CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("The database is on fire");

        public Task<HardshipApplication> UpdateAsync(
            HardshipApplication application,
            CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("The database is on fire");
    }
}
