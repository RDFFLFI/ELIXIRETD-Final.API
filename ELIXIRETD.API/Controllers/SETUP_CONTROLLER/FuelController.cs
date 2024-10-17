using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO.FuelDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.SERVICES;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIRETD.API.Controllers.SETUP_CONTROLLER
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuelController : ControllerBase
    {
         private readonly IUnitOfWork _unitofwork;

        public FuelController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        [HttpPost("upsert")]
        public async Task<IActionResult> UpsertFuel(UpsertFuelDto fuel)
        {
            var itemNotExist = await _unitofwork.Fuel.ItemNotExist(fuel.MaterialId.Value);
            if (itemNotExist is false)
                return BadRequest("Material not exist!");

            var itemAlreadyExist = await _unitofwork.Fuel.ItemAlreadyExist(fuel.MaterialId.Value);
            if (itemAlreadyExist is false)
                return BadRequest("Material already exist!");

            fuel.Added_By = User.Identity.Name;
            fuel.Modified_By = User.Identity.Name;

            await _unitofwork.Fuel.UpsertFuel(fuel);

            await _unitofwork.CompleteAsync();

            return Ok("Successfully created");
        }



        [HttpGet]
        [Route("page")]
        public async Task<ActionResult<IEnumerable<UomDto>>> GetFuel([FromQuery] UserParams userParams, [FromQuery] string Search, [FromQuery]bool? Status)
        {
            var fuel = await _unitofwork.Fuel.GetFuel(userParams,Search,Status);

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


        [HttpPost("update-status/{id}")]
        public async Task<IActionResult> UpdateFuelStatus([FromRoute]int id)
        {
            var fuelNotExist = await _unitofwork.Fuel.FuelNotExist(id);
            if (fuelNotExist is false)
                return BadRequest("fuel not exist!");

            await _unitofwork.Fuel.UpdateFuelStatus(id);

            await _unitofwork.CompleteAsync();

            return Ok("Successfully created");
        }
    }
}
