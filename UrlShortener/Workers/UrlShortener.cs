using UrlShortener.Models;

namespace UrlShortener.Workers;

public class UrlShortener : IUrlShortener
{
    private const string _base = "https://localhost:5555";
    const string BaseUrlChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private readonly ILogger<UrlShortener> _logger;

    public UrlShortener(ILogger<UrlShortener> logger)
    {
        _logger = logger;
    }

    public async Task<ShortenedUrl> GenerateAsync(LongUrl longUrl)
    {
        if (longUrl is null || string.IsNullOrWhiteSpace(longUrl.Value))
        {
            _logger.LogError("Invalid long Uri");
            throw new ArgumentException("Invalid long Uri");
        }

        var shortenedUrl = new ShortenedUrl();

        await Task.Run(() =>
        {
            const int numberOfCharsToSelect = 5;
            int maxNumber = BaseUrlChars.Length;

            var rnd = new Random();
            var numList = new List<int>();

            for (int i = 0; i < numberOfCharsToSelect; i++)
                numList.Add(rnd.Next(maxNumber));

            var urlCode = numList.Aggregate(string.Empty, (current, num) => 
                current + BaseUrlChars.Substring(num, 1));

            shortenedUrl.Value = $"{_base}/{urlCode}";
        });

        return shortenedUrl;
    }
}