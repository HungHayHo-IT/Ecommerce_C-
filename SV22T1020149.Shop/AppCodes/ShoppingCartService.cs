using SV22T1020149.Models.Sales;

namespace SV22T1020149.Shop.AppCodes
{
    public static class ShoppingCartService
    {
        private const string CART = "ShoppingCart";

        public static List<OrderDetailViewInfo> GetShoppingCart()
        {
            var cart = ApplicationContext.GetSessionData<List<OrderDetailViewInfo>>(CART);
            if (cart == null)
            {
                cart = new List<OrderDetailViewInfo>();
                ApplicationContext.SetSessionData(CART, cart);
            }
            return cart;
        }

        public static OrderDetailViewInfo? GetCartItem(int productID)
        {
            var cart = GetShoppingCart();
            return cart.Find(m => m.ProductID == productID);
        }

        public static void AddCartItem(OrderDetailViewInfo item)
        {
            var cart = GetShoppingCart();
            var existsItem = cart.Find(m => m.ProductID == item.ProductID);
            if (existsItem == null)
            {
                cart.Add(item);
            }
            else
            {
                existsItem.Quantity += item.Quantity;
                existsItem.SalePrice = item.SalePrice;
            }
            ApplicationContext.SetSessionData(CART, cart);
        }

        public static void UpdateCartItem(int productID, int quantity, decimal salePrice)
        {
            var cart = GetShoppingCart();
            var item = cart.Find(m => m.ProductID == productID);
            if (item != null)
            {
                item.Quantity = quantity;
                item.SalePrice = salePrice;
                ApplicationContext.SetSessionData(CART, cart);
            }
        }

        public static void RemoveCartItem(int productID)
        {
            var cart = GetShoppingCart();
            int index = cart.FindIndex(m => m.ProductID == productID);
            if (index >= 0)
            {
                cart.RemoveAt(index);
                ApplicationContext.SetSessionData(CART, cart);
            }
        }

        public static void ClearCart()
        {
            var cart = new List<OrderDetailViewInfo>();
            ApplicationContext.SetSessionData(CART, cart);
        }
    }
}