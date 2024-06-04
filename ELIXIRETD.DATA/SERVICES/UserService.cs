 using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.JWT.AUTHENTICATION;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ELIXIRETD.DATA.SERVICES
{
    public  class UserService : IUserService
    {
        private readonly StoreContext _context;
        private readonly IConfiguration _configuration;

        public UserService(
                                   StoreContext context,
                                   IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        
      

        public AuthenticateResponse Authenticate(AuthenticateRequest request)
        {
            var user = _context.Users.SingleOrDefault(x => x.UserName == request.Username
                                                        && x.Password == request.Password // remove it if encrypted the password
                                                        && x.IsActive != false);
            if (user == null)
                return null;

            //if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password)) // for encrypted Password
            //    return null;

            var token = generateJwtToken(user);
            return new AuthenticateResponse(user, token , _context);
        }

        public  async Task<bool> NewPassword(AutenticateNewPassword newpassword)
        {
            var newpass = await _context.Users.Where(x => x.UserName == newpassword.Username &&
                                                      x.Password != x.Password && x.IsActive == true)
                                              .FirstOrDefaultAsync();

            if(newpass == null) 
                return false;

            newpass.Password = newpassword.OldPassword;
            newpass.Password = newpassword.NewPassword;
            newpass.Password = newpassword.ConfirmPassword;

            return true;

        }

        private string generateJwtToken(User user)
        {
            var key = _configuration.GetValue<string>("JwtConfig:Key");
            var keyBytes = Encoding.ASCII.GetBytes(key);

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor()
            {

                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim("id", user.Id.ToString()),
                    new Claim(ClaimTypes.Name , user.FullName)

                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials
               (new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)

            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
    }
}
