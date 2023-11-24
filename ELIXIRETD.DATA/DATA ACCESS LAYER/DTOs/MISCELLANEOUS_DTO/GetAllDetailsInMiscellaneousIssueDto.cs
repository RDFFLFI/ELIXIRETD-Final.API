using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO
{
    public class GetAllDetailsInMiscellaneousIssueDto
    {

        public int Id { get; set; }
        public int IssuePKey { get; set; }

        public string Customer { get; set; }

        public string CustomerCode { get; set; }

        public string PreparedDate { get; set; }

        public string PreparedBy { get; set; }

        public string ItemCode { get; set; }

        public string ItemDescription { get; set; }

        public decimal TotalQuantity { get; set; }

        public string Remarks { get; set; }

        public string TransactionDate { get; set; }
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


        public string Uom { get; set; }


        public decimal UnitCost { get; set; }

        public decimal TotalCost { get; set; }

        public string Details { get; set; }

    }

}
