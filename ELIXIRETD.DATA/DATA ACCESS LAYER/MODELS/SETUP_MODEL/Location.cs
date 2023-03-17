using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class Location : BaseEntity
    {
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public bool IsActive { get; set; } = true;
        public string AddedBy { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;

    }
}
