using Microsoft.AspNetCore.Mvc;
using SV22T1020149.BusinessLayers;
using SV22T1020149.Models.Catalog;
using SV22T1020149.Shop.AppCodes;

namespace SV22T1020149.Shop.Controllers
{
    public class ProductController : Controller
    {
        public async Task<IActionResult> List(ProductSearchInput input)
        {
            input.Page = input.Page <= 0 ? 1 : input.Page;
            input.PageSize = input.PageSize <= 0 ? ApplicationContext.PageSize : input.PageSize;
            var result = await CatalogDataService.ListProductsAsync(input);
            return View(result);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var product = await CatalogDataService.GetProductAsync(id);
            if (product == null) return RedirectToAction("List");
            ViewBag.Photos = await CatalogDataService.ListPhotosAsync(id);
            ViewBag.Attributes = await CatalogDataService.ListAttributesAsync(id);
            return View(product);
        }
    }
}