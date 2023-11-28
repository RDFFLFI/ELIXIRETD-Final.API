namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class ItemCategory : BaseEntity
    {

        public int ItemCategory_No {  get; set; }

        public string ItemCategoryName { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public string AddedBy { get; set; }
        public bool IsActive { get; set; } = true;


        public string ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }

        public DateTime SyncDate { get; set; }

        public string StatusSync { get; set; }

    }
}
