using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.FUEL_REGISTER_DTO
{
    public record class GetDriverUserDto
    {
        public int Id { get; set; }
        public string EmpId { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }
}
