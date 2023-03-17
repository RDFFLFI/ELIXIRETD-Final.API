using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL
{
    public class Ordering : BaseEntity
    {
        public int TrasactId { get; set; }
        public string CustomerName { get; set; }
        public string Customercode { get; set; }

        public int OrderNo { get; set; }

        [Column(TypeName = "Date")]
        public DateTime OrderDate { get; set; } 

        [Column(TypeName ="Date")]
        public DateTime DateNeeded { get; set; }
  
        public string TransactionType { get; set; }
        public string ItemCode { get; set; }
        public string ItemdDescription { get; set; }
        public string Uom { get; set; }

        [Column(TypeName ="decimal(18,2)")]
        public decimal QuantityOrdered { get; set; }

        public string Category { get; set; }
        public bool IsActive { get; set; }
        public DateTime? PreparedDate { get; set; }

        public string PreparedBy { get; set; }

        public bool? IsApproved { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public bool? IsReject { get; set; }

        public string RejectBy { get; set; }

        public DateTime? RejectedDate { get ; set; }
        public bool IsPrepared { get; set; }
        public bool? IsCancel { get; set; }
        public string IsCancelBy { get; set; }
        public DateTime? CancelDate { get; set; }
        public string AddedBy { get; set; }
        public string Remarks { get; set; }
        public int OrderNoPKey { get; set; }
        public bool IsMove { get; set; }
        public DateTime? SyncDate { get; set; }

        public string AddressOrder { get; set; }


    }
}
