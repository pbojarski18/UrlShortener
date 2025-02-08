using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class BaseController : ControllerBase
{
}