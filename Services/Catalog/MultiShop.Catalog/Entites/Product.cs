﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MultiShop.Catalog.Entites
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal ProductPrice { get; set; }

        public string ProductImageUrl { get; set; }

        public string Color { get; set; }

        public string? Size { get; set; }

        public bool InStock { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string CategoryId { get; set; }

        [BsonIgnore]
        public Category Category { get; set; }
    }
}