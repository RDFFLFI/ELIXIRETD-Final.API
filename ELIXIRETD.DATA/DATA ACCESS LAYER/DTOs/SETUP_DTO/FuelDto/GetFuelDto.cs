using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO.FuelDto
{
    public class GetFuelDto
    {
        public int Id { get; set; }

        public int? MaterialId { get; set; }
        public string Item_Code { get; set; }
        public string Item_Description { get; set; }

        public string Added_By {  get; set; }
        public DateTime Created_At { get; set; }

        public string Modified_By { get; set; }
        public DateTime? Updated_At { get; set;}

        public bool Is_Active { get; set; }

    }
}
