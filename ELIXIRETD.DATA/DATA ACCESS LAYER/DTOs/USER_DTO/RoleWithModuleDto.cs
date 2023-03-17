using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.USER_DTO
{
    public class RoleWithModuleDto
    {
        public string RoleName { get; set; }
        public string MainMenu { get; set; }
        public int MainMenuId { get; set; }
        public string SubMenu { get; set; }
        public string ModuleName { get; set; }
        public string MenuPath { get; set; }
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
        public int MenuOrder { get; set; }

    }
}
