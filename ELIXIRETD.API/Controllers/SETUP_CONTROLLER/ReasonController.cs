using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIRETD.API.Controllers.SETUP_CONTROLLER
{
    public class ReasonController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReasonController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        [Route("GetAllActiveReasons")]
        public async Task<IActionResult> GetAllActiveReasons()
        {
            var reason = await _unitOfWork.Reasons.GetAllActiveReason();

            return Ok(reason);
        }

        [HttpGet]
        [Route("GetAllInActiveReasons")]
        public async Task<IActionResult> GetAllInActiveReasons()
        {
            var reason = await _unitOfWork.Reasons.GetAllActiveReason();

            return Ok(reason);
        }

        [HttpPost]
        [Route("AddNewReason")]
        public async Task<IActionResult> AddNewReason(Reason reason)
        {

            var moduleId = await _unitOfWork.Reasons.ValidateModuleId(reason.MainMenuId);

            var validate = await _unitOfWork.Reasons.ValidateReasonEntry(reason);

            if (validate == false)
                return BadRequest("Menu and reason already exist!");

            if (moduleId == false)
                return BadRequest("Module doesn't exist, Please add data first!");

            await _unitOfWork.Reasons.AddReason(reason);
            await _unitOfWork.CompleteAsync();

            return Ok(reason);
        }

        [HttpPut]
        [Route("UpdateReason")]
        public async Task<IActionResult> UpdateReason([FromBody] Reason reason)
        {
            await _unitOfWork.Reasons.UpdateReason(reason);
            await _unitOfWork.CompleteAsync();

            return Ok(reason);
        }


        [HttpPut]
        [Route("InActiveReason")]
        public async Task<IActionResult> InActiveReason([FromBody] Reason reason)
        {

            await _unitOfWork.Reasons.InActiveReason(reason);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully inactive reason!");
        }


        [HttpPut]
        [Route("ActivateReason")]
        public async Task<IActionResult> ActivateReason([FromBody] Reason reason)
        {
            await _unitOfWork.Reasons.ActivateReason(reason);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully activate reason!");
        }


        [HttpGet]
        [Route("GetAllReasonWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<ReasonDto>>> GetAllReasonWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var reason = await _unitOfWork.Reasons.GetAllReasonWithPagination(status, userParams);

            Response.AddPaginationHeader(reason.CurrentPage, reason.PageSize, reason.TotalCount, reason.TotalPages, reason.HasNextPage, reason.HasPreviousPage);

            var reasonResult = new
            {
                reason,
                reason.CurrentPage,
                reason.PageSize,
                reason.TotalCount,
                reason.TotalPages,
                reason.HasNextPage,
                reason.HasPreviousPage
            };

            return Ok(reasonResult);
        }

        [HttpGet]
        [Route("GetAllReasonWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<ReasonDto>>> GetAllReasonWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllReasonWithPagination(status, userParams);

            var reason = await _unitOfWork.Reasons.GetReasonWithPaginationOrig(userParams, status, search);


            Response.AddPaginationHeader(reason.CurrentPage, reason.PageSize, reason.TotalCount, reason.TotalPages, reason.HasNextPage, reason.HasPreviousPage);

            var reasonResult = new
            {
                reason,
                reason.CurrentPage,
                reason.PageSize,
                reason.TotalCount,
                reason.TotalPages,
                reason.HasNextPage,
                reason.HasPreviousPage
            };

            return Ok(reasonResult);
        }


    }
}
