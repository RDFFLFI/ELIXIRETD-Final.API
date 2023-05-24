using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

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

                if (supplier.Count(x => x.SupplierCode == items.SupplierCode && x.SupplierName == items.SupplierName/* && x.SupplierAddress == items.SupplierAddress*/) > 1)
                {

                    duplicateList.Add(items);

                }

                else
                {
                    var existingSuppliers = await _unitOfWork.Suppliers.GetBySupplierNo(items.Supplier_No);

                    if (existingSuppliers != null)
                    {

                        bool hasChanged = false;

                        if (existingSuppliers.SupplierCode != items.SupplierCode)
                        {
                            existingSuppliers.SupplierCode = items.SupplierCode;
                            hasChanged = true;
                        }

                        if (existingSuppliers.SupplierName != items.SupplierName)
                        {
                            existingSuppliers.SupplierName = items.SupplierName;
                            hasChanged = true;
                        }

                        if(hasChanged)
                        {
                            existingSuppliers.SupplierCode = items.SupplierCode;
                            existingSuppliers.SupplierName = items.SupplierName;

                            existingSuppliers.SupplierName = DateTime.Now;
                            //existingSuppliers.SupplierAddress = items.SupplierAddress;
                            //existingSuppliers.AddedBy = items.AddedBy;
                            existingSuppliers.IsActive = items.IsActive;
                            existingSuppliers.ModifyBy = items.ModifyBy;
                            existingSuppliers.ModifyDate = DateTime.Now;
                            //existingSuppliers.DateAdded = items.DateAdded;

                            await _unitOfWork.Suppliers.Update(existingSuppliers);
                        }

                    }
                    else 
                    {
                        
                        existingSuppliers.DateAdded = DateTime.Now;
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
                return Ok("Successfully updated and added!");
            }
            else
            {

                return BadRequest(resultlist);
            }

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
