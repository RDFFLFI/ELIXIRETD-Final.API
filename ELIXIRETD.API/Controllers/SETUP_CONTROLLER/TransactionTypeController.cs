using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIRETD.API.Controllers.SETUP_CONTROLLER
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionTypeController : ControllerBase
    {
        private readonly IUnitOfWork _unitofwork;

        public TransactionTypeController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        [HttpPost]
        [Route("AddNewTransactionType")]
        public async Task<IActionResult> AddnewTransactionType(TransactionType transactionType)
        {

            if (await _unitofwork.TransactType.ValidationTransactName(transactionType.TransactionName))
                return BadRequest("Transaction name already exist!");

            await _unitofwork.TransactType.AddnewTransactType(transactionType);
            await _unitofwork.CompleteAsync();

             return Ok(transactionType);
        }


        [HttpGet]
        [Route("GetAllActiveTransactionType")]
        public async Task<IActionResult> GetAllActiveTransactionType()
        {
             var transaction = await _unitofwork.TransactType.GetAllActiveTransactionType();    

            return Ok(transaction);

        }

        [HttpGet]
        [Route("GetAllInActiveTransactionType")]
        public async Task<IActionResult> GetAllInActiveTransactionType()
        {
            var transaction = await _unitofwork.TransactType.GetAllInActiveTransactionType();

            return Ok(transaction);

        }


        [HttpPut]
        [Route("UpdateTransactionType")]
        public async Task<IActionResult> UpdateTransactionType (TransactionType transactionType)
        {

            if (await _unitofwork.TransactType.ValidationTransactName(transactionType.TransactionName))
                return BadRequest("Transaction name already exist!");

            await _unitofwork.TransactType.UpdateTransactionType(transactionType);
            await _unitofwork.CompleteAsync();

            return Ok(transactionType);

        }


        [HttpPut]
        [Route("UpdateActive")]
        public async Task<IActionResult> UpdateActive(TransactionType transactionType)
        {

            await _unitofwork.TransactType.UpdateActive(transactionType);
            await _unitofwork.CompleteAsync();

            return Ok(transactionType);

        }


        [HttpPut]
        [Route("UpdateInActive")]
        public async Task<IActionResult> UpdateInActive(TransactionType transactionType)
        {

            await _unitofwork.TransactType.UpdateInActive(transactionType);
            await _unitofwork.CompleteAsync();

            return Ok(transactionType);

        }


        [HttpGet]
        [Route("GetTransactTypePagination/{status}")]
        public async Task<ActionResult<IEnumerable<DtoTransactionType>>> GetTransactTypePagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var Transaction = await _unitofwork.TransactType.GetTransactTypeWithPagination(status, userParams);

            Response.AddPaginationHeader(Transaction.CurrentPage, Transaction.PageSize, Transaction.TotalCount, Transaction.TotalPages, Transaction.HasNextPage, Transaction.HasPreviousPage);

            var supplierResult = new
            {
                Transaction,
                Transaction.CurrentPage,
                Transaction.PageSize,
                Transaction.TotalCount,
                Transaction.TotalPages,
                Transaction.HasNextPage,
                Transaction.HasPreviousPage
            };

            return Ok(supplierResult);
        }


        [HttpGet]
        [Route("GetTransactTypePaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<DtoTransactionType>>> GetTransactTypePaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {
            if (search == null)
                return await GetTransactTypePagination(status, userParams);


            var supplier = await _unitofwork.TransactType.GetTransactTypeWithPaginationOrig(userParams, status, search);

            Response.AddPaginationHeader(supplier.CurrentPage, supplier.PageSize, supplier.TotalCount, supplier.TotalPages, supplier.HasNextPage, supplier.HasPreviousPage);

            var supplierResult = new
            {
                supplier,
                supplier.CurrentPage,
                supplier.PageSize,
                supplier.TotalCount,
                supplier.TotalPages,
                supplier.HasNextPage,
                supplier.HasPreviousPage
            };

            return Ok(supplierResult);
        }


    }
}
