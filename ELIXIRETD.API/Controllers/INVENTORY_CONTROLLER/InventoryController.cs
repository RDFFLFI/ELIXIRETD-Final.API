﻿    using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORY_DTO.MRP;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
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


        [HttpGet]
        [Route("GetAllItemForInventoryPaginationOrig")]
        public async Task<ActionResult<IEnumerable<DtoMRP>>> GetAllItemForInventoryPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search)
        {

            var inventory = await _unitofwork.Inventory.GetallItemForInventoryPaginationOrig(userParams, search);

            Response.AddPaginationHeader(inventory.CurrentPage, inventory.PageSize, inventory.TotalCount, inventory.TotalPages, inventory.HasNextPage, inventory.HasPreviousPage);

            var inventoryResult = new
            {
                inventory,
                inventory.CurrentPage,
                inventory.PageSize,
                inventory.TotalCount,
                inventory.TotalPages,
                inventory.HasNextPage,
                inventory.HasPreviousPage
            };

            return Ok(inventoryResult);
        }


        [HttpGet]
        [Route("YmirSOHList/{itemCode}")]
        public async Task<IActionResult> YmirSOHList([FromRoute] string itemCode)
        {

            var result = await _unitofwork.Inventory.YmirSOHList(itemCode);

            return Ok(result);
        }



    }
}
