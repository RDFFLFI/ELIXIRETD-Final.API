using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class Customer : BaseEntity
    {

        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public CustomerType CustomerTypeP { get; set; }
        public int CustomerTypeId { get; set; }
        public string CompanyName { get; set; }
        //public string MobileNumber { get; set; }
        public string Address { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public string AddedBy { get; set; }


    }
}
