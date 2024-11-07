namespace MultiShop.WebUI.Models
{
    public class TopbarViewModel
    {
        public ExchangeViewModel.Rates rates { get; set; }
        public string CityName { get; set; }
        public string Temperature { get; set; }
        public string WeatherIcon { get; set; }
        public string ErrorMessage { get; set; }
    }
}
