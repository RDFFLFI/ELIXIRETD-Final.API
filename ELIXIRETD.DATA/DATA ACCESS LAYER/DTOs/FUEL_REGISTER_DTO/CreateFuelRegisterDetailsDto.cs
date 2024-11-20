using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.FUEL_REGISTER_DTO
{
    public class CreateFuelRegisterDetailsDto
    {
        public int? Id { get; set; }

        public string Item_Code { get; set; }

        public int? Warehouse_ReceivingId { get; set; }

        public decimal Liters { get; set; }
        public string Asset { get; set; }

        public decimal? Odometer { get; set; }
        public string Added_By { get; set; }
        public string Modified_By { get; set; }


    }
}
