using DocumentFormat.OpenXml.Spreadsheet;
using ELIXIRETD.DATA.CORE.INTERFACES.IMPORT_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.IMPORT_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.WAREHOUSE_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
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
                                                   .Where(x => x.Uom.UomCode == uom)
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
            var validate = await _context.Uoms.Where(x => x.UomCode == uom)
                                              .Where(x => x.IsActive == true)
                                              .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateRRAndItemcodeManual(string ponumber,string rrNo, string itemcode)
        {
            var validate = await _context.PoSummaries.Where(x => x.PO_Number == ponumber && x.RRNo == rrNo)
                                                     .Where(x => x.ItemCode == itemcode)
                                                     .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;
        }


        public async Task<bool> ValidatePOAndItemcodeManual(string ponumber, string itemcode)
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

        public async Task<bool> ImportBufferLevel(ImportBufferLevelDto itemCode)
        {

             var itemCodeExist = await _context.Materials
                .FirstOrDefaultAsync(x => x.ItemCode == itemCode.ItemCode
                && x.IsActive == true);

            itemCodeExist.BufferLevel = itemCode.BufferLevel;  

            return true;
        }

        public async Task<IReadOnlyList<WarehouseOverAllStocksDto>> WarehouseOverAllStocks(string Search)
        {
            var wareHouseStocks = await _context.WarehouseReceived
                //.Where(x => x.ItemCode == Search)
                .Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.Id,
               
                })
                .Select(x => new WarehouseOverAllStocksDto
                {
                    WarehouseId = x.Key.Id,
                    ItemCode = x.First().ItemCode,
                    ItemDescription = x.First().ItemDescription,
                    Quantity = x.First().ActualGood ,
                }).ToListAsync();

            var moveOrderOut = await _context.MoveOrders
                .Where(x => x.IsActive == true )
                .Where(x => x.IsApprove == true)
                .GroupBy(x => new
                {
                    x.WarehouseId,

                }).Select(x => new WarehouseOverAllStocksDto
                {
                    WarehouseId = x.Key.WarehouseId,
                    ItemCode = x.First().ItemCode,
                    Quantity = x.Sum(x => x.QuantityOrdered)

                }).ToListAsync();


            var issueOut = await _context.MiscellaneousIssueDetail
                .Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.WarehouseId,
                }).Select(x => new WarehouseOverAllStocksDto
                {
                    WarehouseId = x.Key.WarehouseId,
                    ItemCode = x.First().ItemCode,
                    Quantity = x.Sum(x => x.Quantity)

                }).Distinct().ToListAsync();

            var borrowOut = await _context.BorrowedIssueDetails
                .Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.WarehouseId,
                }).Select(x => new WarehouseOverAllStocksDto
                {
                    WarehouseId = x.Key.WarehouseId,
                    ItemCode = x.First().ItemCode,
                    Quantity = x.Sum(x => x.Quantity != null ? x.Quantity : 0)

                }).ToListAsync();

            var consumed =  _context.BorrowedConsumes
                .Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.ItemCode,
                    x.BorrowedItemPkey

                }).Select(x => new ItemStocksDto
                {
                    ItemCode = x.Key.ItemCode,
                    BorrowedItemPkey = x.Key.BorrowedItemPkey,
                    Consume = x.Sum(x => x.Consume != null ? x.Consume : 0)

                });

            var borrowedReturn = await  _context.BorrowedIssueDetails
               .Where(x => x.IsActive == true)
               .Where(x => x.IsReturned == true)
               .Where(x => x.IsApprovedReturned == true)
               .GroupJoin(consumed, returned => returned.Id, itemconsume => itemconsume.BorrowedItemPkey, (returned, itemconsume) => new { returned, itemconsume })
               .SelectMany(x => x.itemconsume.DefaultIfEmpty(), (x, itemconsume) => new { x.returned, itemconsume })
               .GroupBy(x => new
               {
                   x.returned.WarehouseId,

               }).Select(x => new WarehouseOverAllStocksDto
               {
                   WarehouseId = x.Key.WarehouseId,
                   ItemCode = x.First().returned.ItemCode,
                   Quantity = x.Sum(x => x.returned.Quantity != null ? x.returned.Quantity : 0) - x.Sum(x => x.itemconsume.Consume),

               }).ToListAsync();

            var fuelRegister = _context.FuelRegisters
             .Include(m => m.Material)
             .Where(fr => fr.Is_Active == true)
             .Where(fr => fr.Is_Approve == true)
             .GroupBy(fr => new
             {
                 fr.Warehouse_ReceivingId,

             }).Select(fr => new
             {
                 itemCode = fr.First().Material.ItemCode,
                 WarehouseId = fr.Key.Warehouse_ReceivingId,
                 Quantity = fr.Sum(fr => fr.Liters.Value)
             });

            var warehouseInventory = wareHouseStocks
                                   .GroupJoin(moveOrderOut, warehouse => warehouse.WarehouseId, moveOrder => moveOrder.WarehouseId, (warehouse, moveOrder) => new { warehouse, moveOrder })
                                   .SelectMany(x => x.moveOrder.DefaultIfEmpty(), (x, moveOrder) => new { x.warehouse, moveOrder })
                                   .GroupJoin(issueOut, warehouse => warehouse.warehouse.WarehouseId, issue => issue.WarehouseId, (warehouse, issue) => new { warehouse, issue })
                                   .SelectMany(x => x.issue.DefaultIfEmpty(), (x, issue) => new { x.warehouse, issue })
                                   .GroupJoin(borrowOut, warehouse => warehouse.warehouse.warehouse.WarehouseId, borrow => borrow.WarehouseId, (warehouse, borrow) => new { warehouse, borrow })
                                   .SelectMany(x => x.borrow.DefaultIfEmpty(), (x, borrow) => new { x.warehouse, borrow })
                                   .GroupJoin(borrowedReturn, warehouse => warehouse.warehouse.warehouse.warehouse.WarehouseId, returned => returned.WarehouseId, (warehouse, returned) => new { warehouse, returned })
                                   .SelectMany(x => x.returned.DefaultIfEmpty(), (x, returned) => new { x.warehouse, returned })
                                   .GroupJoin(fuelRegister, warehouse => warehouse.warehouse.warehouse.warehouse.warehouse.WarehouseId, fuel => fuel.WarehouseId, (warehouse, fuel) => new { warehouse, fuel })
                                   .SelectMany(x => x.fuel.DefaultIfEmpty(), (x, fuel) => new { x.warehouse, fuel })
                                   .Select(x => new WarehouseOverAllStocksDto
                                   {
                                       WarehouseId = x.warehouse.warehouse.warehouse.warehouse.warehouse.WarehouseId,
                                       ItemCode = x.warehouse.warehouse.warehouse.warehouse.warehouse.ItemCode,
                                       ItemDescription = x.warehouse.warehouse.warehouse.warehouse.warehouse.ItemDescription,
                                       Quantity = x.warehouse.warehouse.warehouse.warehouse.warehouse.Quantity
                                       + (x.warehouse.returned?.Quantity ?? 0)
                                       - (x.warehouse.warehouse.warehouse.warehouse.moveOrder?.Quantity ?? 0)
                                       - (x.warehouse.warehouse.warehouse.issue?.Quantity ?? 0)
                                       - (x.warehouse.warehouse.borrow?.Quantity ?? 0)
                                       - (x.fuel.Quantity != null ? x.fuel.Quantity : 0),

                                   }).Where(x => x.Quantity > 0);


            if (!string.IsNullOrEmpty(Search))
            {
                warehouseInventory = warehouseInventory
                    .Where(x => x.ItemCode.ToLower().Contains(Search.Trim().ToLower()));

            }
            warehouseInventory = warehouseInventory.OrderBy(x => x.ItemCode);

            return warehouseInventory.ToList();
        }

        public async Task<bool> AddImportReceipt(ImportMiscReceiptDto items)
        {

            var addItem = new MiscellaneousReceipt
            {
                SupplierCode = items.SupplierCode,
                supplier = items.SupplierCode,
                TotalQuantity = items.TotalQuantity,
                PreparedDate = DateTime.Now,
                PreparedBy = items.PreparedBy,
                Remarks = items.Remarks,
                IsActive = true,
                TransactionDate = DateTime.Now,
                CompanyCode = items.CompanyCode,
                CompanyName = items.CompanyName,
                DepartmentCode = items.DepartmentCode,
                DepartmentName = items.DepartmentName,
                LocationCode = items.LocationCode,
                LocationName = items.LocationName,
                Details = items.Details,

            };

            await _context.MiscellaneousReceipts.AddAsync(addItem);

            return true;
        }

        public async Task<bool> AddImportReceiptToWarehouse(ImportMiscReceiptDto.WarehouseReceiptDto item)
        {

            var addItem = new Warehouse_Receiving
            {
                
                ItemCode = item.ItemCode,
                ItemDescription = item.ItemDescription,
                Uom = item.Uom,
                Supplier = item.SupplierName,
                ReceivingDate = DateTime.Now,
                ActualGood = decimal.Parse(item.Quantity),
                ActualDelivered = decimal.Parse(item.Quantity),
                TransactionType = "MiscellaneousReceipt",
                MiscellaneousReceiptId = item.MiscellaneousReceiptId,
                IsActive = true,
                IsWarehouseReceived = true,
                ActualReceivingDate = DateTime.Now,
                UnitPrice = decimal.Parse(item.UnitCost),
                AccountCode = item.AccountCode,
                AccountTitles = item.AccountTitles,
                EmpId = item.EmpId,
                FullName = item.FullName,

            };

            await _context.WarehouseReceived.AddAsync(addItem);


            return true;

        }
    }
}
