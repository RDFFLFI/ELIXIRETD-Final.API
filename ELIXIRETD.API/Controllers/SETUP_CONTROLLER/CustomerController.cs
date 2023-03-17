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

            var customertypeId = await _unitOfWork.Customers.ValidateFarmId(customer.CustomerTypeId);

            if (customertypeId == false)
                return BadRequest("Farm Type doesn't exist, Please add data first!");

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

            var customertypeId = await _unitOfWork.Customers.ValidateFarmId(customer.CustomerTypeId);

            if (customertypeId == false)
                return BadRequest("Farm Type doesn't exist, Please add data first!");

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


        //------------CUSTOMER TYPE---------------//


        [HttpGet]
        [Route("GetAllActiveCustomerType")]
        public async Task<IActionResult> GetAllActiveCustomerType()
        {
            var customer = await _unitOfWork.Customers.GetAllActiveCustomersType();

            return Ok(customer);
        }

        [HttpGet]
        [Route("GetAllInActiveCustomerType")]
        public async Task<IActionResult> GetAllInActiveCustomerType()
        {
            var customer = await _unitOfWork.Customers.GetAllInActiveCustomersType();

            return Ok(customer);
        }

        [HttpPost]
        [Route("AddNewCustomerType")]
        public async Task<IActionResult> AddNewCustomerType(CustomerType type)
        {

            if (await _unitOfWork.Customers.CustomerTypeExist(type.CustomerName))
                return BadRequest("Customer type already exist, please try something else!");

            await _unitOfWork.Customers.AddCustomerType(type);
            await _unitOfWork.CompleteAsync();

            return Ok(type);

        }


        [HttpPut]
        [Route("UpdateCustomerType")]
        public async Task<IActionResult> UpdateFarm([FromBody] CustomerType type)
        {
 
            await _unitOfWork.Customers.UpdateCustomerType(type);
            await _unitOfWork.CompleteAsync();

            return Ok(type);
        }


        [HttpPut]
        [Route("InActiveCustomerType")]
        public async Task<IActionResult> InActiveCustomerType([FromBody] CustomerType type)
        {
            await _unitOfWork.Customers.InActiveCustomerType(type);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully inActive customer type!");
        }

        [HttpPut]
        [Route("ActivateCustomerType")]
        public async Task<IActionResult> ActivateFarm([FromBody] CustomerType type)
        {

            await _unitOfWork.Customers.ActivateCustomerType(type);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Activate customer type!");
        }

        [HttpGet]
        [Route("GetAllCustomerTypeWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomerTypeWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var type = await _unitOfWork.Customers.GetAllCustomerTypeWithPagination(status, userParams);

            Response.AddPaginationHeader(type.CurrentPage, type.PageSize, type.TotalCount, type.TotalPages, type.HasNextPage, type.HasPreviousPage);

            var customerType = new
            {
                type,
                type.CurrentPage,
                type.PageSize,
                type.TotalCount,
                type.TotalPages,
                type.HasNextPage,
                type.HasPreviousPage
            };

            return Ok(customerType);
        }

        [HttpGet]
        [Route("GetAllCustomerTypeWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomerTypeWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllCustomerTypeWithPagination(status, userParams);

            var type = await _unitOfWork.Customers.GetCustomerTypeWithPaginationOrig(userParams, status, search);


            Response.AddPaginationHeader(type.CurrentPage, type.PageSize, type.TotalCount, type.TotalPages, type.HasNextPage, type.HasPreviousPage);

            var customerType = new
            {
                type,
                type.CurrentPage,
                type.PageSize,
                type.TotalCount,
                type.TotalPages,
                type.HasNextPage,
                type.HasPreviousPage
            };

            return Ok(customerType);
        }




    }
}
