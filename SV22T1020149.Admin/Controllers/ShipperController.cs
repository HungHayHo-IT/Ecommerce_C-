using Microsoft.AspNetCore.Mvc;

namespace SV22T1020149.Admin.Controllers
{
    public class ShipperController : Controller
    {
        /// <summary>
        /// Trang giao diện tổng quan các chức năng của shipper
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Tạo mới bổ sung người giao hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung người giao hàng";

            return View("Edit");
        }

        /// <summary>
        /// Chỉnh sửa thông tin người giao hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa người giao hàng";
            return View();
        }

        /// <summary>
        /// Xóa người giao hàng ra khỏi hệ thống 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id)
        {
            return View();
        }
    }
}
