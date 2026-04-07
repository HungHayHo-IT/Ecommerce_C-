using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020149.BusinessLayers;
using SV22T1020149.Models.Catalog;
using SV22T1020149.Models.Common;

namespace SV22T1020149.Admin.Controllers
{
    /// <summary>
    /// Controller quản lý các hoạt động liên quan đến mặt hàng (sản phẩm)
    /// </summary>
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.DataManager}")]
    public class ProductController : Controller
    {
        private const int PAGESIZE = 10;
        private const string PRODUCT_SEARCH = "ProductSearchInput";

        /// <summary>
        /// Giao diện nhập đầu vào tìm kiếm
        /// </summary>
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if (input == null)
            {
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = ApplicationContext.PageSize,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                    MinPrice = 0,
                    MaxPrice = 0
                };
            }
            return View(input);
        }

        /// <summary>
        /// Tìm kiếm và trả về kết quả dưới dạng PartialView để Ajax đắp vào Index
        /// </summary>
        public async Task<IActionResult> Search(ProductSearchInput input)
        {
            // Logic: Làm sạch dữ liệu đầu vào
            input.SearchValue ??= "";

            // Xử lý giá nếu người dùng nhập số âm (tùy chọn)
            if (input.MinPrice < 0) input.MinPrice = 0;
            if (input.MaxPrice < 0) input.MaxPrice = 0;
            if (input.MaxPrice > 0 && input.MaxPrice < input.MinPrice) input.MaxPrice = input.MinPrice;

            // Lấy kết quả tìm kiếm
            var result = await CatalogDataService.ListProductsAsync(input);

            // Lưu lại điều kiện tìm kiếm vào Session để khi quay lại trang Index không bị mất filter
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);

            return PartialView(result);
        }

        /// <summary>
        /// Giao diện thêm mới mặt hàng
        /// </summary>
        public IActionResult Create()
        {
            ViewBag.Title = "Thêm mới mặt hàng";
            var model = new Product() { ProductID = 0, Photo = "images/products/nophoto.png", IsSelling = true };
            return View("Edit", model);
        }

        /// <summary>
        /// Giao diện cập nhật thông tin mặt hàng
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật mặt hàng";
            var model = await CatalogDataService.GetProductAsync(id);
            if (model == null) return RedirectToAction("Index");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveData(Product data, IFormFile? uploadPhoto)
        {
            try
            {
                // 1. Kiểm tra các trường bắt buộc
                if (string.IsNullOrWhiteSpace(data.ProductName))
                    ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng không được để trống");
                if (data.CategoryID == 0)
                    ModelState.AddModelError(nameof(data.CategoryID), "Vui lòng chọn loại hàng");
                if (data.SupplierID == 0)
                    ModelState.AddModelError(nameof(data.SupplierID), "Vui lòng chọn nhà cung cấp");
                if (string.IsNullOrWhiteSpace(data.Unit))
                    ModelState.AddModelError(nameof(data.Unit), "Vui lòng nhập đơn vị tính");

                if (!ModelState.IsValid)
                {
                    ViewBag.Title = data.ProductID == 0 ? "Thêm mới mặt hàng" : "Cập nhật mặt hàng";
                    return View("Edit", data);
                }

                // 2. Tiền xử lý dữ liệu để tránh lỗi NULL ở Database
                if (string.IsNullOrWhiteSpace(data.ProductDescription))
                    data.ProductDescription = "";

                // 3. Xử lý ảnh upload
                if (uploadPhoto != null)
                {
                    string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                    string folder = Path.Combine(ApplicationContext.WWWRootPath, "images", "products");

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string filePath = Path.Combine(folder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadPhoto.CopyToAsync(stream);
                    }
                    // Lưu đường dẫn mới
                    data.Photo = $"images/products/{fileName}";
                }
                else
                {
                    // TRƯỜNG HỢP: NGƯỜI DÙNG KHÔNG CHỌN FILE HOẶC GIAO DIỆN LỖI ĐẨY LÊN CHUỖI BASE64
                    if (string.IsNullOrWhiteSpace(data.Photo) || data.Photo.StartsWith("data:image") || data.Photo.Length > 255)
                    {
                        if (data.ProductID == 0)
                        {
                            // Đang Thêm mới -> Dùng ảnh mặc định
                            data.Photo = "images/products/nophoto.png";
                        }
                        else
                        {
                            // Đang Cập nhật -> Lấy lại đường dẫn ảnh cũ trong Database để không bị mất ảnh
                            var oldProduct = await CatalogDataService.GetProductAsync(data.ProductID);
                            data.Photo = oldProduct != null ? oldProduct.Photo : "images/products/nophoto.png";
                        }
                    }
                }

                // 4. Thực thi lưu vào Database
                if (data.ProductID == 0)
                {
                    await CatalogDataService.AddProductAsync(data);
                }
                else
                {
                    await CatalogDataService.UpdateProductAsync(data);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Lỗi hệ thống: " + ex.Message);
                return View("Edit", data);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SavePhoto(ProductPhoto data, IFormFile? uploadPhoto)
        {
            try
            {
                // 1. Xử lý lưu ảnh thật (nếu có chọn file)
                if (uploadPhoto != null)
                {
                    string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                    string folder = Path.Combine(ApplicationContext.WWWRootPath, "images", "products");

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string filePath = Path.Combine(folder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadPhoto.CopyToAsync(stream);
                    }
                    data.Photo = $"images/products/{fileName}";
                }
                else
                {
                    // 2. Tiền xử lý nếu không có file hoặc gặp lỗi base64
                    if (string.IsNullOrWhiteSpace(data.Photo) || data.Photo.StartsWith("data:image"))
                    {
                        if (data.PhotoID == 0)
                        {
                            data.Photo = "images/products/nophoto.png";
                        }
                        else
                        {
                            // Đang cập nhật thì lấy lại ảnh cũ
                            var oldPhoto = await CatalogDataService.GetPhotoAsync(data.PhotoID);
                            data.Photo = oldPhoto != null ? oldPhoto.Photo : "images/products/nophoto.png";
                        }
                    }
                }

                // 3. Thực thi lưu vào Database
                if (data.PhotoID == 0)
                {
                    await CatalogDataService.AddPhotoAsync(data);
                }
                else
                {
                    await CatalogDataService.UpdatePhotoAsync(data);
                }

                // Lưu xong thì quay lại trang cập nhật của Mặt hàng đó
                return RedirectToAction("Edit", new { id = data.ProductID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Lỗi hệ thống: " + ex.Message);
                return View("Photo", data); // Tùy thuộc vào tên file View của bạn có thể là CreatePhoto/EditPhoto
            }
        }

        /// <summary>
        /// Xác nhận xóa mặt hàng
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            // Trường hợp: Thực hiện xóa (Yêu cầu gửi lên bằng POST)
            if (HttpMethods.IsPost(Request.Method))
            {
                // Kiểm tra ràng buộc: Nếu mặt hàng đã có đơn hàng thì không cho xóa
                if (await CatalogDataService.IsUsedProductAsync(id))
                {
                    ModelState.AddModelError(string.Empty, "Không thể xóa mặt hàng này vì đã có dữ liệu liên quan (đơn hàng).");
                    var modelErr = await CatalogDataService.GetProductAsync(id);
                    ViewBag.CanDelete = false;
                    return View(modelErr);
                }

                // Thực hiện xóa mặt hàng (Logic trong Service nên bao gồm xóa luôn cả Ảnh và Thuộc tính)
                await CatalogDataService.DeleteProductAsync(id);
                return RedirectToAction("Index");
            }

            // Trường hợp: Hiển thị giao diện xác nhận (Yêu cầu GET)
            var model = await CatalogDataService.GetProductAsync(id);
            if (model == null)
                return RedirectToAction("Index");

            // Kiểm tra khả năng xóa để điều khiển UI (ẩn/hiện nút xóa)
            ViewBag.CanDelete = !await CatalogDataService.IsUsedProductAsync(id);

            return View(model);
        }

        // --- CÁC HÀM XỬ LÝ ẢNH VÀ THUỘC TÍNH (PHOTOS & ATTRIBUTES) ---
        // (Tạm thời giữ nguyên hoặc bổ sung logic lấy dữ liệu thật khi bạn code đến phần này)

        public async Task<IActionResult> Photo(int id, string method, long photoId = 0)
        {
            var product = await CatalogDataService.GetProductAsync(id);
            if (product == null) return RedirectToAction("Index");
            ViewBag.ProductName = product.ProductName;

            switch (method)
            {
                case "add":
                    var newPhoto = new ProductPhoto { ProductID = id, DisplayOrder = 1, IsHidden = false };
                    // CHỈ ĐỊNH RÕ TÊN VIEW LÀ "CreatePhoto"
                    return View("CreatePhoto", newPhoto);

                case "edit":
                    var editPhoto = await CatalogDataService.GetPhotoAsync(photoId);
                    if (editPhoto == null) return RedirectToAction("Edit", new { id = id });
                    // CHỈ ĐỊNH RÕ TÊN VIEW LÀ "EditPhoto"
                    return View("EditPhoto", editPhoto);

                case "delete":
                    await CatalogDataService.DeletePhotoAsync(photoId);
                    return RedirectToAction("Edit", new { id = id });

                default:
                    return RedirectToAction("Edit", new { id = id });
            }
        }

        [HttpGet("Product/Attribute/{id}")]
        [HttpGet("Product/EditAttribute/{id}")]
        public async Task<IActionResult> Attribute(int id, string method, long attributeId = 0)
        {
            var product = await CatalogDataService.GetProductAsync(id);
            if (product == null) return RedirectToAction("Index");
            ViewBag.ProductName = product.ProductName;

            switch (method)
            {
                case "add":
                    var newAttr = new ProductAttribute { ProductID = id, DisplayOrder = 1 };
                    return View(newAttr);

                case "edit":
                    var editAttr = await CatalogDataService.GetAttributeAsync(attributeId);
                    if (editAttr == null) return RedirectToAction("Edit", new { id = id });
                    return View(editAttr);

                case "delete":
                    await CatalogDataService.DeleteAttributeAsync(attributeId);
                    return RedirectToAction("Edit", new { id = id });

                default:
                    return RedirectToAction("Edit", new { id = id });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveAttribute(ProductAttribute data)
        {
            try
            {
                // 1. Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(data.AttributeName))
                    ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không được để trống");
                if (string.IsNullOrWhiteSpace(data.AttributeValue))
                    ModelState.AddModelError(nameof(data.AttributeValue), "Giá trị thuộc tính không được để trống");

                // Nếu có lỗi thì trả lại form
                if (!ModelState.IsValid)
                {
                    var product = await CatalogDataService.GetProductAsync(data.ProductID);
                    ViewBag.ProductName = product?.ProductName;
                    return View("Attribute", data);
                }

                // 2. Thực thi lưu vào Database
                if (data.AttributeID == 0)
                {
                    await CatalogDataService.AddAttributeAsync(data);
                }
                else
                {
                    await CatalogDataService.UpdateAttributeAsync(data);
                }

                // Lưu xong quay lại trang Edit của mặt hàng
                return RedirectToAction("Edit", new { id = data.ProductID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Lỗi hệ thống: " + ex.Message);
                return View("Attribute", data);
            }
        }
    }
}