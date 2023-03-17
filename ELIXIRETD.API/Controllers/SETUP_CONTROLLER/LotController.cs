using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIRETD.API.Controllers.SETUP_CONTROLLER
{
    public class LotController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public LotController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //-------LOT NAME----------------


        [HttpGet]
        [Route("GetAllActiveLotNames")]
        public async Task<IActionResult> GetAllActiveLotNames()
        {
            var lots = await _unitOfWork.Lots.GetAllActiveLotName();

            return Ok(lots);
        }

        [HttpGet]
        [Route("GetAllInActiveLotNames")]
        public async Task<IActionResult> GetAllInActiveLotNames()
        {
            var lot = await _unitOfWork.Lots.GetAllInActiveLotName();

            return Ok(lot);
        }

        [HttpPost]
        [Route("AddNewLotName")]
        public async Task<IActionResult> CreateNewLotName(LotName lotname)
        {
          
                var categoryId = await _unitOfWork.Lots.ValidateLotCategoryId(lotname.LotCategoryId);

                if (categoryId == false)
                    return BadRequest("Lot Category doesn't exist, Please add data first!");

                var validate = await _unitOfWork.Lots.ValidateLotNameAndSection(lotname);

                if (validate == true)
                    return BadRequest("Lotname and section already exist!");

                await _unitOfWork.Lots.AddLotName(lotname);
                await _unitOfWork.CompleteAsync();

            return Ok(lotname);
        }

        [HttpPut]
        [Route("UpdateLotName")]
        public async Task<IActionResult> UpdateLotName([FromBody] LotName lotname)
        {
            var categoryId = await _unitOfWork.Lots.ValidateLotCategoryId(lotname.LotCategoryId);

            if (categoryId == false)
                return BadRequest("Lot category doesn't exist, please add data first!");

            var validate = await _unitOfWork.Lots.ValidateLotNameAndSection(lotname);

            if (validate == true)
                return BadRequest("Lotname and section already exist!");

            await _unitOfWork.Lots.UpdateLotName(lotname);
            await _unitOfWork.CompleteAsync();

            return Ok(lotname);
        }

        [HttpPut]
        [Route("InActiveLotName")]
        public async Task<IActionResult> InActiveLotName([FromBody] LotName lotname)
        {
            await _unitOfWork.Lots.InActiveLotName(lotname);
            await _unitOfWork.CompleteAsync();

            return Ok("Successfully inactive lot name!");
        }

        [HttpPut]
        [Route("ActivateLotName")]
        public async Task<IActionResult> ActivateLotName([FromBody] LotName lotname)
        {
           
            await _unitOfWork.Lots.ActivateLotName(lotname);
            await _unitOfWork.CompleteAsync();

            return Ok("Successfully activated lot name!");
        }

        [HttpGet]
        [Route("GetAllLotNameWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<LotNameDto>>> GetAllLotNameWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var lotname = await _unitOfWork.Lots.GetAllLotNameWithPagination(status, userParams);

            Response.AddPaginationHeader(lotname.CurrentPage, lotname.PageSize, lotname.TotalCount, lotname.TotalPages, lotname.HasNextPage, lotname.HasPreviousPage);

            var lotnameResult = new
            {
                lotname,
                lotname.CurrentPage,
                lotname.PageSize,
                lotname.TotalCount,
                lotname.TotalPages,
                lotname.HasNextPage,
                lotname.HasPreviousPage
            };

            return Ok(lotnameResult);
        }

        [HttpGet]
        [Route("GetAllLotNameWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<LotNameDto>>> GetAllLotNameWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllLotNameWithPagination(status, userParams);

            var lotname = await _unitOfWork.Lots.GetLotNameWithPaginationOrig(userParams, status, search);


            Response.AddPaginationHeader(lotname.CurrentPage, lotname.PageSize, lotname.TotalCount, lotname.TotalPages, lotname.HasNextPage, lotname.HasPreviousPage);

            var lotnameResult = new
            {
                lotname,
                lotname.CurrentPage,
                lotname.PageSize,
                lotname.TotalCount,
                lotname.TotalPages,
                lotname.HasNextPage,
                lotname.HasPreviousPage
            };

            return Ok(lotnameResult);
        }





        //-------LOT CATEGORY-----------


        [HttpGet]
        [Route("GetAllActiveLotCategories")]
        public async Task<IActionResult> GetAllActiveLotCategories()
        {
            var lots = await _unitOfWork.Lots.GetAllActiveLotCategories();

            return Ok(lots);
        }

        [HttpGet]
        [Route("GetAllInActiveLotCategories")]
        public async Task<IActionResult> GetAllInActiveLotCategories()
        {
            var lot = await _unitOfWork.Lots.GetAllInActiveLotCategories();

            return Ok(lot);
        }

        [HttpPost]
        [Route("AddNewLotCategory")]
        public async Task<IActionResult> CreateNewLotCategory(LotCategory category)
        {
        
                if (await _unitOfWork.Lots.LotCategoryNameExist(category.LotName))
                    return BadRequest("Category Name already Exist!, Please try something else!");

                await _unitOfWork.Lots.AddLotCategory(category);
                await _unitOfWork.CompleteAsync();

            return Ok(category);
   
        }



        [HttpPut]
        [Route("UpdateLotCategories")]
        public async Task<IActionResult> UpdateLotCategories([FromBody] LotCategory category)
        {
            if (await _unitOfWork.Lots.LotCategoryNameExist(category.LotName))
                return BadRequest("Category Name already Exist!, Please try something else!");

            await _unitOfWork.Lots.UpdateLotCategory(category);
            await _unitOfWork.CompleteAsync();

            return Ok(category);
        }


        [HttpPut]
        [Route("InActiveLotCategories")]
        public async Task<IActionResult> InActiveLotCategories([FromBody] LotCategory category)
        {
            await _unitOfWork.Lots.InActiveLotCategory(category);
            await _unitOfWork.CompleteAsync();

            return Ok("Successfully inactive lot category!");
        }

        [HttpPut]
        [Route("ActivateLotCategories")]
        public async Task<IActionResult> ActivateLotCategories([FromBody] LotCategory category)
        {

            await _unitOfWork.Lots.ActivateLotCategory(category);
            await _unitOfWork.CompleteAsync();

            return Ok("Successfully activated lot category!");
        }



        [HttpGet]
        [Route("GetAllLotCategoryWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<LotCategoryDto>>> GetAllLotCategoryWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var category = await _unitOfWork.Lots.GetAllLotCategoryWithPagination(status, userParams);

            Response.AddPaginationHeader(category.CurrentPage, category.PageSize, category.TotalCount, category.TotalPages, category.HasNextPage, category.HasPreviousPage);

            var categoryResult = new
            {
                category,
                category.CurrentPage,
                category.PageSize,
                category.TotalCount,
                category.TotalPages,
                category.HasNextPage,
                category.HasPreviousPage
            };

            return Ok(categoryResult);
        }

        [HttpGet]
        [Route("GetAllLotCategoryWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<LotCategoryDto>>> GetAllLotCategoryWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllLotCategoryWithPagination(status, userParams);

            var category = await _unitOfWork.Lots.GetLotCategoryWithPaginationOrig(userParams, status, search);


            Response.AddPaginationHeader(category.CurrentPage, category.PageSize, category.TotalCount, category.TotalPages, category.HasNextPage, category.HasPreviousPage);

            var categoryResult = new
            {
                category,
                category.CurrentPage,
                category.PageSize,
                category.TotalCount,
                category.TotalPages,
                category.HasNextPage,
                category.HasPreviousPage
            };

            return Ok(categoryResult);
        }


        






    }
}
