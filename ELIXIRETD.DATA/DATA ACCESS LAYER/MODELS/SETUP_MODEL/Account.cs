using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class Account : BaseEntity
    {
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public bool IsActive { get; set; } = true;
        public string AddedBy { get; set; }
        public DateTime DateAdded { get; set; }

    }
}
