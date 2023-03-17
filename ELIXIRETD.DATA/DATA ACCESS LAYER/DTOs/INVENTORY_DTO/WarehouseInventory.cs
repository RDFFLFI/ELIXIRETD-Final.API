namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO
{
    public class WarehouseInventory
    {
        public int WarehouseId { get; set; }
        public string ItemCode { get; set; }
        public decimal ActualGood { get; set; }
        public string RecievingDate { get; set; }


    }
}
