namespace MultiShop.WebUI.Models
{
    public class ExchangeViewModel
    {

        public class Rootobject
        {
            public int timestamp { get; set; }
            public string _base { get; set; }
            public bool success { get; set; }
            public string date { get; set; }
            public Rates rates { get; set; }
        }

        public class Rates
        {
            public float USD { get; set; }
            public int EUR { get; set; }
            public float GBP { get; set; }
            public float JPY { get; set; }
            public float CNY { get; set; }
            public float TRY { get; set; }
        }


    }
}
