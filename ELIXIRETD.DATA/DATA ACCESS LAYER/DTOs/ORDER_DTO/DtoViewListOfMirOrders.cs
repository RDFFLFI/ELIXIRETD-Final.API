using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO
{
    public class DtoViewListOfMirOrders
    {
        public int MirId { get; set; }

        public int OrderNo { get; set; }

        public string OrderDate { get; set; }

        public string DateNeeded { get; set; }

        
        public string CustomerCode { get; set; }

        public string CustomerName { get; set; }

        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }

        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }

        public string LocationCode { get; set; }
        public string LocationName { get; set; }

        public List<ListOrders> ListOrder { get; set; }

        public class ListOrders
        {
            public int Id { get; set; }

            public string ItemCode { get; set; }

            public string ItemDescription { get; set; }

            public string Uom { get; set; } 

            public string ItemRemarks { get; set; }

            public decimal Quantity { get; set; }

            public string AccountCode { get; set; }
            public string AccountTitles { get; set; }
            public string EmpId { get; set; }

            public string FullName { get; set; }

            public string AssetTag { get; set; }



        }






    }
}
