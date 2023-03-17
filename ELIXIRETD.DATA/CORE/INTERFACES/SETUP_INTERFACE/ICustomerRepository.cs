using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE
{
    public interface ICustomerRepository
    {

        //-----------CUSTOMER--------------//
        Task<IReadOnlyList<CustomerDto>> GetAllActiveCustomers();
        Task<IReadOnlyList<CustomerDto>> GetAllInActiveCustomers();
        Task<bool> AddCustomer(Customer customer);
        Task<bool> UpdateCustomer(Customer customer);
        Task<bool> InActiveCustomer(Customer customer);
        Task<bool> ActivateCustomer(Customer customer);
        Task<PagedList<CustomerDto>> GetAllCustomerWithPagination(bool status, UserParams userParams);
        Task<PagedList<CustomerDto>> GetCustomerWithPaginationOrig(UserParams userParams, bool status, string search);


        Task<bool> ValidateFarmId(int id);
        Task<bool> CustomerCodeExist(string customer);
        Task<bool> CustomerTypeExist(string type);


        //--------------CUSTOMER TYPE---------------------//

        Task<IReadOnlyList<CustomerTypeDto>> GetAllActiveCustomersType();
        Task<IReadOnlyList<CustomerTypeDto>> GetAllInActiveCustomersType();
        Task<bool> AddCustomerType(CustomerType type);
        Task<bool> UpdateCustomerType(CustomerType type);
        Task<bool> InActiveCustomerType(CustomerType type);
        Task<bool> ActivateCustomerType(CustomerType type);
        Task<PagedList<CustomerTypeDto>> GetAllCustomerTypeWithPagination(bool status, UserParams userParams);
        Task<PagedList<CustomerTypeDto>> GetCustomerTypeWithPaginationOrig(UserParams userParams, bool status, string search);



    }


}
