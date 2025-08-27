using Microsoft.AspNetCore.Mvc;
using ABCRetails.Models;
using ABCRetails.Services;
using System.Threading.Tasks;

namespace ABCRetails.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IAzureStorageService _storage;

        public CustomerController(IAzureStorageService storage)
        {
            _storage = storage;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _storage.GetAllCustomersAsync();
            return View(customers);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            customer.RowKey = Guid.NewGuid().ToString();
            await _storage.AddCustomerAsync(customer);
            return RedirectToAction("Index");
        }
    }
}
