namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class SubCategory : BaseEntity
    {
        public string SubCategoryName { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;

        public ItemCategory ItemCategory { get; set; }
        public int ItemCategoryId { get; set; }
        public string AddedBy { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
