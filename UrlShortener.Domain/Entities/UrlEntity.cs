namespace UrlShortener.Domain.Entities;

public class UrlEntity
{
    public int Id { get; set; }

    public string OriginalUrl { get; set; }

    public string ShortUrl { get; set; }

    public DateTime CreatedTime { get; set; }

    public DateTime ExpirationDate { get; set; }
}