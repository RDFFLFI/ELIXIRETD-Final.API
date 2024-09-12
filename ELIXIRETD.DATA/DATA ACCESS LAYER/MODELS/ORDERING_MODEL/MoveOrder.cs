﻿using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL
{
    public class MoveOrder : BaseEntity
    {
        public int OrderNo { get; set; }
        public int OrderNoGenus { get; set; }
        public string Department { get; set; }
        public string CustomerName { get; set; }
        public string Customercode { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }

        [Column (TypeName ="decimal(18,2)")]
        public decimal QuantityOrdered { get; set; }
        public string Category { get; set; }

        [Column(TypeName = "Date")]
        public DateTime OrderDate { get; set; }

        [Column(TypeName ="Date")]
        public DateTime DateNeeded { get; set; }
        
        public int WarehouseId { get; set; }

        public bool IsActive { get; set; }

        public bool? IsApprove { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? ApprovedDate { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? ApproveDateTempo { get; set; }

        public bool IsPrepared { get; set; }

        public string PreparedBy { get; set; }
        public DateTime ? PreparedDate { get; set; }
        public bool? IsCancel { get; set; }
        public string CancelBy { get; set; }

        public DateTime? CancelledDate { get; set; }

        public int OrderNoPkey { get; set; }
        public bool? IsReject { get; set; }
        public string RejectBy { get; set; }
        
        public DateTime? RejectedDate { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? RejectedDateTempo { get; set; }

        public string Remarks { get; set; }

        public bool IsTransact { get; set; }

        public bool? IsPrint { get; set; }

        public bool? IsApproveReject { get; set; }

        public string CompanyCode { get; set; }
        public string CompanyName { get; set;}

        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set;}

        public string LocationCode { get; set; }
        public string LocationName { get; set;}

        public string AccountCode { get; set; }
        public string AccountTitles { get; set; }

        public string AddressOrder { get; set; }

        public string Rush { get; set; }    

        public string CustomerType { get; set; }

        public string ItemRemarks { get; set; }

        public decimal UnitPrice { get; set; }

        public string EmpId { get; set; }

        public string FullName { get; set; }

        public string Cip_No { get; set; }

        public string AssetTag { get; set; }

        public int ? HelpdeskNo { get; set; }


        public string Requestor { get; set; }

        public string Approver { get; set; }

        [Column(TypeName = "Date")]
        public DateTime ? DateApproved { get; set; }

        public string Asset_Code { get; set; }
        public string Asset_Name { get; set; }

        public string Plate_No { get; set; }


    }
}
