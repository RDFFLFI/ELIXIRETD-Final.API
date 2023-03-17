using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;

namespace ELIXIRETD.API.Controllers.SETUP_CONTROLLER
{

    public class SupplierController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public SupplierController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("GetAllActiveSupplier")]
        public async Task<IActionResult> GetAllActiveSupplier()
        {
            var supplier = await _unitOfWork.Suppliers.GetAllActiveSupplier();

            return Ok(supplier);
        }


        [HttpGet]
        [Route("GetAllInActiveSupplier")]
        public async Task<IActionResult> GetAllInActiveSupplier()
        {
            var supplier = await _unitOfWork.Suppliers.GetAllInActiveSupplier();

            return Ok(supplier);
        }

        [HttpPost]
        [Route("AddNewSupplier")]
        public async Task<IActionResult> AddNewSupplier(Supplier supplier)
        {
            if (await _unitOfWork.Suppliers.SupplierCodeExist(supplier.SupplierCode))
                return BadRequest("Supplier code already exist, please try something else!");

            var validation = await _unitOfWork.Suppliers.ValidationDescritandAddress(supplier);

            if (validation == true)
                return BadRequest("Supplier Name and Address was already exist");


            await _unitOfWork.Suppliers.AddSupplier(supplier);
            await _unitOfWork.CompleteAsync();

            return Ok(supplier);
        }


        [HttpPut]
        [Route("UpdateSupplier")]
        public async Task<IActionResult> UpdateSupplier([FromBody] Supplier supplier)
        {
            var validation = await _unitOfWork.Suppliers.ValidationDescritandAddress(supplier);

            if (validation == true)
                return BadRequest("Supplier Name and Address was already exist");


            await _unitOfWork.Suppliers.UpdateSupplier(supplier);
            await _unitOfWork.CompleteAsync();

            return Ok(supplier);
        }

        [HttpPut]
        [Route("InActiveSupplier")]
        public async Task<IActionResult> InActiveSupplier([FromBody] Supplier supplier)
        {

            await _unitOfWork.Suppliers.InActiveSupplier(supplier);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully inactive Supplier!");
        }

        [HttpPut]
        [Route("ActivateSupplier")]
        public async Task<IActionResult> ActivateSupplier([FromBody] Supplier supplier)
        {
            await _unitOfWork.Suppliers.ActivateSupplier(supplier);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully activate supplier!");
        }

        [HttpGet]
        [Route("GetAllSupplierithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<UomDto>>> GetAllSupplierithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var supplier = await _unitOfWork.Suppliers.GetAllSupplierWithPagination(status, userParams);

            Response.AddPaginationHeader(supplier.CurrentPage, supplier.PageSize, supplier.TotalCount, supplier.TotalPages, supplier.HasNextPage, supplier.HasPreviousPage);

            var supplierResult = new
            {
                supplier,
                supplier.CurrentPage,
                supplier.PageSize,
                supplier.TotalCount,
                supplier.TotalPages,
                supplier.HasNextPage,
                supplier.HasPreviousPage
            };

            return Ok(supplierResult);
        }

        [HttpGet]
        [Route("GetAllSupplierithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<UomDto>>> GetAllSupplierithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {
            if (search == null)
                return await GetAllSupplierithPagination(status, userParams);


            var supplier = await _unitOfWork.Suppliers.GetSupplierWithPaginationOrig(userParams, status, search);

            Response.AddPaginationHeader(supplier.CurrentPage, supplier.PageSize, supplier.TotalCount, supplier.TotalPages, supplier.HasNextPage, supplier.HasPreviousPage);

            var supplierResult = new
            {
                supplier,
                supplier.CurrentPage,
                supplier.PageSize,
                supplier.TotalCount,
                supplier.TotalPages,
                supplier.HasNextPage,
                supplier.HasPreviousPage
            };

            return Ok(supplierResult);
        }


    }
}
