using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO
{
    public class ItemStocksDto
    {

        public int BorrowedItemPkey { get; set; }
        public int warehouseId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
       public string DateReceived { get; set; } 
       public decimal ActualGood { get; set; }     
        public decimal Remaining { get; set; }

        public decimal Out { get; set; }
        public decimal In { get; set; }
        public decimal TotalMoveOrder { get; set; }

        public decimal Consume { get; set; }
    }
}
