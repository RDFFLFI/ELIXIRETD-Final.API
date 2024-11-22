using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.FUEL_REGISTER_DTO
{
    public class GetMaterialStockByWarehouseDto
    {
        public int WarehouseId { get; set; }    
        public string ItemCode { get; set; }

        public decimal? Remaining_Stocks { get; set; }
        public string Receiving_Date { get; set; }

        public decimal? Unit_Cost { get; set; }
    }
}
