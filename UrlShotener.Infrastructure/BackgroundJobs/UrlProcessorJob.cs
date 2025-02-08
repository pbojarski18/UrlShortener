using Microsoft.EntityFrameworkCore;
using Quartz;
using UrlShortener.Application.Abstractions.Data;
using UrlShortener.Infrastructure.Services;

namespace UrlShortener.Infrastructure.BackgroundJobs;

public class UrlProcessorJob(IApplicationDbContext _dbContext,
                             IUrlDeletionService _urlDeletionService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await ProcessAsync(context.CancellationToken);
    }

    public async Task ProcessAsync(CancellationToken ct)
    {
        var expiredUrls = await _dbContext.Urls
            .Where(p => p.ExpirationDate <= DateTime.Now)
            .ToListAsync(ct);

        expiredUrls.ForEach(u => _urlDeletionService.DeleteUrlAsync(u, ct));

        await _dbContext.SaveChangesAsync(ct);
    }
}