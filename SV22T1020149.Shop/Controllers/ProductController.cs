using Microsoft.AspNetCore.Mvc;
using SV22T1020149.BusinessLayers;
using SV22T1020149.Models.Catalog;
using SV22T1020149.Models.Common;

namespace SV22T1020149.Shop.Controllers
{
    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 12;

        // Thêm tham số minPrice và maxPrice
        public async Task<IActionResult> Index(int categoryId = 0, string searchValue = "", decimal minPrice = 0, decimal maxPrice = 0, int page = 1)
        {
            var catInput = new PaginationSearchInput { Page = 1, PageSize = 100, SearchValue = "" };
            var categoryResult = await CatalogDataService.ListCategoriesAsync(catInput);
            ViewBag.Categories = categoryResult.DataItems;

            // Cập nhật ProductSearchInput để nhận khoảng giá
            var searchInput = new ProductSearchInput
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                CategoryID = categoryId,
                MinPrice = minPrice, // Đảm bảo class ProductSearchInput của bạn có thuộc tính này
                MaxPrice = maxPrice  // Đảm bảo class ProductSearchInput của bạn có thuộc tính này
            };
            var productResult = await CatalogDataService.ListProductsAsync(searchInput);

            // Truyền dữ liệu ra ViewBag để hiển thị lại lên form
            ViewBag.CategoryId = categoryId;
            ViewBag.SearchValue = searchInput.SearchValue;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(productResult);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var product = await CatalogDataService.GetProductAsync(id);
            if (product == null) return RedirectToAction("Index");
            return View(product);
        }
    }
}