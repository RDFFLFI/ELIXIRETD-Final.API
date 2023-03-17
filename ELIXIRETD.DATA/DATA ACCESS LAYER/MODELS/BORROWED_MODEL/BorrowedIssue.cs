using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.BORROWED_MODEL
{
     public class BorrowedIssue : BaseEntity
    {
        public string CustomerName { get; set; }

        public string CustomerCode { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalQuantity { get; set; }

        public DateTime PreparedDate { get; set; }

        public string PreparedBy { get; set; }

        public string Details { get; set; }

        public string Remarks { get; set;  }
        public bool IsActive { get; set; }

        public bool? IsTransact { get; set; }
        public bool ? IsReturned { get; set; }

        [Column(TypeName = "Date")]
        public DateTime ? ReturnedDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Consume { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ReturnedQuantity { get; set; }



    }
}
