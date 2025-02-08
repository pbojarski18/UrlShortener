namespace UrlShortener.Infrastructure.Services;

public interface IShortUrlGeneratorService
{
    Task<string> GenerateShortUrl(CancellationToken ct);
}