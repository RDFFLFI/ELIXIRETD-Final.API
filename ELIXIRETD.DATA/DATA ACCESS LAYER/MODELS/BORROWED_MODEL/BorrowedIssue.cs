using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.BORROWED_MODEL
{
     public class BorrowedIssue : BaseEntity
    {
        public string CustomerName { get; set; }

        public string CustomerCode { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalQuantity { get; set; }

        public DateTime PreparedDate { get; set; }

        public string PreparedBy { get; set; }

        public string Details { get; set; }

        public string Remarks { get; set;  }
        public bool IsActive { get; set; }

        public bool? IsTransact { get; set; }

        public bool? IsReturned { get; set; }

        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }

        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }

        public string LocationCode { get; set; }
        public string LocationName { get; set; }

        public string AccountCode { get; set; }
        public string AccountTitles { get; set; }


        // Update Borrowed

        [Column(TypeName = "Date")]
        public DateTime? IsApprovedDate { get; set; }
        public bool  IsApproved { get; set; }


        [Column(TypeName = "Date")]
        public DateTime? IsApprovedReturnedDate { get; set; }
        public bool ? IsApprovedReturned { get; set; }

        public bool ? IsReject { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? IsRejectDate { get; set; }


        public string ApproveBy { get; set; }
        public string ApprovedReturnedBy { get; set; }
        public string RejectBy { get; set; }

        public int AgingDays { get; set; }

        public string ReturnBy { get; set; }

        public DateTime TransactionDate { get; set; }


        public string Reason { get; set; }


        public string StatusApproved { get; set; }



    }
}
