using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.PreperationDto
{
    public class GetAllCalendarApproveDto
    {
        public int MIRId { get; set; }

        public string Department { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }

        public string Category { get; set; }

        public string PreparedDate { get; set; }

        public bool IsMove { get; set; }

        public bool IsReject { get; set; }

        public string Remarks { get; set; }

        public decimal TotalOrders { get; set; }

        public bool IsRush { get; set; }

        public string Rush { get; set; }






    }
}
