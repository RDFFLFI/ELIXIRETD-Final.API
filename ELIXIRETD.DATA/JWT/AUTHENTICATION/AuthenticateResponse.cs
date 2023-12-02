using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;

namespace ELIXIRETD.DATA.JWT.AUTHENTICATION
{


    public class AuthenticateResponse
    {

        private readonly StoreContext _context;

        public int Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
        public string RoleName { get; set; }
      //  public string RoleName { get; set; }

        public string Deparment { get; set; }
        public string Token { get; set; }


        public AuthenticateResponse(User user, string token , StoreContext context)
        {
            _context = context;
          

            Id = user.Id;
            FullName = user.FullName;
            UserName = user.UserName;
            Password = user.Password;
            Role = user.UserRoleId;
            Token = token;
            Deparment = user.Department;

       

            var role = _context.Roles.FirstOrDefault(x => x.Id == Role);
            if (role != null)
            {
                RoleName = role.RoleName;
            }

        }


    }
}
