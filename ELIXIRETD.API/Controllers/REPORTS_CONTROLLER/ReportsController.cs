using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIRETD.API.Controllers.REPORTS_CONTROLLER
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IUnitOfWork _unitofwork;

        public ReportsController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        [HttpGet]
        [Route("WareHouseReceivingReports")]
        public async Task<ActionResult<IEnumerable<DtoWarehouseReceivingReports>>> WarehouseReceiveController([FromQuery] UserParams userParams, [FromQuery] string DateFrom , [FromQuery] string DateTo , [FromQuery] string Search )
        {
            var inventory = await _unitofwork.Reports.WarehouseReceivingReports(userParams, DateFrom, DateTo , Search);

            Response.AddPaginationHeader(inventory.CurrentPage, inventory.PageSize, inventory.TotalCount, inventory.TotalPages, inventory.HasNextPage, inventory.HasPreviousPage);

            var inventoryResult = new
            {
                inventory,
                inventory.CurrentPage,
                inventory.PageSize,
                inventory.TotalCount,
                inventory.TotalPages,
                inventory.HasNextPage,
                inventory.HasPreviousPage
            };

            return Ok(inventoryResult);
        }

        [HttpGet]
        [Route("MoveOrderHistory")]
        public async Task<ActionResult<IEnumerable<DtoMoveOrderReports>>> MoveOrderHistory([FromQuery] UserParams userParams, [FromQuery] string DateFrom, [FromQuery] string DateTo, [FromQuery] string Search)
        {

            var inventory = await _unitofwork.Reports.WarehouseMoveOrderReports(userParams, DateFrom, DateTo, Search);

            Response.AddPaginationHeader(inventory.CurrentPage, inventory.PageSize, inventory.TotalCount, inventory.TotalPages, inventory.HasNextPage, inventory.HasPreviousPage);

            var inventoryResult = new
            {
                inventory,
                inventory.CurrentPage,
                inventory.PageSize,
                inventory.TotalCount,
                inventory.TotalPages,
                inventory.HasNextPage,
                inventory.HasPreviousPage
            };

            return Ok(inventoryResult);

        }


        [HttpGet]
        [Route("TransactionReport")]
        public async Task<ActionResult<IEnumerable<DtoMoveOrderReports>>> TransactionReports([FromQuery] UserParams userParams, [FromQuery] string DateFrom, [FromQuery] string DateTo,[FromQuery] string Search)
        {

            var inventory = await _unitofwork.Reports.TransactedMoveOrderReport(userParams, DateFrom, DateTo , Search);

            Response.AddPaginationHeader(inventory.CurrentPage, inventory.PageSize, inventory.TotalCount, inventory.TotalPages, inventory.HasNextPage, inventory.HasPreviousPage);

            var inventoryResult = new
            {
                inventory,
                inventory.CurrentPage,
                inventory.PageSize,
                inventory.TotalCount,
                inventory.TotalPages,
                inventory.HasNextPage,
                inventory.HasPreviousPage
            };

            return Ok(inventoryResult);

        }


        [HttpGet]
        [Route("MiscellaneousReceiptReport")]
        public async Task<ActionResult<IEnumerable<DtoMiscReports>>> MiscellaneousReceiptReport([FromQuery] UserParams userParams, [FromQuery] string DateFrom, [FromQuery] string DateTo, [FromQuery] string Search)
        {

            var inventory = await _unitofwork.Reports.MiscReports(userParams, DateFrom, DateTo, Search);

            Response.AddPaginationHeader(inventory.CurrentPage, inventory.PageSize, inventory.TotalCount, inventory.TotalPages, inventory.HasNextPage, inventory.HasPreviousPage);

            var inventoryResult = new
            {
                inventory,
                inventory.CurrentPage,
                inventory.PageSize,
                inventory.TotalCount,
                inventory.TotalPages,
                inventory.HasNextPage,
                inventory.HasPreviousPage
            };

            return Ok(inventoryResult);


        }


        [HttpGet]
        [Route("MiscellaneousIssueReport")]
        public async Task<ActionResult<IEnumerable<DtoMiscIssue>>> MiscellaneousIssueReport([FromQuery] UserParams userParams, [FromQuery] string DateFrom, [FromQuery] string DateTo, [FromQuery] string Search)
        {

            var inventory = await _unitofwork.Reports.MiscIssue(userParams, DateFrom, DateTo, Search);

            Response.AddPaginationHeader(inventory.CurrentPage, inventory.PageSize, inventory.TotalCount, inventory.TotalPages, inventory.HasNextPage, inventory.HasPreviousPage);

            var inventoryResult = new
            {
                inventory,
                inventory.CurrentPage,
                inventory.PageSize,
                inventory.TotalCount,
                inventory.TotalPages,
                inventory.HasNextPage,
                inventory.HasPreviousPage
            };

            return Ok(inventoryResult);
        }


        [HttpGet]
        [Route("BorrowedTransactionReports")]
        public async Task<ActionResult<IEnumerable<BorrowedTransactionReportsDto>>> BorrowedTransactionReports([FromQuery] UserParams userParams, [FromQuery] string DateFrom, [FromQuery] string DateTo, [FromQuery] string Search)
        {

                var inventory = await _unitofwork.Reports.BorrowedTransactionReports(userParams, DateFrom, DateTo, Search);

                Response.AddPaginationHeader(inventory.CurrentPage, inventory.PageSize, inventory.TotalCount, inventory.TotalPages, inventory.HasNextPage, inventory.HasPreviousPage);

                var inventoryResult = new
                {
                    inventory,
                    inventory.CurrentPage,
                    inventory.PageSize,
                    inventory.TotalCount,
                    inventory.TotalPages,
                    inventory.HasNextPage,
                    inventory.HasPreviousPage
                };


                return Ok(inventoryResult);
       

        }




        [HttpGet]
        [Route("BorrowedReturnedReports")]
        public async Task<ActionResult<IEnumerable<DtoBorrowedAndReturned>>> BorrowedReturnedReports([FromQuery] UserParams userParams, [FromQuery] string DateFrom, [FromQuery] string DateTo, [FromQuery] string Search)
        {

            var inventory = await _unitofwork.Reports.ReturnBorrowedReports(userParams, DateFrom, DateTo ,Search );

            Response.AddPaginationHeader(inventory.CurrentPage, inventory.PageSize, inventory.TotalCount, inventory.TotalPages, inventory.HasNextPage, inventory.HasPreviousPage);

            var inventoryResult = new
            {
                inventory,
                inventory.CurrentPage,
                inventory.PageSize,
                inventory.TotalCount,
                inventory.TotalPages,
                inventory.HasNextPage,
                inventory.HasPreviousPage
            };

            return Ok(inventoryResult);

        }


        [HttpGet]
        [Route("CancelledOrderedReports")]
        public async Task<ActionResult<IEnumerable<DtoCancelledReports>>> CancelledOrderedReports([FromQuery] UserParams userParams, [FromQuery] string DateFrom, [FromQuery] string DateTo, [FromQuery] string Search)
        {
            var inventory = await _unitofwork.Reports.CancelledReports(userParams, DateFrom, DateTo, Search);

            Response.AddPaginationHeader(inventory.CurrentPage, inventory.PageSize, inventory.TotalCount, inventory.TotalPages, inventory.HasNextPage, inventory.HasPreviousPage);

            var inventoryResult = new
            {
                inventory,
                inventory.CurrentPage,
                inventory.PageSize,
                inventory.TotalCount,
                inventory.TotalPages,
                inventory.HasNextPage,
                inventory.HasPreviousPage
            };

            return Ok(inventoryResult);

        }



        [HttpGet]
        [Route("InventoryMovementReport")]
        public async Task<ActionResult<IEnumerable<DtoInventoryMovement>>> InventoryMovementReport([FromQuery] UserParams userParams, [FromQuery] string DateFrom , [FromQuery] string PlusOne , [FromQuery]string Search)
        {

            var inventory = await _unitofwork.Reports.InventoryMovementReports(userParams, DateFrom, PlusOne , Search);

            Response.AddPaginationHeader(inventory.CurrentPage, inventory.PageSize, inventory.TotalCount, inventory.TotalPages, inventory.HasNextPage, inventory.HasPreviousPage);

            var inventoryResult = new
            {
                inventory,
                inventory.CurrentPage,
                inventory.PageSize,
                inventory.TotalCount,
                inventory.TotalPages,
                inventory.HasNextPage,
                inventory.HasPreviousPage
            };

            return Ok(inventoryResult);

        }







    }
}
