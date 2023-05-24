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
        Task<PagedList<CustomerDto>> GetAllCustomerWithPagination(bool status, UserParams userParams);
        Task<PagedList<CustomerDto>> GetCustomerWithPaginationOrig(UserParams userParams, bool status, string search);


        Task<bool> CustomerCodeExist(string customer);


        //----------Validation -------//


        Task<Customer> GetByCustomerNo(int customerNo);
        //Task<Customer> GetByCustomerNoAndCustomerType(int customerNo, string customerType);
        Task<Customer> GetById(int id);
        Task Update(Customer Customer);




    }


}
