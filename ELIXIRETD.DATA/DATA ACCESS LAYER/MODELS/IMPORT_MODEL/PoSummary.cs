using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL
{
    public class PoSummary : BaseEntity
    {
        public int PR_Number { get; set; }


        [Column(TypeName = "Date")]
        public DateTime PR_Date { get; set; }

        public int PO_Number { get; set; }


        [Column(TypeName = "Date")]
        public DateTime PO_Date { get; set; }


        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Ordered { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal Delivered { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal Billed { get; set; }

        public string Uom { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public string VendorName { get; set; }

        public string AddedBy{ get; set; }
        public bool IsActive { get; set; } = true;
        public string Reason { get; set; }

        public DateTime ImportDate { get; set; } = DateTime.Now;

        public DateTime? DateCancelled { get; set; }

        public string CancelBy { get; set; }
        public bool? IsCancelled { get; set; }




    }
}
