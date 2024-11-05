using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.FUEL_REGISTER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORY_DTO.MRP;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIRETD.API.Controllers.FUEL_REGISTER_CONTROLLER
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuelRegisterController : ControllerBase
    {
        private readonly IUnitOfWork _unitofwork;

        public FuelRegisterController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateFuelRegister(CreateFuelRegisterDto fuel)
        {

            fuel.Added_By = User.Identity.Name;
            fuel.Modified_By = User.Identity.Name;

            await _unitofwork.FuelRegister.CreateFuelRegister(fuel);
            await _unitofwork.CompleteAsync();

            return Ok("Successfully created");
        }

        [HttpGet("material-available")]
        public async Task<IActionResult> GetMaterialByStocks()
        {
            var results = await _unitofwork.FuelRegister.GetMaterialByStocks();

            return Ok(results); 
        }

        [HttpGet("user-role")]
        public async Task<IActionResult> GetDriverUser()
        {
            var results = await _unitofwork.FuelRegister.GetDriverUser();

            return Ok(results);
        }


        [HttpGet("material-available-item")]
        public async Task<IActionResult> GetMaterialStockByWarehouse()
        {
            var results = await _unitofwork.FuelRegister.GetMaterialStockByWarehouse();

            return Ok(results);
        }

        [HttpGet("page")]
        public async Task<ActionResult<IEnumerable<GetFuelRegisterDto>>> GetFuelRegister([FromQuery] UserParams userParams, [FromQuery] string Search, [FromQuery]string Status, [FromQuery]int? UserId)
        {

            var fuel = await _unitofwork.FuelRegister.GetFuelRegister(userParams, Search, Status,UserId);

            Response.AddPaginationHeader(fuel.CurrentPage, fuel.PageSize, fuel.TotalCount, fuel.TotalPages, fuel.HasNextPage, fuel.HasPreviousPage);

            var fuelResult = new
            {
                fuel,
                fuel.CurrentPage,
                fuel.PageSize,
                fuel.TotalCount,
                fuel.TotalPages,
                fuel.HasNextPage,
                fuel.HasPreviousPage
            };

            return Ok(fuelResult);
        }


        [HttpPut("approve")]
        public async Task<IActionResult> ApproveFuel([FromBody]ApproveFuelDto[] fuel)
        {


            foreach(var item in fuel)
            {
                var fuelNotExist = await _unitofwork.FuelRegister.FuelRegisterNotExist(item.Id);
                if (fuelNotExist is false)
                    return BadRequest("fuel not exist!");

                item.Approve_By = User.Identity.Name;

                await _unitofwork.FuelRegister.ApproveFuel(item);

            }

            await _unitofwork.CompleteAsync();

            return Ok("Successfully approve");
        }

        [HttpPut("reject")]
        public async Task<IActionResult> RejectFuel([FromBody] RejectFuelDto[] fuel)
        {


            foreach (var item in fuel)
            {
                var fuelNotExist = await _unitofwork.FuelRegister.FuelRegisterNotExist(item.Id);
                if (fuelNotExist is false)
                    return BadRequest("fuel not exist!");

                item.Reject_By = User.Identity.Name;

                await _unitofwork.FuelRegister.RejectFuel(item);

            }

            await _unitofwork.CompleteAsync();

            return Ok("Successfully reject");
        }

        [HttpPut("transact")]
        public async Task<IActionResult> TransactFuel([FromBody] TransactedFuelDto[] fuel)
        {


            foreach (var item in fuel)
            {
                var fuelNotExist = await _unitofwork.FuelRegister.FuelRegisterNotExist(item.Id);
                if (fuelNotExist is false)
                    return BadRequest("fuel not exist!");

                item.Transacted_By = User.Identity.Name;

                await _unitofwork.FuelRegister.TransactFuel(item);

            }

            await _unitofwork.CompleteAsync();

            return Ok("Successfully transacted");
        }

        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelFuel([FromRoute] int id)
        {
            var fuelNotExist = await _unitofwork.FuelRegister.FuelRegisterNotExist(id);
            if (fuelNotExist is false)
                return BadRequest("fuel not exist!");


            await _unitofwork.FuelRegister.CancelFuel(id);


            await _unitofwork.CompleteAsync();

            return Ok("Successfully cancel");
        }
    }
}