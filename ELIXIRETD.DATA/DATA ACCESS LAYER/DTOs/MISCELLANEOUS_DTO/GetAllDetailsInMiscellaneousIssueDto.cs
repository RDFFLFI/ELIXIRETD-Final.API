using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO
{
    public class GetAllDetailsInMiscellaneousIssueDto
    {

        public int IssuePKey { get; set; }

        public string Customer { get; set; }

        public string CustomerCode { get; set; }

        public string PreparedDate { get; set; }

        public string PreparedBy { get; set; }

        public string ItemCode { get; set; }    

        public string ItemDescription { get; set; }

        public decimal TotalQuantity { get; set; }

        public string Remarks { get; set; } 
    }
}
