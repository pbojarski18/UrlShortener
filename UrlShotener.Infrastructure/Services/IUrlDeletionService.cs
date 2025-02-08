using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Services;

public interface IUrlDeletionService
{
    Task DeleteUrlAsync(UrlEntity url, CancellationToken ct);
}