using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SV22T1020149.BusinessLayers;
using SV22T1020149.Models.Partner;
using System.Security.Claims;
using SV22T1020149.Shop.Models;
using SV22T1020149.Models.Security;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SV22T1020149.Shop.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Register()
        {
            // Keep existing HTML-only page; it posts to same URL via JS/fetch or form POST
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterPost()
        {
            // Read form values directly
            var form = Request.Form;
            var model = new RegisterViewModel
            {
                CustomerName = form["CustomerName"],
                Province = form["Province"],
                Address = form["Address"],
                Phone = form["Phone"],
                Email = form["Email"],
                Password = form["Password"],
                ConfirmPassword = form["ConfirmPassword"]
            };

            // basic server-side validation
            if (string.IsNullOrWhiteSpace(model.CustomerName))
                return BadRequest("Vui lòng nh?p h? tên");

            if (string.IsNullOrWhiteSpace(model.Email))
                return BadRequest("Vui lòng nh?p email");

            if (model.Password != model.ConfirmPassword)
                return BadRequest("M?t kh?u xác nh?n không kh?p");

            if (model.Password.Length < 6)
                return BadRequest("M?t kh?u ph?i có ít nh?t 6 ký t?");

            // check duplicate email and phone
            if (!await PartnerDataService.ValidatelCustomerEmailAsync(model.Email))
                return BadRequest("Email ?ã ???c s? d?ng");

            if (!await PartnerDataService.ValidatelCustomerPhoneAsync(model.Phone))
                return BadRequest("S? ?i?n tho?i ?ã ???c s? d?ng");

            var customer = new Customer
            {
                CustomerName = model.CustomerName,
                ContactName = model.CustomerName,
                Province = model.Province,
                Address = model.Address,
                Phone = model.Phone,
                Email = model.Email,
                IsLocked = true,
                Password = PasswordHelper.HashPassword(model.Password)
            };

            try
            {
                var id = await PartnerDataService.AddCustomerAsync(customer);
                if (id > 0)
                {
                    // if request is AJAX, return JSON; otherwise redirect
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = true, id });

                    return RedirectToAction("Login");
                }
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                // return DB error message to client for easier debugging
                return BadRequest("Database error: " + ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return BadRequest("??ng ký không thành công");
        }

        public IActionResult Login(string returnUrl = "")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string returnUrl = "")
        {
            // allow login via email or phone
            var user = await PartnerDataService.AuthenticateCustomerAsync(email, password);
            if (user == null)
            {
                ModelState.AddModelError("", "Email ho?c m?t kh?u không ?úng");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.CustomerID.ToString()),
                new Claim(ClaimTypes.Name, user.CustomerName ?? user.Email),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            // Kiểm tra đăng nhập
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login");

            // 1. Lấy ID chuẩn của .NET (Không cần GetUserData)
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // 2. Lấy thông tin khách hàng từ DB
            var customer = await PartnerDataService.GetCustomerAsync(id);
            if (customer == null)
                return RedirectToAction("Login");

            // 3. Đưa dữ liệu sang ViewModel để hiển thị lên View
            var model = new UserProfileViewModel()
            {
                CustomerID = customer.CustomerID,
                CustomerName = customer.CustomerName ?? "",
                Email = customer.Email ?? "",
                Phone = customer.Phone ?? "",
                Address = customer.Address ?? "",
                Province = customer.Province ?? ""
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 1. Lấy ID chuẩn
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Lấy lại thông tin cũ để cập nhật
            var currentCustomer = await PartnerDataService.GetCustomerAsync(id);
            if (currentCustomer == null) return RedirectToAction("Login");

            // 2. Xử lý đổi mật khẩu (Nếu người dùng có nhập Mật khẩu mới)
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                if (string.IsNullOrEmpty(model.OldPassword) || string.IsNullOrEmpty(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Vui lòng nhập đầy đủ mật khẩu cũ và xác nhận mật khẩu mới.");
                    return View(model);
                }

                // --- SỬA Ở ĐÂY: Dùng chính hàm Đăng nhập để verify mật khẩu cũ ---
                var verifyOldPassword = await PartnerDataService.AuthenticateCustomerAsync(currentCustomer.Email, model.OldPassword);

                if (verifyOldPassword == null) // Bằng null nghĩa là mật khẩu cũ sai
                {
                    ModelState.AddModelError("OldPassword", "Mật khẩu hiện tại không chính xác.");
                    return View(model);
                }

                // Đổi mật khẩu mới trong đối tượng
                currentCustomer.Password = SV22T1020149.Models.Security.PasswordHelper.HashPassword(model.NewPassword);
            }

            // 3. Cập nhật các thông tin cá nhân khác
            currentCustomer.CustomerName = model.CustomerName;
            currentCustomer.Phone = model.Phone;
            currentCustomer.Address = model.Address;
            currentCustomer.Province = model.Province;

            // 4. Lưu xuống Database
            var ok = await PartnerDataService.UpdateCustomerAsync(currentCustomer);
            if (ok)
            {
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Profile");
            }
            else
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật thông tin.");
                return View(model);
            }
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
    }
}