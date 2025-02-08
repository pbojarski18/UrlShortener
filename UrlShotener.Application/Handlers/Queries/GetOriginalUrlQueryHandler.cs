using MediatR;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Abstractions.Data;

namespace UrlShortener.Application.Handlers.Queries;

public class GetOriginalUrlQueryHandler(IApplicationDbContext _dbContext) : IRequestHandler<GetOriginalUrlQuery, string?>
{
    public async Task<string> Handle(GetOriginalUrlQuery request, CancellationToken ct)
    {
        var urlEntity = await _dbContext.Urls.FirstOrDefaultAsync(p => p.ShortUrl == request.ShortUrl, ct);
        return urlEntity?.OriginalUrl;
    }
}