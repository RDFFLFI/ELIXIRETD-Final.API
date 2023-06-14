using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.Notification_Dto
{
    public class DtoForTransactNotif
    {

        public int MIRId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }

        public decimal TotalOrder { get; set; }

        public string OrderDate { get; set; }
        public string DateNeeded { get; set; }

        public string PrepareDate { get; set; }
        public bool IsApproved { get; set; }
        public string Rush { get; set; }
        

       


    }

}
