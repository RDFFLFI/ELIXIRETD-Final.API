using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.MoveOrderDto
{
    public class ForApprovalMoveOrderPaginationDto
    {

        public int MIRId { get; set; }
        public string Department { get; set; }
        public string CustomerName { get; set; }

        public string Customercode { get; set; }
        public string Itemcode { get; set; }

        public string Category { get; set; }
        public decimal Quantity { get; set; }

        public string OrderDate { get; set; }

        public string PreparedDate { get; set; }

        public string Address { get; set; }

        public bool IsRush { get; set; }

        public string Rush { get; set; }

        public string Remarks { get; set; }


        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }

        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }

        public string LocationCode { get; set; }
        public string LocationName { get; set; }

        public string AccountCode { get; set; }
        public string AccountTitles { get; set; }

        public bool IsMove { get; set; }



    }
}
