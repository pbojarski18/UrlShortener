using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Abstractions.Data;
using UrlShortener.Domain.Entities;

namespace UrlShotener.Infrastructure.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<UrlEntity> Urls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}