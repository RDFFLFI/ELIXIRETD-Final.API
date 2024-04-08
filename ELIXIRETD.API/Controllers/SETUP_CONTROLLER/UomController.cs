using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;

namespace ELIXIRETD.API.Controllers.SETUP_CONTROLLER
{

    public class UomController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public UomController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("GetAllActiveUoms")]
        public async Task<IActionResult> GetAllActiveUoms()
        {
            var uom = await _unitOfWork.Uoms.GetAllActiveUoms();

            return Ok(uom);
        }


        [HttpGet]
        [Route("GetAllInActiveUoms")]
        public async Task<IActionResult> GetAllInActiveUoms()
        {
            var uom = await _unitOfWork.Uoms.GetAllInActiveUoms();

            return Ok(uom);
        }


        [HttpPost]
        [Route("AddNewUom")]
        public async Task<IActionResult> AddNewUom(Uom uoms)
        {

            if (await _unitOfWork.Uoms.UomCodeExist(uoms.UomCode))
                return BadRequest("Uom code already exist, Please try something else!");

            if (await _unitOfWork.Uoms.UomDescriptionExist(uoms.UomDescription))
                return BadRequest("Uom code description already exist, Please try something else!");

            await _unitOfWork.Uoms.AddNewUom(uoms);
            await _unitOfWork.CompleteAsync();

            return Ok(uoms);

        }


        [HttpPut]
        [Route("UpdateUom")]
        public async Task<IActionResult> UpdateUom([FromBody] Uom uom)
        {

            //var validate = await _unitOfWork.Uoms.validateItemUse(uom);
            //if (validate == true)
            //    return BadRequest("The uom cannot be changed because you entered the same uom!");

            if (await _unitOfWork.Uoms.ValidateUomInUse(uom.Id))
                return BadRequest("Uom is in use!");

            if (await _unitOfWork.Uoms.UomDescriptionExist(uom.UomDescription))
                return BadRequest("Uom code description already exist, Please try something else!");

            await _unitOfWork.Uoms.UpdateUom(uom);
            await _unitOfWork.CompleteAsync();

            return Ok(uom);

            //var updated = await _unitOfWork.Uoms.UpdateUom(uom);
            //if (!updated)
            //    return BadRequest("The uom cannot be updated because a uom with the same description already exists!");

            //await _unitOfWork.CompleteAsync();

            //return Ok(uom);

        }

        [HttpPut]
        [Route("InActiveUom")]
        public async Task<IActionResult> InActiveUom([FromBody] Uom uom)
        {

            if (await _unitOfWork.Uoms.ValidateUomInUse(uom.Id))
                return BadRequest("Uom is in use!");

            await _unitOfWork.Uoms.InActiveUom(uom);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully InActive UOM!");
        }

        [HttpPut]
        [Route("ActivateUom")]
        public async Task<IActionResult> ActivateUom([FromBody] Uom uom)
        {
            await _unitOfWork.Uoms.ActivateUom(uom);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Activate UOM!");
        }

        [HttpGet]
        [Route("GetAllUomWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<UomDto>>> GetAllUomWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var uom = await _unitOfWork.Uoms.GetAllUomWithPagination(status, userParams);

            Response.AddPaginationHeader(uom.CurrentPage, uom.PageSize, uom.TotalCount, uom.TotalPages, uom.HasNextPage, uom.HasPreviousPage);

            var uomResult = new
            {
                uom,
                uom.CurrentPage,
                uom.PageSize,
                uom.TotalCount,
                uom.TotalPages,
                uom.HasNextPage,
                uom.HasPreviousPage
            };

            return Ok(uomResult);
        }

        [HttpGet]
        [Route("GetAllUomWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<UomDto>>> GetAllUomWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllUomWithPagination(status, userParams);

            var uom = await _unitOfWork.Uoms.GetUomWithPaginationOrig (userParams, status, search);


            Response.AddPaginationHeader(uom.CurrentPage, uom.PageSize, uom.TotalCount, uom.TotalPages, uom.HasNextPage, uom.HasPreviousPage);

            var uomResult = new
            {
                uom,
                uom.CurrentPage,
                uom.PageSize,
                uom.TotalCount,
                uom.TotalPages,
                uom.HasNextPage,
                uom.HasPreviousPage
            };

            return Ok(uomResult);
        }



        [HttpGet]
        [Route("GetAllUom")]
        public async Task<IActionResult> GetAllUom()
        {
            var uom = await _unitOfWork.Uoms.GetAllUom();

            return Ok(uom);

        }



        //Sync Uom

        [HttpPut]
        [Route("SyncUom")]

        public async Task<IActionResult> SyncUom([FromBody] Uom[] uom)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult("Something went wrong!") { StatusCode = 500 };
            }

            List<Uom> duplicateList = new List<Uom>();
            List<Uom> availableImport = new List<Uom>();
            List<Uom> availableUpdate = new List<Uom>();
            List<Uom> uomDescriptionEmpty = new List<Uom>();
            List<Uom> uomCodeEmpty = new List<Uom>();

            foreach(Uom items in uom)
            {

                if (uom.Count(x => x.UomCode == items.UomCode && x.UomDescription == x.UomDescription) > 1)
                {
                    duplicateList.Add(items);
                }
                if (items.UomCode == string.Empty || items.UomCode == null)
                {
                    uomCodeEmpty.Add(items);
                    continue;

                }
                if (items.UomDescription == string.Empty || items.UomDescription == null)
                {
                    uomCodeEmpty.Add(items);
                    continue;

                }

                else
                {
                    var existingUom = await _unitOfWork.Uoms.GetByUomNo(items.UomNo);
                    if (existingUom != null)
                    {
                        bool hasChanged = false;

                        if (existingUom.UomCode != items.UomCode)
                        {

                            existingUom.UomCode = items.UomCode;
                            hasChanged = true;
                        }

                        if (existingUom.UomDescription != items.UomDescription)
                        {

                            existingUom.UomDescription = items.UomDescription;
                            hasChanged = true;
                        }

                        if (hasChanged)
                        {
                            existingUom.IsActive = items.IsActive;
                            existingUom.ModifyBy = User.Identity.Name;
                            existingUom.ModifyDate = DateTime.Now;
                            existingUom.StatusSync = "New update";
                            existingUom.SyncDate = DateTime.Now;

                            availableUpdate.Add(existingUom);
                            await _unitOfWork.Uoms.UpdateSyncUom(existingUom);
                        }

                        if (!hasChanged)
                        {
                            existingUom.SyncDate = DateTime.Now;
                            existingUom.StatusSync = "No new update";
                           
                        }



                    }
                    else
                    {

                        items.StatusSync = "New Added";
                        availableImport.Add(items);
                        await _unitOfWork.Uoms.AddNewUom(items);


                    }

                }



            }

            var resultlist = new
            {
                AvailableImport = availableImport,
                AvailableUpdate = availableUpdate,
                DuplicateList = duplicateList,
                UomDescriptionEmpty = uomDescriptionEmpty,
                UomCodeEmpty = uomCodeEmpty,
               


            };

            if (duplicateList.Count == 0 && uomDescriptionEmpty.Count == 0 && uomCodeEmpty.Count == 0)
            {
                await _unitOfWork.CompleteAsync();
                return Ok("Successfully updated and added!");
            }
            else
            {
                return BadRequest(resultlist);
            }



        }



    }
}
