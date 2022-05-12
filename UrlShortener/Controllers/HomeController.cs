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

        [Route("/{urlCode}")]
        public async Task<IActionResult> ShortenedRedirect(string urlCode)
        {
            if (await _urlShortenedProvider.Exists(x => x.UrlCode == urlCode))
            {
                var redirectUrl = await _urlShortenedProvider.Get(x => x.UrlCode == urlCode);
                return Redirect(redirectUrl.OriginalUrl);
            }

            return Unauthorized("Wrong url code");
        }

        [HttpPost]
        public async Task<IActionResult> Shorten(LongUrl url)
        {
            if (!ModelState.IsValid || !url.IsValid())
            {
                _logger.LogError("Invalid url: {url.Value}");
                return new UnauthorizedResult();
            }

            if (await _urlShortenedProvider.Exists(x => x.Value == url.Value) || 
                await _urlShortenedProvider.Exists(x => x.OriginalUrl == url.Value))
            {
                var redirectUrl = await _urlShortenedProvider.Get(x 
                    => x.OriginalUrl == url.Value || x.Value == url.Value);

                _logger.LogInformation("Shortened url exists. Redirect to: {shortenedUrl}");
                return Redirect(redirectUrl.OriginalUrl);
            }

            var shortHost = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var shortenedUrl = await _urlShortener.GenerateAsync(shortHost, url);
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