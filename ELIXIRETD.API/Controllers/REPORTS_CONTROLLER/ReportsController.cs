using ELIXIRETD.DATA.CORE.ICONFIGURATION;
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
        public async Task<IActionResult> WarehouseReceiveController([FromQuery] string DateFrom , [FromQuery] string DateTo)
        {
            var orders = await _unitofwork.Reports.WarehouseReceivingReports(DateFrom, DateTo);

            return Ok(orders);
        }

        [HttpGet]
        [Route("MoveOrderHistory")]
        public async Task<IActionResult> MoveOrderHistory([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {

            var orders = await _unitofwork.Reports.WarehouseMoveOrderReports(DateFrom, DateTo);

            return Ok(orders);

        }

        [HttpGet]
        [Route("MiscellaneousReceiptReport")]
        public async Task<IActionResult> MiscellaneousReceiptReport([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {

            var receipt = await _unitofwork.Reports.MiscReports(DateFrom, DateTo);

            return Ok(receipt);
            

        }


        [HttpGet]
        [Route("MiscellaneousIssueReport")]
        public async Task<IActionResult> MiscellaneousIssueReport([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {

            var issue = await _unitofwork.Reports.MiscIssue(DateFrom, DateTo);

            return Ok(issue);

        }



        [HttpGet]
        [Route("BorrowedReturnedReports")]
        public async Task<IActionResult> BorrowedReturnedReports([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {

            var issue = await _unitofwork.Reports.ReturnBorrowedReports(DateFrom, DateTo);

            return Ok(issue);

        }

        [HttpGet]
        [Route("ReturnedReports")]
        public async Task<IActionResult> ReturnedReports([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {

            var issue = await _unitofwork.Reports.ReturnedReports(DateFrom, DateTo);

            return Ok(issue);

        }



        [HttpGet]
        [Route("CancelledOrderedReports")]
        public async Task<IActionResult> CancelledOrderedReports([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {

            var issue = await _unitofwork.Reports.CancelledReports(DateFrom, DateTo);

            return Ok(issue);

        }



        [HttpGet]
        [Route("InventoryMovementReport")]
        public async Task<IActionResult> InventoryMovementReport([FromQuery] string DateFrom, [FromQuery] string DateTo , [FromQuery] string PlusOne)
        {

            var issue = await _unitofwork.Reports.InventoryMovementReports(DateFrom, DateTo , PlusOne);

            return Ok(issue);

        }







    }
}
