using Microsoft.AspNetCore.Mvc;
using ABCRetails.Models;
using ABCRetails.Services;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ABCRetails.Controllers
{
    public class OrderController : Controller
    {
        private readonly IAzureStorageService _storage;

        public OrderController(IAzureStorageService storage)
        {
            _storage = storage;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _storage.GetAllOrdersAsync();
            return View(orders);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        {
            order.RowKey = Guid.NewGuid().ToString();
            order.OrderDate = DateTime.UtcNow;
            order.Status = "Pending";
            order.TotalPrice = order.UnitPrice * order.Quantity;

            await _storage.AddOrderAsync(order);

            var message = JsonConvert.SerializeObject(new { OrderId = order.RowKey, Action = "ProcessOrder" });
            await _storage.EnqueueOrderMessageAsync(message);

            return RedirectToAction("Index");
        }
    }
}
