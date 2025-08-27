namespace ABCRetails.Models
{
    using Azure;
    using Azure.Data.Tables;
    using System;

    public class Customer : ITableEntity
    {
        // Table entity system properties
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }       // Use CustomerId as RowKey
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }

        // Domain properties
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string ShippingAddress { get; set; }

        // Default constructor (needed by Table SDK)
        public Customer() { }

        // Factory method to safely create a Customer with keys set
        public static Customer Create(string customerId)
        {
            return new Customer
            {
                PartitionKey = "CUSTOMER",
                RowKey = customerId
            };
        }
    }
}