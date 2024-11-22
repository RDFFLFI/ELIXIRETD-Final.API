namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO
{
    public class DtoWarehouseReceivingReports
    {
        public int WarehouseId { get; set; }
        public string ReceiveDate { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescrption { get; set; }
        public string Category { get; set; }
        public string PoNumber { get; set; }
        public string RRNumber { get; set; }
        public DateTime ? RRDate { get; set; }
        public string PR_Year_Number { get; set; }
        public string SINumber { get; set; }
        public string SupplierName { get; set; }
        public decimal Quantity { get; set; }
        public string Uom { get; set; }
        public decimal TotalReject { get; set; }
        public string ReceivedBy { get; set; }
        public string TransactionType { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }

    }
}
