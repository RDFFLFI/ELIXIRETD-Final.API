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

        public int OrderNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public decimal Quantity { get; set; }
        public string MoveOrderDate { get; set; }
        public string TransactedBy { get; set; }
        public bool TransactionType { get; set; }
        public string TransactedDate { get; set; }
        public string DeliveryDate { get; set; }

        public string CustomerType { get; set; }


    }
}
