using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL
{
    public class Warehouse_Reject : BaseEntity
    {
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }
        public string Remarks { get; set; }
        public int WarehouseReceivingId { get; set; }

        [Column(TypeName = "Date")]
        public DateTime RejectedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public string RejectedBy { get; set; }

    }
}
