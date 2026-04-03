using SV22T1020149.DataLayers.Interfaces;
using SV22T1020149.DataLayers.SQLServer;
using SV22T1020149.Models.Partner;

namespace SV22T1020149.BusinessLayers
{
    public static class CommonDataService
    {
        private static readonly ICustomerRepository customerDB;

        static CommonDataService()
        {
            customerDB = new CustomerRepository(Configuration.ConnectionString);
        }

        public static async Task<Customer?> GetCustomerAsync(int customerId)
        {
            return await customerDB.GetAsync(customerId);
        }

        public static async Task<int> AddCustomerAsync(Customer data)
        {
            return await customerDB.AddAsync(data);
        }

        public static async Task<bool> UpdateCustomerAsync(Customer data)
        {
            return await customerDB.UpdateAsync(data);
        }

        public static async Task<bool> ValidateCustomerEmailAsync(string email, int customerId = 0)
        {
            return await customerDB.ValidateEmailAsync(email, customerId);
        }

        public static async Task<Customer?> AuthorizeCustomerAsync(string email, string password)
        {
            return await customerDB.AuthorizeAsync(email, password);
        }

        public static async Task<bool> DeleteCustomerAsync(int customerId)
        {
            return await customerDB.DeleteAsync(customerId);
        }
    }
}