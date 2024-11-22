using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO.ConsolidationDto
{
    public class ConsolidateAuditReportDto
    {
        public int Id { get; set; }
        public string TransactionDate { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public string Category { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Quantity { get; set; }
        public decimal? UnitCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? LineAmount { get; set; }
        public string? Source { get; set; }
        public string TransactionType { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Reference { get; set; }
        public string SupplierName { get; set; }
        public string EncodedBy { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string EmpId { get; set; }
        public string Fullname { get; set; }
        public string AccountTitleCode { get; set; }
        public string AccountTitle { get; set; }
        public string AssetTag { get; set; }
        public string CIPNo { get; set; }
        public int? Helpdesk { get; set; }
        public string Rush { get; set; }

    }
}
