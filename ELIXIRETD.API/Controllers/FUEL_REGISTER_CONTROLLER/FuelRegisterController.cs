﻿using ELIXIRETD.DATA.CORE.ICONFIGURATION;
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

            var fuelnotExist = await _unitofwork.FuelRegister.MaterialNotExist(fuel.MaterialId.Value);
            if (fuelnotExist is false)
                return BadRequest("material not exist!");

            var warehousenotExist = await _unitofwork.FuelRegister.WarehouseNotExist(fuel.MaterialId.Value);
            if (fuelnotExist is false)
                return BadRequest("material not exist!");

            fuel.Added_By = User.Identity.Name;

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


        [HttpGet("material-available-item")]
        public async Task<IActionResult> GetMaterialStockByWarehouse(string itemCode)
        {
            var results = await _unitofwork.FuelRegister.GetMaterialStockByWarehouse(itemCode);

            return Ok(results);
        }

        [HttpGet("page")]
        public async Task<ActionResult<IEnumerable<GetFuelRegisterDto>>> GetFuelRegister([FromQuery] UserParams userParams, [FromQuery] string Search, [FromQuery]string Status)
        {

            var fuel = await _unitofwork.FuelRegister.GetFuelRegister(userParams, Search, Status);

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


        [HttpPost("approve")]
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

        [HttpPost("reject")]
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

        [HttpPost("transact")]
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

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelFuel([FromBody] int id)
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