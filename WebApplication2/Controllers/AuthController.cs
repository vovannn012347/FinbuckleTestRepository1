using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Security.Claims;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMultiTenantContextAccessor<AppTenantInfo> _tenantContextAccessor;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthController(
            ILogger<HomeController> logger,
            IMultiTenantContextAccessor<AppTenantInfo> tenantContextAccessor,
            ApplicationDbContext context,
            SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _tenantContextAccessor = tenantContextAccessor;
            _context = context;
            _signInManager = signInManager;
        }

        /*
        public IActionResult Index(LoginViewModel model)
        {
            if (model == null)
            {
                model = new LoginViewModel();
            }

            var tenantInfo = _tenantContextAccessor.MultiTenantContext.TenantInfo;
            model.TenantInfo = tenantInfo;
            if (tenantInfo != null && !string.IsNullOrEmpty(tenantInfo.Id))
            {
                model.TenantId = tenantInfo.Id;
                
                var count = _context.Users.Count();
                IdentityUser user = null;
                if (count > 0)
                {
                    user = _context.Users.FirstOrDefault();
                }

                if (count == 0 || user == null)
                {
                    if(tenantInfo.Identifier == "tenant-2")
                    {
                        user = new Microsoft.AspNetCore.Identity.IdentityUser
                        {
                            Id = Guid.NewGuid().ToString(),

                            UserName = "TestUser2",
                            PasswordHash = "userPassword2",
                            Email = "vovann012347@gmail.com1",
                            EmailConfirmed = true
                        };
                    }
                    else
                    {
                        user = new Microsoft.AspNetCore.Identity.IdentityUser
                        {
                            Id = Guid.NewGuid().ToString(),

                            UserName = "TestUser",
                            PasswordHash = "userPassword",
                            Email = "vovann012347@gmail.com",
                            EmailConfirmed = true
                        };
                    }
                    _context.AspNetUsers.Add(user);
                    _context.SaveChanges();
                }
            }


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(string.IsNullOrEmpty(model.Login) || string.IsNullOrEmpty(model.Password))
            {
                return RedirectToAction("Index", "Auth");
            }

            var tenantInfo = _tenantContextAccessor.MultiTenantContext.TenantInfo;
            var user = _context.Users.FirstOrDefault(u => u.UserName == model.Login);
            if (user != null && user.PasswordHash == model.Password)
            {
                await _signInManager.SignInWithClaimsAsync(
                    user,
                    isPersistent: true,
                    new List<Claim> { new Claim(Constants.TenantClaimType, model.TenantId) }
                    );

                return RedirectToAction("Index", "Home");
            }

            return View("Index", model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
        */
    }
}
