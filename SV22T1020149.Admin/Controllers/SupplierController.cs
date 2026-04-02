using Microsoft.AspNetCore.Mvc;
using SV22T1020149.Models.Common;

namespace SV22T1020149.Admin.Controllers
{
    public class SupplierController : Controller
    {
        private const int PAGESIZE = 10;
        /// <summary>
        /// Tên của biến dùng để lưu điều kiện tìm kiếm nhà cung cấp trong session
        /// </summary>
        private const string SUPPLIER_SEARCH = "SupplierSearchInput";
        /// <summary>
        /// Nhập đầu vào tìm kiếm
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(SUPPLIER_SEARCH);
            if (input == null)
                input = new PaginationSearchInput()
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
        /// <returns></returns>
        public async Task<IActionResult> Search(PaginationSearchInput input)
        {
            var result = await PartnerDataService.ListSuppliersAsync(input);
            ApplicationContext.SetSessionData(SUPPLIER_SEARCH, input);
            return View(result);
        }
        /// <summary>
        /// Tạo mới nhà cung cấp
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhà cung cấp";
            return View("Edit");
        }

        /// <summary>
        /// Sửa nhà cung cấp
        /// </summary>
        /// <param name="id">Mã nhà cung cấp cần sửa</param>
        /// <returns></returns>
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Cập nhật nhà cung cấp";
            return View();
        }

        /// <summary>
        /// Xóa nhà cung cấp
        /// </summary>
        /// <param name="id">Mã nhà cung cấp cần xóa</param>
        /// <returns></returns>
        public IActionResult Delete(int id)
        {
            return View();
        }
    }
}
