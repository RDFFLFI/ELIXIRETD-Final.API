using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.SERVICES;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIRETD.API.Controllers.INVENTORY_CONTROLLER
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {

        private readonly IUnitOfWork _unitofwork;

        public InventoryController(IUnitOfWork unitOfWork)
        {
            _unitofwork= unitOfWork;
        }

        [HttpGet]
        [Route("RawmaterialInventory")]
        public async Task<IActionResult> GetAllAvailableRawmaterial()
        {
            var rawmaterial = await _unitofwork.Inventory.GetAllAvailableInRawmaterialInventory();

            return Ok(rawmaterial);
        }


    }
}
