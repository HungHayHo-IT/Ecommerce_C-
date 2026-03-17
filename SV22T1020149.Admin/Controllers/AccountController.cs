using Microsoft.AspNetCore.Mvc;

namespace SV22T1020149.Admin.Controllers
{
    public class AccountController : Controller
    {   
         /// <summary>
         /// Đăng nhập vào hệ thống 
         /// </summary>
         /// <returns></returns>
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Đăng xuất ra khỏi hệ thống
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Đổi mật khẩu tài khoản
        /// </summary>
        /// <returns></returns>
        public IActionResult ChangePassword()
        {
            return View();
        }
    }
}
