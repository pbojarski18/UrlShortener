using MediatR;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Abstractions.Data;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Services;

namespace UrlShortener.Application.Handlers.Commands;

public class CreateShortUrlCommandHandler(IApplicationDbContext _dbContext,
                                          IShortUrlGeneratorService _shortUrlGeneratorService) : IRequestHandler<CreateShortUrlCommand, string>
{
    public async Task<string> Handle(CreateShortUrlCommand request, CancellationToken ct)
    {
        if (!IsValidUrl(request.OriginalUrl))
        {
            throw new ArgumentException("Invalid URL");
        }
        var existingUrl = await _dbContext.Urls.FirstOrDefaultAsync(p => p.OriginalUrl == request.OriginalUrl, ct);

        if (existingUrl != null)
        {
            return existingUrl.ShortUrl;
        }

        var shortUrl = await _shortUrlGeneratorService.GenerateShortUrl(ct);

        var urlEntity = new UrlEntity()
        {
            OriginalUrl = request.OriginalUrl,
            ShortUrl = shortUrl,
            CreatedTime = DateTime.Now,
            ExpirationDate = DateTime.Now.AddDays(14)
        };

        _dbContext.Urls.Add(urlEntity);
        await _dbContext.SaveChangesAsync(ct);

        return shortUrl;
    }

    private bool IsValidUrl(string originalUrl)
    {
        return Uri.TryCreate(originalUrl, UriKind.Absolute, out var uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}