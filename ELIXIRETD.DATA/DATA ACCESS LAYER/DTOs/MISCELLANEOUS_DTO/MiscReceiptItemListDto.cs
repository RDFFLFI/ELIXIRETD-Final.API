using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO
{
    public class MiscReceiptItemListDto
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string UomCode { get; set; }
        public decimal UnitCost { get; set; }
    }
}
