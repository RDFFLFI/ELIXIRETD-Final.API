using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.FUEL_REGISTER_MODEL
{
    public class FuelRegister : BaseEntity
    {
       
        public string Source { get; set; }
        public string Plate_No { get; set; }

        public int ? UserId { get; set; }
        public virtual User User { get; set; }

        public int MaterialId { get; set; }
        public virtual Material Material { get; set; }

        public int? Warehouse_ReceivingId { get; set; }
        public virtual Warehouse_Receiving Warehouse_Receiving { get; set; }

        public decimal? Liters {  get; set; }
        public string Asset { get; set; }

        public decimal? Odometer { get; set; }
        public string Company_Code { get; set; }
        public string Company_Name { get; set; }

        public string Department_Code { get; set; }
        public string Department_Name { get; set; }

        public string Location_Code { get; set; }
        public string Location_Name { get; set; }

        public string Account_Title_Code { get; set; }
        public string Account_Title_Name { get; set; }
        public string EmpId { get; set; }
        public string Fullname { get; set; }

        public string Added_By { get; set; }
        public DateTime Created_At {  get; set; } = DateTime.Now;
        public string Modified_By {  get; set; }
        public DateTime? Updated_At { get; set; }

        public bool Is_Approve { get; set; }
        public string Approve_By { get; set; }
        public DateTime? Approve_At { get; set; }

        public bool? Is_Transact {  get; set; }
        public string Transact_By { get; set; }
        public DateTime? Transact_At { get; set; }

        public bool Is_Reject { get; set; } = false;
        public string Reject_Remarks { get; set; }
        public string Reject_By { get; set; }

        public bool Is_Active { get; set; } = true;
        public string Remarks { get; set; }

    }
}
