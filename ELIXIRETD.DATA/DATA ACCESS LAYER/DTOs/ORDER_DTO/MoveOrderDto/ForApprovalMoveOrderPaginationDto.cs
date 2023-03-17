using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.MoveOrderDto
{
    public class ForApprovalMoveOrderPaginationDto
    {

        public int OrderNo { get; set; }
        public string CustomerName { get; set; }

        public string Customercode { get; set; }
        public string Itemcode { get; set; }

        public string Category { get; set; }
        public decimal Quantity { get; set; }

        public string OrderDate { get; set; }

        public string PreparedDate { get; set; }

        public string Address { get; set; }



    }
}
