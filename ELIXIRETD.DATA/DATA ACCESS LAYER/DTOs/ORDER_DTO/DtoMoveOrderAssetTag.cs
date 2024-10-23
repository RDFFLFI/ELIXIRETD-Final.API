using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO
{
    public class DtoMoveOrderAssetTag
    {
        public string Id { get; set; }
        public string? PoNumber { get; set; }
        public string PrNumber { get; set; }
        public int MIRId { get; set; }
        public int WareHouseId { get; set; }
        public string AcquisitionDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public decimal ServedQuantity { get; set; }
        public string AssetTag { get; set; }
        public string ApproveDate { get; set; }

        public string Release_Date { get; set; }

        public decimal Unit_Price { get; set; }

        public string Company_Code { get; set; }

        public string Company_Name { get; set; }
        public string Business_Unit_Code { get; set; }
        public string Business_Unit_Name { get; set; }

        public string Department_Code { get; set; }
        public string Department_Name { get; set; }
        public string Unit_Code { get; set; }
        public string Unit_Name { get; set; }

        public string Sub_Unit_Code { get; set; }
        public string Sub_Unit_Name { get;set; }

        public string Location_Code { get; set; }
        public string Location_Name { get; set; }

        public string Account_Title_Code { get; set; }
        public string Account_Title_Name { get; set; }

        public string EmpId { get; set; }
        public string Fullname { get; set; }

        public string Major_Category_Name { get; set; }
        public string Minor_Category_Name { get;set;}

    }
}
