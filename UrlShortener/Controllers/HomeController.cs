using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UrlShortener.Models;
using UrlShortener.Workers;

namespace UrlShortener.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUrlShortenedProvider _urlShortenedProvider;
        private readonly IUrlShortener _urlShortener;

        public HomeController(ILogger<HomeController> logger, 
            IUrlShortenedProvider urlShortenedProvider, IUrlShortener urlShortener)
        {
            _logger = logger;
            this._urlShortenedProvider = urlShortenedProvider;
            _urlShortener = urlShortener;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ShortenedUrls()
        {
            return View((IEnumerable<ShortenedUrl>)await _urlShortenedProvider.ShortenedUrls());
        }

        [HttpPost]
        public async Task<IActionResult> Shorten(LongUrl url)
        {
            if (!ModelState.IsValid || !url.IsValid())
            {
                _logger.LogError("Invalid url: {url.Value}");
                return new UnauthorizedResult();
            }

            if (await _urlShortenedProvider.Exists(url.Value))
            {
                _logger.LogInformation("Shortened url exists. Redirect to: {shortenedUrl}");
                return Redirect(url.Value);
            }

            var shortenedUrl = await _urlShortener.GenerateAsync(url);
            shortenedUrl.OriginalUrl = url.Value;

            await _urlShortenedProvider.Load(shortenedUrl);

            ViewBag.Uploaded = true;
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}