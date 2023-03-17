using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORY_DTO.MRP
{
    public class DtoGetAllAvailableInRawmaterialInventory
    {

        public string ItemCode { get; set; }    
        public string ItemDescription { get; set; }
        public string LotCategory { get; set; } 

        public string Uom { get; set; }

        public decimal SOH { get; set; }
        public decimal ReceiveIn { get; set; }
        public decimal RejectOrder { get; set; }
        public bool ? IsWarehouseReceived { get; set; }   

    }
}
