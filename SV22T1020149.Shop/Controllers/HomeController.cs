using Microsoft.AspNetCore.Mvc;
using SV22T1020149.BusinessLayers;
using SV22T1020149.Models.Catalog;

namespace SV22T1020149.Shop.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var input = new ProductSearchInput { Page = 1, PageSize = 12, SearchValue = "", CategoryID = 0 };
            var result = await CatalogDataService.ListProductsAsync(input);
            return View(result.DataItems);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}