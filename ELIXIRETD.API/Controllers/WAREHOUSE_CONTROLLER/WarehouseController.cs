using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.IMPORT_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.WAREHOUSE_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;

namespace ELIXIRETD.API.Controllers.WAREHOUSE_CONTROLLER
{

    public class WarehouseController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public WarehouseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("GetAllAvailablePoWithPagination")]
        public async Task<ActionResult<IEnumerable<PoSummaryDto>>> GetAllPoWithPagination([FromQuery] UserParams userParams)
        {
            var posummary = await _unitOfWork.Receives.GetAllPoSummaryWithPagination(userParams);

            Response.AddPaginationHeader(posummary.CurrentPage, posummary.PageSize, posummary.TotalCount, posummary.TotalPages, posummary.HasNextPage, posummary.HasPreviousPage);

            var posummaryResult = new
            {
                posummary,
                posummary.CurrentPage,
                posummary.PageSize,
                posummary.TotalCount,
                posummary.TotalPages,
                posummary.HasNextPage,
                posummary.HasPreviousPage
            };

            return Ok(posummaryResult);
        }


        [HttpPut]
        [Route("ReceiveRawMaterialsById")]
        public async Task<IActionResult> UpdateReceiveInfo([FromBody] Warehouse_Receiving receiving)
        {


            if (receiving.ActualDelivered <= 0)
                return BadRequest("Received failed, please check your input in actual delivered!");

            if (receiving.TotalReject != 0)
                receiving.ActualDelivered = receiving.ActualDelivered - receiving.TotalReject;

            var validatePoId = await _unitOfWork.Receives.ValidatePoId(receiving.PoSummaryId);

            if (validatePoId == false)
                return BadRequest("Update failed, PO does not exist!");

            var validateActualgood = await _unitOfWork.Receives.ValidateActualRemaining(receiving);

            if (validateActualgood == false)
                return BadRequest("Receive failed, You're trying to input greater than the total received!");


            await _unitOfWork.Receives.AddNewReceivingDetails(receiving);
            await _unitOfWork.CompleteAsync();

            return Ok(receiving);
        }



        [HttpGet]
        [Route("GetAllAvailablePoWithPaginationOrig")]
        public async Task<ActionResult<IEnumerable<PoSummaryDto>>> GetAllAvailablePoWithPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllPoWithPagination(userParams);

            var posummary = await _unitOfWork.Receives.GetPoSummaryByStatusWithPaginationOrig(userParams, search);

            Response.AddPaginationHeader(posummary.CurrentPage, posummary.PageSize, posummary.TotalCount, posummary.TotalPages, posummary.HasNextPage, posummary.HasPreviousPage);

            var posummaryResult = new
            {
                posummary,
                posummary.CurrentPage,
                posummary.PageSize,
                posummary.TotalCount,
                posummary.TotalPages,
                posummary.HasNextPage,
                posummary.HasPreviousPage
            };

            return Ok(posummaryResult);
        }


        [HttpPut]
        [Route("CancelPO")]
        public async Task<IActionResult> CancelPO([FromBody] PoSummary summary)
        {

            var validate = await _unitOfWork.Receives.CancelPo(summary);

            if (validate == false)
                return BadRequest("Cancel failed, you have materials for receiving in warehouse!");

            await _unitOfWork.Receives.CancelPo(summary);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Cancelled PO!");
        }

        [HttpGet]
        [Route("GetAllCancelledPoWithPagination")]
        public async Task<ActionResult<IEnumerable<WarehouseReceivingDto>>> GetAllCancelledPoWithPagination([FromQuery] UserParams userParams)
        {
            var cancel = await _unitOfWork.Receives.GetAllCancelledPOWithPagination(userParams);

            Response.AddPaginationHeader(cancel.CurrentPage, cancel.PageSize, cancel.TotalCount, cancel.TotalPages, cancel.HasNextPage, cancel.HasPreviousPage);

            var warehouseResult = new
            {
                cancel,
                cancel.CurrentPage,
                cancel.PageSize,
                cancel.TotalCount,
                cancel.TotalPages,
                cancel.HasNextPage,
                cancel.HasPreviousPage
            };

            return Ok(warehouseResult);
        }

        [HttpGet]
        [Route("GetAllCancelledPoWithPaginationOrig")]
        public async Task<ActionResult<IEnumerable<WarehouseReceivingDto>>> GetAllCancelledPoWithPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllCancelledPoWithPagination(userParams);

            var cancel = await _unitOfWork.Receives.GetAllCancelledPOWithPaginationOrig(userParams, search);

            Response.AddPaginationHeader(cancel.CurrentPage, cancel.PageSize, cancel.TotalCount, cancel.TotalPages, cancel.HasNextPage, cancel.HasPreviousPage);

            var warehouseResult = new
            {
                cancel,
                cancel.CurrentPage,
                cancel.PageSize,
                cancel.TotalCount,
                cancel.TotalPages,
                cancel.HasNextPage,
                cancel.HasPreviousPage
            };

            return Ok(warehouseResult);
        }


        [HttpPut]
        [Route("ReturnPoInAvailableList")]
        public async Task<IActionResult> ReturnPoInAvailableList([FromBody] PoSummary summary)
        {
            await _unitOfWork.Receives.ReturnPoInAvailableList(summary);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Returned PO!");
        }

        [HttpGet]
        [Route("GetAllReceivedMaterialsPagination")]
        public async Task<ActionResult<IEnumerable<WarehouseReceivingDto>>> GetAllReceivedMaterialsPagination([FromQuery] UserParams userParams)
        {
            var warehouse = await _unitOfWork.Receives.ListOfWarehouseReceivingIdWithPagination(userParams);

            Response.AddPaginationHeader(warehouse.CurrentPage, warehouse.PageSize, warehouse.TotalCount, warehouse.TotalPages, warehouse.HasNextPage, warehouse.HasPreviousPage);

            var warehouseResult = new
            {
                warehouse,
                warehouse.CurrentPage,
                warehouse.PageSize,
                warehouse.TotalCount,
                warehouse.TotalPages,
                warehouse.HasNextPage,
                warehouse.HasPreviousPage
            };

            return Ok(warehouseResult);
        }

        [HttpGet]
        [Route("GetAllReceivedMaterialsPaginationOrig")]
        public async Task<ActionResult<IEnumerable<WarehouseReceivingDto>>> GetAllReceivedMaterialsPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllReceivedMaterialsPagination(userParams);

            var warehouse = await _unitOfWork.Receives.ListOfWarehouseReceivingIdWithPaginationOrig(userParams, search);

            Response.AddPaginationHeader(warehouse.CurrentPage, warehouse.PageSize, warehouse.TotalCount, warehouse.TotalPages, warehouse.HasNextPage, warehouse.HasPreviousPage);

            var warehouseResult = new
            {
                warehouse,
                warehouse.CurrentPage,
                warehouse.PageSize,
                warehouse.TotalCount,
                warehouse.TotalPages,
                warehouse.HasNextPage,
                warehouse.HasPreviousPage
            };

            return Ok(warehouseResult);
        }



        // ADDITIONAL





        [HttpGet]
        [Route("GetAllListOfWarehouseReceivingIdNull")]
        public async Task<IActionResult> GetAllListOfWarehouseReceivingIdNull()
        {
            var warehouse = await _unitOfWork.Receives.ListOfWarehouseReceivingId();

            return Ok(warehouse);
        }

        [HttpGet]
        [Route("GetAllListOfWarehouseReceivingId")]
        public async Task<IActionResult> GetAllListOfWarehouseReceivingId([FromQuery] string search)
        {

            if (search == null)

                return await GetAllListOfWarehouseReceivingIdNull();


            var warehouse = await _unitOfWork.Receives.ListOfWarehouseReceivingId(search);

            return Ok(warehouse);
        }






    }
}
