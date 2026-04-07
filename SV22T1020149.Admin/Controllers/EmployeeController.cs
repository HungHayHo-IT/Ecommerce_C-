using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020149.BusinessLayers;
using SV22T1020149.Models.Common;
using SV22T1020149.Models.HR;

namespace SV22T1020149.Admin.Controllers
{

    [Authorize(Roles = WebUserRoles.Administrator)]
    public class EmployeeController : Controller
    {
        private const int PAGESIZE = 10;
        /// <summary>
        /// Tên của biến dùng để lưu điều kiện tìm kiếm nhân viên trong session
        /// </summary>
        private const string EMPLOYEE_SEARCH = "EmployeeSearchInput";
        /// <summary>
        /// Nhập đầu vào tìm kiếm
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH);
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
            var result = await HRDataService.ListEmployeesAsync(input);
            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH, input);
            return View(result);
        }

        /// <summary>
        /// Thêm mới 1 nhân viên
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên";
            var model = new Employee()
            {
                EmployeeID = 0,
                IsWorking = true
            };
            return View("Edit", model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            var model = await HRDataService.GetEmployeeAsync(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveData(Employee data, IFormFile? uploadPhoto)
        {
            try
            {
                ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật thông tin nhân viên";

                //Kiểm tra dữ liệu đầu vào: FullName và Email là bắt buộc, Email chưa được sử dụng bởi nhân viên khác
                if (string.IsNullOrWhiteSpace(data.FullName))
                    ModelState.AddModelError(nameof(data.FullName), "Vui lòng nhập họ tên nhân viên");

                if (string.IsNullOrWhiteSpace(data.Email))
                    ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email nhân viên");
                else if (!await HRDataService.ValidateEmployeeEmailAsync(data.Email, data.EmployeeID))
                    ModelState.AddModelError(nameof(data.Email), "Email đã được sử dụng bởi nhân viên khác");

                if (!ModelState.IsValid)
                    return View("Edit", data);

                //Xử lý upload ảnh
                if (uploadPhoto != null)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(uploadPhoto.FileName)}";
                    var filePath = Path.Combine(ApplicationContext.WWWRootPath, "images/employees", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadPhoto.CopyToAsync(stream);
                    }
                    data.Photo = fileName;
                }

                //Tiền xử lý dữ liệu trước khi lưu vào database
                if (string.IsNullOrEmpty(data.Address)) data.Address = "";
                if (string.IsNullOrEmpty(data.Phone)) data.Phone = "";
                if (string.IsNullOrEmpty(data.Photo)) data.Photo = "nophoto.png";

                //Lưu dữ liệu vào database (bổ sung hoặc cập nhật)
                if (data.EmployeeID == 0)
                {
                    await HRDataService.AddEmployeeAsync(data);
                }
                else
                {
                    await HRDataService.UpdateEmployeeAsync(data);
                }
                return RedirectToAction("Index");
            }
            catch //(Exception ex)
            {
                //TODO: Ghi log lỗi căn cứ vào ex.Message và ex.StackTrace
                ModelState.AddModelError(string.Empty, "Hệ thống đang bận hoặc dữ liệu không hợp lệ. Vui lòng kiểm tra dữ liệu hoặc thử lại sau");
                return View("Edit", data);
            }
        }

        /// <summary>
        /// Xóa 1 nhân viên
        /// </summary>
        /// <param name="id">Mã nhân viên cần xóa</param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int id)
        {
            // Trường hợp thực hiện xóa (POST)
            if (HttpMethods.IsPost(Request.Method))
            {
                // Kiểm tra điều kiện xóa (giả sử dùng IsUsedEmployeeAsync hoặc kết quả từ hàm Delete)
                if (await HRDataService.IsUsedEmployeeAsync(id))
                {
                    ModelState.AddModelError(string.Empty, "Không thể xóa nhân viên này vì đã có dữ liệu liên quan (lập đơn hàng).");
                    var modelErr = await HRDataService.GetEmployeeAsync(id);
                    ViewBag.CanDelete = false;
                    return View(modelErr);
                }

                await HRDataService.DeleteEmployeeAsync(id);
                return RedirectToAction("Index");
            }

            // Trường hợp hiển thị xác nhận (GET)
            var model = await HRDataService.GetEmployeeAsync(id);
            if (model == null) return RedirectToAction("Index");

            // Kiểm tra xem nhân viên này có xóa được không để báo cho View
            ViewBag.CanDelete = !await HRDataService.IsUsedEmployeeAsync(id);

            return View(model);
        }
        /// <summary>
        /// Đổi mật khẩu 1 nhân viên
        /// </summary>
        /// <param name="id">Mã nhân viên cần đổi mật khẩu</param>
        /// <returns></returns>
        public IActionResult ChangePassword(int id)
        {
            return View();
        }

        /// <summary>
        /// Phân quyền cho nhân viên
        /// </summary>
        /// <param name="id">Mã nhân viên cần phân quyền</param>
        /// <returns></returns>
        public IActionResult ChangeRole(int id)
        {
            return View();
        }
    }
}
