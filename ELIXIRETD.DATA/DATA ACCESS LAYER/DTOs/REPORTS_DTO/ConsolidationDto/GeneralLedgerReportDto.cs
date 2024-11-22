using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO.ConsolidationDto
{
    public class GeneralLedgerReportDto
    {
        public int ? SyncId { get; set; }
        public string Mark { get; set; }
        public string Mark_2 { get; set; }
        public string Asset_Cip { get; set; }
        public DateTime Transaction_Date { get; set; }
        public string Accounting_Tag { get; set; }
        public string Supplier {  get; set; }
        public string Account_Title_Code { get; set; }
        public string Account_Title_Name { get; set; }

        public string Company_Code { get; set; }
        public string Company_Name { get; set; }

        

        public string Division_Code { get; set; }
        public string Division { get; set; }

        public string Business_Unit_Code { get; set; }
        public string Business_Unit_Name { get; set; }

        public string Department_Code { get; set; }
        public string Department_Name { get; set; }

        public string Unit_Code { get; set; }

        public string Unit { get; set; }

        public string Sub_Unit_Code { get; set; }
        public string Sub_Unit { get; set; }

        public string Location_Code { get; set; }
        public string Location { get; set; }

        public string Po { get; set; }

        public string Reference_No { get; set; }    

        public string Item_Code { get; set; }

        public string Description { get; set; }

        public decimal? Quantity { get; set; }

        public string Category {  set; get; }


        public string Uom {  get; set; }

        public decimal? Unit_Price { get; set; }
        public decimal? Line_Amount { get; set; }

        public string Voucher_No { get; set; }

        public string Account_Type { get; set; }

        public string DR_CR {  get; set; }


        public string Asset_Code { get; set; }

        public string Asset { get; set; }

        public string Service_Provider_Code { get; set; }
        public string Service_Provider { get; set; }

        public string BOA { get; set; }

        public string Allocation { get; set; }

        public string Account__Group {  set; get; }

        public string Account_Sub_Group { get; set; }

        public string Financial_Statement { get; set; }

        public string Unit_Responsible { get; set; }

        public string Batch {  get; set; }

        public string Remarks { get; set; }
        public string Position { get; set; }
        public string Payroll_Period_1 { get; set; }

        public string Payroll_Period_2 { get; set; }


        public string Additional_Description_DEPR { get; set; }

        public string Remaining_By_For_DEPR { get; set; }

        public string Useful_Life { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }
        
        public string Particular { get; set; }

        public string Month_2 { get; set; }
        public string Farm_Type { get; set; }

        public string Jean_Remarks { get; set; }

        public string From {  get; set; }

        public string Change_To { get; set; }

        public string Reason { get; set; }

        public string Checking_Remarks { get; set; }

        public string BOA_2 { get; set; }

        public string System {  get; set; }

        public string Books { get; set; }






    }
}
