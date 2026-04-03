using SV22T1020149.DataLayers.Interfaces;
using SV22T1020149.DataLayers.SQLServer;
using SV22T1020149.Models.Common;
using SV22T1020149.Models.Sales;

namespace SV22T1020149.BusinessLayers
{
    public static class OrderDataService
    {
        private static readonly IOrderRepository orderDB;
        private static readonly IOrderDetailRepository orderDetailDB;

        static OrderDataService()
        {
            orderDB = new OrderRepository(Configuration.ConnectionString);
            orderDetailDB = new OrderDetailRepository(Configuration.ConnectionString);
        }

        // Order
        public static async Task<PagedResult<OrderViewInfo>> ListOrdersAsync(OrderSearchInput input)
        {
            return await orderDB.ListAsync(input);
        }

        public static async Task<OrderViewInfo?> GetOrderAsync(int orderId)
        {
            return await orderDB.GetAsync(orderId);
        }

        public static async Task<int> AddOrderAsync(Order data)
        {
            data.Status = OrderStatusEnum.New;
            data.OrderTime = DateTime.Now;
            return await orderDB.AddAsync(data);
        }

        public static async Task<bool> UpdateOrderAsync(Order data)
        {
            return await orderDB.UpdateAsync(data);
        }

        public static async Task<bool> DeleteOrderAsync(int orderId)
        {
            return await orderDB.DeleteAsync(orderId);
        }

        // Order Detail
        public static async Task<List<OrderDetail>> GetOrderDetailsAsync(int orderId)
        {
            return await orderDetailDB.GetByOrderIdAsync(orderId);
        }

        public static async Task<int> AddOrderDetailAsync(OrderDetail data)
        {
            return await orderDetailDB.AddAsync(data);
        }

        public static async Task<bool> DeleteOrderDetailAsync(int orderDetailId)
        {
            return await orderDetailDB.DeleteAsync(orderDetailId);
        }
    }
}