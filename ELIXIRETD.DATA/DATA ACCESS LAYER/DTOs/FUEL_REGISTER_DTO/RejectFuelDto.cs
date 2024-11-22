using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.FUEL_REGISTER_DTO
{
    public class RejectFuelDto
    {
        public int Id {  get; set; }
        public string Reject_Remarks { get; set; }
        public string Reject_By { get; set; }
    }
}
