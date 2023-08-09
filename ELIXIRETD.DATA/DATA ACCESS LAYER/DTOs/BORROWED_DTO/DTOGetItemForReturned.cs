﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO
{
    public class DTOGetItemForReturned
    {

        public int Id { get; set; }
        //public int EmpId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public decimal BorrowedQuantity { get; set; }
        public decimal ConsumedQuantity { get; set; }

        public decimal UnitCost { get; set; }

        public decimal RemainingQuantity { get; set; }

        //////public string ReturnedDate { get; set; }

    }
}
