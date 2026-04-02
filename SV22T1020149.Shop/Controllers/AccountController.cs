using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020149.BusinessLayers;
using SV22T1020149.Models.Partner;
using SV22T1020149.Shop.Models;

namespace SV22T1020149.Shop.Controllers
{
    public class AccountController : Controller
    {
        private async Task LoadProvincesAsync()
        {
            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
        }

        private static ClaimsPrincipal CreatePrincipal(Customer customer)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, customer.CustomerID.ToString()),
                new Claim(ClaimTypes.Email, customer.Email),
                new Claim(ClaimTypes.Name, customer.CustomerName),
                new Claim("CustomerId", customer.CustomerID.ToString())
            };
            var id = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(id);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var customer = await PartnerDataService.AuthenticateCustomerAsync(model.Email, model.Password);

            if (customer == null)
            {
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng, hoặc tài khoản chưa được kích hoạt.");
                return View(model);
            }

            var principal = CreatePrincipal(customer);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    AllowRefresh = true
                });

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register()
        {
            await LoadProvincesAsync();
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            await LoadProvincesAsync();

            if (!ModelState.IsValid)
                return View(model);

            if (!await PartnerDataService.ValidatelCustomerEmailAsync(model.Email, 0))
            {
                ModelState.AddModelError(nameof(model.Email), "Email này đã được sử dụng.");
                return View(model);
            }

            var customer = new Customer
            {
                CustomerID = 0,
                CustomerName = model.CustomerName.Trim(),
                ContactName = string.IsNullOrWhiteSpace(model.ContactName)
                    ? model.CustomerName.Trim()
                    : model.ContactName.Trim(),
                Email = model.Email.Trim(),
                Phone = model.Phone.Trim(),
                Province = model.Province.Trim(),
                Address = string.IsNullOrWhiteSpace(model.Address) ? "" : model.Address.Trim(),
                Password = model.Password,
                IsLocked = true
            };

            var newId = await PartnerDataService.AddCustomerAsync(customer);
            var created = await PartnerDataService.GetCustomerAsync(newId);
            if (created == null)
            {
                TempData["Message"] = "Đăng ký thành công. Vui lòng đăng nhập.";
                return RedirectToAction(nameof(Login));
            }

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                CreatePrincipal(created),
                new AuthenticationProperties { IsPersistent = false });

            TempData["Message"] = "Đăng ký thành công. Chào mừng bạn!";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
