using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;

namespace ELIXIRETD.API.Controllers.IMPORT_CONTROLLER
{

    public class ImportController : BaseApiController
    {
        private IUnitOfWork _unitOfWork;
        private readonly StoreContext _context;

        public ImportController(IUnitOfWork unitOfWork , StoreContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
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
                List<PoSummary> uomCodeNotExist = new List<PoSummary>();
                List<PoSummary> quantityInValid = new List<PoSummary>();
                List<PoSummary> itemcodeanduomNotExist = new List<PoSummary>();
              




                foreach (PoSummary items in posummary)
                {
                    var uomExist = await _context.Uoms.FirstOrDefaultAsync(x => x.UomDescription == items.Uom);

                    if (uomExist is null) 
                    {
                        uomCodeNotExist.Add(items);
                        continue;
                    }
                    items.Uom = uomExist.UomCode;

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
                        var validateItemCode = await _unitOfWork.Imports.CheckItemCode(items.ItemCode);
                        var validatePoandItem = await _unitOfWork.Imports.ValidatePOAndItemcodeManual(items.PO_Number, items.ItemCode);
                        var validateUom = await _unitOfWork.Imports.CheckUomCode(items.Uom);
                        var validateQuantity = await _unitOfWork.Imports.ValidateQuantityOrder(items.Ordered);
                        var validateItemcodeAndUom = await _unitOfWork.Imports.ValidationItemcodeandUom(items.ItemCode /*, items.ItemDescription */, items.Uom);


                        if (validatePoandItem == true)
                        {
                            duplicateList.Add(items);
                        }

                        else if (validateSupplier == false)
                        {
                            supplierNotExist.Add(items);
                        }

                        else if (validateUom == false)
                        {
                            uomCodeNotExist.Add(items);
                        }

                        else if (validateItemCode == false)
                        {
                            itemcodeNotExist.Add(items);
                        }
                        else if (validateItemcodeAndUom == false)
                        {
                        itemcodeanduomNotExist.Add(items);
                        }
                        else if(validateQuantity == false)
                             quantityInValid.Add(items);

                        else
                        {
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
                    uomCodeNotExist,
                    quantityInValid,
                    itemcodeanduomNotExist,
                    //unitPriceInvalid
                };

                if (duplicateList.Count == 0 && supplierNotExist.Count == 0 && itemcodeNotExist.Count == 0 && uomCodeNotExist.Count == 0 && quantityInValid.Count == 0 && itemcodeanduomNotExist.Count == 0 /*&& unitPriceInvalid.Count == 0*/)
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





    }

}
