using UrlShortener.Models;

namespace UrlShortener.Workers;

public interface IUrlShortenedProvider
{
    Task<IEnumerable<ShortenedUrl>> ShortenedUrls();
    Task Load(ShortenedUrl url);
    Task<bool> Exists(string url);
}