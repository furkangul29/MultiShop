using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MultiShop.Catalog.Entites
{
    public class DealsOfDay
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DealsOfDayId { get; set; }
        public string ProductId { get; set; }
        public string CategoryId { get; set; }
        public int DiscountPercentage { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
