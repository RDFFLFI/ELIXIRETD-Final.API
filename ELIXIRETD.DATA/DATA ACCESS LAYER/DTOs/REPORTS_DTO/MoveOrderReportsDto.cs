using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO
{
    public class MoveOrderReportsDto
    {

        public int ItemCount { get; set; }
        public int MIRId { get; set; }
        public int OrderNoGenus { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int BarcodeNo { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; } 
        public string Uom { get; set; }
        public string ItemRemarks { get; set; }
        public string Status { get; set; }
        public string ApprovedDate { get; set; }
        public string DeliveryDate { get; set; }

        public decimal OrderedQuantity { get; set; }
        public decimal ServedOrder {  get; set; }
        public decimal UnservedOrder { get; set; }

        public decimal PreparedItem { get; set; }
        public decimal SOH { get; set; }

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
        public string AssetTag { get; set; }
        public string Cip_No { get; set; }
        public int? HelpdeskNo { get; set; }
        public string IsRush { get; set; }
        public string Remarks { get; set; }



    }
}
