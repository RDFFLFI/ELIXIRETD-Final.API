using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIRETD.API.Controllers.SETUP_CONTROLLER
{

    public class CustomerController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        //----------------CUSTOMER------------//


        [HttpGet]
        [Route("GetAllActiveCustomers")]
        public async Task<IActionResult> GetAllActiveCustomers()
        {
            var customer = await _unitOfWork.Customers.GetAllActiveCustomers();

            return Ok(customer);
        }

        [HttpGet]
        [Route("GetAllInActiveCustomers")]
        public async Task<IActionResult> GetAllInActiveCustomers()
        {
            var customer = await _unitOfWork.Customers.GetAllInActiveCustomers();

            return Ok(customer);
        }

        [HttpPut]
        [Route("AddNewCustomer")]
        public async Task<IActionResult> AddNewCustomer([FromBody] Customer[] customer)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult("Something went wrong!") { StatusCode = 500 };
            }

            List<Customer> duplicateList = new List<Customer>();
            List<Customer> availableImport = new List<Customer>();

            foreach ( Customer items in customer)
            {

                if (customer.Count(x => x.CustomerCode == items.CustomerCode && x.CustomerName == items.CustomerName
                //&& x.CompanyCode == items.CompanyCode && x.DepartmentCode == items.DepartmentCode && x.LocationCode == items.LocationCode 
               /* && x.AccountCode == items.AccountCode*/) > 1)
                {

                    duplicateList.Add(items);
                }

                else
                {

                    var existingCustomer = await _unitOfWork.Customers.GetById(items.Customer_No);

                    if (existingCustomer != null)
                    { 

                        existingCustomer.CustomerCode = items.CustomerCode;
                        existingCustomer.CustomerName = items.CustomerName;

                        //existingCustomer.CompanyCode = items.CompanyCode;
                        //existingCustomer.CompanyName = items.CompanyName;
                        //existingCustomer.DepartmentCode = items.DepartmentCode;
                        //existingCustomer.DepartmentName = items.DepartmentName;
                        //existingCustomer.LocationCode = items.LocationCode;
                        //existingCustomer.LocationName = items.LocationName;
                        //existingCustomer.AccountCode = items.AccountCode;
                        //existingCustomer.AccountTitles = items.AccountTitles;

                        existingCustomer.AddedBy = items.AddedBy;
                        existingCustomer.IsActive = items.IsActive;
                        existingCustomer.DateAdded = items.DateAdded;


                        await _unitOfWork.Customers.Update(existingCustomer);
                    }
                    else if (await _unitOfWork.Customers.GetByCustomerNo(items.Customer_No) == null)
                    {

                        availableImport.Add(items);
                        await _unitOfWork.Customers.AddCustomer(items);
                    }

                }

            }

            var resultlist = new
            {
                AvailableImport = availableImport,
                DuplicateList = duplicateList,
            };

            if (duplicateList.Count == 0)
            {
                await _unitOfWork.CompleteAsync();
                return Ok("Successfully added!");
            }
            else
            {

                return BadRequest(resultlist);
            }
        }



        [HttpGet]
        [Route("GetAllCustomerWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomerWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var customer = await _unitOfWork.Customers.GetAllCustomerWithPagination(status, userParams);

            Response.AddPaginationHeader(customer.CurrentPage, customer.PageSize, customer.TotalCount, customer.TotalPages, customer.HasNextPage, customer.HasPreviousPage);

            var customerResult = new
            {
                customer,
                customer.CurrentPage,
                customer.PageSize,
                customer.TotalCount,
                customer.TotalPages,
                customer.HasNextPage,
                customer.HasPreviousPage
            };

            return Ok(customerResult);
        }

        [HttpGet]
        [Route("GetAllCustomerWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomerWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllCustomerWithPagination(status, userParams);

            var customer = await _unitOfWork.Customers.GetCustomerWithPaginationOrig(userParams, status, search);


            Response.AddPaginationHeader(customer.CurrentPage, customer.PageSize, customer.TotalCount, customer.TotalPages, customer.HasNextPage, customer.HasPreviousPage);

            var customerResult = new
            {
                customer,
                customer.CurrentPage,
                customer.PageSize,
                customer.TotalCount,
                customer.TotalPages,
                customer.HasNextPage,
                customer.HasPreviousPage
            };

            return Ok(customerResult);
        }

      



    }
}
