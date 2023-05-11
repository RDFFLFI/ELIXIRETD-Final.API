using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class TransactionType : BaseEntity
    {
        public string TransactionName { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public string Addedby { get; set; }
        public bool IsActive { get; set; }

    }
}
