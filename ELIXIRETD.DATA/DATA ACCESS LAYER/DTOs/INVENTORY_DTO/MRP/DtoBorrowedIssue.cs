using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORY_DTO.MRP
{
    public class DtoBorrowedIssue
    {
        public int WarehouseId { get; set; }
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }   
        public decimal ReturnQuantity { get; set; }
        public string PreparedDate { get; set; }
        public decimal ConsumeQuantity { get; set; }

    }
}
