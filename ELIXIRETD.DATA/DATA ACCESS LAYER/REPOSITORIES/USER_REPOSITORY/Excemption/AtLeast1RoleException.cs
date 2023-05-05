using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.USER_REPOSITORY.Excemption
{
    public class AtLeast1RoleException : Exception
    {
        public AtLeast1RoleException() : base("At least one mainmenu and module must be tagged for the specified role") {  }
    }
}
