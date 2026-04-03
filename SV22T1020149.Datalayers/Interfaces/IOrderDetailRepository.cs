using SV22T1020149.Models.Sales;

namespace SV22T1020149.DataLayers.Interfaces
{
    public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
    {
        Task<List<OrderDetail>> GetByOrderIdAsync(int orderId);
    }
}