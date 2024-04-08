using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.BORROWED_MODEL
{
    public  class BorrowedIssueDetails : BaseEntity 
    {

        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }

        public string ItemCode { get; set; }

        public string ItemDescription { get; set; }


        public string Uom { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        [Column(TypeName = "Date")]
        public DateTime BorrowedDate { get; set; }

        public DateTime PreparedDate { get; set; }

        public string PreparedBy { get; set; }

        public int WarehouseId { get; set; }    

        public int BorrowedPKey { get; set; }

        public string Remarks { get; set; }

        public bool IsActive { get; set; }

        public bool ? IsTransact { get; set; }

        public bool ? IsReturned { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Consume { get; set; }

        //[Column(TypeName = "decimal(18,2)")]
        //public decimal ReturnQuantity { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? ReturnedDate { get; set; }



        [Column(TypeName = "Date")]
        public DateTime? IsApprovedDate { get; set; }
        public bool  IsApproved { get; set; }

        public bool? IsReject { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? IsRejectDate { get; set; }


        [Column(TypeName = "Date")]
        public DateTime? IsApprovedReturnedDate { get; set; }
        public bool ? IsApprovedReturned { get; set; }



        public string ApproveBy { get; set; }
        public string ApprovedReturnedBy { get; set; }
        public string RejectBy { get; set; }



        public string ReturnBy { get; set; }

        public int AgingDays { get; set; }

        public string ItemRemarks { get; set; }

        public decimal UnitPrice { get; set; }

        public string EmpId { get; set; }
        public string FullName { get; set; }






    }
}
