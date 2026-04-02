using Microsoft.AspNetCore.Mvc;
using SV22T1020149.BusinessLayers;
using SV22T1020149.Models.Catalog;
using SV22T1020149.Models.Common;

namespace SV22T1020149.Admin.Controllers
{
    /// <summary>
    /// Controller quản lý các hoạt động liên quan đến mặt hàng (sản phẩm)
    /// </summary>
    public class ProductController : Controller
    {
        private const int PAGESIZE = 10;
        /// <summary>
        /// Tên của biến dùng để lưu điều kiện tìm kiếm mặt hàng trong session
        /// </summary>
        private const string PRODUCT_SEARCH = "ProductSearchInput";
        /// <summary>
        /// Nhập đầu vào tìm kiếm
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if (input == null)
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = ApplicationContext.PageSize,
                    SearchValue = ""
                };
            return View(input);
        }
        /// <summary>
        /// Tìm kiếm và trả về kết quả
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IActionResult> Search(ProductSearchInput input)
        {
            var result = await CatalogDataService.ListProductsAsync(input);
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);
            return View(result);
        }

        /// <summary>
        /// Giao diện thêm mới mặt hàng
        /// </summary>
        public IActionResult Create()
        {
            ViewBag.Title = "Thêm mới mặt hàng";
            return View("Edit");
        }

        /// <summary>
        /// Giao diện cập nhật thông tin mặt hàng
        /// </summary>
        /// <param name="id">Mã mặt hàng</param>
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Cập nhật mặt hàng";
            return View();
        }

        /// <summary>
        /// Xác nhận xóa mặt hàng
        /// </summary>
        /// <param name="id">Mã mặt hàng</param>
        public IActionResult Delete(int id)
        {
            return View();
        }

        // --- QUẢN LÝ THUỘC TÍNH (ATTRIBUTES) ---

        /// <summary>
        /// Hiển thị danh sách thuộc tính của một mặt hàng
        /// </summary>
        /// <param name="id">Mã mặt hàng</param>
        public IActionResult ListAttributes(int id)
        {
            return View();
        }

        /// <summary>
        /// Giao diện thêm mới thuộc tính cho mặt hàng
        /// </summary>
        /// <param name="id">Mã mặt hàng (Sở hữu thuộc tính)</param>
        public IActionResult CreateAttribute(int id)
        {
            return View();
        }

        /// <summary>
        /// Giao diện cập nhật thuộc tính của mặt hàng
        /// </summary>
        /// <param name="id">Mã mặt hàng</param>
        /// <param name="attributeId">Mã thuộc tính cụ thể</param>
        public IActionResult EditAttribute(int id, int attributeId)
        {
            return View();
        }

        /// <summary>
        /// Xóa thuộc tính của mặt hàng
        /// </summary>
        /// <param name="id">Mã mặt hàng</param>
        /// <param name="attributeId">Mã thuộc tính cụ thể</param>
        public IActionResult DeleteAttribute(int id, int attributeId)
        {
            return View();
        }

        // --- QUẢN LÝ ẢNH (PHOTOS) ---

        /// <summary>
        /// Hiển thị danh sách ảnh của mặt hàng
        /// </summary>
        /// <param name="id">Mã mặt hàng</param>
        public IActionResult ListPhotos(int id)
        {
            return View();
        }

        /// <summary>
        /// Giao diện thêm mới ảnh cho mặt hàng
        /// </summary>
        /// <param name="id">Mã mặt hàng (Sở hữu ảnh)</param>
        public IActionResult CreatePhoto(int id)
        {
            return View();
        }

        /// <summary>
        /// Giao diện cập nhật ảnh của mặt hàng
        /// </summary>
        /// <param name="id">Mã mặt hàng</param>
        /// <param name="photoId">Mã ảnh cụ thể</param>
        public IActionResult EditPhoto(int id, int photoId)
        {
            return View();
        }

        /// <summary>
        /// Xóa ảnh của mặt hàng
        /// </summary>
        /// <param name="id">Mã mặt hàng</param>
        /// <param name="photoId">Mã ảnh cụ thể</param>
        public IActionResult DeletePhoto(int id, int photoId)
        {
            return View();
        }
    }
}