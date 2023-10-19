using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO
{
    public class GetAllMIssueWithPaginationDto
    {
        public int IssuePKey { get; set; }

        public string Customer { get; set; }
        public string CustomerCode { get; set; }
        public decimal TotalQuantity { get; set; }

        public string PreparedDate { get; set; }    

        public string Remarks { get; set; }

        public string PreparedBy { get; set; }

        public bool IsActive { get; set; }

        public string TransactionDate { get; set; }

        public string Details { get; set; }


    }
}
