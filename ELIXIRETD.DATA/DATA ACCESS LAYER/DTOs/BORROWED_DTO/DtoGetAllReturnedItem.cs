using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO
{
    public class DtoGetAllReturnedItem
    {

        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode {get; set;}
        public string EmpId { get; set; }
        public string FullName { get; set; }
        public string PreparedBy { get; set; }
        public decimal ConsumedQuantity { get; set; }
        public string ReturnedDate { get; set; }
        public decimal TotalBorrowedQuantity { get; set; }

        public decimal ReturnedBorrow { get; set; }
        
        public string BorrowedDate { get; set; }

        public string ReturnBy { get; set; }

        public bool IsApproveReturn { get; set; }

        public bool IsReturned { get; set; }
        public string ApproveReturnDate { get; set; }
        public string IsApproveDate { get; set; }

        public int AgingDays { get; set; }
        public string StatusApprove { get; set; }

        public bool IsActive { get; set; }

        //public decimal UnitCost { get; set; }

        //public decimal TotalCost { get; set; }
        public string Details { get; set; }
    }
}
