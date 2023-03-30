using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.BORROWED_MODEL
{
    public class ReturnedBorrowed : BaseEntity
    {

        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ReturnedQuantity { get; set; }

        [Column(TypeName = "Date")]
        public DateTime BorrowedDate { get; set; }
        public int WarehouseId { get; set; }

        public bool IsActive { get; set; }

        public bool ? IsReturned { get; set; }

        public string Remarks { get; set; }

        public int BorrowedPKey { get; set; }

        public DateTime PreparedDate { get; set; }


        [Column(TypeName = "Date")]
        public DateTime? ReturnedDate { get; set; }

        public int TotalbPkey { get; set; }
    }
}
