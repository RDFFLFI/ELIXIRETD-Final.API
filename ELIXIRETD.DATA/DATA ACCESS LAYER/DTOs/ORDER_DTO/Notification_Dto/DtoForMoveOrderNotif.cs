using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.Notification_Dto
{
    public class DtoForMoveOrderNotif
    {

        public int MIRId { get; set; }
        public string CustomerName { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }

        public bool Rush { get; set; }

    }
}
