WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    // Development only serves HTTP, and the Vite dev server proxies to it. Redirecting
    // to HTTPS locally would turn every proxied API call into a 307 the proxy cannot
    // follow, so this is on everywhere except the environment that would break.
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();
