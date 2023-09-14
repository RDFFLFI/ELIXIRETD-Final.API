using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO
{
    public class ListofServedDto
    {
        public int TransactId { get; set; }

        public string DateNeeded { get; set; }
        public string OrderDate { get; set; }

        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public List<Orders> Order { get; set; }


        public class Orders
        {
            public int OrderNo { get; set; }
            public string CustomerType { get; set; }

            public string ItemCode { get; set; }

            public string ItemCategory { get; set; }

            public string Uom { get; set; }

            public decimal QuantityOrdered { get; set; }


            public string CompanyCode { get; set; }

            public string CompanyName { get; set; }


            public string DepartmentCode { get; set; }
            public string DepartmentName { get; set; }

            public string LocationCode { get; set; }
            public string LocationName { get; set; }

            public string Rush { get; set; }

            public string ItemRemarks { get; set; }

            public decimal? QuantityServed { get; set; }

            public decimal ? QuantityUnServed { get; set; }
        }


    }
}
