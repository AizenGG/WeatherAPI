using Microsoft.EntityFrameworkCore;
using Polly;
using WeatherAPI.Application.Interfaces;
using WeatherAPI.Application.Services;
using WeatherAPI.Infrastructure.ExternalApi;
using WeatherAPI.Infrastructure.Persistance;
using WeatherAPI.Middlewares;

var  builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient<IWeatherProvider, OpenMeteoClient>()
    .ConfigureHttpClient(c => c.Timeout = TimeSpan.FromSeconds(5))
    .AddTransientHttpErrorPolicy(p =>
        p.WaitAndRetryAsync(2, attempt =>
            TimeSpan.FromMilliseconds(200 * attempt) +
            TimeSpan.FromMilliseconds(Random.Shared.Next(0, 100))));
builder.Services.AddHttpClient<IGeocodingService, GeocodingService>()
    .AddTransientHttpErrorPolicy(p =>
        p.WaitAndRetryAsync(2, attempt =>
            TimeSpan.FromMilliseconds(200 * attempt) +
            TimeSpan.FromMilliseconds(Random.Shared.Next(0, 100))))
    .AddTransientHttpErrorPolicy(p =>
        p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<ExceptionsMiddleware>();
app.UseStaticFiles(new StaticFileOptions
{
    RequestPath = "/static"
});
app.UseHttpsRedirection();
app.UseAuthorization();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
app.MapControllers();
app.Run();
