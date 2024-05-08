using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class Supplier : BaseEntity
    {

        public int ? Supplier_No { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAddress { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime DateAdded { get; set; }
        public string AddedBy { get; set; }


        public string ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }

        public DateTime SyncDate { get; set; }

        public string StatusSync { get; set; }

        public string Manual {  get; set; }

    }
}
