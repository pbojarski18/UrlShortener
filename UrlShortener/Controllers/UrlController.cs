using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using UrlShortener.Application.Handlers.Commands;
using UrlShortener.Application.Handlers.Queries;

namespace UrlShortener.API.Controllers;

public class UrlController(IMediator _mediator,
                           IHttpContextAccessor _httpContext,
                           HybridCache _cache) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateShortUrlCommand command)
    {
        var shortUrl = await _mediator.Send(command);
        shortUrl = $"{_httpContext.HttpContext.Request.Scheme}://{_httpContext.HttpContext.Request.Host}/api/v1/url/{shortUrl}";
        await _cache.SetAsync(shortUrl, command.OriginalUrl, default);
        return Ok(shortUrl);
    }

    [HttpGet("{shortUrl}")]
    public async Task<IActionResult> RedirectAsync([FromRoute] string shortUrl)
    {
        if (string.IsNullOrWhiteSpace(shortUrl))
        {
            return BadRequest("Invalid short URL");
        }

        var cachedKey = shortUrl;
        var cachedValue = await _cache.GetOrCreateAsync<string>(cachedKey, async ct =>
        {
            return await _mediator.Send(new GetOriginalUrlQuery { ShortUrl = shortUrl }, ct);
        });

        if (string.IsNullOrWhiteSpace(cachedValue))
        {
            return NotFound("Short URL not found.");
        }

        return Redirect(cachedValue);
    }
}