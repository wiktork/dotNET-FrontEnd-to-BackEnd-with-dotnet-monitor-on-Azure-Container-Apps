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

        public HomeController(IAccountBackendClient accountBackendClient, ILogger<HomeController> logger)
        {
            _accountBackendClient = accountBackendClient;
            _logger = logger;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}