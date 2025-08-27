using Azure;
using Azure.Data.Tables;
using System;

namespace ABCRetails.Models
{
    public class Product : ITableEntity
    {
        // Table entity system properties
        public string PartitionKey { get; set; } = "PRODUCT";
        public string RowKey { get; set; } = Guid.NewGuid().ToString();
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }

        // Domain properties
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }            // Use double for Azure Tables
        public int StockAvailable { get; set; }
        public string? ImageUrl { get; set; }        // Optional for future blob storage

        // Factory to ensure keys are set
        public static Product Create(string productId = null)
        {
            return new Product
            {
                PartitionKey = "PRODUCT",
                RowKey = string.IsNullOrWhiteSpace(productId) ? Guid.NewGuid().ToString() : productId
            };
        }
    }
}
