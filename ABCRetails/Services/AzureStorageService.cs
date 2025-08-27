using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using Azure.Storage.Files.Shares;
using ABCRetails.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ABCRetails.Services
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly TableClient _productTable;
        private readonly TableClient _customerTable;
        private readonly TableClient _orderTable;
        private readonly BlobContainerClient _blobContainer;
        private readonly QueueClient _queueClient;
        private readonly ShareClient _fileShareClient;

        public AzureStorageService(IConfiguration config)
        {
            var connStr = config["AzureStorage:ConnectionString"];
            _productTable = new TableClient(connStr, config["AzureStorage:ProductsTable"]);
            _customerTable = new TableClient(connStr, config["AzureStorage:CustomersTable"]);
            _orderTable = new TableClient(connStr, config["AzureStorage:OrdersTable"]);

            _blobContainer = new BlobContainerClient(connStr, config["AzureStorage:BlobContainerName"]);
            _queueClient = new QueueClient(connStr, config["AzureStorage:QueueName"]);
            _fileShareClient = new ShareClient(connStr, config["AzureStorage:FileShareName"]);
        }

        public async Task EnsureResourcesAsync()
        {
            await _productTable.CreateIfNotExistsAsync();
            await _customerTable.CreateIfNotExistsAsync();
            await _orderTable.CreateIfNotExistsAsync();
            await _blobContainer.CreateIfNotExistsAsync(PublicAccessType.None);
            await _queueClient.CreateIfNotExistsAsync();
            await _fileShareClient.CreateIfNotExistsAsync();
        }

        // === Products ===
        public async Task<List<Product>> GetProductsAsync()
        {
            var list = new List<Product>();
            await foreach (var item in _productTable.QueryAsync<Product>(p => p.PartitionKey == "PRODUCT"))
                list.Add(item);
            return list;
        }

        public async Task AddProductAsync(Product product)
        {
            if (string.IsNullOrEmpty(product.PartitionKey)) product.PartitionKey = "PRODUCT";
            if (string.IsNullOrEmpty(product.RowKey)) product.RowKey = Guid.NewGuid().ToString();
            await _productTable.AddEntityAsync(product);
        }

        // === Customers ===
        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            var list = new List<Customer>();
            await foreach (var c in _customerTable.QueryAsync<Customer>(x => x.PartitionKey == "CUSTOMER"))
                list.Add(c);
            return list;
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            if (string.IsNullOrEmpty(customer.PartitionKey)) customer.PartitionKey = "CUSTOMER";
            if (string.IsNullOrEmpty(customer.RowKey)) customer.RowKey = Guid.NewGuid().ToString();
            await _customerTable.AddEntityAsync(customer);
        }

        // === Orders ===
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var list = new List<Order>();
            await foreach (var o in _orderTable.QueryAsync<Order>(x => x.PartitionKey == "ORDER"))
                list.Add(o);
            return list;
        }

        public async Task AddOrderAsync(Order order)
        {
            if (string.IsNullOrEmpty(order.PartitionKey)) order.PartitionKey = "ORDER";
            if (string.IsNullOrEmpty(order.RowKey)) order.RowKey = Guid.NewGuid().ToString();
            await _orderTable.AddEntityAsync(order);
        }

        public async Task EnqueueOrderMessageAsync(string message)
        {
            await _queueClient.SendMessageAsync(message);
        }

        // === Uploads ===
        public async Task<string> UploadBlobAsync(string fileName, Stream fileStream)
        {
            var blobClient = _blobContainer.GetBlobClient($"{Guid.NewGuid()}-{fileName}");
            await blobClient.UploadAsync(fileStream, overwrite: true);
            return blobClient.Uri.ToString();
        }

        public async Task UploadContractAsync(string fileName, Stream fileStream)
        {
            var directory = _fileShareClient.GetRootDirectoryClient();
            await directory.CreateIfNotExistsAsync();
            var fileClient = directory.GetFileClient(fileName);
            await fileClient.CreateAsync(fileStream.Length);
            await fileClient.UploadAsync(fileStream);
        }
    }
}
