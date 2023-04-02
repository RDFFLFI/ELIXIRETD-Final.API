using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO
{
    public class DtoReturnedReports
    {
        public int ReturnedId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public decimal ReturnedQuantity { get; set; }
        public string Uom { get; set; }
        public string Category { get; set; }
        public string TransactBy { get; set; }
        public string TransactDate { get; set; }





    }


}
