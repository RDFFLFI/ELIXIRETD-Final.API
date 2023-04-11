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

        [HttpPut]
        [Route("AddNewSupplier")]
        public async Task<IActionResult> AddNewSupplier([FromBody] Supplier[] supplier)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult("Something went wrong!") { StatusCode = 500 };
            }

            List<Supplier> duplicateList = new List<Supplier>();
            List<Supplier> availableImport = new List<Supplier>();

            foreach (Supplier items in supplier)
            {

                if (supplier.Count(x => x.SupplierCode == items.SupplierCode && x.SupplierName == items.SupplierName && x.SupplierAddress == items.SupplierAddress) > 1)
                {

                    duplicateList.Add(items);
                }

                else
                {

                    var existingSuppliers = await _unitOfWork.Suppliers.GetById(items.Supplier_No);

                    if (existingSuppliers != null)
                    {

                        existingSuppliers.SupplierCode = items.SupplierCode;
                        existingSuppliers.SupplierName = items.SupplierName;
                        existingSuppliers.SupplierAddress = items.SupplierAddress;
                        existingSuppliers.AddedBy = items.AddedBy;
                        existingSuppliers.IsActive = items.IsActive;
                        existingSuppliers.DateAdded = items.DateAdded;


                        await _unitOfWork.Suppliers.Update(existingSuppliers);
                    }
                    else if (await _unitOfWork.Suppliers.GetBySupplierNo(items.Supplier_No) == null)
                    {

                        availableImport.Add(items);
                        await _unitOfWork.Suppliers.AddSupplier(items);
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
