using System;
using System.Threading.Tasks;
using ABCRetails.Models;
using ABCRetails.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetails.Controllers
{
    public class ProductController : Controller
    {
        private readonly IAzureStorageService _storage;

        public ProductController(IAzureStorageService storage)
        {
            _storage = storage;
        }

        // GET: /Product
        public async Task<IActionResult> Index()
        {
            var products = await _storage.GetProductsAsync();
            return View(products);
        }

        // GET: /Product/Create
        public IActionResult Create()
        {
            var product = new Product
            {
                PartitionKey = "PRODUCT",
                RowKey = Guid.NewGuid().ToString()
            };
            return View(product);
        }

        // POST: /Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
        {
            if (string.IsNullOrWhiteSpace(product.PartitionKey))
                product.PartitionKey = "PRODUCT";

            if (string.IsNullOrWhiteSpace(product.RowKey))
                product.RowKey = Guid.NewGuid().ToString();

            if (!ModelState.IsValid)
                return View(product);

            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    using var stream = imageFile.OpenReadStream();
                    product.ImageUrl = await _storage.UploadBlobAsync(imageFile.FileName, stream);
                }

                await _storage.AddProductAsync(product);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error saving product: " + ex.Message);
                return View(product);
            }
        }
    }
}
