using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL
{
    public class Module : BaseEntity
    {

        public MainMenu MainMenu { get; set; }

        public int MainMenuId { get; set; }
        public string SubMenuName { get; set; }
        public string ModuleName { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public string AddedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public string Reason { get; set; }

    }
}
