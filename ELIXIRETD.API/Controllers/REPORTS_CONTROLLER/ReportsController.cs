using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO.ConsolidationDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.SERVICES;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using static ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORTS_REPOSITORY.ConsolidateAuditExport;
using static ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORTS_REPOSITORY.ConsolidateFinanceExport;
using static ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORTS_REPOSITORY.GeneralLedgerExport;
using static ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORTS_REPOSITORY.MoveOrderReportExport;

namespace ELIXIRETD.API.Controllers.REPORTS_CONTROLLER
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly IMediator _mediator;

        public ReportsController(IUnitOfWork unitofwork , IMediator mediator)
        {
            _unitofwork = unitofwork;
            _mediator = mediator;
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
        public async Task<ActionResult<IEnumerable<DtoForTransactedReports>>> MoveOrderHistory([FromQuery] UserParams userParams, [FromQuery] string DateFrom, [FromQuery] string DateTo, [FromQuery] string Search)
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
        public async Task<ActionResult<IEnumerable<DtoForTransactedReports>>> TransactionReports([FromQuery] UserParams userParams, [FromQuery] string DateFrom, [FromQuery] string DateTo,[FromQuery] string Search)
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
        [Route("MoveOrderReport")]
        public async Task<IActionResult> MoveOrderReport([FromQuery] string DateFrom, [FromQuery] string DateTo, [FromQuery] string Search)
        {

            var inventory = await _unitofwork.Reports.MoveOrderReport( DateFrom, DateTo, Search);
            return Ok(inventory);

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
        [Route("FuelRegisterReports")]
        public async Task<ActionResult<IEnumerable<FuelRegisterReportsDto>>> FuelRegisterReports([FromQuery] UserParams userParams, [FromQuery] string DateFrom, [FromQuery] string DateTo, [FromQuery] string Search)
        {

            var inventory = await _unitofwork.Reports.FuelRegisterReports (userParams, DateFrom, DateTo, Search);

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



        [HttpGet]
        [Route("ConsolidationFinanceReports")]
        public async Task<IActionResult> ConsolidationFinanceReports([FromQuery] string DateFrom, [FromQuery] string DateTo, [FromQuery] string Search)
        {
            var reports = await _unitofwork.Reports.ConsolidateFinanceReport(DateFrom,DateTo,Search);

            return Ok(reports);
        }


        [HttpGet("ExportConsolidateFinance")]
        public async Task<IActionResult> ExportConsolidateFinance([FromQuery] ConsolidateFinanceExportCommand command)
        {
            var filePath = $"ConsolidatedReports {command.DateFrom} - {command.DateTo}.xlsx";

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

        [HttpGet]
        [Route("ConsolidateAuditReport")]
        public async Task<IActionResult> ConsolidateAuditReport([FromQuery] string DateFrom, [FromQuery] string DateTo, [FromQuery] string Search)
        {
            var reports = await _unitofwork.Reports.ConsolidateAuditReport(DateFrom, DateTo, Search);

            return Ok(reports);
        }

        [HttpGet("ConsolidateAuditExport")]
        public async Task<IActionResult> ConsolidateAuditExport([FromQuery] ConsolidateAuditExportCommand command)
        {
            var filePath = $"ConsolidatedAuditReports {command.DateFrom} - {command.DateTo}.xlsx";

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


        [HttpGet("ExportMoveOrderReports")]
        public async Task<IActionResult> ExportMoveOrderReports([FromQuery] MoveOrderReportExportCommand command)
        {
            var filePath = $"MoveOrderReports {command.DateFrom} - {command.DateTo}.xlsx";

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


        [HttpGet("GeneralLedgerExport")]
        public async Task<IActionResult> GeneralLedgerExport([FromQuery] GeneralLedgerExportCommand command)
        {
            var filePath = $"GeneralLedgerReports {DateTime.Today.Date.ToString("MM/dd/yyyy")}.xlsx";

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


        [HttpGet]
        [Route("GeneralLedgerReport")]
        public async Task<IActionResult> GeneralLedgerReport([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {
            var reports = await _unitofwork.Reports.GeneralLedgerReport(DateFrom, DateTo);

            return Ok(reports);
        }



    }
}
