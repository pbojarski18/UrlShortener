using MediatR;

namespace UrlShortener.Application.Handlers.Commands;

public class CreateShortUrlCommand : IRequest<string>
{
    public string OriginalUrl { get; set; }
}