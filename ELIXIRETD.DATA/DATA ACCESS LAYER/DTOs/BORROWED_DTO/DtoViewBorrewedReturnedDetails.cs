using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO
{
    public class DtoViewBorrewedReturnedDetails
    {

        public int Id { get; set; }
        public int BorrowedPKey { get; set; }
        public string CustomerCode { get; set; }
        public string Customer { get; set; }
        public string EmpId { get; set; }
        public string FullName { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public decimal Consume { get; set; }
        public string ReturnedDate { get; set; }

        public decimal ReturnQuantity { get; set; }

        public decimal BorrowedQuantity { get; set; }

    
        public string PreparedBy { get; set; }

        public string BorrowedDate { get; set; }

     

        //public string CompanyCode { get; set; }
        //public string CompanyName { get; set; }

        //public string DepartmentCode { get; set; }
        //public string DepartmentName { get; set; }

        //public string LocationCode { get; set; }
        //public string LocationName { get; set; }

        //public string AccountCode { get; set; }
        //public string AccountTitles { get; set; }

        //public string FullName { get; set; }

        //public string EmpId { get; set; }

        public string Remarks { get; set; }

        public string TransactionDate { get; set; }

        public string Reason { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }

        public string Details { get; set; }









    }
}
