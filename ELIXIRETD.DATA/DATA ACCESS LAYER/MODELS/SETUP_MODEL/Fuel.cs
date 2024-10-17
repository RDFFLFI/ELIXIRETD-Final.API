using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class Fuel : BaseEntity
    {
        public int MaterialId { get; set; }
        public virtual Material Material { get; set; }

        public string Added_By { get; set; }
        public DateTime Created_At { get; set; } = DateTime.Now;

        public string Modified_By { get; set; }
        public DateTime? Updated_At { get; set; }

        public bool Is_Active { get; set; } = true;

    }
}
