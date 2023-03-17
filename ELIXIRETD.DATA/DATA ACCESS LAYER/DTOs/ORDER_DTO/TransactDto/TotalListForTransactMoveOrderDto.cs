using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.TransactDto
{
    public class TotalListForTransactMoveOrderDto
    {

        public int OrderNo { get; set; }

        public string CustomerName { get; set; }

        public string CustomerCode { get; set; }


        public string Category { get; set; }

        public decimal TotalOrders { get; set; }

        public string DateNeeded { get; set; }

        public string PreparedDate { get; set; }

        public bool IsApproved { get; set; }    

    }
}
