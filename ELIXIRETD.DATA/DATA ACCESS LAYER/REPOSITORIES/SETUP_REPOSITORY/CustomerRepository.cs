using ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.SETUP_REPOSITORY
{
    public class CustomerRepository : ICustomerRepository
    {
        private new readonly StoreContext _context;
        public CustomerRepository(StoreContext context)
        {
            _context = context; 
        }


        public async Task<IReadOnlyList<CustomerDto>> GetAllActiveCustomers()
        {
            var customer = _context.Customers.Where(x => x.IsActive == true)
                                             .Select(x => new CustomerDto
                                             {
                                                 Id = x.Id, 
                                                 CustomerCode = x.CustomerCode, 
                                                 CustomerName = x.CustomerName,
                                                 CustomerType = x.CustomerType,

                                                 SyncStatus = x.StatusSync,


                                                 ModifyBy = x.ModifyBy,
                                                 ModifyDate = x.ModifyDate.ToString(),
                                                 SyncDate = x.SyncDate.ToString(),

                                                 CompanyCode = x.CompanyCode,
                                                 CompanyName = x.CompanyName,
                                                 DepartmentCode = x.DepartmentCode,
                                                 DepartmentName = x.DepartmentName,
                                                 LocationCode = x.LocationCode,
                                                 LocationName = x.LocationName,
                                                 //AccountCode = x.AccountCode,
                                                 //AccountTitles = x.AccountTitles,

                                                 AddedBy = x.AddedBy,
                                                 
                                                 DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                 IsActive = x.IsActive 

                                             });

            return await customer.ToListAsync();

        }

        public async Task<IReadOnlyList<CustomerDto>> GetAllInActiveCustomers()
        {
            var customer = _context.Customers.Where(x => x.IsActive == false)
                                          .Select(x => new CustomerDto
                                          {
                                              Id = x.Id,
                                              CustomerCode = x.CustomerCode,
                                              CustomerName = x.CustomerName,
                                              CustomerType = x.CustomerType,

                                              SyncStatus = x.StatusSync,

                                              ModifyBy = x.ModifyBy,
                                              ModifyDate = x.ModifyDate.ToString(),

                                              SyncDate = x.SyncDate.ToString(),

                                              CompanyCode = x.CompanyCode,
                                              CompanyName = x.CompanyName,
                                              DepartmentCode = x.DepartmentCode,
                                              DepartmentName = x.DepartmentName,
                                              LocationCode = x.LocationCode,
                                              LocationName = x.LocationName,
                                              //AccountCode = x.AccountCode,
                                              //AccountTitles = x.AccountTitles,

                                              AddedBy = x.AddedBy,
                                              DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                              IsActive = x.IsActive
                                          });

            return await customer.ToListAsync();
        }

        public async Task<bool> AddCustomer(Customer customer)
        {

            customer.Id = 0;
            await _context.Customers.AddAsync(customer);

            return true;
        }


        public async Task<PagedList<CustomerDto>> GetAllCustomerWithPagination(bool status, UserParams userParams)
        {
            var customer = _context.Customers.Where(x => x.IsActive == status)
                                      .Select(x => new CustomerDto
                                      {
                                          Id = x.Id,
                                          CustomerCode = x.CustomerCode,
                                          CustomerName = x.CustomerName,
                                          CustomerType = x.CustomerType,


                                          ModifyBy = x.ModifyBy,
                                          ModifyDate = x.ModifyDate.ToString(),

                                          SyncDate = x.SyncDate.ToString(),

                                          SyncStatus = x.StatusSync,

                                          CompanyCode = x.CompanyCode,
                                          CompanyName = x.CompanyName,
                                          DepartmentCode = x.DepartmentCode,
                                          DepartmentName = x.DepartmentName,
                                          LocationCode = x.LocationCode,
                                          LocationName = x.LocationName,
                                          //AccountCode = x.AccountCode,
                                          //AccountTitles = x.AccountTitles,

                                          AddedBy = x.AddedBy,
                                          DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                          IsActive = x.IsActive

                                      });

            return await PagedList<CustomerDto>.CreateAsync(customer, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<CustomerDto>> GetCustomerWithPaginationOrig(UserParams userParams, bool status, string search)
        {

            

            var customer = _context.Customers.Where(x => x.IsActive == status)
                                      .Select(x => new CustomerDto
                                      {
                                          Id = x.Id,
                                          CustomerCode = x.CustomerCode,
                                          CustomerName = x.CustomerName,
                                          CustomerType = x.CustomerType,
                                          

                                          ModifyBy = x.ModifyBy,
                                          ModifyDate = x.ModifyDate.ToString(),

                                          SyncDate = x.SyncDate.ToString(),

                                          SyncStatus = x.StatusSync,

                                          CompanyCode = x.CompanyCode,
                                          CompanyName = x.CompanyName,
                                          DepartmentCode = x.DepartmentCode,
                                          DepartmentName = x.DepartmentName,
                                          LocationCode = x.LocationCode,
                                          LocationName = x.LocationName,
                                          //AccountCode = x.AccountCode,
                                          //AccountTitles = x.AccountTitles,

                                          AddedBy = x.AddedBy,
                                          DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                          IsActive = x.IsActive

                                      }).Where(x => x.CustomerName.ToLower().Contains(search.Trim().ToLower())
                                        || x.CustomerCode.ToLower().Contains(search.Trim().ToLower())
                                        );

            return await PagedList<CustomerDto>.CreateAsync(customer, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<bool> CustomerCodeExist(string customer)
        {
            return await _context.Customers.AnyAsync(x => x.CustomerCode == customer);
        }

      
        public async Task<Customer> GetByCustomerNo(int customerNo)
        {
            return await _context.Customers.FirstOrDefaultAsync(x => x.Customer_No == customerNo);
        }

        public async Task<Customer> GetById(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task Update(Customer Customer)
        {
   
            //Customer.ModifyDate = DateTime.Now;
            _context.Customers.Update(Customer);
            await Task.CompletedTask;
        }

        
    }
}
