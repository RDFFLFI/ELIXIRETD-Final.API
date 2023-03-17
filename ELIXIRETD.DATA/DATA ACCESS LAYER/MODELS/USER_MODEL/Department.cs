using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL
{
    public class Department : BaseEntity
    {

        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string AddedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public string Reason { get; set; }


    }
}
