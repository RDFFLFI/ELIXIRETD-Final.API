using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                return BadRequest("Uom code already exist, please try something else!");

            if (await _unitOfWork.Uoms.UomDescriptionExist(uoms.UomDescription))
                return BadRequest("Uom code description already exist, please try something else!");

            await _unitOfWork.Uoms.AddNewUom(uoms);
            await _unitOfWork.CompleteAsync();

            return Ok(uoms);

        }


        [HttpPut]
        [Route("UpdateUom")]
        public async Task<IActionResult> UpdateUom( [FromBody] Uom uom)
        {
     
            await _unitOfWork.Uoms.UpdateUom(uom);
            await _unitOfWork.CompleteAsync();

            return Ok(uom);
        }

        [HttpPut]
        [Route("InActiveUom")]
        public async Task<IActionResult> InActiveUom([FromBody] Uom uom)
        {
        
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



    }
}
