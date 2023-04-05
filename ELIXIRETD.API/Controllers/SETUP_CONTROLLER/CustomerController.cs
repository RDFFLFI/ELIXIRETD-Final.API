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

        [HttpPost]
        [Route("AddNewCustomer")]
        public async Task<IActionResult> AddNewCustomer(Customer customer)
        {


            if (await _unitOfWork.Customers.CustomerCodeExist(customer.CustomerCode))
                return BadRequest("Customer already Exist!, Please try something else!");

            await _unitOfWork.Customers.AddCustomer(customer);
            await _unitOfWork.CompleteAsync();

            return Ok(customer);

        }


        [HttpPut]
        [Route("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomerInfo([FromBody] Customer customer)
        {


            await _unitOfWork.Customers.UpdateCustomer(customer);
            await _unitOfWork.CompleteAsync();

            return Ok(customer);
        }


        [HttpPut]
        [Route("InActiveCustomer")]
        public async Task<IActionResult> InActiveCustomer([FromBody] Customer customer)
        {
   
            await _unitOfWork.Customers.InActiveCustomer(customer);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully inactive customer!");
        }

        [HttpPut]
        [Route("ActivateCustomer")]
        public async Task<IActionResult> ActivateCustomer([FromBody] Customer customer)
        {
         
            await _unitOfWork.Customers.ActivateCustomer(customer);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully activate customer!");
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
