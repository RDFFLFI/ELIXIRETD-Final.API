namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class Material : BaseEntity
    {

        public int ? Material_No {  get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }

        public Uom Uom { get; set; }
        public int UomId { get; set; }
        public int BufferLevel { get; set; }
        public bool IsActive { get; set; } = true;
        public string UomCode { get; set; }
        public ItemCategory ItemCategory { get; set; }
        public int ? ItemCategoryId { get; set; }
        public string ItemCategoryName { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public string AddedBy { get; set; }
        public string ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }

        public DateTime SyncDate { get; set; }

        public string StatusSync { get; set; }

        public int? LotSectionId { get; set; }
        public virtual LotSection LotSection { get; set; }


    }
}
