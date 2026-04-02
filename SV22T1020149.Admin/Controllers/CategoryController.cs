using Microsoft.AspNetCore.Mvc;
using SV22T1020149.BusinessLayers;
using SV22T1020149.Models.Common;

namespace SV22T1020149.Admin.Controllers
{
    public class CategoryController : Controller
    {
        private const int PAGESIZE = 10;
        /// <summary>
        /// Tên của biến dùng để lưu điều kiện tìm kiếm loại hàng trong session
        /// </summary>
        private const string CATEGORY_SEARCH = "CategorySearchInput";
        /// <summary>
        /// Nhập đầu vào tìm kiếm
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH);
            if (input == null)
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = ApplicationContext.PageSize,
                    SearchValue = ""
                };
            return View(input);
        }
        public async Task<IActionResult> Search(PaginationSearchInput input)
        {
            var result = await CatalogDataService.ListCategoriesAsync(input);
            ApplicationContext.SetSessionData(CATEGORY_SEARCH, input);
            return View(result);
        }
        /// <summary>
        /// Thêm mới 1 loại hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewBag.Title = "Thêm mới loại hàng";
            return View();
        }

        /// <summary>
        /// Cập nhật 1 loại hàng
        /// </summary>
        /// <param name="id">Mã nhà loại hàng cần cập nhật</param>
        /// <returns></returns>
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Cập nhật loại hàng";
            return View();
        }

        /// <summary>
        /// Xóa 1 loại hàng
        /// </summary>
        /// <param name="id">Mã loại hàng cần xóa</param>
        /// <returns></returns>
        public IActionResult Delete(int id)
        {
            return View();
        }
    }
}
