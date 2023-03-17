using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ELIXIRETD.API.Controllers.SETUP_CONTROLLER
{
    public class CompanyController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;


        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("GetAllActiveCompany")]
        public async Task<IActionResult> GetAllActiveCompany()
        {
            var company = await _unitOfWork.Companies.GetAllActiveCompany();

            return Ok(company);
        }

        [HttpGet]
        [Route("GetAllInActiveCompany")]
        public async Task<IActionResult> GetAllInActiveCompany()
        {
            var company = await _unitOfWork.Companies.GetAllInActiveCompany();

            return Ok(company);
        }


        [HttpPost]
        [Route("AddNewCompany")]
        public async Task<IActionResult> AddNewCompany(Company company)
        {

            if (await _unitOfWork.Companies.CompanyCodeExist(company.CompanyCode))
                return BadRequest("Company code already exist, please try something else!");

            await _unitOfWork.Companies.AddCompany(company);
            await _unitOfWork.CompleteAsync();

            return Ok(company);
        }

        [HttpPut]
        [Route("UpdateCompany")]
        public async Task<IActionResult> UpdateCompany([FromBody] Company company)
        {
            await _unitOfWork.Companies.UpdateCompany(company);
            await _unitOfWork.CompleteAsync();

            return Ok(company);
        }


        [HttpPut]
        [Route("InActiveCompany")]
        public async Task<IActionResult> InActiveCompany([FromBody] Company company)
        {

            await _unitOfWork.Companies.InActiveCompany(company);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully inactive company!");
        }

        [HttpPut]
        [Route("ActivateCompany")]
        public async Task<IActionResult> ActivateCompany([FromBody] Company company)
        {
      
            await _unitOfWork.Companies.ActivateCompany(company);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully inactive company!");
        }


        [HttpGet]
        [Route("GetAllCompanyWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<UomDto>>> GetAllCompanyWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var company = await _unitOfWork.Companies.GetAllCompanyWithPagination(status, userParams);

            Response.AddPaginationHeader(company.CurrentPage, company.PageSize, company.TotalCount, company.TotalPages, company.HasNextPage, company.HasPreviousPage);

            var companyResult = new
            {
                company,
                company.CurrentPage,
                company.PageSize,
                company.TotalCount,
                company.TotalPages,
                company.HasNextPage,
                company.HasPreviousPage
            };

            return Ok(companyResult);
        }

        [HttpGet]
        [Route("GetAllCompanyWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<UomDto>>> GetAllCompanyithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {
            if (search == null)
                return await GetAllCompanyWithPagination(status, userParams);


            var company = await _unitOfWork.Companies.GetCompanyWithPaginationOrig(userParams, status, search);

            Response.AddPaginationHeader(company.CurrentPage, company.PageSize, company.TotalCount, company.TotalPages, company.HasNextPage, company.HasPreviousPage);


            var companyResult = new
            {
                company,
                company.CurrentPage,
                company.PageSize,
                company.TotalCount,
                company.TotalPages,
                company.HasNextPage,
                company.HasPreviousPage
            };

            return Ok(companyResult);
        }





    }
}
