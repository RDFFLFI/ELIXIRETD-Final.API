using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.Notification_Dto
{
    public class DtoForApprovalMoveOrderNotif
    {
        public int MIRId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public decimal Quantity { get; set; }
        public string OrderDate { get; set; }
        public string PreparedDate { get; set; }

        public bool Rush { get;set; }

        public bool IsMove { get; set; }

    }
}
