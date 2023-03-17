using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.MoveOrderDto
{
    public class GetMoveOrderDetailsForMoveOrderDto
    {

        public int Id { get; set; }
        public int OrderNo { get; set; }
        public string CustomerName { get; set; }

        public string CustomerCode { get; set; }

        public string ItemCode { get; set; }

        public string ItemDescription { get; set; }

        public string Uom { get; set; }

        public decimal QuantityOrder { get; set; }

        public string Category { get; set; }

        public string OrderDate { get; set; }

        public string DateNeeded { get; set; }

        public string PrepareDate { get; set; }
        public string Address { get; set; }




    }
}
