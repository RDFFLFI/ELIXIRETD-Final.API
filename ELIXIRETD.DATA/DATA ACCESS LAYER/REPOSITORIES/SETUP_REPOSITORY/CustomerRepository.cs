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
                                                 CustomerTypeId = x.CustomerTypeId, 
                                                 CustomerType = x.CustomerTypeP.CustomerName, 
                                                 //MobileNumber = x.MobileNumber, 
                                                 CompanyName = x.CompanyName, 
                                                 Address = x.Address, 
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
                                              CustomerTypeId = x.CustomerTypeId,
                                              CustomerType = x.CustomerTypeP.CustomerName,
                                              //MobileNumber = x.MobileNumber,
                                              CompanyName = x.CompanyName,
                                              Address = x.Address,
                                              AddedBy = x.AddedBy,
                                              DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                              IsActive = x.IsActive
                                          });

            return await customer.ToListAsync();
        }

        public async Task<bool> AddCustomer(Customer customer)
        {
            await _context.AddAsync(customer);
            return true;
        }

        public async Task<bool> UpdateCustomer(Customer customer)
        {
            var existingCustomer = await _context.Customers.Where(x => x.Id == customer.Id)
                                                           .FirstOrDefaultAsync();

            existingCustomer.CustomerName = customer.CustomerName;
            existingCustomer.CustomerTypeId = customer.CustomerTypeId;
            existingCustomer.Address = customer.Address;
            existingCustomer.CompanyName = customer.CompanyName;

            return true;

        }

        public async Task<bool> ActivateCustomer(Customer customer)
        {
            var customers = await _context.Customers.Where(x => x.Id == customer.Id)
                                                    .FirstOrDefaultAsync();

            customers.IsActive = true;

            return true;
        }

        public async Task<bool> InActiveCustomer(Customer customer)
        {
            var customers = await _context.Customers.Where(x => x.Id == customer.Id)
                                                   .FirstOrDefaultAsync();

            customers.IsActive = false;

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
                                          CustomerTypeId = x.CustomerTypeId,
                                          CustomerType = x.CustomerTypeP.CustomerName,
                                          //MobileNumber = x.MobileNumber,
                                          CompanyName = x.CompanyName,
                                          Address = x.Address,
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
                                          CustomerTypeId = x.CustomerTypeId,
                                          CustomerType = x.CustomerTypeP.CustomerName,
                                          //MobileNumber = x.MobileNumber,
                                          CompanyName = x.CompanyName,
                                          Address = x.Address,
                                          AddedBy = x.AddedBy,
                                          DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                          IsActive = x.IsActive
                                      }).Where(x => x.CustomerName.ToLower()
                                        .Contains(search.Trim().ToLower()));
          
            return await PagedList<CustomerDto>.CreateAsync(customer, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> ValidateFarmId(int id)
        {
            var validateExisting = await _context.CustomerTypes.FindAsync(id);

            if (validateExisting == null)
                return false;

            return true;
        }


        public async Task<bool> CustomerCodeExist(string customer)
        {
            return await _context.Customers.AnyAsync(x => x.CustomerCode == customer);
        }


        //----------------------CUSTOMER TYPE-------------------------//


        public async Task<IReadOnlyList<CustomerTypeDto>> GetAllActiveCustomersType()
        {
            var type = _context.CustomerTypes.Where(x => x.IsActive == true)
                                             .Select(x => new CustomerTypeDto
                                             {
                                                 Id = x.Id, 
                                                 CustomerName = x.CustomerName,
                                                 AddedBy = x.AddedBy, 
                                                 DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                 IsActive = x.IsActive     
                                             });
            return await type.ToListAsync();

        }

        public async Task<IReadOnlyList<CustomerTypeDto>> GetAllInActiveCustomersType()
        {
            var type = _context.CustomerTypes.Where(x => x.IsActive == false)
                                             .Select(x => new CustomerTypeDto
                                             {
                                                 Id = x.Id,
                                                 CustomerName = x.CustomerName,
                                                 AddedBy = x.AddedBy,
                                                 DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                 IsActive = x.IsActive
                                             });
            return await type.ToListAsync();
        }

        public async Task<bool> AddCustomerType(CustomerType type)
        {
            await _context.CustomerTypes.AddAsync(type);
            return true;
        }

        public async Task<bool> UpdateCustomerType(CustomerType type)
        {
            var types = await _context.CustomerTypes.Where(x => x.Id == type.Id)
                                                    .FirstOrDefaultAsync();

            types.CustomerName = type.CustomerName;

            return true;
        }

        public async Task<bool> InActiveCustomerType(CustomerType type)
        {

            var types = await _context.CustomerTypes.Where(x => x.Id == type.Id)
                                                    .FirstOrDefaultAsync();

            types.IsActive = false;

            return true;
        }

        public async Task<bool> ActivateCustomerType(CustomerType type)
        {
            var types = await _context.CustomerTypes.Where(x => x.Id == type.Id)
                                                    .FirstOrDefaultAsync();

            types.IsActive = true;

            return true;
        }

        public async Task<PagedList<CustomerTypeDto>> GetAllCustomerTypeWithPagination(bool status, UserParams userParams)
        {
            var type = _context.CustomerTypes.Where(x => x.IsActive == status)
                                             .OrderByDescending(x => x.DateAdded)
                                            .Select(x => new CustomerTypeDto
                                            {
                                                Id = x.Id,
                                                CustomerName = x.CustomerName,
                                                AddedBy = x.AddedBy,
                                                DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                IsActive = x.IsActive

                                            });

            return await PagedList<CustomerTypeDto>.CreateAsync(type, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<CustomerTypeDto>> GetCustomerTypeWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var type = _context.CustomerTypes.Where(x => x.IsActive == status)
                                   .OrderByDescending(x => x.DateAdded)
                                   .Select(x => new CustomerTypeDto
                                   {
                                       Id = x.Id,
                                       CustomerName = x.CustomerName,
                                       AddedBy = x.AddedBy,
                                       DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                       IsActive = x.IsActive
                                   }).Where(x => x.CustomerName.ToLower()
                                     .Contains(search.Trim().ToLower()));

            return await PagedList<CustomerTypeDto>.CreateAsync(type, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> CustomerTypeExist(string type)
        {
            return await _context.CustomerTypes.AnyAsync(x => x.CustomerName == type);
        }
    }
}
