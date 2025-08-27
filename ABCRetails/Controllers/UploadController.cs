using Microsoft.AspNetCore.Mvc;
using ABCRetails.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ABCRetails.Controllers
{
    public class UploadController : Controller
    {
        private readonly IAzureStorageService _storage;

        public UploadController(IAzureStorageService storage)
        {
            _storage = storage;
        }

        // Show upload page
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> UploadContract(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // ✅ Correct order: filename first, then stream
                using var stream = file.OpenReadStream();
                await _storage.UploadContractAsync(file.FileName, stream);

                ViewBag.Message = "File uploaded!";
            }
            else
            {
                ViewBag.Message = "Please select a file.";
            }

            return View("Index");
        }
    }
}
