using Microsoft.AspNetCore.Mvc;
using SV22T1020149.Models.Common;

namespace SV22T1020149.Admin.Controllers
{
    public class ShipperController : Controller
    {
        private const int PAGESIZE = 10;
        /// <summary>
        /// Tên của biến dùng để lưu điều kiện tìm kiếm người giao hàng trong session
        /// </summary>
        private const string SHIPPER_SEARCH = "ShipperSearchInput";
        /// <summary>
        /// Nhập đầu vào tìm kiếm
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH);
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
            var result = await PartnerDataService.ListShippersAsync(input);
            ApplicationContext.SetSessionData(SHIPPER_SEARCH, input);
            return View(result);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Thêm mới đơn vị vận chuyển";
            return View("Edit");
        }

        /// <summary>
        /// Cập nhật 1 đơn vị vận chuyển
        /// </summary>
        /// <param name="id">Mã đơn vị vận chuyển cần cập nhật</param>
        /// <returns></returns>
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Cập nhật đơn vị vận chuyển";
            return View();
        }

        /// <summary>
        /// Xóa 1 đơn vị vận chuyển
        /// </summary>
        /// <param name="id">Mã đơn vị vận chuyển cần xóa</param>
        /// <returns></returns>
        public IActionResult Delete(int id)
        {
            return View();
        }
    }
}
