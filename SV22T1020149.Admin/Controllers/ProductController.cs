using Microsoft.AspNetCore.Mvc;

namespace SV22T1020149.Admin.Controllers
{
    public class ProductController : Controller
    {
        /// <summary>
        /// trang hiển thị các sản phẩm và các chức năng quản lý sản phẩm
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Bổ sung khởi tạo sản phẩm mới
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung ";

            return View("Edit");
        }

        /// <summary>
        /// chỉnh sửa thông tin sản phẩm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public IActionResult Edit(int id)
        {
            
            return View();
        }

        /// <summary>
        /// xóa sản phẩm ra khỏi hệ thống
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id)
        {
            return View();
        }

        /// <summary>
        /// xem chi tiết thông tin sản phẩm, bao gồm các thuộc tính và ảnh liên quan
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Detail(int id)
        {
            
            return View();
        }


        /// <summary>
        /// xem danh sách các thuộc tính của sản phẩm, có thể bao gồm các chức năng thêm, sửa, xóa thuộc tính
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ListAttributes(int id)
        {
            return View();
        }


        /// <summary>
        /// thêm mới một thuộc tính cho sản phẩm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult CreateAttribute(int id)
        {
            ViewBag.Title = "Bổ sung thuộc tính";
            return View("EditAttribute");
        }

        /// <summary>
        /// chỉnh sửa thông tin 1 thuộc tính sản phẩm
        /// </summary>
        /// <param name="id"></param>
        /// <param name="attributeId"></param>
        /// <returns></returns>
        public IActionResult EditAttribute(int id, int attributeId)
        {
            ViewBag.Title = "Thay đổi thuộc tính";
            return View();
        }

        
        /// <summary>
        /// Xóa một thuộc tính sản phẩm
        /// </summary>
        /// <param name="id"></param>
        /// <param name="attributeId"></param>
        /// <returns></returns>
        public IActionResult DeleteAttribute(int id, int attributeId)
        {
            
            return View();
        }

        /// <summary>
        /// hiển thị danh sách các ảnh liên quan đến sản phẩm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ListPhoto(int id)
        {
            return View();
        }

        /// <summary>
        /// Thêm ảnh mới cho sản phẩm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult CreatePhoto(int id)
        {
            ViewBag.Title = "Bổ sung ảnh";
            return View("EditPhoto");
        }

        /// <summary>
        /// Chỉnh sửa thông tin ảnh
        /// </summary>
        /// <param name="id"></param>
        /// <param name="photoId"></param>
        /// <returns></returns>
        public IActionResult EditPhoto(int id, int photoId)
        {
            ViewBag.Title = "Thay đổi ảnh";
            return View();
        }

        /// <summary>
        /// Xóa ảnh
        /// </summary>
        /// <param name="id"></param>
        /// <param name="photoId"></param>
        /// <returns></returns>
        public IActionResult DeletePhoto(int id, int photoId)
        {
            return View();
        }
    }
}
