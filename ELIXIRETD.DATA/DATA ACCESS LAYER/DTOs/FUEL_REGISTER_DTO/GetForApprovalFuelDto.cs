using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.FUEL_REGISTER_DTO
{
    public class GetForApprovalFuelDto
    {
        public int Id { get; set; }

        public int? MaterialId { get; set; }
        public string Item_Code { get; set; }
        public string Item_Description { get; set; }

        public string Uom { get; set; }
        public string Item_Categories { get; set; }

        public int? Warehouse_ReceivingId { get; set; }

        public decimal Unit_Cost { get; set; }

        public decimal Liters { get; set; }
        public string Asset { get; set; }

        public decimal? Odometer { get; set; }

        public string Added_By { get; set; }
        public DateTime Created_At { get; set; }
        public string Modified_By { get; set; }
        public DateTime? Updated_At { get; set; }
    }
}
