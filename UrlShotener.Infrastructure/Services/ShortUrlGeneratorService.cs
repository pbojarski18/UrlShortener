using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Abstractions.Data;

namespace UrlShortener.Infrastructure.Services;

public class ShortUrlGeneratorService(IApplicationDbContext _dbContext) : IShortUrlGeneratorService
{
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const int ShortUrlLength = 8;
    private readonly Random _random = new Random();

    public async Task<string> GenerateShortUrl(CancellationToken ct)
    {
        var chars = new char[ShortUrlLength];
        for (var i = 0; i < ShortUrlLength; i++)
        {
            chars[i] = Alphabet[_random.Next(Alphabet.Length)];
        }
        if (await IsShortUrlExists(new string(chars), ct))
        {
            return await GenerateShortUrl(ct);
        }
        return new string(chars);
    }

    private async Task<bool> IsShortUrlExists(string shortUrl, CancellationToken ct)
    {
        return await _dbContext.Urls.AnyAsync(p => p.ShortUrl == shortUrl, ct);
    }
}