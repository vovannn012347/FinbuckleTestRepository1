using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Security.Claims;
using WebApplication2.Code;
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
        private readonly UserManager<IdentityUser> _userMgr;
        private readonly KeyManager _keyManager;
        private readonly IUserClaimsPrincipalFactory<IdentityUser> _userClaimsFactory;

        public AuthController(
            ILogger<HomeController> logger,
            IMultiTenantContextAccessor<AppTenantInfo> tenantContextAccessor,
            ApplicationDbContext context,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userMgr,
            KeyManager keyManager,
            IUserClaimsPrincipalFactory<IdentityUser> userClaimsFactory)
        {
            _logger = logger;
            _tenantContextAccessor = tenantContextAccessor;
            _context = context;
            _signInManager = signInManager;
            _userMgr = userMgr;
            _keyManager = keyManager;
            _userClaimsFactory = userClaimsFactory;

        }

        public async Task<IActionResult> Index()
        {
            await CreateTestUser();

            if (_tenantContextAccessor.MultiTenantContext.IsResolved)
            {
                return View(new IndexModel
                {
                    TenantInfo = _tenantContextAccessor.MultiTenantContext.TenantInfo
                });
            }
            else
            {
                return View(new IndexModel());
            }
        }

        public async Task CreateTestUser()
        {
            if (_tenantContextAccessor.MultiTenantContext.IsResolved)
            {
                var tenantInfo = _tenantContextAccessor.MultiTenantContext.TenantInfo;

                var user = _context.Users.FirstOrDefault(u => u.UserName == "TestUser" + tenantInfo.Identifier);
                if(user == null)
                {
                    user = new Microsoft.AspNetCore.Identity.IdentityUser
                    {
                        Id = Guid.NewGuid().ToString(),

                        UserName = "TestUser" + tenantInfo.Identifier,
                        Email = "vovann012347@gmail.com",
                        EmailConfirmed = true
                    };
                    await _userMgr.CreateAsync(user, password: "userPassword" + tenantInfo.Identifier);
                    await _userMgr.AddClaimAsync(user, new Claim("role", "tester"));
                }
            }
        }

        [Authorize]
        public IActionResult UserClaims()
        {
            var user = HttpContext.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                return Ok(user.Claims.Select(x => KeyValuePair.Create(x.Type, x.Value)));
            }
            return Ok();

        }

        [Authorize]
        [Authorize(Policy = "any_policy")]
        public IActionResult AnySecret()
        {
            return Ok("any_secret");
        }

        [Authorize]
        [Authorize(Policy = "cookie_policy")]
        public IActionResult CookieSecret()
        {
            return Ok("cookie_secret");
        }

        [Authorize]
        [Authorize(Policy = "token_policy")]
        public IActionResult TokenSecret()
        {
            return Ok("token_secret");
        }

        public async Task<IActionResult> CookieSignin()
        {
            await CreateTestUser();
            if (_tenantContextAccessor.MultiTenantContext.IsResolved)
            {
                var tenantInfo = _tenantContextAccessor.MultiTenantContext.TenantInfo;
                var result = await _signInManager.PasswordSignInAsync("TestUser" + tenantInfo.Identifier, "userPassword" + tenantInfo.Identifier, false, false);

                return RedirectToAction("Index", "Home");
            }

            return new BadRequestResult();
        }

        public async Task<IActionResult> TokenSignin()
        {
            await CreateTestUser();
            if (_tenantContextAccessor.MultiTenantContext.IsResolved)
            {
                var tenantInfo = _tenantContextAccessor.MultiTenantContext.TenantInfo;

                var user = await _userMgr.FindByNameAsync("TestUser" + tenantInfo.Identifier);
                if(user != null)
                {
                    var check = await _signInManager.CheckPasswordSignInAsync(user, "userPassword" + tenantInfo.Identifier, false);
                    if (check.Succeeded)
                    {
                        var principal = await _userClaimsFactory.CreateAsync(user);
                        var identity = principal.Identities.FirstOrDefault();
                        identity.AddClaim(new Claim("amr", "pwd"));
                        identity.AddClaim(new Claim("tenant", _tenantContextAccessor.MultiTenantContext.TenantInfo.Identifier));


                        var handler = new JsonWebTokenHandler();
                        var key = new RsaSecurityKey(_keyManager.RsaKey);

                        var token = handler.CreateToken(new SecurityTokenDescriptor
                        {
                            Issuer = "https://localhost:7194/",
                            Subject = identity,
                            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
                        });

                        return Ok(token);
                    }
                }
            }

            return new BadRequestResult();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
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
