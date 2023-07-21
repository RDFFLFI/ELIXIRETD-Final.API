using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.USER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
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


            if (await _unitOfWork.Users.ValidateEmpIdAndFullName(user.EmpId , user.FullName))
                return BadRequest("User already exist, Please try something else");

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
        [Route("UpdateUserInfo")]
        public async Task<IActionResult> UpdateUserInfo(User user)
        {

         
            //if(user.Password == user.UserName)
            //    return BadRequest("Password must not be equal to the username!");

            //var validate = await _unitOfWork.Users.ValidationPassword(user);

            //if (validate == true)
            //    return BadRequest("The password cannot be changed because you entered the same password!");

            await _unitOfWork.Users.UpdateUserInfo(user);
            await _unitOfWork.CompleteAsync();

            return Ok("Successfully updated!");

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
        [Route("InactiveUser")]
        public async Task<IActionResult> InActiveUser([FromBody]User user)
        {
            await _unitOfWork.Users.InActiveUser(user);
            await _unitOfWork.CompleteAsync();

            return Ok("Successfully inactive user!");
        }


        [HttpPut]
        [Route("ActivateUser")]
        public async Task<IActionResult> ActivateUser(User user)
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



        [HttpPost]
        [Route("AddNewUsersImport")]
        public async Task<IActionResult> AddNewUsersImport([FromBody] User[] users)
        {

            if (ModelState.IsValid != true) return new JsonResult("Something went wrong!") { StatusCode = 500 };
            {

                List<User> DuplicateList = new List<User>();
                List<User> AvailableImport = new List<User>();
                List<User> UserRoleNotExist = new List<User>();
                List<User> EmpIdNULL = new List<User>();
                List<User> FullNameNULL = new List<User>();
                List<User> DepartmentNULL = new List<User>();
                List<User> FullNameIncomplete = new List<User>();





                foreach (User items in users)
                {

                    UserRole userRole = await _unitOfWork.Roles.GetByCodeAsync(items.UserRoleName);
                    if (userRole == null)
                    {
                        UserRoleNotExist.Add(items);
                        continue;
                    }
                    items.UserRoleId =userRole.Id;


                    if (users.Count(x => x.EmpId == items.EmpId && x.FullName == items.FullName ) > 1)
                    {
                        DuplicateList.Add(items);
                        continue;
                    }

                 


                    var validateDuplicate = await _unitOfWork.Users.ValidateEmpIdAndFullName(items.EmpId, items.FullName);
                    //var validationFullname = await _unitOfWork.Users.GenerateUsername(items.FullName);
                    string fullName = items.FullName?.Trim();
                    string[] nameParts = fullName?.Split(',');

                    //string[] nameParts = items.FullName.Split(',');


                    if (items.EmpId == string.Empty || items.EmpId == null)
                    {
                        EmpIdNULL.Add(items);
                        continue;
                    }

                    if (items.FullName == string.Empty || items.FullName == null)
                    {
                        FullNameNULL.Add(items);
                        continue;
                    }
                    
                    if(items.Department == string.Empty || items.Department == null)
                    {
                        DepartmentNULL.Add(items);
                        continue;
                    }
                  

                    if (validateDuplicate == true)
                        {
                            DuplicateList.Add(items);
                        }


                    if(nameParts.Length < 2 || nameParts.Any(part => part.Trim().Contains(" ")) || nameParts[1].Trim().Length == 0 || nameParts[0].Trim().Length == 0)
                    {
                        FullNameIncomplete.Add(items);
                        continue;
                    }

                        else
                        {
                            AvailableImport.Add(items);
                            await _unitOfWork.Users.AddNewUserImport(items);
                        }

                    
                }


                    var resultList = new
                    {
                        AvailableImport,
                        DuplicateList,
                        UserRoleNotExist,
                        EmpIdNULL,
                        FullNameNULL,
                        DepartmentNULL,
                        FullNameIncomplete,
                     


                    };


                    if (DuplicateList.Count == 0 && UserRoleNotExist.Count == 0 && EmpIdNULL.Count == 0 && FullNameNULL.Count == 0 && FullNameIncomplete.Count == 0 && DepartmentNULL.Count == 0)
                    {
                        await _unitOfWork.CompleteAsync();
                        return Ok("Successfully Add!");
                    }
                    else
                    {
                        return BadRequest(resultList);
                    }


            }


            
        }





    }
}
