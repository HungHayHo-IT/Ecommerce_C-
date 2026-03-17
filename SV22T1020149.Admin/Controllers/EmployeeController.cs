using Microsoft.AspNetCore.Mvc;

namespace SV22T1020149.Admin.Controllers
{
    public class EmployeeController : Controller
    {
        /// <summary>
        /// trang tổng quan của nhân viên
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// bổ sung khởi tạo nhân viên mới
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên";

            return View("Edit");
        }

        /// <summary>
        /// chỉnh sửa thông tin nhân viên
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa nhân viên";
            return View();
        }

        /// <summary>
        /// xóa nhân viên ra khỏi hệ thống
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public IActionResult Delete(int id)
        {
            return View();
        }

        /// <summary>
        /// Đổi mật khẩu tài khoản nhân viên
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ChangePassword(int id)
        {
            return View();
        }

        /// <summary>
        /// Cấp quyền cho nhân viên
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ChangeRole(int id)
        {
            return View();
        }
    
    }
}
