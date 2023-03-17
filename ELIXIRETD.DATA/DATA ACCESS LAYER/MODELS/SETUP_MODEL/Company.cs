using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class Company : BaseEntity
    {

        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public bool IsActive { get; set; } = true;
        public string AddedBy { get; set; }
        public DateTime DateAdded { get; set; }




    }
}
