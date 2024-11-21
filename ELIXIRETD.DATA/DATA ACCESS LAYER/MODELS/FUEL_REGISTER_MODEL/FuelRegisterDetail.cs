using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.FUEL_REGISTER_MODEL
{
    public class FuelRegisterDetail : BaseEntity
    {
        public int Id { get; set; }

        public int? FuelRegisterId { get; set; }
        public virtual FuelRegister FuelRegister { get; set; }

        public int MaterialId { get; set; }
        public virtual Material Material { get; set; }
        public int? Warehouse_ReceivingId { get; set; }
        public virtual Warehouse_Receiving Warehouse_Receiving { get; set; }

        public decimal? Liters { get; set; }
        public string Added_By { get; set; }
        public DateTime Created_At { get; set; } = DateTime.Now;
        public string Modified_By { get; set; }
        public DateTime? Updated_At { get; set; }
        public bool Is_Active { get; set; } = true;

    }
}
