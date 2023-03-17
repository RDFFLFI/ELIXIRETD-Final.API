using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO
{
    public class OrderingInventory
    {
        public string ItemCode { get; set; }
        public decimal QuantityOrdered { get; set; }
        public int warehouseId { get; set; }
    }
}
