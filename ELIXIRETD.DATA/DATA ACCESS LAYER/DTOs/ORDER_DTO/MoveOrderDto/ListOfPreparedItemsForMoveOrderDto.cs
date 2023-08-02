using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.MoveOrderDto
{
    public class ListOfPreparedItemsForMoveOrderDto
    {
        public int Id { get; set; }

        public int OrderNo { get; set; }

        public int BarCodes { get; set; }

        public string ItemCode { get; set; }
        public string  ItemDescription { get; set; }

        public decimal Quantity { get; set; }

        public bool IsActive { get; set; }

        public string Rush { get; set; }
        public string ItemRemarks { get; set; }

        public decimal  UnitCost { get; set; }

        public  decimal TotalCost { get; set; }

    }
}
