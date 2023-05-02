using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL
{
    public class MainMenu : BaseEntity
    {

        
        public string ModuleName { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public string AddedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public string MenuPath { get; set; }
        public string Reason { get; set; }
        public int MenuOrder { get; set; }


    }
}
