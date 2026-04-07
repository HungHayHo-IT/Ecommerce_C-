using Microsoft.AspNetCore.Mvc;
using SV22T1020149.BusinessLayers;
using SV22T1020149.Models.Sales;
using SV22T1020149.Shop.AppCodes;

namespace SV22T1020149.Shop.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Cart()
        {
            var cart = ShoppingCartService.GetShoppingCart();
            return View(cart);
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            // Lấy giỏ hàng hiện tại
            var cart = ShoppingCartService.GetShoppingCart();

            // Nếu giỏ hàng trống thì bắt quay lại trang giỏ hàng
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Cart");
            }

            // Truyền giỏ hàng sang giao diện Thanh toán
            return View(cart);
        }

        public IActionResult CartCount()
        {
            var cart = ShoppingCartService.GetShoppingCart();
            int count = cart.Sum(i => i.Quantity);
            return Json(new { count });
        }

        [HttpPost]
        public async Task<IActionResult> AddCartItem(int productId, int quantity, decimal price)
        {
            if (quantity <= 0) return Json(new { code = 0, message = "S? l??ng không h?p l?" });
            if (price < 0) return Json(new { code = 0, message = "Giá không h?p l?" });

            var product = await CatalogDataService.GetProductAsync(productId);
            if (product == null) return Json(new { code = 0, message = "S?n ph?m không t?n t?i" });

            if (!product.IsSelling) return Json(new { code = 0, message = "S?n ph?m không còn bán" });

            var item = new OrderDetailViewInfo
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                Quantity = quantity,
                SalePrice = price,
                Unit = product.Unit,
                Photo = product.Photo ?? "nophoto.png"
            };
            ShoppingCartService.AddCartItem(item);
            return Json(new { code = 1 });
        }

        [HttpPost]
        public IActionResult UpdateCartItem(int productId, int quantity, decimal salePrice)
        {
            ShoppingCartService.UpdateCartItem(productId, quantity, salePrice);
            return Json(new { code = 1 });
        }

        [HttpPost]
        public IActionResult RemoveCartItem(int productId)
        {
            ShoppingCartService.RemoveCartItem(productId);
            return Json(new { code = 1 });
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(string province, string address)
        {
            // 1. Kiểm tra khách hàng đã đăng nhập chưa
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Json(new { code = 0, message = "Vui lòng đăng nhập để đặt hàng" });
            }

            // 2. Lấy CustomerID từ User Identity đang đăng nhập
            var claimValue = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int customerID = 0;
            int.TryParse(claimValue, out customerID);

            if (customerID == 0)
            {
                return Json(new { code = 0, message = "Không xác định được thông tin tài khoản." });
            }

            // 3. Kiểm tra giỏ hàng và dữ liệu đầu vào
            var cart = ShoppingCartService.GetShoppingCart();
            if (cart.Count == 0) return Json(new { code = 0, message = "Giỏ hàng rỗng" });
            if (string.IsNullOrWhiteSpace(province)) return Json(new { code = 0, message = "Vui lòng chọn tỉnh" });
            if (string.IsNullOrWhiteSpace(address)) return Json(new { code = 0, message = "Vui lòng nhập địa chỉ" });

            // 4. Tạo đơn hàng và gán CustomerID
            var order = new Order
            {
                CustomerID = customerID, // Gắn ID của tài khoản đang đăng nhập vào đây
                DeliveryProvince = province,
                DeliveryAddress = address
            };

            int orderId = await SalesDataService.AddOrderAsync(order);

            // 5. Lưu chi tiết đơn hàng
            foreach (var it in cart)
            {
                await SalesDataService.AddDetailAsync(new OrderDetail
                {
                    OrderID = orderId,
                    ProductID = it.ProductID,
                    Quantity = it.Quantity,
                    SalePrice = it.SalePrice
                });
            }

            ShoppingCartService.ClearCart();
            return Json(new { code = orderId });
        }

        public async Task<IActionResult> History()
        {
            if (!User.Identity?.IsAuthenticated ?? true) return RedirectToAction("Login", "Account");
            var customerID = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var input = new SV22T1020149.Models.Sales.OrderSearchInput { Page = 1, PageSize = 20, SearchValue = "" };
            input.Status = default;
            var result = await SalesDataService.ListOrdersAsync(input);
            // filter by customer
            result.DataItems = result.DataItems.Where(o => o.CustomerID == customerID).ToList();
            return View(result);
        }

        public async Task<IActionResult> Detail(int id)
        {
            // Lấy thông tin đơn hàng
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return RedirectToAction("History");

            // Lấy danh sách chi tiết các mặt hàng trong đơn
            var details = await SalesDataService.ListDetailsAsync(id);
            ViewBag.Details = details;

            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> AddToCart(int id)
        {
            var product = await CatalogDataService.GetProductAsync(id);
            if (product == null) return RedirectToAction("List", "Product");
            if (!product.IsSelling) return RedirectToAction("List", "Product");

            var item = new OrderDetailViewInfo
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                Quantity = 1,
                SalePrice = product.Price,
                Unit = product.Unit,
                Photo = product.Photo ?? "nophoto.png"
            };
            ShoppingCartService.AddCartItem(item);

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer)) return Redirect(referer);
            return RedirectToAction("Cart");
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return Json(new { code = 0, message = "Đơn hàng không tồn tại." });

            // Chỉ cho phép hủy khi đang Vừa tạo (New - 1) hoặc Đã duyệt (Accepted - 2)
            if (order.Status == OrderStatusEnum.New || order.Status == OrderStatusEnum.Accepted)
            {
                // Logic cập nhật trạng thái thành Cancelled (-1) trong CSDL
                bool result = await SalesDataService.CancelOrderAsync(id);
                if (result) return Json(new { code = 1, message = "Đã hủy đơn hàng thành công." });
                return Json(new { code = 0, message = "Không thể hủy đơn hàng lúc này." });
            }

            return Json(new { code = 0, message = "Trạng thái đơn hàng không cho phép hủy." });
        }

        [HttpPost]
        public async Task<IActionResult> FinishOrder(int id)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return Json(new { code = 0, message = "Đơn hàng không tồn tại." });

            // Chỉ cho phép xác nhận nhận hàng khi đơn đang Giao hàng (Shipping - 3)
            if (order.Status == OrderStatusEnum.Shipping)
            {
                // Logic cập nhật trạng thái thành Completed (4) và set thời gian hoàn thành
                bool result = await SalesDataService.CompleteOrderAsync(id);
                if (result) return Json(new { code = 1, message = "Cảm ơn bạn đã mua sắm!" });
                return Json(new { code = 0, message = "Cập nhật thất bại." });
            }

            return Json(new { code = 0, message = "Trạng thái đơn hàng không hợp lệ." });
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            // Gọi hàm xóa sạch giỏ hàng từ Service
            ShoppingCartService.ClearCart();

            // Trả về kết quả JSON để Javascript xử lý
            return Json(new { code = 1, message = "Giỏ hàng đã được xóa sạch." });
        }
    }
}