using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIRETD.API.Controllers.SETUP_CONTROLLER
{
    public class LocationController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public LocationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        [Route("GetAllActiveLocations")]
        public async Task<IActionResult> GetAllActiveLocations()
        {
            var location = await _unitOfWork.Locations.GetAllActiveLocation();

            return Ok(location);
        }

        [HttpGet]
        [Route("GetAllInActiveLocations")]
        public async Task<IActionResult> GetAllInActiveLocations()
        {
            var location = await _unitOfWork.Locations.GetAllInActiveLocation();

            return Ok(location);
        }


        [HttpPost]
        [Route("AddNewLocation")]
        public async Task<IActionResult> AddNewLocation(Location location)
        {

            if (await _unitOfWork.Locations.LocationCodeExist(location.LocationCode))
                return BadRequest("Company code already exist, please try something else!");

            await _unitOfWork.Locations.AddLocation(location);
            await _unitOfWork.CompleteAsync();

            return Ok(location);
        }

        [HttpPut]
        [Route("UpdateLocation")]
        public async Task<IActionResult> UpdateLocation([FromBody] Location location)
        {
            await _unitOfWork.Locations.UpdateLocation(location);
            await _unitOfWork.CompleteAsync();

            return Ok(location);
        }


        [HttpPut]
        [Route("InActiveLocation")]
        public async Task<IActionResult> InActiveLocation([FromBody] Location location)
        {

            await _unitOfWork.Locations.InActiveLocation(location);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully inactive location!");
        }

        [HttpPut]
        [Route("ActivateLocation")]
        public async Task<IActionResult> ActivateLocation([FromBody] Location location)
        {

            await _unitOfWork.Locations.ActivateLocation(location);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully inactive location!");
        }

        [HttpGet]
        [Route("GetAllLocationWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<UomDto>>> GetAllLocationWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var location = await _unitOfWork.Locations.GetLocationWithPagination(status, userParams);

            Response.AddPaginationHeader(location.CurrentPage, location.PageSize, location.TotalCount, location.TotalPages, location.HasNextPage, location.HasPreviousPage);

            var locationResult = new
            {
                location,
                location.CurrentPage,
                location.PageSize,
                location.TotalCount,
                location.TotalPages,
                location.HasNextPage,
                location.HasPreviousPage
            };

            return Ok(locationResult);
        }

        [HttpGet]
        [Route("GetAllLocationWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<UomDto>>> GetAllLocationWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {
            if (search == null)
                return await GetAllLocationWithPagination(status, userParams);


            var location = await _unitOfWork.Locations.GetLocationWithPaginationOrig(userParams, status, search);

            Response.AddPaginationHeader(location.CurrentPage, location.PageSize, location.TotalCount, location.TotalPages, location.HasNextPage, location.HasPreviousPage);

            var locationResult = new
            {
                location,
                location.CurrentPage,
                location.PageSize,
                location.TotalCount,
                location.TotalPages,
                location.HasNextPage,
                location.HasPreviousPage
            };

            return Ok(locationResult);


        }




    }
}
