using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.FUEL_REGISTER_DTO
{
    public record GetMaterialByStocksDto
    {
        public int Id { get; set; }
        public string Item_Code { get; set; }
        public string Item_Description { get; set; }
        public string Uom { get; set; }
        public decimal RemainingStocks { get; set; }
        
    }
}
