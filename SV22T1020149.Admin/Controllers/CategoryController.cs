using Microsoft.AspNetCore.Mvc;

namespace SV22T1020149.Admin.Controllers
{
    public class CategoryController : Controller
    {
        /// <summary>
        /// trang hiển thị các loại hàng và các chức năng quản lý loại hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Bổ sung khởi tạo loại hàng mới
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung Loại hàng";

            return View("Edit");
        }

        /// <summary>
        /// chỉnh sửa thông tin loại hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa Loại hàng";
            return View();
        }

        /// <summary>
        /// Xóa loại hàng ra khỏi hệ thống
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id)
        {
            return View();
        }
    }
}
