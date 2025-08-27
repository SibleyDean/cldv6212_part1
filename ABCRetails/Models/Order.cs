using Azure.Data.Tables;
using Azure;

namespace ABCRetails.Models
{
    public class Order : ITableEntity
    {
        public string PartitionKey { get; set; } = "ORDER";
        public string RowKey { get; set; } // orderId
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }

        public string CustomerId { get; set; }
        public string Username { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public DateTime OrderDate { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } // Pending, Processing, Completed, Cancelled
    }

}
