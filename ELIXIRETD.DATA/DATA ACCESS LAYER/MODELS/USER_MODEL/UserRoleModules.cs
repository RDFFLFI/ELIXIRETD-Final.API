using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL
{
    public class UserRoleModules : BaseEntity
    {
        public int RoleId { get; set; }
        public int ModuleId { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
