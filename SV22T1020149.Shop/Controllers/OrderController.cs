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
            var cart = ShoppingCartService.GetShoppingCart();
            if (cart.Count == 0) return Json(new { code = 0, message = "Gi? hàng r?ng" });
            if (string.IsNullOrWhiteSpace(province)) return Json(new { code = 0, message = "Vui lòng ch?n t?nh" });
            if (string.IsNullOrWhiteSpace(address)) return Json(new { code = 0, message = "Vui lòng nh?p ??a ch?" });

            var order = new Order
            {
                DeliveryProvince = province,
                DeliveryAddress = address
            };
            int orderId = await SalesDataService.AddOrderAsync(order);
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
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return RedirectToAction("History");
            ViewBag.OrderDetails = await SalesDataService.ListDetailsAsync(id);
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
    }
}