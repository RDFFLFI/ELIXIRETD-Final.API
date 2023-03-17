using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.USER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIRETD.API.Controllers.USER_CONTROLLER
{
   
    public class UserController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StoreContext _context;

        public UserController(StoreContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var user = await _unitOfWork.Users.GetAllActiveUsers();

            return Ok(user);
        }

        [HttpPost]
        [Route("AddNewUser")]
        public async Task<IActionResult> AddNewUser(User user)
        {

            var getRoleId = await _unitOfWork.Users.ValidateRoleId(user.UserRoleId);
            var getDepId = await _unitOfWork.Users.ValidateDepartmentId(user.DepartmentId);
            var validateuserRolemodules = await _unitOfWork.Users.ValidateUserRolesModule(user);

           
            if (await _unitOfWork.Users.ValidateUserExist(user.UserName))
                return BadRequest("Username already exist, Please try something else!");

            if (getRoleId == false)
                return BadRequest("Role doesn't exist, Please input data first!");

            if (getDepId == false)
                return BadRequest("Department doesn't exist, Please input data first!");

            if (validateuserRolemodules == true)
                return BadRequest("No Rolemodules has been tag!");


            await _unitOfWork.Users.AddNewUser(user);
            await _unitOfWork.CompleteAsync();

            return Ok(user);
        }

        [HttpPut]
        [Route("UpdateUserInfo")]
        public async Task<IActionResult> UpdateUserInfo([FromBody]User user)
        {
            var getRoleId = await _unitOfWork.Users.ValidateRoleId(user.UserRoleId);
            var getDepId = await _unitOfWork.Users.ValidateDepartmentId(user.DepartmentId);
            var validateuserRolemodules = await _unitOfWork.Users.ValidateUserRolesModule(user);


            if (await _unitOfWork.Users.ValidateUserExist(user.UserName))
                return BadRequest("Username already exist, Please try something else!");

            if (getRoleId == false)
                return BadRequest("Role doesn't exist, Please input data first!");

            if (getDepId == false)
                return BadRequest("Department doesn't exist, Please input data first!");

            if (validateuserRolemodules == true)
                return BadRequest("No Rolemodules has been tag!");

            await _unitOfWork.Users.UpdateUserInfo(user);
            await _unitOfWork.CompleteAsync();

            return Ok("Successfully updated!");
        }

        [HttpPut]
        [Route("InactiveUser")]
        public async Task<IActionResult> InActiveUser([FromBody]User user)
        {
            await _unitOfWork.Users.InActiveUser(user);
            await _unitOfWork.CompleteAsync();

            return Ok("Successfully inactive user!");
        }


        [HttpPut]
        [Route("ActivateUser")]
        public async Task<IActionResult> ActivateUser([FromBody] User user)
        {
            await _unitOfWork.Users.ActivateUser(user);
            await _unitOfWork.CompleteAsync();

            return Ok("Successfully activate user!");
        }


        [HttpGet]
        [Route("GetAllUserWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsersWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var user = await _unitOfWork.Users.GetAllUserWithPagination(status, userParams);

            Response.AddPaginationHeader(user.CurrentPage, user.PageSize, user.TotalCount, user.TotalPages, user.HasNextPage, user.HasPreviousPage);

            var userResult = new
            {
                user,
                user.CurrentPage,
                user.PageSize,
                user.TotalCount,
                user.TotalPages,
                user.HasNextPage,
                user.HasPreviousPage
            };

            return Ok(userResult);
        }

        [HttpGet]
        [Route("GetAllUserWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsersWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllUsersWithPagination(status, userParams);

            var user = await _unitOfWork.Users.GetAllUserWithPaginationOrig(userParams, status, search);


            Response.AddPaginationHeader(user.CurrentPage, user.PageSize, user.TotalCount, user.TotalPages, user.HasNextPage, user.HasPreviousPage);

            var userResult = new
            {
                user,
                user.CurrentPage,
                user.PageSize,
                user.TotalCount,
                user.TotalPages,
                user.HasNextPage,
                user.HasPreviousPage
            };

            return Ok(userResult);
        }


        //------------DEPARTMENT

        [HttpGet]
        [Route("GetAllActiveDepartment")]
        public async Task<IActionResult> GetAllActiveDepartment()
        {
            var dep = await _unitOfWork.Users.GetAllActiveDepartment();

            return Ok(dep);
        }

        [HttpPost]
        [Route("AddNewDepartment")]
        public async Task<IActionResult> AddNewDepartment(Department department)
        {

            if (await _unitOfWork.Users.ValidateDepartmentCodeExist(department.DepartmentCode))
                return BadRequest("Department code already exist, please try something else!");

            await _unitOfWork.Users.AddNewDepartment(department);
            await _unitOfWork.CompleteAsync();

            return Ok(department);
        }

        [HttpPut]
        [Route("UpdateDepartment")]
        public async Task<IActionResult> UpdateDepartment([FromBody] Department department)
        {
            if (await _unitOfWork.Users.ValidateDepartmentCodeExist(department.DepartmentCode))
                return BadRequest("Department code already exist, please try something else!");

            await _unitOfWork.Users.UpdateDepartment(department);
            await _unitOfWork.CompleteAsync();

            return Ok("Successfully updated!");
        }


        [HttpPut]
        [Route("InActiveDepartment")]
        public async Task<IActionResult> InActiveDepartment([FromBody] Department department)
        {
            await _unitOfWork.Users.InActiveDepartment(department);
            await _unitOfWork.CompleteAsync();

            return Ok("Successfully inactive department!");
        }


        [HttpPut]
        [Route("ActivateDepartment")]
        public async Task<IActionResult> ActivateDepartment([FromBody] Department department)
        {
            await _unitOfWork.Users.ActivateDepartment(department);
            await _unitOfWork.CompleteAsync();

            return Ok("Successfully activate department!");
        }

        [HttpGet]
        [Route("GetAllDepartmentWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllDepartmentWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var department = await _unitOfWork.Users.GetAllDepartmentWithPagination(status, userParams);

            Response.AddPaginationHeader(department.CurrentPage, department.PageSize, department.TotalCount, department.TotalPages, department.HasNextPage, department.HasPreviousPage);

            var departmentResult = new
            {
                department,
                department.CurrentPage,
                department.PageSize,
                department.TotalCount,
                department.TotalPages,
                department.HasNextPage,
                department.HasPreviousPage
            };

            return Ok(departmentResult);
        }


        [HttpGet]
        [Route("GetAllDepartmentWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllDepartmentWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllDepartmentWithPagination(status, userParams);

            var department = await _unitOfWork.Users.GetAllDepartmentWithPaginationOrig(userParams, status, search);


            Response.AddPaginationHeader(department.CurrentPage, department.PageSize, department.TotalCount, department.TotalPages, department.HasNextPage, department.HasPreviousPage);

            var departmentResult = new
            {
                department,
                department.CurrentPage,
                department.PageSize,
                department.TotalCount,
                department.TotalPages,
                department.HasNextPage,
                department.HasPreviousPage
            };

            return Ok(departmentResult);
        }

    }
}
