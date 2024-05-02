using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMultiTenantContextAccessor<AppTenantInfo> _tenantContextAccessor;

        public HomeController(ILogger<HomeController> logger,
            IMultiTenantContextAccessor<AppTenantInfo> tenantContextAccessor)
        {
            _logger = logger;
            _tenantContextAccessor = tenantContextAccessor;
        }

        public IActionResult Index()
        {
            var model = new IndexModel();
            model.TenantInfo = _tenantContextAccessor.MultiTenantContext.TenantInfo;

            return View(model);
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
