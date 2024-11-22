
using System.ComponentModel.DataAnnotations.Schema;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL
{
    public class Ordering : BaseEntity
    {
        public int MIRId { get; set; }
        public int TrasactId { get; set; }

        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }

        public string DepartmentCode { get; set; }
        public string Department { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }

        public string AccountCode { get; set; }
        public string AccountTitles { get; set; }

        public string EmpId { get; set; }

        public string FullName { get; set; }

        //public string AccountCode { get; set; }
        //public string AccountTitles { get; set; }
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

        public string Cip_No { get; set; }
        public int ? HelpdeskNo { get; set; }
        public string Rush { get; set; }
        public bool ? IsRush { get; set; }

        public string CustomerType { get; set; }

     
        public string ItemRemarks { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal StandartQuantity { get; set; }

        public string AssetTag { get; set; }

        public string Requestor { get; set; }

        public string Approver { get; set; }

        [Column(TypeName = "Date")]
        public DateTime ? DateApproved { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? Modified_Date { get;set; }
        public string Modified_By { get; set;}

        public string Asset_Code { get; set; }
        public string Asset_Name { get; set; }

        public string Plate_No { get; set; }

    }
}
