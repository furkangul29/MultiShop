namespace MultiShop.WebUI.Models
{
    public class FilterViewModel
    {
        public List<string> SelectedColors { get; set; } = new List<string>();
        public List<string> SelectedPrices { get; set; } = new List<string>();
        public List<string> SelectedSizes { get; set; } = new List<string>();
    }
}