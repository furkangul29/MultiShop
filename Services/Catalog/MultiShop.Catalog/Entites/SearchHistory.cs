using MongoDB.Bson;

namespace MultiShop.Catalog.Entites
{
    public class SearchHistory
    {
        public ObjectId Id { get; set; }
        public string UserId { get; set; }
        public string SearchQuery { get; set; }
        public DateTime SearchDate { get; set; }
        public string UserIp { get; set; } // İsteğe bağlı
        public bool IsSuccess { get; set; } // Aramanın sonuç verip vermediğini takip etmek için
        public int ResultCount { get; set; } // Bulunan sonuç sayısı
    }
}
