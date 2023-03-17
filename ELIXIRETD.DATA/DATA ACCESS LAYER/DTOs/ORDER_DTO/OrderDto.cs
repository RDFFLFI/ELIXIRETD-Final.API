namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderDate { get; set; }
        public int OrderNo { get; set; }
        public string DateNeeded { get; set; }
        public string CustomerName { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public decimal QuantityOrder { get; set; }
        public string PreparedDate { get; set; }
        public bool IsApproved { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrepared { get; set; }
        public decimal StockOnHand { get; set; }
        public decimal TotalOrders { get; set; }
        public decimal Difference { get; set; }
        public string CancelDate { get; set; }
        public string CancelBy { get; set; }
        public decimal PreparedQuantity { get; set; }
        public bool IsMove { get; set; }
        public bool IsReject { get; set; }
        public string Remarks { get; set; }
        public int OrderNoPKey { get; set; }
        public string DeliveryStatus { get; set; }

        public int Days { get; set; }


    }
}
