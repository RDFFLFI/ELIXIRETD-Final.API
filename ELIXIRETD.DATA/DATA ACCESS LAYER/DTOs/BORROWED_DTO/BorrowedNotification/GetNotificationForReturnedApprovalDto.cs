using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO.BorrowedNotification
{
    public class GetNotificationForReturnedApprovalDto
    {

        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }

        public bool IsApprovedReturned { get; set; }
        public bool IsActive { get; set; }
        public string ReturnedDate { get; set; }
    }
}
