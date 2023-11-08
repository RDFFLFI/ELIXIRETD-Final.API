using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.PreperationDto
{
    public class GetallApproveListDto
    {
        public int MIRId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public decimal TotalOrders { get; set; }
        public string PreparedDate { get; set; }

        public string OrderDate { get; set; }
        public string DateNeeded { get; set; }
        public bool IsRush { get; set; }


        public string Status { get; set; }

        public List<Orders> Order { get ; set; }


        public class Orders
        {
            public int OrderNo { get; set; }

            //public string DateNeeded { get; set; }

            public string ItemCode { get; set; }

            public string ItemDescription { get; set; }

            public string Category { get; set; }

            public string Uom { get; set; }

            public decimal QuantityOrder { get; set; }

            public string ItemRemarks { get; set; }

       




        }
    }
}
