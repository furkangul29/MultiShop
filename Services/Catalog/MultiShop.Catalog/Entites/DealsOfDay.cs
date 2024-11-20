using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MultiShop.Catalog.Entites
{
    public class DealsOfDay
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DealsOfDayId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string CategoryId { get; set; }

        public int DiscountPercentage { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal OriginalPrice { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal DiscountedPrice { get; set; }

        public bool IsActive { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [BsonIgnore]
        public Product Product { get; set; }
    }
}
