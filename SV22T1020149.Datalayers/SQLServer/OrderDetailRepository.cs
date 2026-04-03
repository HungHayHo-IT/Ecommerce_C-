using Dapper;
using Microsoft.Data.SqlClient;
using SV22T1020149.DataLayers.Interfaces;
using SV22T1020149.Models.Common;
using SV22T1020149.Models.Sales;

namespace SV22T1020149.DataLayers.SQLServer
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly string _connectionString;

        public OrderDetailRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> AddAsync(OrderDetail data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sql = @"INSERT INTO OrderDetails(OrderID, ProductID, Quantity, SalePrice)
                            VALUES(@OrderID, @ProductID, @Quantity, @SalePrice);
                            SELECT SCOPE_IDENTITY();";
                return await connection.ExecuteScalarAsync<int>(sql, data);
            }
        }

        public async Task<bool> UpdateAsync(OrderDetail data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sql = @"UPDATE OrderDetails SET OrderID = @OrderID, ProductID = @ProductID, 
                            Quantity = @Quantity, SalePrice = @SalePrice WHERE OrderDetailID = @OrderDetailID";
                return await connection.ExecuteAsync(sql, data) > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sql = @"DELETE FROM OrderDetails WHERE OrderDetailID = @OrderDetailID";
                return await connection.ExecuteAsync(sql, new { OrderDetailID = id }) > 0;
            }
        }

        public async Task<OrderDetail?> GetAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sql = @"SELECT * FROM OrderDetails WHERE OrderDetailID = @OrderDetailID";
                return await connection.QueryFirstOrDefaultAsync<OrderDetail>(sql, new { OrderDetailID = id });
            }
        }

        public async Task<PagedResult<OrderDetail>> ListAsync(PaginationSearchInput input)
        {
            var result = new PagedResult<OrderDetail>() { Page = input.Page, PageSize = input.PageSize };

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sql = @"SELECT COUNT(*) FROM OrderDetails";
                result.RowCount = await connection.ExecuteScalarAsync<int>(sql);

                sql = @"SELECT * FROM OrderDetails ORDER BY OrderDetailID
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var items = await connection.QueryAsync<OrderDetail>(sql,
                    new { Offset = (input.Page - 1) * input.PageSize, PageSize = input.PageSize });
                result.DataItems = items.ToList();

                return result;
            }
        }

        public async Task<List<OrderDetail>> GetByOrderIdAsync(int orderId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sql = @"SELECT * FROM OrderDetails WHERE OrderID = @OrderID";
                var items = await connection.QueryAsync<OrderDetail>(sql, new { OrderID = orderId });
                return items.ToList();
            }
        }

        public async Task<bool> IsUsedAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sql = @"IF EXISTS (SELECT 1 FROM OrderDetails WHERE OrderDetailID = @OrderDetailID)
                                SELECT 1
                            ELSE
                                SELECT 0";
                var result = await connection.ExecuteScalarAsync<int>(sql, new { OrderDetailID = id });
                return result == 1;
            }
        }
    }
}