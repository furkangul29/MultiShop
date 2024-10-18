namespace MultiShop.WebUI.Models
{
    public class FilterViewModel
    {
        public List<PriceRangeViewModel>? SelectedPrices { get; set; }
        public List<string>? SelectedColors { get; set; }
        public List<string>? SelectedSizes { get; set; }
        public string CategoryId { get; set; }
    }

    public class PriceRangeViewModel
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
