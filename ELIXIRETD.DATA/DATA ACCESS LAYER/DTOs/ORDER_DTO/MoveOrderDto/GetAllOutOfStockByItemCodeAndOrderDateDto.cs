namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.MoveOrderDto
{
    public class GetAllOutOfStockByItemCodeAndOrderDateDto
    {

        public int Id { get; set; }
        public string OrderDate { get; set; }
        public string DateNeeded { get; set; }

        public string Department { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string Category { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }

        public decimal QuantityOrder { get; set; }

        public bool IsActive { get; set; }

        public bool IsPrepared { get; set; }

        public decimal StockOnHand { get; set; }

        public decimal Difference { get; set; }
        public string PreparedDate { get; set; }

        public bool IsApproved { get; set; }

        public decimal TotalOrders { get; set; }










    }
}
