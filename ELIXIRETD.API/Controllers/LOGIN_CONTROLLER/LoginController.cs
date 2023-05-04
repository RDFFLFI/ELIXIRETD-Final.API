using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ELIXIRETD.DATA.JWT.AUTHENTICATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;

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
