using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.USER_DTO
{
    public class UserDto
    {
        public int Id { get; set; }
        public string EmpId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public int UserRoleId { get; set; }
        public string UserRole { get; set; }
        public string Department { get; set; }
        public string DateAdded { get; set; }
        public string AddedBy { get; set; }
        public string ModifiedBy { get; set; }


    }
}
