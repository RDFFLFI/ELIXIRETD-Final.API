using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO
{
    public class GetAllAvailableBorrowIssueDto
    {

       public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string  Uom { get; set; }
        public decimal TotalQuantity { get; set; }
        public string BorrowDate { get; set; }   




    }
}
