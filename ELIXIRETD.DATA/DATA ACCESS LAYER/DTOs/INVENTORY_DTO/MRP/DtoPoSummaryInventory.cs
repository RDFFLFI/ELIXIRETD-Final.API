using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORY_DTO.MRP
{
    public class DtoPoSummaryInventory
    {
        public string ItemCode { get; set; }
        public decimal Ordered { get; set; }

        public decimal ActualGood { get; set; }

    }
}
