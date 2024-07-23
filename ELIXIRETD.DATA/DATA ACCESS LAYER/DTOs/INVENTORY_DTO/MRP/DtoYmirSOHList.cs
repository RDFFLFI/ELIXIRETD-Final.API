using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORY_DTO.MRP
{
    public class DtoYmirSOHList
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public int bufferLevel {  get; set; } 
        public decimal stockOnHand { get; set; }
        public decimal averageIssuance { get; set; }
    }
}
