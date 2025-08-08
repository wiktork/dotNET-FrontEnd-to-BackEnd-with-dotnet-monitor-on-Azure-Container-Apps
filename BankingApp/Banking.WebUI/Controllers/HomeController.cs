using Banking.WebUI.Models;
using Banking.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Banking.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAccountBackendClient _accountBackendClient;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;

        public HomeController(IAccountBackendClient accountBackendClient, ILogger<HomeController> logger, IWebHostEnvironment environment)
        {
            _accountBackendClient = accountBackendClient;
            _logger = logger;
            _environment = environment;
        }

        public IActionResult SerializeAddress()
        {
            Address address1 = new Address("123 Main St", "Springfield", "IL", "12345");
            string serialized = address1.ToJson();
            Address address2 = Address.FromJson(serialized);

            return View("Index");
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Transfer(AccountTransfer accountTransfer)
        {
            HttpResponseMessage? resp = null;
            lock (accountTransfer)
            {
                resp = _accountBackendClient.AccountTransfer(accountTransfer).Result;
            }
            var result = await resp.Content.ReadAsStringAsync();
            _logger.LogInformation($"Status: [{resp.StatusCode}] {result}");

            return RedirectToAction("Index");
        }

        public IActionResult DownloadFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                _logger.LogWarning("DownloadFile called with empty or null filename");
                return BadRequest("Filename is required");
            }

            // Sanitize the filename to prevent directory traversal attacks
            fileName = Path.GetFileName(fileName);
            
            // Construct the file path within the Files directory
            var filePath = Path.Combine(_environment.ContentRootPath, fileName);

            // Verify the file exists and is within the allowed directory
            if (!System.IO.File.Exists(filePath))
            {
                _logger.LogWarning($"File not found or access denied: {fileName}");
                return NotFound($"File '{filePath}' not found");
            }

            try
            {
                // Open the file as a stream
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var contentType = GetContentType(fileName);
                _logger.LogInformation($"Downloading file: {fileName}, Size: {fileStream.Length} bytes");
                // Return the file stream for download
                return File(fileStream, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading file: {fileName}");
                return StatusCode(500, "Error occurred while downloading the file");
            }
        }

        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".txt" => "text/plain",
                ".zip" => "application/zip",
                _ => "application/octet-stream"
            };
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult EnvironmentVariables()
        {
            var envVars = Environment.GetEnvironmentVariables();
            var lines = new List<string>();
            foreach (var key in envVars.Keys)
            {
                lines.Add($"{key}={envVars[key]}");
            }
            var result = string.Join("\n", lines);
            return Content(result, "text/plain");
        }
    }
}