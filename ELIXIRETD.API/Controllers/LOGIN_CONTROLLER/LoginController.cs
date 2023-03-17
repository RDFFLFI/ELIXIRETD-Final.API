using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ELIXIRETD.DATA.JWT.AUTHENTICATION;

namespace ELIXIRETD.API.Controllers.LOGIN_CONTROLLER
{
  
    public class LoginController : BaseApiController
    {
        private readonly IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
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



    }
}
