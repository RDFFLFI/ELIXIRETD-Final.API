using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class LotSection : BaseEntity
    {

        public LotNames LotNames { get; set; }
        public int LotNamesId { get; set; }
        public string SectionName { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public string AddedBy { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
