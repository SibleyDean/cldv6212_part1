using ABCRetails.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ABCRetails.Services
{
    public interface IAzureStorageService
    {
        // === Products ===
        Task<List<Product>> GetProductsAsync();
        Task AddProductAsync(Product product);

        // === Customers ===
        Task<List<Customer>> GetAllCustomersAsync();
        Task AddCustomerAsync(Customer customer);

        // === Orders ===
        Task<List<Order>> GetAllOrdersAsync();
        Task AddOrderAsync(Order order);
        Task EnqueueOrderMessageAsync(string message);

        // === Uploads (File Share) ===
        Task UploadContractAsync(string fileName, Stream stream);

        // === Uploads (Blob Storage) ===
        Task<string> UploadBlobAsync(string fileName, Stream stream);

        // === Ensure Resources Exist ===
        Task EnsureResourcesAsync();
    }
}
