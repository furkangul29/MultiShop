using Microsoft.AspNetCore.Mvc;
using MultiShop.DtoLayer.CatalogDtos.AboutDtos;
using Newtonsoft.Json;

namespace MultiShop.WebUI.ViewComponents.UILayoutViewComponents
{
    public class _FooterUILayoutComponentPartial:ViewComponent
    {
        private readonly IHttpClientFactory _clientFactory;

        public _FooterUILayoutComponentPartial(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task< IViewComponentResult> InvokeAsync()
        {
            var client = _clientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7000/api/Abouts");
            if (responseMessage.IsSuccessStatusCode)
            {
                var response = await responseMessage.Content.ReadAsStringAsync();
                var abouts = JsonConvert.DeserializeObject<List<ResultAboutDto>>(response);
                return View(abouts);
            }
            return View();
        }
    }
}
