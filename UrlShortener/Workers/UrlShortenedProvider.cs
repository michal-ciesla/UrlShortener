using UrlShortener.Models;

namespace UrlShortener.Workers;

public class UrlShortenedProvider : IUrlShortenedProvider
{
    private HashSet<ShortenedUrl> _localMemoryCache; 

    public UrlShortenedProvider()
    {
        _localMemoryCache = new HashSet<ShortenedUrl>();
    }

    public async Task<IEnumerable<ShortenedUrl>> ShortenedUrls() => _localMemoryCache;
    public async Task Load(ShortenedUrl url) => _localMemoryCache.Add(url);
    public async Task<bool> Exists(string url) => _localMemoryCache.Any(x => x.Value == url);
}