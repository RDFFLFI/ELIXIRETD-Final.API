using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIRETD.DATA.SERVICES;

namespace ELIXIRETD.API.Controllers.INVENTORY_CONTROLLER
{

    public class MiscellaneousController : BaseApiController
    {

        private readonly IUnitOfWork _unitofwork;

        public MiscellaneousController(IUnitOfWork unitOfWork)
        {
            _unitofwork= unitOfWork;
        }


        [HttpPost]
        [Route("AddNewMiscellaneousReceipt")]

        public async Task<IActionResult> AddNewMiscellaneousReceipt([FromBody] MiscellaneousReceipt receipt)
        {
            await _unitofwork.miscellaneous.AddMiscellaneousReceipt(receipt);
            await _unitofwork.CompleteAsync();

            return Ok(receipt);
        }

        [HttpPost]
        [Route("AddNewMiscellaneousReceiptInWarehouse")]

        public async Task<IActionResult> AddNewMiscellaneousReceiptInWarehouse([FromBody] Warehouse_Receiving[] receive)
        {
            foreach(Warehouse_Receiving items in receive)
            {

                items.IsActive = true;
                items.ReceivingDate = DateTime.Now;
                items.IsWarehouseReceived = true;
                items.TransactionType = "MiscellaneousReceipt";

                await _unitofwork.miscellaneous.AddMiscellaneousReceiptInWarehouse(items);
                await _unitofwork.CompleteAsync();
                

            }

            return Ok("SuccessfullyAd new miscellaneous receipt in wareouse!");

        }

        [HttpPut]
        [Route("InActiveReceipt")]
        public async Task<IActionResult> InActiveReceipt([FromBody] MiscellaneousReceipt receipt)
        {
            var validate = await _unitofwork.miscellaneous.ValidateMiscellaneousInIssue(receipt);

            if (validate == false)
                return BadRequest("InActive Failed, you already use the receiving id");

            await _unitofwork.miscellaneous.InActiveMiscellaneousReceipt(receipt);
            await _unitofwork.CompleteAsync();

            return new JsonResult("Successfully InActive!");
  
        }

        [HttpPut]
        [Route("ActivateReceipt")]
        public async Task<IActionResult> ActivateReceipt([FromBody] MiscellaneousReceipt receipt)
        {
            await _unitofwork.miscellaneous.ActivateMiscellaenousReceipt(receipt);
            await _unitofwork.CompleteAsync();

            return new JsonResult("Successfully active receipt");
        }

        [HttpGet]
        [Route("GetAllMReceiptWithPagination")]
        public async Task<ActionResult<IEnumerable<GetAllMReceiptWithPaginationdDto>>> GetAllMiscellaneousReceiptPagination([FromQuery] UserParams userParams, [FromQuery] bool status)
        {
            var receipt = await _unitofwork.miscellaneous.GetAllMReceiptWithPaginationd(userParams, status);

            Response.AddPaginationHeader(receipt.CurrentPage, receipt.PageSize, receipt.TotalCount, receipt.TotalPages, receipt.HasNextPage, receipt.HasPreviousPage);

            var receiptResult = new
            {
                receipt,
                receipt.CurrentPage,
                receipt.PageSize,
                receipt.TotalCount,
                receipt.TotalPages,
                receipt.HasNextPage,
                receipt.HasPreviousPage
            };

            return Ok(receiptResult);
        }

        [HttpGet]
        [Route("GetAllMiscellaneousReceiptPaginationOrig")]
        public async Task<ActionResult<IEnumerable<GetAllMReceiptWithPaginationdDto>>> GetAllMiscellaneousReceiptPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search, [FromQuery] bool status)
        {

            if (search == null)

                return await GetAllMiscellaneousReceiptPagination(userParams, status);

            var receipt = await _unitofwork.miscellaneous.GetAllMReceiptWithPaginationOrig(userParams, search, status);

            Response.AddPaginationHeader(receipt.CurrentPage, receipt.PageSize, receipt.TotalCount, receipt.TotalPages, receipt.HasNextPage, receipt.HasPreviousPage);

            var receiptResult = new
            {
                receipt,
                receipt.CurrentPage,
                receipt.PageSize,
                receipt.TotalCount,
                receipt.TotalPages,
                receipt.HasNextPage,
                receipt.HasPreviousPage
            };

            return Ok(receiptResult);
        }

        [HttpGet]
        [Route("GetAllDetailsFromWarehouseByMReceipt")]
        public async Task<IActionResult> GetAllListofOrders([FromQuery] int id)
        {
            var receipt = await _unitofwork.miscellaneous.GetWarehouseDetailsByMReceipt(id);

            return Ok(receipt);
        }

        // ================================================= Miscellaneous Issue ==================================================================

        [HttpPost]
        [Route("AddNewMiscellaneousIssueDetails")]
        public async Task<IActionResult> AddNewMiscellaneousDetails([FromBody] MiscellaneousIssueDetails issue)
        {
            issue.IsActive = true;
            issue.PreparedDate = DateTime.Now;
            await _unitofwork.miscellaneous.AddMiscellaneousIssueDetails(issue);
            await _unitofwork.CompleteAsync();

            return Ok("Successfully add new miscellaneous issue!");
        }
       
        
        [HttpPost]
        [Route("AddNewMiscellaneousIssue")]
        public async Task<IActionResult> AddNewMiscellaneousIssueDetails([FromBody] MiscellaneousIssue issue)
        {
            issue.IsActive = true;
            issue.IsTransact = true;
            issue.PreparedDate = DateTime.Now;

            await _unitofwork.miscellaneous.AddMiscellaneousIssue(issue);
            await _unitofwork.CompleteAsync();

            return Ok(issue);
        }

        [HttpPut]
        [Route("UpdateMiscellaneousIssuePKey")]
        public async Task<IActionResult> UpdateMiscellaneousIssuePKey([FromBody] MiscellaneousIssueDetails[] details )
        {
            foreach (MiscellaneousIssueDetails items in details)
            {
                items.IsActive = true;
                items.PreparedDate = DateTime.Now;

                await _unitofwork.miscellaneous.UpdateIssuePKey(items);
            }

            await _unitofwork.CompleteAsync();

            return Ok(details);
        }

        [HttpGet]
        [Route("GetAllAvailableStocksForMIsssue")]
        public async Task<IActionResult> GetAllAvailableStocksFOrMIssue([FromQuery] string itemcode)
        {
            var reciept = await _unitofwork.miscellaneous.GetAvailableStocksForIssue(itemcode);

            return Ok(reciept);
        }

        [HttpGet]
        [Route("GetAllMIssueWithPagination")]
        public async Task<ActionResult<IEnumerable<GetAllMIssueWithPaginationDto>>> GetAllMiscellaneousIssuePagination([FromQuery] UserParams userParams, [FromQuery] bool status)
        {

            var issue = await _unitofwork.miscellaneous.GetAllMIssueWithPagination(userParams, status);

            Response.AddPaginationHeader(issue.CurrentPage, issue.PageSize, issue.TotalCount, issue.TotalPages, issue.HasNextPage, issue.HasPreviousPage);

            var issueResult = new
            {
                issue,
                issue.CurrentPage,
                issue.PageSize,
                issue.TotalCount,
                issue.TotalPages,
                issue.HasNextPage,
                issue.HasPreviousPage
            };

            return Ok(issueResult);
        }

        [HttpGet]
        [Route("GetAllMiscellaneousIssuePaginationOrig")]
        public async Task<ActionResult<IEnumerable<GetAllMIssueWithPaginationDto>>> GetAllMiscellaneousIssuePaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search, [FromQuery] bool status)
        {

            if (search == null)

                return await GetAllMiscellaneousIssuePagination(userParams, status);

            var issue = await _unitofwork.miscellaneous.GetAllMIssueWithPaginationOrig(userParams, search, status);

            Response.AddPaginationHeader(issue.CurrentPage, issue.PageSize, issue.TotalCount, issue.TotalPages, issue.HasNextPage, issue.HasPreviousPage);

            var issueResult = new
            {
                issue,
                issue.CurrentPage,
                issue.PageSize,
                issue.TotalCount,
                issue.TotalPages,
                issue.HasNextPage,
                issue.HasPreviousPage
            };

            return Ok(issueResult);
        }

        [HttpPut]
        [Route("InActiveIssue")]
        public async Task<IActionResult> InActiveIssue([FromBody] MiscellaneousIssue issue)
        { 

            await _unitofwork.miscellaneous.InActivateMiscellaenousIssue(issue);
            await _unitofwork.CompleteAsync();

            return new JsonResult("Successfully inactive issue!");
        }

        [HttpPut]
        [Route("ActivateIssue")]
        public async Task<IActionResult> ActivateIssue([FromBody] MiscellaneousIssue issue)
        {

            await _unitofwork.miscellaneous.ActivateMiscellaenousIssue(issue);
            await _unitofwork.CompleteAsync();

            return new JsonResult("Successfully active issue!");
        }

        [HttpGet]
        [Route("GetAllDetailsInMiscellaneousIssue")]
        public async Task<IActionResult> GetAllDetailsInMiscellaneousIssue([FromQuery] int id)
        {

            var receipt = await _unitofwork.miscellaneous.GetAllDetailsInMiscellaneousIssue(id);

            return Ok(receipt);

        }

        [HttpGet]
        [Route("GetAllActiveMiscellaneousIssueTransaction")]
        public async Task<IActionResult> GetAllActiveMiscellaneousIssueTransaction([FromQuery] int empid)
        {

            var issue = await _unitofwork.miscellaneous.GetAllAvailableIssue(empid);

            return Ok(issue);

        }

        [HttpPut]
        [Route("CancelItemCodeInMiscellaneousIssue")]
        public async Task<IActionResult> CancelItemCodeInMiscellaneoussIssue([FromBody] MiscellaneousIssueDetails[] issue)
        {

            foreach (MiscellaneousIssueDetails items in issue)
            {
                await _unitofwork.miscellaneous.CancelItemCodeInMiscellaneousIssue(items);
                await _unitofwork.CompleteAsync();
            }

            return new JsonResult("Successfully cancelled transaction!");
        }



    }
}
