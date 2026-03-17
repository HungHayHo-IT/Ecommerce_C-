using Microsoft.AspNetCore.Mvc;
using SV22T1020149.BussinessLayers;
using SV22T1020149.Models.Common;
using System.Threading.Tasks;

namespace SV22T1020149.Admin.Controllers
{
    public class CustomerController : Controller
    {

        private const int PAGESIZE = 10;

        /// <summary>
        /// Trang tổng quan các chức năng khách hàng
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index(int page = 1 , string searchValue = "")
        {
            var input = new PaginationSearchInput()
            {
                Page = page,
                PageSize = PAGESIZE,
                SearchValue = searchValue
            };

            var result = await PartnerDataService.ListCustomerAsync(input);
            ViewBag.SearchValue = searchValue;
            return View(result);
        }

        /// <summary>
        /// Tạo mới bổ sung khách hàng 
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung Khách hàng";

            return View("Edit");
        }

        /// <summary>
        /// Chỉnh sửa thông tin khách hàng 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa khách hàng";
            return View();
        }


        /// <summary>
        /// Xóa khách hàng ra 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id)
        {
            return View();
        }

        /// <summary>
        /// Đổi mật khẩu tài khoản khách hàng 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ChangePassword(int id)
        {
            return View();
        }

    }
}
