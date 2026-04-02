using Microsoft.AspNetCore.Mvc;
using SV22T1020149.BusinessLayers;
using SV22T1020149.Models.Catalog;
using SV22T1020149.Models.Sales;
using System.Threading.Tasks;

namespace SV22T1020149.Admin.Controllers
{
    /// <summary>
    /// Controller quản lý các nghiệp vụ liên quan đến đơn hàng
    /// </summary>
    
    

    public class OrderController : Controller
        
    {
        private const string PRODUCT_SEARCH = "ProductSearchInput";
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
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IActionResult> SearchProduct(ProductSearchInput input)
        {
            var result = await CatalogDataService.ListProductsAsync(input);
            ApplicationContext.SetSessionData("SearchProductResult", result);
            return View(result);
        }


        /// <summary>
        /// Hien thi gio hang
        /// </summary>
        /// <returns></returns>
        public IActionResult ShowCart()
        {
            var cart = ShoppingCartService.GetShoppingCart();
            return View(cart);
        }


        public async Task<IActionResult> AddCartItem(int productId, int quantity,decimal price)
        {
            if (quantity <= 0)
                return Json(new ApiResult(0, "Số lượng không hợp lệ"));

            if (price < 0)  
                return Json(new ApiResult(0, "Giá bán không hợp lệ"));
            
            var product = await CatalogDataService.GetProductAsync(productId);
            if(product == null)
                return Json(new ApiResult(0, "Sản phẩm không tồn tại"));

            if(product.IsSelling)
                return Json(new ApiResult(0, "Sản phẩm không còn bán"));

            var item = new OrderDetailViewInfo()
            {
                ProductID = productId,
                ProductName = product.ProductName,
                Quantity = quantity,
                SalePrice = price,
                Unit = product.Unit,
                Photo = product.Photo ?? "nophoto.png"

            };
            ShoppingCartService.AddCartItem(item);

            return Json(new ApiResult(1));
        }

        /// <summary>
        /// Giao diện lập đơn hàng mới
        /// </summary>
        public IActionResult Create()
        {
            var input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if (input == null)
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = 3,
                    SearchValue = ""
                };
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
        public IActionResult EditCartItem(int productId=0)
        {   
            var item = ShoppingCartService.GetCartItem(productId);
            return PartialView(item);
        }
        public IActionResult UpdateCartItem(int productId, int quantity, decimal salePrice)
        {
            if (quantity <= 0)
                return Json(new ApiResult(0, "Số lượng không hợp lệ"));
            if (salePrice < 0)  
                return Json(new ApiResult(0, "Giá bán không hợp lệ"));
            
            
            ShoppingCartService.UpdateCartItem(productId,quantity,salePrice);
            return Json(new ApiResult(1));
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder(int customerID = 0 , string province = "",string address ="")
        {
            var cart = ShoppingCartService.GetShoppingCart();
            if(cart.Count == 0)
                return Json(new ApiResult(0, "Giỏ hàng trống"));

            var order = new Order()
            {
                CustomerID = customerID == 0 ? null : customerID,
                DeliveryProvince = province,
                DeliveryAddress = address
            };
            int orderID = SalesDataService.AddOrderAsync(order).Result;
            foreach(var item in cart)
            {
                
                await SalesDataService.AddDetailAsync(new OrderDetail()
                {
                    OrderID = orderID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice
                });
            }
            ShoppingCartService.ClearCart();
            return Json(new ApiResult(orderID));
        }

        /// <summary>
        /// Xóa một sản phẩm khỏi danh sách mặt hàng của đơn hàng
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <param name="productId">Mã sản phẩm</param>
        public IActionResult DeleteCartItem( int productId = 0)
        {
            if(Request.Method == "POST")
            {
                ShoppingCartService.RemoveCartItem(productId);
                return Json(new ApiResult(1));
            }

            var item = ShoppingCartService.GetCartItem(productId);
            return PartialView(item);
        }

        /// <summary>
        /// Xóa toàn bộ sản phẩm đã chọn trong giỏ hàng
        /// </summary>
        public IActionResult ClearCart()
        {
            if(Request.Method == "POST")
            {
                ShoppingCartService.ClearCart();
                return Json(new ApiResult(1));
            }
            return PartialView();
        }

        /// <summary>
        /// Chấp nhận/Duyệt đơn hàng (Chuyển từ trạng thái Chờ sang Đã xác nhận)
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        public IActionResult Accept(int id)
        {
            return PartialView();
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
        /// Từ chối đơn hàng (Trường hợp đơn hàng không hợp lệ ngay từ đầu)
        /// </summary>
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
