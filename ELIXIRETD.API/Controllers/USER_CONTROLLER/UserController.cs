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

        [HttpGet]
        [Route("GetInAllUsers")]
        public async Task<IActionResult> GetInActiveUsers()
        {
            var user = await _unitOfWork.Users.GetAllInActiveUsers();

            return Ok(user);
        }

        [HttpPost]
        [Route("AddNewUser")]
        public async Task<IActionResult> AddNewUser(User user)
        {

            var getRoleId = await _unitOfWork.Users.ValidateRoleId(user.UserRoleId);
            var validateuserRolemodules = await _unitOfWork.Users.ValidateUserRolesModule(user);


            if (await _unitOfWork.Users.ValidateUserExist(user.UserName))
                return BadRequest("Username already exist, Please try something else!");

            if (getRoleId == false)
                return BadRequest("Role doesn't exist, Please input data first!");

            if (validateuserRolemodules == true)
                return BadRequest("No Role modules has been tag!");


            await _unitOfWork.Users.AddNewUser(user);
            await _unitOfWork.CompleteAsync();

            return Ok(user);
        }

        [HttpPut]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] User user)
        {

            await _unitOfWork.Users.ChangePassword(user);
            await _unitOfWork.CompleteAsync();

            return Ok(user);
        }

        [HttpPut]
        [Route("UpdateUserInfo")]
        public async Task<IActionResult> UpdateUserInfo([FromBody]User user)
        {
            var getRoleId = await _unitOfWork.Users.ValidateRoleId(user.UserRoleId);
            var validateuserRolemodules = await _unitOfWork.Users.ValidateUserRolesModule(user);


            if (await _unitOfWork.Users.ValidateUserExist(user.UserName))
                return BadRequest("Username already exist, Please try something else!");

            if (getRoleId == false)
                return BadRequest("Role doesn't exist, Please input data first!");

            if (validateuserRolemodules == true)
                return BadRequest("No Role modules has been tag!");

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


        //------------DEPARTMENT-----------

        [HttpGet]
        [Route("GetAllActiveDepartment")]
        public async Task<IActionResult> GetAllActiveDepartment()
        {
            var dep = await _unitOfWork.Users.GetAllActiveDepartment();

            return Ok(dep);
        }

        [HttpGet]
        [Route("GetAllInActiveDepartment")]
        public async Task<IActionResult> GetAllInActiveDepartment()
        {
            var dep = await _unitOfWork.Users.GetAllInActiveDepartment();

            return Ok(dep);
        }


        [HttpPut]
        [Route("AddNewDepartment")]
        public async Task<IActionResult> AddNewDepartment([FromBody] Department[] departments)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult("Something went wrong!") { StatusCode = 500 };
            }

            List<Department> duplicateList = new List<Department>();
            List<Department> availableImport = new List<Department>();

            foreach (Department department in departments)
            {

               if (departments.Count(x => x.DepartmentName == department.DepartmentName && x.DepartmentCode == department.DepartmentCode) > 1)
                {

                   
                    duplicateList.Add(department);
                }

                else
                {

                var existingDepartment = await _unitOfWork.Users.GetById(department.Department_No);

                if (existingDepartment != null)
                {


                    existingDepartment.DepartmentCode = department.DepartmentCode;
                    existingDepartment.DepartmentName = department.DepartmentName;
                    existingDepartment.AddedBy = department.AddedBy;
                    existingDepartment.IsActive = department.IsActive;
                    existingDepartment.DateAdded = department.DateAdded;
                    existingDepartment.Reason = department.Reason;

                    await _unitOfWork.Users.Update(existingDepartment);
                }
                else if (await _unitOfWork.Users.GetByDepartmentNo(department.Department_No) == null)
                {

                        availableImport.Add(department);
                        await _unitOfWork.Users.AddNewDepartment(department);    
                }

                }

            }
             
            var resultlist = new
            {
                AvailableImport = availableImport,
                DuplicateList = duplicateList,
            };

            if (duplicateList.Count == 0)
            {
                await _unitOfWork.CompleteAsync();
                return Ok("Successfully added!");
            }
            else
            {

                return BadRequest(resultlist);
            }
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
