using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO.ConsolidationDto
{
    public class GeneralLedgerReportDto
    {
        public string Mark { get; set; }
        public string Mark_2 { get; set; }

        public string Cip_No { get; set; }
        public string Accounting_Tag { get; set; }

        public DateTime Transaction_Date  { get; set; }

        public string Supplier {  get; set; }
        public string Company_Code { get; set; }

        public string Company_Name { get; set; }

        public string Business_Unit_Code { get; set; }

        public string Business_Unit_Name { get;set; }

        public string Department_Code { get; set; }
        public string Department_Name { get; set; }

        public string Unit_Code { get; set; }

        public string Unit_Name { get; set; }

        public string Sub_Unit_Code { get; set; }
        public string Sub_Unit_Name { get; set; }

        public string Location_Code { get; set; }
        public string Location_Name { get; set; }   

        public string Account_Title_Code { get; set; }
        public string Account_Title_Name { get;  set; }

        public string Reference_No { get; set; }    

        public string Po {  get; set; }

        public string Item_Code { get; set; }

        public string Description { get; set; }

        public string Category {  set; get; }
        public decimal ? Quantity { get; set; }

        public string Uom {  get; set; }

        public decimal? Unit_Price { get; set; }
        public decimal? Line_Amount { get; set; }

        public string Voucher_No { get; set; }

        public string Transaction_No { get; set; }

        public string Transaction_Type { get; set; }

        public string DR_CR { get; set; }

        public string Asset_Code { get; set; }

        public string Asset { set; get; }
        public string Cip { get; set; }

        public int? Helpdesk_No { get; set; }

        public string Service_Provider_Code { get; set; }

        public string Service_Provider_Name { get;  set;}

        public string BOA { get; set; }
        public string Allocation_No { get; set; }

        public string Batch { get; set; }

        public string Reason { get; set; }

        public string Remarks { get; set; }

        public int Month {  get; set; }

        public int Year { get; set; }

        public string Divisible { get; set; }   

    }
}
