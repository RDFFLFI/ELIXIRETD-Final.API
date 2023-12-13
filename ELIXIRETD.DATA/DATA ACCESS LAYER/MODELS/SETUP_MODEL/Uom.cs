using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS
{
    public class Uom : BaseEntity
    {

        public int UomNo { get; set; }
        public string  UomCode { get; set; }
        public string UomDescription { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public string AddedBy { get; set; }

        public bool IsActive { get; set; } = true;


        public string ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public DateTime SyncDate { get; set; }
        public string StatusSync { get; set; }


    }
}
