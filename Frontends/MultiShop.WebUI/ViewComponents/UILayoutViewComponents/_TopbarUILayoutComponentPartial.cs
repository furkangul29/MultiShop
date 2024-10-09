using Microsoft.AspNetCore.Mvc;
using MultiShop.RapidApiWebUI.Controllers;
using System.Net.Http;
using System.Threading.Tasks;

namespace MultiShop.WebUI.ViewComponents.UILayoutViewComponents
{
    public class _TopbarUILayoutComponentPartial : ViewComponent
    {
        private readonly DefaultController _defaultController;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public _TopbarUILayoutComponentPartial(DefaultController defaultController, IHttpContextAccessor httpContextAccessor)
        {
            _defaultController = defaultController;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Kullanıcının IP adresini alıyoruz
            var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                ViewBag.ErrorMessage = "IP adresi bulunamadı.";
                return View("Error");
            }

            // IP adresi ile şehir ismini alıyoruz
            var cityName = await _defaultController.GetCityFromIpAsync(ipAddress);

            if (string.IsNullOrWhiteSpace(cityName))
            {
                ViewBag.ErrorMessage = "Şehir bilgisi bulunamadı.";
                return View("Error");
            }

            // Şehir ismi ile hava durumu verilerini alıyoruz
            var weatherData = await _defaultController.GetWeatherData(cityName);
            if (weatherData?.current != null)
            {
                ViewBag.tempCity = weatherData.current.temp_c;
                ViewBag.CityName = cityName;
            }
            else
            {
                ViewBag.ErrorMessage = "Hava durumu verileri alınamadı.";
            }

            return View(weatherData);
        }
    }
}
