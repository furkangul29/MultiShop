using Microsoft.AspNetCore.Mvc;
using MultiShop.RapidApiWebUI.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace MultiShop.RapidApiWebUI.Controllers
{
    public class DefaultController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _rapidApiKey;

        public DefaultController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _rapidApiKey = configuration["RapidApiKey"];
        }

        // IP adresine göre şehir bilgisi alınıyor
        public async Task<string> GetCityFromIpAsync(string ipAddress)
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"http://ip-api.com/json/{ipAddress}")
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var locationData = JsonConvert.DeserializeObject<dynamic>(body);
                return locationData?.city;
            }
        }

        // Şehir bilgisine göre hava durumu verisi alınıyor
        public async Task<WeatherViewModel.Rootobject> GetWeatherData(string cityName)
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://weather-api-by-any-city.p.rapidapi.com/weather/{cityName}"),
                Headers =
                {
                    { "x-rapidapi-key", _rapidApiKey },
                    { "x-rapidapi-host", "weather-api-by-any-city.p.rapidapi.com" },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var weatherData = JsonConvert.DeserializeObject<WeatherViewModel.Rootobject>(body);
                return weatherData;
            }
        }

        public async Task<IActionResult> Exchange()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://currency-conversion-and-exchange-rates.p.rapidapi.com/latest?from=USD&to=EUR%2CGBP%2CTRY"),
                Headers =
    {
        { "x-rapidapi-key", _rapidApiKey },
        { "x-rapidapi-host", "currency-conversion-and-exchange-rates.p.rapidapi.com" },
    },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var exchangeData = JsonConvert.DeserializeObject<ExchangeViewModel.Rootobject>(body);
                return Json(exchangeData);

            }

        }
    }

}
