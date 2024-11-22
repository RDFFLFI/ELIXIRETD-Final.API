using DocumentFormat.OpenXml.Spreadsheet;
using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.IMPORT_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.IMPORT_REPOSITORY;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.IMPORT_REPOSITORY.ExportMaterial;
using static ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORTS_REPOSITORY.MoveOrderReportExport;

namespace ELIXIRETD.API.Controllers.IMPORT_CONTROLLER
{

    public class ImportController : BaseApiController
    {
        private IUnitOfWork _unitOfWork;
        private readonly StoreContext _context;
        private readonly IMediator _mediator;

        public ImportController(IUnitOfWork unitOfWork, StoreContext context , IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mediator = mediator;
        }


        [HttpPost("import-manual")]
        public async Task<IActionResult> ManualImport([FromBody] PoSummary[] posummary)
        {
            if (!ModelState.IsValid)
                return new JsonResult("Something went Wrong!") { StatusCode = StatusCodes.Status500InternalServerError };
            {

                List<PoSummary> duplicateList = new List<PoSummary>();
                List<PoSummary> availableImport = new List<PoSummary>();
                List<PoSummary> supplierNotExist = new List<PoSummary>();
                List<PoSummary> itemcodeNotExist = new List<PoSummary>();
                List<PoSummary> quantityInValid = new List<PoSummary>();

                foreach (PoSummary items in posummary)
                {

                    if (items.Ordered <= 0)
                    {
                        quantityInValid.Add(items);
                    }

                    else if (posummary.Count(x => x.PO_Number == items.PO_Number && x.ItemCode == items.ItemCode) > 1)
                    {
                        duplicateList.Add(items);
                        continue;
                    }

                    var validateSupplier = await _unitOfWork.Imports.CheckSupplier(items.VendorName);
                    var validateItemCode = await _context.Materials
                        .Include(x => x.Uom)
                        .FirstOrDefaultAsync(x => x.ItemCode == items.ItemCode && x.IsActive);
                    var validatePoandItem = await _unitOfWork.Imports.ValidatePOAndItemcodeManual(items.PO_Number, items.ItemCode);
                    var validateQuantity = await _unitOfWork.Imports.ValidateQuantityOrder(items.Ordered);
                    var validateItemcodeAndUom = await _unitOfWork.Imports.ValidationItemcodeandUom(items.ItemCode, items.Uom);

                    if (validatePoandItem == true)
                    {
                        duplicateList.Add(items);
                    }

                    else if (validateSupplier == false)
                    {
                        supplierNotExist.Add(items);
                    }

                    else if (validateItemCode is null)
                    {
                        itemcodeNotExist.Add(items);
                    }
                    else if (validateQuantity == false)
                        quantityInValid.Add(items);

                    else
                    {
                        items.ItemDescription = validateItemCode.ItemDescription;
                        items.Uom = validateItemCode.Uom.UomCode;
                        availableImport.Add(items);
                        await _unitOfWork.Imports.AddNewPORequest(items);
                    }
                }

                var resultList = new
                {
                    availableImport,
                    duplicateList,
                    supplierNotExist,
                    itemcodeNotExist,
                    quantityInValid,
                };

                if (duplicateList.Count == 0 && supplierNotExist.Count == 0 && itemcodeNotExist.Count == 0 && quantityInValid.Count == 0  /*&& unitPriceInvalid.Count == 0*/)
                {
                    await _unitOfWork.CompleteAsync();
                    return Ok("Successfully Add!");
                }

                else
                {
                    return BadRequest(resultList);
                }

            }
        }


        [HttpPost]
        [Route("AddNewPOSummary")]
        public async Task<IActionResult> AddNewPo([FromBody] PoSummary[] posummary)
        {

            if (!ModelState.IsValid)
                return new JsonResult("Something went Wrong!") { StatusCode = StatusCodes.Status500InternalServerError };
            {

                List<PoSummary> duplicateList = new List<PoSummary>();
                List<PoSummary> availableImport = new List<PoSummary>();
                List<PoSummary> supplierNotExist = new List<PoSummary>();
                List<PoSummary> itemcodeNotExist = new List<PoSummary>();
                List<PoSummary> quantityInValid = new List<PoSummary>();

                foreach (PoSummary items in posummary)
                {

                    if (items.Ordered <= 0)
                    {
                        quantityInValid.Add(items);
                    }

                    else if (posummary.Count(x => x.PO_Number == items.PO_Number && x.RRNo == items.RRNo && x.ItemCode == items.ItemCode) > 1)
                    {
                        duplicateList.Add(items);
                        continue;
                    }

                    var validateSupplier = await _unitOfWork.Imports.CheckSupplier(items.VendorName);
                    var validateItemCode = await _context.Materials
                        .Include(x => x.Uom)
                        .FirstOrDefaultAsync(x => x.ItemCode == items.ItemCode && x.IsActive);
                    var validatePoandItem = await _unitOfWork.Imports.ValidateRRAndItemcodeManual(items.PO_Number,items.RRNo, items.ItemCode);
                    var validateQuantity = await _unitOfWork.Imports.ValidateQuantityOrder(items.Ordered);
                    var validateItemcodeAndUom = await _unitOfWork.Imports.ValidationItemcodeandUom(items.ItemCode, items.Uom);

                    if (validatePoandItem == true)
                    {
                        duplicateList.Add(items);
                    }

                    else if (validateSupplier == false)
                    {
                        supplierNotExist.Add(items);
                    }

                    else if (validateItemCode is null)
                    {
                        itemcodeNotExist.Add(items);
                    }
                    else if (validateQuantity == false)
                        quantityInValid.Add(items);

                    else
                    {
                        items.ItemDescription = validateItemCode.ItemDescription;
                        items.Uom = validateItemCode.Uom.UomCode;
                        availableImport.Add(items);
                        await _unitOfWork.Imports.AddNewPORequest(items);
                    }
                }

                var resultList = new
                {
                    availableImport,
                    duplicateList,
                    supplierNotExist,
                    itemcodeNotExist,
                    quantityInValid,
                };

                if (duplicateList.Count == 0 && supplierNotExist.Count == 0 && itemcodeNotExist.Count == 0 && quantityInValid.Count == 0  /*&& unitPriceInvalid.Count == 0*/)
                {
                    await _unitOfWork.CompleteAsync();
                    return Ok("Successfully Add!");
                }

                else
                {
                    return BadRequest(resultList);
                }
            }

        }


        [HttpPost("ImportBufferLevel")]
        public async Task<IActionResult> ImportBufferLevel([FromBody] ImportBufferLevelDto[] itemCode)
        {

            var availableItemList = new List<ImportBufferLevelDto>();
            var duplicateList = new List<ImportBufferLevelDto>();
            var itemNotExist = new List<ImportBufferLevelDto>();


            foreach (var item in itemCode)
            {

                if (itemCode.Count(x => x.ItemCode == item.ItemCode) > 1)
                {
                    duplicateList.Add(item);
                    continue;
                }
                else
                {
                    var itemCodeNotExist = await _unitOfWork.Imports.CheckItemCode(item.ItemCode);


                    if (itemCodeNotExist == false)
                    {
                        itemNotExist.Add(item);
                    }
                    else
                    {
                        availableItemList.Add(item);
                        await _unitOfWork.Imports.ImportBufferLevel(item);

                    }

                }

            }

            var resultList = new
            {
                availableItemList,
                duplicateList,
                itemNotExist,

            };

            if (!resultList.duplicateList.Any() && !resultList.itemNotExist.Any())
            {
                await _unitOfWork.CompleteAsync();
                return Ok("Success");

            }
            else
            {
                return BadRequest(resultList);
            }

        }


        [HttpGet("WarehouseOverAllStocks")]
        public async Task<IActionResult> WarehouseOverAllStocks([FromQuery] string Search)
        {
            var warehouseStocks = await _unitOfWork.Imports.WarehouseOverAllStocks(Search);

            return Ok(warehouseStocks);
        }

        [HttpGet("ExportMaterial")]
        public async Task<IActionResult> ExportMaterial([FromQuery] ExportMaterialCommand command)
        {
            var filePath = $"MaterialList.xlsx";

            try
            {
                await _mediator.Send(command);
                var memory = new MemoryStream();
                await using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                var result = File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    filePath);
                System.IO.File.Delete(filePath);
                return result;

            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }

        }

        [HttpPost("ImportMiscReceipt")]
        public async Task<IActionResult> ImportMiscReceipt([FromBody]ImportMiscReceiptDto items)
        {
            var availableList = new List<ImportMiscReceiptDto.WarehouseReceiptDto>();
            var itemCodeNotExist = new List<ImportMiscReceiptDto.WarehouseReceiptDto>();
            var uomNotExist = new List<ImportMiscReceiptDto.WarehouseReceiptDto>();
            //var unitCostEmpty = new List<ImportMiscReceiptDto.WarehouseReceiptDto>();

            var supplierCodeExist = await _context.Suppliers
                .FirstOrDefaultAsync(x => x.IsActive == true && x.SupplierCode == items.SupplierCode);

            if (supplierCodeExist is null)
            {
                return BadRequest("Supplier not Exist");
            }

            var addMiscReceipt = new MiscellaneousReceipt
            {
                SupplierCode = items.SupplierCode,
                supplier = items.SupplierName,
                TotalQuantity = items.WarehouseReceipt.Sum(x => decimal.Parse(x.Quantity)),
                PreparedDate = DateTime.Now,
                PreparedBy = items.PreparedBy,
                Remarks = items.Remarks,
                IsActive = true,
                TransactionDate = DateTime.Now,
                CompanyCode = items.CompanyCode,
                CompanyName = items.CompanyName,
                DepartmentCode = items.DepartmentCode,
                DepartmentName = items.DepartmentName,
                LocationCode = items.LocationCode,
                LocationName = items.LocationName,
                Details = items.Details,
            };
            
            await _context.MiscellaneousReceipts.AddAsync(addMiscReceipt);
            await _context.SaveChangesAsync();

            
            foreach(var item in items.WarehouseReceipt)
            {

                var uomExist = await _context.Uoms.FirstOrDefaultAsync(x => x.UomCode == item.Uom);

                if (uomExist is null)
                {
                    uomNotExist.Add(item);
                }

                var itemCodeExist = await _context.Materials
                    .Where(x => x.ItemCode == item.ItemCode)
                    .FirstOrDefaultAsync();    

                if (itemCodeExist is null)
                {
                    itemCodeNotExist.Add(item);
                }
                else
                {
                    item.MiscellaneousReceiptId = addMiscReceipt.Id;
                    item.ItemDescription = itemCodeExist.ItemDescription;
                    item.SupplierName = addMiscReceipt.supplier;
                    availableList.Add(item);
                    await _unitOfWork.Imports.AddImportReceiptToWarehouse(item);
                }
            }

            var resultList = new
            {
                availableList,
                itemCodeNotExist,
                uomNotExist,

            };

            if (!resultList.itemCodeNotExist.Any() && !resultList.uomNotExist.Any())
            {
                await _unitOfWork.CompleteAsync();
                return Ok("Success Misc");

            }
            else
            {
                return BadRequest(resultList);
            }

        }

    }

}
