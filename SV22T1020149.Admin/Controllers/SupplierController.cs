using Microsoft.AspNetCore.Mvc;

namespace SV22T1020149.Admin.Controllers
{
    public class SupplierController : Controller
    {
        /// <summary>
        /// Trang tổng quan của nhà cung cấp
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Tạo mới bổ sung nhà cung cấp
        /// </summary>
        /// <returns></returns>
        public IActionResult Create() {
            ViewBag.Title = "Bổ sung nhà cung cấp";

            return View("Edit");
        }

        /// <summary>
        /// Chỉnh sửa thông tin nhà cung cấp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Edit(int id) {
            ViewBag.Title = "Chỉnh sửa sung nhà cung cấp";
            return View();
        }

        /// <summary>
        /// Xóa nhà cung cấp ra khỏi hệ thống 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public IActionResult Delete(int id)
        {
            return View();
        }

    }
}
