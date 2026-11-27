using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HardshipApp.IntegrationTests.Support;

/// <summary>
/// The real application, over its own throwaway SQLite file.
/// </summary>
public sealed class HardshipApiFactory : WebApplicationFactory<Program>
{
    private readonly string _path =
        Path.Combine(Path.GetTempPath(), $"hardship-api-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Default", $"Data Source={_path}");

        // Development, so the pipeline does not try to redirect the test client to HTTPS.
        builder.UseEnvironment("Development");
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing && File.Exists(_path))
        {
            File.Delete(_path);
        }
    }
}
