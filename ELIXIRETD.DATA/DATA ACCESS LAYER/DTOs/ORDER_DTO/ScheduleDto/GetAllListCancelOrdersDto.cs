namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO
{
    public class GetAllListCancelOrdersDto
    {
        public int Id { get; set; }
        public string Department { get; set; }
        public string CustomerName { get; set; }
        public string Category { get; set; }
        public decimal QuantityOrder { get; set; }
        public string OrderDate { get; set; }
        public string DateNeeded { get; set; }
        public string PreparedDate { get; set; }
        public string CancelDate { get; set; }

        public string CancelBy { get; set; }
        public bool IsActive { get; set; }

     }



    
}
