using Microsoft.AspNetCore.Mvc;

namespace SV22T1020149.Admin.Controllers
{
    /// <summary>
    /// Controller quản lý các nghiệp vụ liên quan đến đơn hàng
    /// </summary>
    public class OrderController : Controller
    {
        /// <summary>
        /// Giao diện danh sách đơn hàng
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Tìm kiếm, lọc và phân trang đơn hàng
        /// </summary>
        public IActionResult Search()
        {
            return View();
        }

        /// <summary>
        /// Giao diện lập đơn hàng mới
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Hiển thị thông tin chi tiết của một đơn hàng
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        public IActionResult Detail(int id)
        {
            return View();
        }

        /// <summary>
        /// Cập nhật số lượng hoặc thông tin sản phẩm trong giỏ hàng (đơn hàng đang lập)
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <param name="productId">Mã sản phẩm</param>
        public IActionResult EditCartItem(int id, int productId)
        {
            return View();
        }

        /// <summary>
        /// Xóa một sản phẩm khỏi danh sách mặt hàng của đơn hàng
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <param name="productId">Mã sản phẩm</param>
        public IActionResult DeleteCartItem(int id, int productId)
        {
            return View();
        }

        /// <summary>
        /// Xóa toàn bộ sản phẩm đã chọn trong giỏ hàng
        /// </summary>
        public IActionResult ClearCart()
        {
            return View();
        }

        /// <summary>
        /// Chấp nhận/Duyệt đơn hàng (Chuyển từ trạng thái Chờ sang Đã xác nhận)
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        public IActionResult Accept(int id)
        {
            return View();
        }

        /// <summary>
        /// Chuyển đơn hàng sang trạng thái đang giao hàng và cập nhật đơn vị vận chuyển
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        public IActionResult Shipping(int id)
        {
            return View();
        }

        /// <summary>
        /// Xác nhận đơn hàng đã giao thành công và kết thúc quy trình
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        public IActionResult Finish(int id)
        {
            return View();
        }

        /// <summary>
        /// Từ chối đơn hàng (Trường hợp đơn hàng không hợp lệ ngay từ đầu)/// </summary>
        /// <param name="id">Mã đơn hàng</param>
        public IActionResult Reject(int id)
        {
            return View();
        }

        /// <summary>
        /// Hủy đơn hàng (Trường hợp khách yêu cầu hủy khi đơn đã được duyệt hoặc đang giao)
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        public IActionResult Cancel(int id)
        {
            return View();
        }

        /// <summary>
        /// Xóa vĩnh viễn đơn hàng khỏi hệ thống
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        public IActionResult Delete(int id)
        {
            return View();
        }
    }
}