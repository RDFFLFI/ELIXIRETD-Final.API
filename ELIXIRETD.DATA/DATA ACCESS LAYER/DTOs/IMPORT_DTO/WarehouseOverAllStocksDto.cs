using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.IMPORT_DTO
{
    public class WarehouseOverAllStocksDto
    {
        public int WarehouseId {  get; set; }
        public string ItemCode { get; set; }    
        public string ItemDescription { get; set; }
        public decimal Quantity { get; set; }
    }
}
