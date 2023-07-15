using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO.BorrowedNotification
{
    public class RejectBorrowedNotificationDto
    {


        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public decimal TotalQuantity { get; set; }
        public bool IsActive { get; set; }
        public bool ? IsReject { get; set; }
       
 
    }
}
