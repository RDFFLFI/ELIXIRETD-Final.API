using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO
{
    public class DtoTransactReports
    {

        public int MIRId { get; set; }
        public string Requestor { get; set; }
        public string Approver { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public decimal Quantity { get; set; }
        public string MoveOrderDate { get; set; }
        public string MoveOrderBy { get; set; }
        public string TransactedBy { get; set; }
        public bool TransactionType { get; set; }
        public string TransactedDate { get; set; }
        public string DeliveryDate { get; set; }

        public string DateNeeded { get; set; }

        public string OrderDate { get; set; }

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


        public string CustomerType { get; set; }

        public string ItemRemarks { get; set; }


        public decimal UnitCost { get; set; }

        public decimal LineAmount { get; set; }

        public string Cip_No { get; set; }

        public int ? HelpDesk { get; set; }

        public string Remarks { get; set; }

        public string Rush {  get; set; }

        public string Status { get; set; }
        public string Category { get; set; }

        public string AssetTag { get; set; }

        public string DateApproved { get; set; }

       

    }
}
