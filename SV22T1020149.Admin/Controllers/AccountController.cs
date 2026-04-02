using Microsoft.AspNetCore.Mvc;

namespace SV22T1020149.Admin.Controllers
{
    public class AccountController : Controller
    {
        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        {
            return View();
        }
        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        /// <returns></returns>
        public IActionResult ChangePassword()
        {
            return View("ChangePassword");
        }

    }
}
