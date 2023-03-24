using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO
{
    public class DtoBorrowedAndReturned
    {

     public int BorrowedId { get; set; }
      
     public string CustomerName { get; set; }
      
     public string CustomerCode { get; set; }

     public string ItemCode { get; set; }
     
        public string ItemDescription { get; set; }
        public decimal BorrowedQuantity { get; set; }

        public string TransactedBy { get; set; }

     public string Uom { get; set; }

        public string Remarks { get; set; }

        public decimal Consumes { get; set; }

    public decimal  ReturnQuantity { get; set; }

        public string BorrowedDate { get; set; }
      
     




    }
}
