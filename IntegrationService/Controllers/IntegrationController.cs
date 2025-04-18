using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using IntegrationService.Helpers;

namespace IntegrationService.Controllers
{
    [ApiController]
    [Route("api/integrated-data")]
    public class IntegrationController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDistributedCache _cache;
        private readonly SoapHelper _soapHelper;

        public IntegrationController(IHttpClientFactory httpClientFactory, IDistributedCache cache, SoapHelper soapHelper)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _soapHelper = soapHelper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string cacheKey = "integrated_data";
            /*
            var cached = await _cache.GetStringAsync(cacheKey);

            if (cached != null)
            {
                return Ok(JsonSerializer.Deserialize<object>(cached));
            }
            */
            var restClient = _httpClientFactory.CreateClient("RestService");
            string restResult = await restClient.GetStringAsync("api/data");

            string jobId = await _soapHelper.StartJobAsync("user123");
            string soapResult = await _soapHelper.PollJobResultAsync(jobId);

            var combinedResult = new
            {
                rest = restResult,
                soap = soapResult
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(combinedResult),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            return Ok(combinedResult);
        }
    }
}
