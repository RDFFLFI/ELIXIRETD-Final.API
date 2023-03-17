using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIRETD.API.Controllers.SETUP_CONTROLLER
{

    public class AccountController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        [Route("GetAllActiveAccounts")]
        public async Task<IActionResult> GetAllActiveAccounts()
        {
            var account = await _unitOfWork.Accounts.GetAllActiveAccount();

            return Ok(account);
        }

        [HttpGet]
        [Route("GetAllInActiveAccounts")]
        public async Task<IActionResult> GetAllInActiveAccounts()
        {
            var account = await _unitOfWork.Accounts.GetAllInActiveAccount();

            return Ok(account);
        }


        [HttpPost]
        [Route("AddNewAccounts")]
        public async Task<IActionResult> AddNewAccounts(Account account)
        {

            if (await _unitOfWork.Accounts.AccountCodeExist(account.AccountCode))
                return BadRequest("Company code already exist, please try something else!");

            await _unitOfWork.Accounts.AddAccount(account);
            await _unitOfWork.CompleteAsync();

            return Ok(account);
        }

        [HttpPut]
        [Route("UpdateAccount")]
        public async Task<IActionResult> UpdateAccount([FromBody] Account account)
        {
            await _unitOfWork.Accounts.AddAccount(account);
            await _unitOfWork.CompleteAsync();

            return Ok(account);
        }

        [HttpPut]
        [Route("InActiveAccount")]
        public async Task<IActionResult> InActiveAccount([FromBody] Account account)
        {

            await _unitOfWork.Accounts.InActiveAccount(account);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully inactive account!");
        }

        [HttpPut]
        [Route("ActivateAccount")]
        public async Task<IActionResult> ActivateAccount([FromBody] Account account)
        {

            await _unitOfWork.Accounts.ActivateAccount(account);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully activate account!");
        }


        [HttpGet]
        [Route("GetAllAccountWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<UomDto>>> GetAllAccountWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var account = await _unitOfWork.Accounts.GetAllAccountWithPagination(status, userParams);

            Response.AddPaginationHeader(account.CurrentPage, account.PageSize, account.TotalCount, account.TotalPages, account.HasNextPage, account.HasPreviousPage);

            var accountResult = new
            {
                account,
                account.CurrentPage,
                account.PageSize,
                account.TotalCount,
                account.TotalPages,
                account.HasNextPage,
                account.HasPreviousPage
            };

            return Ok(accountResult);
        }

        [HttpGet]
        [Route("GetAllAccountWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<UomDto>>> GetAllAccountWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {
            if (search == null)
                return await GetAllAccountWithPagination(status, userParams);


            var account = await _unitOfWork.Accounts.GetAccountWithPaginationOrig(userParams, status, search);

            Response.AddPaginationHeader(account.CurrentPage, account.PageSize, account.TotalCount, account.TotalPages, account.HasNextPage, account.HasPreviousPage);

            var accountResult = new
            {
                account,
                account.CurrentPage,
                account.PageSize,
                account.TotalCount,
                account.TotalPages,
                account.HasNextPage,
                account.HasPreviousPage
            };

            return Ok(accountResult);
        }


    }
}
