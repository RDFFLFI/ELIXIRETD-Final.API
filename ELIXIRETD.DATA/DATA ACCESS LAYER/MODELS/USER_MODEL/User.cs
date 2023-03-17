using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL
{
    public  class User : BaseEntity
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; } = true;
        public UserRole UserRole { get; set; }
        public int UserRoleId { get; set; }
        public Department Department { get; set; }
        public int DepartmentId { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public string AddedBy { get; set; }
        public string Reason { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? DateModified { get; set; }


    }
}
