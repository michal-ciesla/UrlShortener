using UrlShortener.Models;

namespace UrlShortener.Workers;

public interface IUrlShortener
{
    public Task<ShortenedUrl> GenerateAsync(LongUrl longUrl);
}