﻿using ELIXIRETD.DATA.CORE.INTERFACES.IMPORT_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.IMPORT_REPOSITORY
{
    public class PoSummaryRepository : IPoSummaryRepository
    {
        private readonly StoreContext _context;

        public PoSummaryRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<bool> AddNewPORequest(PoSummary posummary)
        {
            posummary.PR_Date = Convert.ToDateTime(posummary.PR_Date);
            posummary.PO_Date = Convert.ToDateTime(posummary.PO_Date);

    
            var existingInfo = await _context.Materials.Where(x => x.ItemCode == posummary.ItemCode)
                                                       .FirstOrDefaultAsync();

            if (existingInfo == null)
                return false;


            await _context.PoSummaries.AddAsync(posummary);
            return true;
        }

        public async Task<bool> CheckItemCode(string rawmaterial)
        {
            var validate = await _context.Materials.Where(x => x.ItemCode == rawmaterial)
                                                   .Where(x => x.IsActive == true)
                                                   .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> ValidationItemcodeandUom(string itemcode /*,string itemdescription */ , string uom)
        {
            var validate = await _context.Materials.Where(x => x.ItemCode == itemcode)
                                                   //.Where(x => x.ItemDescription == itemdescription)
                                                   .Where(x => x.Uom.UomDescription == uom)
                                                   .Where(x => x.IsActive == true)
                                                   .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> CheckSupplier(string supplier)
        {
            var validate = await _context.Suppliers.Where(x => x.SupplierName == supplier)
                                                   .Where(x => x.IsActive == true)
                                                   .FirstOrDefaultAsync();
            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> CheckUomCode(string uom)
        {
            var validate = await _context.Uoms.Where(x => x.UomDescription == uom)
                                              .Where(x => x.IsActive == true)
                                              .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> ValidatePOAndItemcodeManual(int ponumber, string itemcode)
        {
            var validate = await _context.PoSummaries.Where(x => x.PO_Number == ponumber)
                                                     .Where(x => x.ItemCode == itemcode)
                                                     .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateQuantityOrder(decimal quantity)
        {

            var existingQuantity = await _context.PoSummaries
                                 .Where(x => x.Ordered == quantity)
                                 .FirstOrDefaultAsync();


            return true;
        }















    }
}
