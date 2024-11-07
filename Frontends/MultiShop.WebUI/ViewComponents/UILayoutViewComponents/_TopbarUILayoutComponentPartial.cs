using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace MultiShop.WebUI.ViewComponents.UILayoutViewComponents
{
    public class _TopbarUILayoutComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _rapidApiKey;

        public _TopbarUILayoutComponentPartial(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _rapidApiKey = configuration["RapidApiKey"];
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var exchangeData = await GetExchangeRatesAsync();

            string cityName = await GetCityFromIpAsync(ipAddress);

            var model = new TopbarViewModel();

            if (!string.IsNullOrWhiteSpace(cityName))
            {
                var weatherData = await GetWeatherData(cityName);

                // Hava durumu verilerini ViewBag'e atıyoruz
                model.CityName = cityName;
                model.Temperature = weatherData?.list[0]?.main?.temp.ToString("0.0"); // Celsius değeri, virgülden sonra bir hane

                // Hava durumu durumuna göre simge seçimi
                var weatherCondition = weatherData?.list[0]?.weather[0]?.main.ToLower();
                switch (weatherCondition)
                {
                    case "clear":
                        model.WeatherIcon = "fas fa-sun"; // Güneş
                        break;
                    case "clouds":
                        model.WeatherIcon = "fas fa-cloud"; // Bulut
                        break;
                    case "rain":
                        model.WeatherIcon = "fas fa-cloud-showers-heavy"; // Yağmur
                        break;
                    case "snow":
                        model.WeatherIcon = "fas fa-snowflake"; // Kar
                        break;
                    case "thunderstorm":
                        model.WeatherIcon = "fas fa-bolt"; // Fırtına
                        break;
                    default:
                        model.WeatherIcon = "fas fa-question-circle"; // Bilinmeyen hava durumu
                        break;
                }
            }
            else
            {
                model.ErrorMessage = "Konum bilgisi alınamadı.";
            }

            model.rates = exchangeData.rates;

            return View(model);
        }

        private async Task<string> GetCityFromIpAsync(string ipAddress)
        {
            if (ipAddress == "127.0.0.1" || ipAddress == "::1")
            {
                return "Istanbul"; // Örnek şehir adı
            }
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

        private async Task<WeatherViewModel.Rootobject> GetWeatherData(string cityName)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://weather-pro-api.p.rapidapi.com/forecast?city={cityName}&units=metric"),
                Headers =
            {
                { "x-rapidapi-key",_rapidApiKey },
                { "x-rapidapi-host", "weather-pro-api.p.rapidapi.com" },
            },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<WeatherViewModel.Rootobject>(body);
            }
        }

        private async Task<ExchangeViewModel.Rootobject> GetExchangeRatesAsync()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://currency-conversion-and-exchange-rates.p.rapidapi.com/latest?from=EUR&to=USD,GBP,TRY"),
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
                return JsonConvert.DeserializeObject<ExchangeViewModel.Rootobject>(body);
            }
        }
    }
}
