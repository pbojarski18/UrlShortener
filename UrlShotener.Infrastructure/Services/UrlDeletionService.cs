using UrlShortener.Application.Abstractions.Data;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Services;

public class UrlDeletionService(IApplicationDbContext _dbContext) : IUrlDeletionService
{
    public async Task DeleteUrlAsync(UrlEntity url, CancellationToken ct)
    {
        if (url == null)
        {
            throw new ArgumentNullException(nameof(url));
        }

        _dbContext.Urls.Remove(url);
    }
}