using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.JWT.AUTHENTICATION
{
    public class AutenticateNewPassword
    {

        public string Username { get; set; }

        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }


        [Required]
        //[Compare("NewPassword", ErrorMessage = "New password and confirm password do not match!")]
        public string ConfirmPassword { get; set; }
    }
}
