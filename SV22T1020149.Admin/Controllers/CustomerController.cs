using Microsoft.AspNetCore.Mvc;
using SV22T1020149.BusinessLayers;
using SV22T1020149.Models.Common;
using SV22T1020149.Models.Partner;
using System.Threading.Tasks;

namespace SV22T1020149.Admin.Controllers
{
    public class CustomerController : Controller
    {
        private const int PAGESIZE = 10;
        /// <summary>
        /// Tên của biến dùng để lưu điều kiện tìm kiếm khách hàng trong session
        /// </summary>
        private const string CUSTOMER_SEARCH = "CustomerSearchInput";
        /// <summary>
        /// Nhập đầu vào tìm kiếm
        /// </summary>
        /// <returns></returns>
        public  IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH);
            if(input == null)
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
            var result = await PartnerDataService.ListCustomersAsync (input);
            ApplicationContext.SetSessionData(CUSTOMER_SEARCH, input);
            return View(result);
        }

        /// <summary>
        /// Tạo mới 1 khách hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            
            ViewBag.Title = "Thêm mới khách hàng";
            var model = new Customer()
            {
                CustomerID = 0
            };
            return View("Edit",model);
        }

        /// <summary>
        /// Cập nhật 1 khách hàng
        /// </summary>
        /// <param name="id">Mã khách hàng cần cập nhật</param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật khách hàng";
            var model = await PartnerDataService.GetCustomerAsync(id);
            if(model == null)
                return RedirectToAction("Index");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveData(Customer data)
        {
            try
            {

                ViewBag.Title = data.CustomerID == 0 ? "Bổ sung khách hàng" : "Cập nhật thông tin khách hàng";

                // kiem tra du lieu co hop le hay khong]

                // cach lam: su dung ModelState de luu cac tinh huong loi va thong bao loi cho nguoi dung
                if (string.IsNullOrWhiteSpace(data.CustomerName))
                    ModelState.AddModelError("CustomerName", "Vui lòng nhập tên khách hàng");

                if (string.IsNullOrWhiteSpace(data.Email))
                    ModelState.AddModelError(nameof(data.Email), "Email không được để trống");
                else if (!await PartnerDataService.ValidatelCustomerEmailAsync(data.Email, data.CustomerID))
                    ModelState.AddModelError(nameof(data.Email), "Email đã được sử dụng");

                if (string.IsNullOrEmpty(data.Province))
                    ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh thành phố");

                if (!ModelState.IsValid)
                    return View("Edit", data);

                //Hiệu chỉnh dữ liệu theo quy định hệ thống
                if (string.IsNullOrWhiteSpace(data.ContactName)) data.ContactName = data.CustomerName;
                if (string.IsNullOrEmpty(data.Phone)) data.Phone = "";
                if (string.IsNullOrEmpty(data.Address)) data.Address = "";


                //lưu vào database
                if (data.CustomerID == 0)
                    await PartnerDataService.AddCustomerAsync(data);
                else
                    await PartnerDataService.UpdateCustomerAsync(data);
                return RedirectToAction("Index");
            }

            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Hệ thống hiện đang bận vui lòng thử lại sau vài phút ");
                return View("Edit", data);
            }
        }

         /// <summary>
         /// Xóa 1 khách hàng
                /// </summary>
                /// <param name="id">Mã khách hàng cần xóa</param>
                /// <returns></returns>
        public async Task<IActionResult> Delete(int id)
        {
            if(Request.Method == "POST")
            {
                await PartnerDataService.DeleteCustomerAsync(id);
                return RedirectToAction("Index"); 
            }

            var model = await PartnerDataService.GetCustomerAsync(id);
            if (model == null)
                return RedirectToAction("index");

            ViewBag.CanDelete = !await PartnerDataService.IsUsedCustomerAsync(id);

            return View(model);
            
                    
         }
         
        /// <summary>
        /// Đổi mật khẩu khách hàng
        /// </summary>
        /// <param name="id">Mã khách hàng cần đổi mật khẩu </param>
        /// <returns></returns>
        public IActionResult ChangePassword(int id)
        {
            return View();
        }
    }
}
