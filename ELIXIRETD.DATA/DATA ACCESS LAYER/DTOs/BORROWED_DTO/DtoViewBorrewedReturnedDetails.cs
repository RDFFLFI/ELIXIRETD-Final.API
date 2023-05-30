using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO
{
    public class DtoViewBorrewedReturnedDetails
    {

        public string CustomerCode { get; set; }
        public string Customer { get; set; }

        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }

        public decimal Consume { get; set; }

        public string ReturnedDate { get; set; }

        public decimal ReturnQuantity { get; set; }

        public int Id { get; set; }

        public int BorrowedPKey { get; set; }

        public string PreparedBy { get; set; }


        public string Uom { get; set; }


    }
}
