using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER
{
    public class ListofOrderDto
    {
        public int TransactId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }

        public bool IsPrepared { get; set; }

        public bool? IsApproved { get; set; }

        public bool? IsServed { get; set; }

        public bool IsActive { get; set; }

    }
}
