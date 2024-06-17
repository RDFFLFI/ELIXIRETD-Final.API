using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO.ConsolidationDto
{
    public class BorrowedConsolidatedDto
    {
        public int Id { get; set; }
        public int BorrowedId { get; set; }

        public string CustomerName { get; set; }

        public string CustomerCode { get; set; }

        public string EmpIdByIssue { get; set; }

        public string FullNameByIssue { get; set; }

        public string BorrowedDate { get; set; }

        public string Remarks { get; set; }

        public string Details { get; set; }

        public string Status { get; set; }

        public string TransactedBy { get; set; }

        public string ItemCode { get; set; }

        public string ItemDescription { get; set; }
        public string Uom { get; set; }

        public decimal BorrowedQuantity { get; set; }
        public decimal Consumed { get; set; }
        public decimal ReturnedQuantity { get; set; }

        public string IsApproveReturnDate { get; set; }

        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }

        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }

        public string LocationCode { get; set; }
        public string LocationName { get; set; }

        public string AccountCode { get; set; }
        public string AccountTitles { get; set; }

        public string EmpId { get; set; }
        public string FullName { get; set; }

        public int? ReportNumber { get; set; }

        public int? AgingDays { get; set; }
        public bool IsActive { get; set; }
        public decimal ? UnitPrice { get; set; }
    }
}
