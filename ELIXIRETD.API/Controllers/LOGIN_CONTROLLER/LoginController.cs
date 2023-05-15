using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ELIXIRETD.DATA.JWT.AUTHENTICATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;

namespace ELIXIRETD.API.Controllers.LOGIN_CONTROLLER
{

    public class LoginController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly StoreContext _context;

        public LoginController(IUserService userService, IUnitOfWork unitOfWork, StoreContext context)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
            _context = context;

        }


        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest request)
        {
            var response = _userService.Authenticate(request);

            if (response == null)
                return BadRequest(new { message = " User or Password is incorrect!" });

            return Ok(response);


        }

        [HttpPost("changepassword")]
        public  IActionResult ChangePassword(AutenticateNewPassword request)
        {
            var user =  _context.Users.SingleOrDefault(x => x.UserName == request.Username
                                                        && x.IsActive == true);
            if (user == null)
                return BadRequest("User or Password is incorrect!" );

            if (request.OldPassword != user.Password)
                return BadRequest("Password is incorrect!");

            if (request.NewPassword == user.Password && request.ConfirmPassword == user.Password)
                return BadRequest("New Password is same to the old Password!");

            if (request.NewPassword != request.ConfirmPassword)
                return BadRequest("New password and confirm password do not match!");

            user.Password = request.NewPassword;
            _context.SaveChanges();

            return Ok(new { message = "Password changed successfully!" });
        }


        //[HttpPut("NewPassword")]
        //public async Task<IActionResult> UpdateMiscellaneousIssuePKey([FromBody] AutenticateNewPassword newPasswords)
        //{
        //    var response = await _userService.();

        //    if (response == null)
        //        return BadRequest(new { message = " User or Password is incorrect!" });

        //    return Ok(response);


        //}


        //public IActionResult Authenticate(AuthenticateRequest request)
        //{
        //    var response = _userService.Authenticate(request);

        //    if (response == null)
        //        return BadRequest(new { message = " User or Password is incorrect!" });

        //    var roleModuleCount = _context.RoleModules.Where(x => x.RoleId == response.Role.Id && x.Module.IsMainMenu == true && x.IsActive == true).Count();
        //    var mainMenuCount = _context.Modules.Where(x => x.IsMainMenu == true && x.IsActive == true).Count();
        //    var roleModuleTaggedCount = _context.RoleModules.Where(x => x.RoleId == response.Role.Id && x.IsActive == true).Count();

        //    if (roleModuleTaggedCount == 0 || roleModuleCount != mainMenuCount)
        //        return BadRequest(new { message = "Needed to tag modules first." });

        //    return Ok(response);
        //}


    }
}
