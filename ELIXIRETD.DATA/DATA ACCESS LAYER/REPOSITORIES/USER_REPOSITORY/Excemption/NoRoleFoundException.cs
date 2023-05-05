using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.USER_REPOSITORY.Excemption
{
    public class NoRoleFoundException : Exception
    {
      public  NoRoleFoundException() :base ("No Role Found") { }

        

    }
}
