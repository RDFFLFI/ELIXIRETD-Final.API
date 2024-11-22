using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORY_DTO.MRP
{
    public class DtoYmirSOHList
    {
        public string id { get; set; }
        public string ItemDescription { get; set; }
        public int BufferLevel {  get; set; } 
        public decimal Reserve { get; set; }
        public decimal AverageIssuance { get; set; }
    }
}
