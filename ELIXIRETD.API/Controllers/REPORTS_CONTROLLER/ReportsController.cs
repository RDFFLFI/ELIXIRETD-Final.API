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








    }
}
