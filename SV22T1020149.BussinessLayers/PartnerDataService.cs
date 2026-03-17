using SV22T1020149.BusinessLayers;
using SV22T1020149.DataLayers.Interfaces;
using SV22T1020149.DataLayers.SQLServer;
using SV22T1020149.Models.Common;
using SV22T1020149.Models.Partner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1020149.BussinessLayers
{
    /// <summary>
    /// Lớp cung cấp các chức năng nghiệp vụ liên quan đến các đối tác của hệ thống.
    /// Bao gồm các đối tượng:
    /// - Nhà cung cấp (Supplier)
    /// - Người giao hàng (Shipper)
    /// - Khách hàng (Customer)
    /// 
    /// Các chức năng chính:
    /// - Lấy danh sách theo phân trang
    /// - Lấy thông tin chi tiết
    /// - Thêm mới
    /// - Cập nhật
    /// - Xóa
    /// - Kiểm tra dữ liệu có đang được sử dụng hay không
    /// </summary>
    public static class PartnerDataService
    {
        private static readonly IGenericRepository<Supplier> supplierDB;
        private static readonly IGenericRepository<Shipper> shipperDB;
        private static readonly ICustomerRepository customerDB;
        private static readonly IEmployeeRepository employeeDB;

        /// <summary>
        /// Hàm khởi tạo tĩnh.
        /// Khởi tạo các đối tượng Repository để thao tác dữ liệu với cơ sở dữ liệu SQL Server.
        /// Chuỗi kết nối được lấy từ lớp Configuration.
        /// </summary>
        static PartnerDataService()
        {
            supplierDB = new SupplierRepository(Configuration.ConnectionString);
            shipperDB = new ShipperRepository(Configuration.ConnectionString);
            customerDB = new CustomerRepository(Configuration.ConnectionString);
            employeeDB = new EmployeeRepository(Configuration.ConnectionString);
        }

        //=====================================================
        // SUPPLIER
        //=====================================================

        /// <summary>
        /// Lấy danh sách nhà cung cấp theo điều kiện tìm kiếm và phân trang
        /// </summary>
        public static async Task<PagedResult<Supplier>> ListSupplierAsync(PaginationSearchInput input)
        {
            return await supplierDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một nhà cung cấp dựa vào mã nhà cung cấp
        /// </summary>
        public static async Task<Supplier?> GetSupplierAsync(int supplierID)
        {
            return await supplierDB.GetAsync(supplierID);
        }

        /// <summary>
        /// Bổ sung một nhà cung cấp mới vào hệ thống
        /// </summary>
        public static async Task<int> AddSupplierAsync(Supplier supplier)
        {
            return await supplierDB.AddAsync(supplier);
        }

        /// <summary>
        /// Cập nhật thông tin nhà cung cấp
        /// </summary>
        public static async Task<bool> UpdateSupplierAsync(Supplier supplier)
        {
            return await supplierDB.UpdateAsync(supplier);

        }

        /// <summary>
        /// Xóa nhà cung cấp khỏi hệ thống
        /// </summary>
        public static async Task<bool> DeleteSupplierAsync(int supplierID)
        {
            if (await supplierDB.IsUsedAsync(supplierID)) return false;
            return await supplierDB.DeleteAsync(supplierID);
        }

        /// <summary>
        /// Kiểm tra nhà cung cấp có đang được sử dụng trong dữ liệu hay không
        /// </summary>
        public static async Task<bool> IsUsedSupplierAsync(int supplierID)
        {
            return await supplierDB.IsUsedAsync(supplierID);
        }

        //=====================================================
        // SHIPPER
        //=====================================================

        /// <summary>
        /// Lấy danh sách người giao hàng theo điều kiện tìm kiếm và phân trang
        /// </summary>
        public static async Task<PagedResult<Shipper>> ListShipperAsync(PaginationSearchInput input)
        {
            return await shipperDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin của một người giao hàng dựa vào mã shipper
        /// </summary>
        public static async Task<Shipper?> GetShipperAsync(int shipperID)
        {
            return await shipperDB.GetAsync(shipperID);
        }

        /// <summary>
        /// Bổ sung người giao hàng mới
        /// </summary>
        public static async Task<int> AddShipperAsync(Shipper shipper)
        {
            return await shipperDB.AddAsync(shipper);
        }

        /// <summary>
        /// Cập nhật thông tin người giao hàng
        /// </summary>
        public static async Task<bool> UpdateShipperAsync(Shipper shipper)
        {
            return await shipperDB.UpdateAsync(shipper);
        }

        /// <summary>
        /// Xóa người giao hàng
        /// </summary>
        public static async Task<bool> DeleteShipperAsync(int shipperID)
        {
            if (await shipperDB.IsUsedAsync(shipperID)) return false;
            return await shipperDB.DeleteAsync(shipperID);
        }

        /// <summary>
        /// Kiểm tra người giao hàng có đang được sử dụng hay không
        /// </summary>
        public static async Task<bool> IsUsedShipperAsync(int shipperID)
        {
            return await shipperDB.IsUsedAsync(shipperID);
        }

        //=====================================================
        // CUSTOMER
        //=====================================================

        /// <summary>
        /// Lấy danh sách khách hàng theo điều kiện tìm kiếm và phân trang
        /// </summary>
        public static async Task<PagedResult<Customer>> ListCustomerAsync(PaginationSearchInput input)
        {
            return await customerDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một khách hàng dựa vào mã khách hàng
        /// </summary>
        public static async Task<Customer?> GetCustomerAsync(int customerID)
        {
            return await customerDB.GetAsync(customerID);
        }

        /// <summary>
        /// Bổ sung khách hàng mới
        /// </summary>
        public static async Task<int> AddCustomerAsync(Customer customer)
        {
            return await customerDB.AddAsync(customer);
        }

        /// <summary>
        /// Cập nhật thông tin khách hàng
        /// </summary>
        public static async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            return await customerDB.UpdateAsync(customer);
        }

        /// <summary>
        /// Xóa khách hàng khỏi hệ thống
        /// </summary>
        public static async Task<bool> DeleteCustomerAsync(int customerID)
        {
            if (await customerDB.IsUsedAsync(customerID)) return false;
            return await customerDB.DeleteAsync(customerID);
        }

        /// <summary>
        /// Kiểm tra khách hàng có đang được sử dụng hay không
        /// </summary>
        public static async Task<bool> IsUsedCustomerAsync(int customerID)
        {
            return await customerDB.IsUsedAsync(customerID);
        }

        /// <summary>
        /// Kiểm tra email khách hàng có hợp lệ hay không 
        /// (Email hợp lệ nếu không trùng với khách hàng khác
        /// </summary>
        /// <param name="email"></param>
        /// <param name="customerID"></param>
        /// Nếu bằng 0 , tức là kiểm tra email của khách hàng mới 
        /// Nếu bằng 0, tức là kiểm tra email của khách hàng có mã là <paramef name="customerID"/>
        /// 
        /// <returns></returns>
        public static async Task<bool> ValidateCustomerEmailAsync(String email, int customerID = 0)
        {
            return await customerDB.ValidateEmailAsync(email, customerID);
        }
    }
}