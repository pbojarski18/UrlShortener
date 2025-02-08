using MediatR;

namespace UrlShortener.Application.Handlers.Queries;

public class GetOriginalUrlQuery : IRequest<string>
{
    public string? ShortUrl { get; set; }
}