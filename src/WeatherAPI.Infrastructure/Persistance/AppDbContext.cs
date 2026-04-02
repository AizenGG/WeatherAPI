using Microsoft.EntityFrameworkCore;
using WeatherAPI.Domain.Entites;

namespace WeatherAPI.Infrastructure.Persistance;

public class AppDbContext : DbContext
{
    public DbSet<RequestLog> Requests => Set<RequestLog>();
    public DbSet<DailyStats> DailyStats => Set<DailyStats>();
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RequestLog>()
            .HasIndex(x => new
            {
                x.City,
                x.Date
            });
        modelBuilder.Entity<RequestLog>()
            .HasIndex(x => new
            { 
                x.TimestampUtc
            });
        modelBuilder.Entity<DailyStats>()
            .HasKey(x => new
            {
                x.Date,
                x.City
            });
    }
}