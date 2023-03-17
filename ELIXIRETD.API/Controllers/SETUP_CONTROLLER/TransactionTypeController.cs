using ELIXIRETD.DATA.CORE.ICONFIGURATION;
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
            await _unitofwork.TransactType.AddnewTransactType(transactionType);
            await _unitofwork.CompleteAsync();

             return Ok(transactionType);
        }


    }
}
