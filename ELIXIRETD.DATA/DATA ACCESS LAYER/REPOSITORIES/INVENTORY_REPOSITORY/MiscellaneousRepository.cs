using ELIXIRETD.DATA.CORE.INTERFACES.INVENTORY_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORY_DTO.MRP;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System.IO.IsolatedStorage;
using System.Security.Cryptography.X509Certificates;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.INVENTORY_REPOSITORY
{
    public class MiscellaneousRepository :IMiscellaneous
    {
        private readonly StoreContext _context;

        public MiscellaneousRepository( StoreContext context)
        {

           _context = context;

        }

        public async Task<bool> AddMiscellaneousReceipt(MiscellaneousReceipt receipt)
        {

            receipt.IsActive = true;
            receipt.PreparedDate = DateTime.Now;

            await _context.MiscellaneousReceipts.AddAsync(receipt); 
            return true;
        }


        public async Task<bool> AddMiscellaneousReceiptInWarehouse(Warehouse_Receiving receive)
        {

            receive.ActualDelivered = receive.ActualGood;

            await _context.WarehouseReceived.AddAsync(receive);
            return true;
        }

        public async Task<bool> InActiveMiscellaneousReceipt(MiscellaneousReceipt receipt)
        {
            var existing = await _context.MiscellaneousReceipts.Where(x => x.Id == receipt.Id)
                                                               .FirstOrDefaultAsync();

            var existingWH = await _context.WarehouseReceived
                .Where(x => x.MiscellaneousReceiptId == receipt.Id)
                .ToListAsync();


            if (existing == null)
                return false;

            existing.IsActive = false;

            foreach (var items in existingWH)
            {
                items.IsActive = false;
            }
            return true;

        }



        public async Task<bool> ActivateMiscellaenousReceipt(MiscellaneousReceipt receipt)
        {
            var existing = await _context.MiscellaneousReceipts.Where(x => x.Id == receipt.Id)
                                                               .FirstOrDefaultAsync();

            var existingWH = await _context.WarehouseReceived.Where(x => x.MiscellaneousReceiptId == receipt.Id)
                                                             .ToListAsync();

            if (existing == null) 
                return false;

            existing.IsActive = true;

            foreach(var items in existingWH)
            {
                items.IsActive = true;
            }
            return true;    
        }




        public async Task<PagedList<GetAllMReceiptWithPaginationdDto>> GetAllMReceiptWithPaginationd(UserParams userParams, bool status)
        {

           


            var receipt = _context.MiscellaneousReceipts.OrderByDescending(x => x.PreparedDate)
                                                        .Where(x => x.IsActive == status)
                                                        .Select(x => new GetAllMReceiptWithPaginationdDto
                                                        {

                                                            Id= x.Id,
                                                            SupplierCode = x.SupplierCode,
                                                            SupplierName = x.supplier,
                                                            TotalQuantity= x.TotalQuantity,
                                                            PreparedBy = x.PreparedBy,
                                                            PreparedDate = x.PreparedDate.ToString("MM/dd/yyyy"),
                                                            //TransactionType = x.TransactionType,

                                                            TransactionDate = x.TransactionDate.ToString("MM/dd/yyyy"),

                                                            Remarks = x.Remarks,
                                                            IsActive= x.IsActive,
                                                            Details = x.Details
                                                            
                                                            
                                                        });

            return await PagedList<GetAllMReceiptWithPaginationdDto>.CreateAsync(receipt, userParams.PageNumber , userParams.PageSize);
        }

        public async Task<PagedList<GetAllMReceiptWithPaginationdDto>> GetAllMReceiptWithPaginationOrig(UserParams userParams, string search, bool status)
        {
            var receipt = _context.MiscellaneousReceipts.OrderByDescending(x => x.PreparedDate)
                                                        .Where(x => x.IsActive == status)
                                                        .Select(x => new GetAllMReceiptWithPaginationdDto
                                                        {

                                                            Id = x.Id,
                                                            SupplierCode = x.SupplierCode,
                                                            SupplierName = x.supplier,
                                                            TotalQuantity = x.TotalQuantity,
                                                            PreparedBy = x.PreparedBy,
                                                            PreparedDate = x.PreparedDate.ToString("MM/dd/yyyy"),
                                                            //TransactionType = x.TransactionType,

                                                            TransactionDate = x.TransactionDate.ToString("MM/dd/yyyy"),

                                                            Remarks = x.Remarks,
                                                            IsActive = x.IsActive,
                                                            Details = x.Details,

                                                        }).Where(x => (Convert.ToString(x.Id)).ToLower().Contains(search.Trim().ToLower())
                                                       || (Convert.ToString(x.SupplierCode)).ToLower().Contains(search.Trim().ToLower())
                                                         || (Convert.ToString(x.SupplierName)).ToLower().Contains(search.Trim().ToLower()));

            return await PagedList<GetAllMReceiptWithPaginationdDto>.CreateAsync(receipt , userParams.PageNumber , userParams.PageSize);

        }


        public async Task<IReadOnlyList<GetWarehouseDetailsByMReceiptDto>> GetWarehouseDetailsByMReceipt(int id)
        {

            var receipt = _context.WarehouseReceived
                      .Where(x => x.MiscellaneousReceiptId == id && x.IsActive == true)
                      .GroupJoin(_context.MiscellaneousReceipts, warehouse => warehouse.MiscellaneousReceiptId, receiptparents => receiptparents.Id, (warehouse, receiptparents) => new { warehouse, receiptparents })
                      .SelectMany(x => x.receiptparents.DefaultIfEmpty(), (x, receiptparents) => new { x.warehouse, receiptparents })
                      .Select(x => new GetWarehouseDetailsByMReceiptDto
                      {

                          Id = x.receiptparents.Id,
                          WarehouseId = x.warehouse.Id,
                          Itemcode = x.warehouse.ItemCode,
                          ItemDescription = x.warehouse.ItemDescription,
                          TotalQuantity = x.warehouse.ActualGood,
                          SupplierCode = x.receiptparents.SupplierCode,
                          SupplierName = x.receiptparents.supplier,
                          PreparedDate = x.receiptparents.PreparedDate.ToString(),
                          Remarks = x.receiptparents.Remarks,
                          PreparedBy = x.receiptparents.PreparedBy,

                          UnitCost = x.warehouse.UnitPrice,
                          TotalCost = x.warehouse.UnitPrice * x.warehouse.ActualDelivered,
                          Uom = x.warehouse.Uom,

                          TransactionDate =x.receiptparents.TransactionDate.ToString(),
                          CompanyCode = x.receiptparents.CompanyCode,
                          CompanyName = x.receiptparents.CompanyName,
                          DepartmentCode = x.receiptparents.DepartmentCode,
                          DepartmentName = x.receiptparents.DepartmentName,
                          LocationCode = x.receiptparents.LocationCode,
                          LocationName = x.receiptparents.LocationName,
                          AccountCode = x.warehouse.AccountCode,
                          AccountTitles = x.warehouse.AccountTitles,
                          EmpId = x.warehouse.EmpId,
                          FullName = x.warehouse.FullName,
                          Details = x.receiptparents.Details


                          //TransactionType = x.receiptparents.TransactionType,


                      });

            return await receipt.ToListAsync();

        }


        //================================================= Miscellaneous Issue ===================================================================

        public async Task<bool> AddMiscellaneousIssueDetails(MiscellaneousIssueDetails details)
        {
            var warehouseUnitcost = await _context.WarehouseReceived.Where(x => x.Id == details.WarehouseId)
                                                                    .Where(x => x.IsActive == true)
                                                                    .FirstOrDefaultAsync();

            details.UnitPrice = warehouseUnitcost.UnitPrice;


            await _context.MiscellaneousIssueDetail.AddAsync(details);
            return true;
        }

        public async Task<bool> AddMiscellaneousIssue(MiscellaneousIssue issue)
        {
            await _context.MiscellaneousIssues.AddAsync(issue);

            return true;

        }

        public  async Task<bool> UpdateIssuePKey(MiscellaneousIssueDetails details)
        {
            var existing = await _context.MiscellaneousIssueDetail.Where(x => x.Id == details.Id)
                                                                  .FirstOrDefaultAsync();

            if (existing == null)
                return false;

            existing.IssuePKey = details.IssuePKey;
            existing.IsTransact = true;

            return true;
        }



        public async Task<IReadOnlyList<GetAvailableStocksForIssueDto>> GetAvailableStocksForIssueNoParameters()
        {
          


            var getWarehouseStocks = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                               .GroupBy(x => new
                                                               {

                                                                   x.ItemCode,
                                             
                                                               }).Select(x => new WarehouseInventory
                                                               {

                                                                   ItemCode = x.Key.ItemCode,
                                                                   ItemDescription = x.First().ItemDescription,
                                                                   Uom = x.First().Uom,
                                                                   ActualGood = x.Sum(x => x.ActualGood),


                                                               });

            var reserveOut = _context.Orders.Where(x => x.IsActive == true)
                                            .Where(x => x.IsPrepared == true)
                                            .GroupBy(x => new
                                            {
                                                x.ItemCode,           

                                            }).Select(x => new MoveOrderInventory
                                            {
                                                ItemCode = x.Key.ItemCode,
                                                QuantityOrdered = x.Sum(x => x.QuantityOrdered)
                                            });

            var issueOut = _context.MiscellaneousIssueDetail.Where(x => x.IsActive == true)
                                                            .GroupBy(x => new
                                                            {
                                                                x.ItemCode,

                                                            }).Select(x => new ItemStocksDto
                                                            {
                                                                ItemCode = x.Key.ItemCode,
                                                                Out = x.Sum(x => x.Quantity)

                                                            });

            var borrowedOut = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                           .GroupBy(x => new
                                                           {

                                                               x.ItemCode,

                                                           }).Select(x => new ItemStocksDto
                                                           {
                                                               ItemCode = x.Key.ItemCode,
                                                               Out = x.Sum(x => x.Quantity)

                                                           });

            var consumed = _context.BorrowedConsumes.Where(x => x.IsActive)
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



            var BorrowedReturn = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                             .Where(x => x.IsReturned == true)
                                                             .Where(x => x.IsApprovedReturned == true)
                                                             .GroupJoin(consumed, returned => returned.Id, itemconsume => itemconsume.BorrowedItemPkey, (returned, itemconsume) => new { returned, itemconsume })
                                                             .SelectMany(x => x.itemconsume.DefaultIfEmpty(), (x, itemconsume) => new { x.returned, itemconsume })
                                                             .GroupBy(x => new
                                                             {
                                                                 x.returned.ItemCode,

                                                             }).Select(x => new ItemStocksDto
                                                             {

                                                                 ItemCode = x.Key.ItemCode,
                                                                 In = x.Sum(x => x.returned.Quantity != null ? x.returned.Quantity : 0) - x.Sum(x => x.itemconsume.Consume),

                                                             });

            var fuelRegister = _context.FuelRegisterDetails
                .Include(m => m.Material)
                .Where(fr => fr.Is_Active == true)
                .GroupBy(fr => new
                {
                    fr.Material.ItemCode,

                }).Select(fr => new
                {
                    itemCode = fr.Key.ItemCode,
                    Quantity = fr.Sum(fr => fr.Liters.Value)

                });




            var getAvailable = getWarehouseStocks
                              .GroupJoin(reserveOut, warehouse => warehouse.ItemCode, reserve => reserve.ItemCode, (warehouse, reserve) => new { warehouse, reserve })
                              .SelectMany(x => x.reserve.DefaultIfEmpty(), (x, reserve) => new { x.warehouse, reserve })
                              .GroupJoin(borrowedOut, warehouse => warehouse.warehouse.ItemCode, borrowed => borrowed.ItemCode, (warehouse, borrowed) => new { warehouse, borrowed })
                              .SelectMany(x => x.borrowed.DefaultIfEmpty(), (x, borrowed) => new { x.warehouse, borrowed })
                              .GroupJoin(BorrowedReturn, warehouse => warehouse.warehouse.warehouse.ItemCode, returned => returned.ItemCode, (warehouse, returned) => new { warehouse, returned })
                              .SelectMany(x => x.returned.DefaultIfEmpty(), (x, returned) => new { x.warehouse, returned })
                              .GroupJoin(issueOut, warehouse => warehouse.warehouse.warehouse.warehouse.ItemCode, issue => issue.ItemCode, (warehouse, issue) => new { warehouse, issue })
                              .SelectMany(x => x.issue.DefaultIfEmpty(), (x, issue) => new {x.warehouse, issue })
                              .GroupJoin(fuelRegister, warehouse => warehouse.warehouse.warehouse.warehouse.warehouse.ItemCode , fuel => fuel.itemCode, (warehouse,fuel) => new {warehouse,fuel })
                              .SelectMany(x => x.fuel.DefaultIfEmpty() , (x, fuel) => new
                              {
                                  itemcode = x.warehouse.warehouse.warehouse.warehouse.warehouse.ItemCode,
                                  itemdescription = x.warehouse.warehouse.warehouse.warehouse.warehouse.ItemDescription,
                                  uom = x.warehouse.warehouse.warehouse.warehouse.warehouse.Uom,
                                  WarehouseActualGood = x.warehouse.warehouse.warehouse.warehouse.warehouse.ActualGood != null ? x.warehouse.warehouse.warehouse.warehouse.warehouse.ActualGood : 0,
                                  ReserveOut = x.warehouse.warehouse.warehouse.warehouse.reserve.QuantityOrdered != null ? x.warehouse.warehouse.warehouse.warehouse.reserve.QuantityOrdered : 0,
                                  IssueOut = x.warehouse.issue.Out != null ? x.warehouse.issue.Out : 0,
                                  BorrowedOut = x.warehouse.warehouse.warehouse.borrowed.Out != null ? x.warehouse.warehouse.warehouse.borrowed.Out : 0,
                                  Borrowedreturn = x.warehouse.warehouse.returned.In != null ? x.warehouse.warehouse.returned.In : 0,
                                  fuel = fuel.Quantity != null ? fuel.Quantity : 0,

                              }).GroupBy(x => new
                              {

                                  x.itemcode,

                              }).Select(total => new GetAvailableStocksForIssueDto
                              {
                                  ItemCode = total.Key.itemcode,
                                  ItemDescription = total.First().itemdescription,
                                  Uom = total.First().uom,
                                  RemainingStocks = total.Sum(x => x.WarehouseActualGood) + total.Sum(x => x.Borrowedreturn)
                                  - total.Sum(x => x.ReserveOut) - total.Sum(x => x.IssueOut) - total.Sum(x => x.BorrowedOut) -
                                  total.Sum(x => x.fuel),

                              }).Where(x => x.RemainingStocks  >= 1 );


            var GetAvailableItem = getAvailable.GroupBy(x => new
            {
                x.ItemCode,

            }).Select(x => new GetAvailableStocksForIssueDto
            {
                ItemCode = x.Key.ItemCode,
                ItemDescription = x.First().ItemDescription,
                Uom = x.First().Uom,
                RemainingStocks = x.Sum(x => x.RemainingStocks)

            }).OrderBy(x => x.ItemCode);

            return await GetAvailableItem.ToListAsync();
        }


        public async Task<IReadOnlyList<GetAvailableStocksForIssueDto>> GetAvailableStocksForIssue(string itemcode)
        {


            var getWarehouseStocks = _context.WarehouseReceived
                .Where(x => x.ItemCode == itemcode)
                                                              .Where(x => x.IsActive == true)
                                                               .GroupBy(x => new
                                                               {


                                                                   x.Id,
                                                                   x.ItemCode,
                                                                   x.ActualReceivingDate,
                                                                   x.UnitPrice

                                                               }).Select(x => new WarehouseInventory
                                                               {

                                                                   WarehouseId = x.Key.Id,
                                                                   ItemCode = x.Key.ItemCode,
                                                                   ActualGood = x.Sum(x => x.ActualDelivered),
                                                                   RecievingDate = x.Key.ActualReceivingDate.ToString(),
                                                                   UnitPrice = x.Key.UnitPrice

                                                               });




            var reserveOut = _context.Orders.Where(x => x.ItemCode == itemcode)
                .Where(x => x.IsActive == true )
                                            .Where(x => x.IsPrepared == true)
                                            .GroupBy(x => new
                                            {
                                                x.ItemCode,
                                                //x.QuantityOrdered

                                            }).Select(x => new MoveOrderInventory
                                            {
                                                ItemCode = x.Key.ItemCode,
                                                QuantityOrdered = x.Sum(x => x.QuantityOrdered)

                                            });


            var moveorderOut = _context.MoveOrders
                .Where(x => x.ItemCode == itemcode)
                .Where(x => x.IsActive == true)
                                                  .Where(x => x.IsPrepared == true)
                                                  .GroupBy(x => new
                                                  {

                                                      x.WarehouseId,
                                                      x.ItemCode

                                                  }).Select(x => new MoveOrderInventory
                                                  {

                                                      WarehouseId = x.Key.WarehouseId,
                                                      ItemCode = x.Key.ItemCode,
                                                      QuantityOrdered = x.Sum(x => x.QuantityOrdered)

                                                  });


            var issueOut = _context.MiscellaneousIssueDetail.Where(x => x.ItemCode == itemcode)
                .Where(x => x.IsActive == true)
                                                            .GroupBy(x => new
                                                            {

                                                                x.ItemCode,
                                                                x.WarehouseId

                                                            }).Select(x => new ItemStocksDto
                                                            {
                                                                ItemCode = x.Key.ItemCode,
                                                                Out = x.Sum(x => x.Quantity),
                                                                warehouseId = x.Key.WarehouseId

                                                            });


            var issueSumOut = _context.MiscellaneousIssueDetail
                .Where(x => x.ItemCode == itemcode)
                .Where(x => x.IsActive == true)
                                                          .GroupBy(x => new
                                                          {

                                                              x.ItemCode,


                                                          }).Select(x => new ItemStocksDto
                                                          {
                                                              ItemCode = x.Key.ItemCode,
                                                              Out = x.Sum(x => x.Quantity),

                                                          });

            var BorrowedOut = _context.BorrowedIssueDetails
                .Where(x => x.ItemCode == itemcode)
                .Where(x => x.IsActive == true)

                                                           .GroupBy(x => new
                                                           {
                                                               x.ItemCode,
                                                               x.WarehouseId

                                                           }).Select(x => new ItemStocksDto
                                                           {
                                                               ItemCode = x.Key.ItemCode,
                                                               Out = x.Sum(x => x.Quantity),
                                                               warehouseId = x.Key.WarehouseId

                                                           });

            var BorrowedSumOut = _context.BorrowedIssueDetails
                .Where(x => x.ItemCode == itemcode)
                .Where(x => x.IsActive == true)

                                               .GroupBy(x => new
                                               {
                                                   x.ItemCode,

                                               }).Select(x => new ItemStocksDto
                                               {
                                                   ItemCode = x.Key.ItemCode,
                                                   Out = x.Sum(x => x.Quantity),

                                               });


            var consumed = _context.BorrowedConsumes.Where(x => x.IsActive)
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

            var BorrowedReturn = _context.BorrowedIssueDetails
                .Where(x => x.ItemCode == itemcode)
                .Where(x => x.IsActive == true)
                                                 .Where(x => x.IsReturned == true)
                                                 .Where(x => x.IsApprovedReturned == true)
                                                 .GroupJoin(consumed, returned => returned.Id, itemconsume => itemconsume.BorrowedItemPkey, (returned, itemconsume) => new { returned, itemconsume })
                                                 .SelectMany(x => x.itemconsume.DefaultIfEmpty(), (x, itemconsume) => new { x.returned, itemconsume })
                                                 .GroupBy(x => new
                                                 {
                                                     x.returned.ItemCode,
                                                     x.returned.WarehouseId

                                                 }).Select(x => new ItemStocksDto
                                                 {

                                                     ItemCode = x.Key.ItemCode,
                                                     In = x.Sum(x => x.returned.Quantity) - x.Sum(x => x.itemconsume.Consume),
                                                     warehouseId = x.Key.WarehouseId
                                                 });



            var BorrowedSumReturn = _context.BorrowedIssueDetails
                .Where(x => x.ItemCode == itemcode).Where(x => x.IsActive == true)
                                                             .Where(x => x.IsReturned == true)
                                                             .Where(x => x.IsApprovedReturned == true)
                                                             .GroupJoin(consumed, returned => returned.Id, itemconsume => itemconsume.BorrowedItemPkey, (returned, itemconsume) => new { returned, itemconsume })
                                                             .SelectMany(x => x.itemconsume.DefaultIfEmpty(), (x, itemconsume) => new { x.returned, itemconsume })
                                                             .GroupBy(x => new
                                                             {
                                                                 x.returned.ItemCode,

                                                             }).Select(x => new ItemStocksDto
                                                             {

                                                                 ItemCode = x.Key.ItemCode,
                                                                 In = x.Sum(x => x.returned.Quantity) - x.Sum(x => x.itemconsume.Consume),

                                                             });

            var fuelRegister = _context.FuelRegisterDetails
                .Include(m => m.Material)
                .Include(m => m.Warehouse_Receiving)
                .Where(fr => fr.Is_Active == true && fr.Material.ItemCode == itemcode)
                .GroupBy(fr => new
                {
                    fr.Material.ItemCode,
                    fr.Warehouse_ReceivingId,

                }).Select(fr => new
                {
                    itemCode = fr.Key.ItemCode,
                    WarehouseId = fr.Key.Warehouse_ReceivingId,
                    Quantity = fr.Sum(fr => fr.Liters.Value)

                });

            var getRemaining = getWarehouseStocks
                              .GroupJoin(moveorderOut, warehouse => warehouse.WarehouseId, moveorder => moveorder.WarehouseId, (warehouse, moveorder) => new { warehouse, moveorder })
                              .SelectMany(x => x.moveorder.DefaultIfEmpty(), (x, moveorder) => new { x.warehouse, moveorder })
                              .GroupJoin(BorrowedOut, warehouse => warehouse.warehouse.WarehouseId, borrowed => borrowed.warehouseId, (warehouse, borrowed) => new { warehouse, borrowed })
                              .SelectMany(x => x.borrowed.DefaultIfEmpty(), (x, borrowed) => new { x.warehouse, borrowed })
                              .GroupJoin(BorrowedReturn, warehouse => warehouse.warehouse.warehouse.WarehouseId, returned => returned.warehouseId, (warehouse, returned) => new { warehouse, returned })
                              .SelectMany(x => x.returned.DefaultIfEmpty(), (x, returned) => new { x.warehouse, returned })
                              .GroupJoin(issueOut, warehouse => warehouse.warehouse.warehouse.warehouse.WarehouseId, issue => issue.warehouseId, (warehouse, issue) => new { warehouse, issue })
                              .SelectMany(x => x.issue.DefaultIfEmpty(), (x, issue) => new {x.warehouse,issue })
                              .GroupJoin(fuelRegister, warehouse => warehouse.warehouse.warehouse.warehouse.warehouse.WarehouseId , fuel => fuel.WarehouseId, (warehouse,fuel) => new {warehouse,fuel})
                              .SelectMany(x => x.fuel.DefaultIfEmpty(), (x,fuel) => new 
                              {
                                  warehouseId = x.warehouse.warehouse.warehouse.warehouse.warehouse.WarehouseId,
                                  itemcode = x.warehouse.warehouse.warehouse.warehouse.warehouse.ItemCode,
                                  ReceivingDate = x.warehouse.warehouse.warehouse.warehouse.warehouse.RecievingDate,
                                  WarehouseActualGood = x.warehouse.warehouse.warehouse.warehouse.warehouse.ActualGood != null ? x.warehouse.warehouse.warehouse.warehouse.warehouse.ActualGood : 0,
                                  MoveOrderOut = x.warehouse.warehouse.warehouse.warehouse.moveorder.QuantityOrdered != null ? x.warehouse.warehouse.warehouse.warehouse.moveorder.QuantityOrdered : 0,
                                  IssueOut = x.warehouse.issue.Out != null ? x.warehouse.issue.Out : 0,
                                  BorrowedOut = x.warehouse.warehouse.warehouse.borrowed.Out != null ? x.warehouse.warehouse.warehouse.borrowed.Out : 0,
                                  Borrowedreturn = x.warehouse.warehouse.returned.In != null ? x.warehouse.warehouse.returned.In : 0,
                                  fuel = fuel.Quantity != null ? fuel.Quantity : 0,
                                  UnitCost = x.warehouse.warehouse.warehouse.warehouse.warehouse.UnitPrice

                              }).GroupBy(x => new
                              {
                                  x.warehouseId,
                                  x.itemcode,
                                  x.ReceivingDate,
                                  x.WarehouseActualGood,
                                  x.MoveOrderOut,
                                  x.IssueOut,
                                  x.BorrowedOut,
                                  x.Borrowedreturn,
                                  x.fuel,
                                  x.UnitCost
                              }).Select(total => new GetAvailableStocksForIssueDto
                              {
                                  WarehouseId = total.Key.warehouseId,
                                  ItemCode = total.Key.itemcode,
                                  RemainingStocks = total.Key.WarehouseActualGood + total.Key.Borrowedreturn 
                                  - total.Key.MoveOrderOut - total.Key.IssueOut - total.Key.BorrowedOut
                                  - total.Key.fuel,
                                  ReceivingDate = total.Key.ReceivingDate,
                                  UnitCost = total.Key.UnitCost

                              }).Where(x => x.RemainingStocks > 0)
                                .Where(x => x.ItemCode == itemcode);


            return await getRemaining.ToListAsync();

        }


        public async Task<PagedList<GetAllMIssueWithPaginationDto>> GetAllMIssueWithPagination(UserParams userParams, bool status)
        {
            var issue = _context.MiscellaneousIssues.OrderByDescending(x => x.PreparedDate)
                                                    .Where(x => x.IsActive == status)
                                                    .Select(x => new GetAllMIssueWithPaginationDto
                                                    {

                                                        IssuePKey = x.Id,
                                                        Customer = x.Customer,
                                                        CustomerCode = x.Customercode,
                                                        TotalQuantity = x.TotalQuantity,
                                                        PreparedDate = x.PreparedDate.ToString("MM/dd/yyyy"),
                                                        Remarks = x.Remarks,
                                                        PreparedBy = x.PreparedBy,
                                                        IsActive = x.IsActive,
                                                        TransactionDate = x.TransactionDate.ToString("MM/dd/yyyy"),
                                                        Details = x.Details
                                                    

                                                    });

            return await PagedList<GetAllMIssueWithPaginationDto>.CreateAsync(issue, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<PagedList<GetAllMIssueWithPaginationDto>> GetAllMIssueWithPaginationOrig(UserParams userParams, string search, bool status)
        {
            var issue = _context.MiscellaneousIssues.OrderByDescending(x => x.PreparedDate)
                                                    .Where(x => x.IsActive == status)
                                                    .Select(x => new GetAllMIssueWithPaginationDto
                                                    {
                                                        IssuePKey = x.Id,
                                                        Customer = x.Customer,
                                                        CustomerCode = x.Customercode,
                                                        TotalQuantity = x.TotalQuantity,
                                                        PreparedDate = x.PreparedDate.ToString("MM/dd/yyyy"),
                                                        Remarks = x.Remarks,
                                                        PreparedBy = x.PreparedBy,
                                                        IsActive = x.IsActive,
                                                        TransactionDate = x.TransactionDate.ToString("MM/dd/yyyy"),
                                                        Details = x.Details


                                                    }).Where(x => (Convert.ToString(x.IssuePKey)).ToLower().Contains(search.Trim().ToLower())
                                                      || (Convert.ToString(x.Customer)).ToLower().Contains(search.Trim().ToLower())
                                                        || (Convert.ToString(x.CustomerCode)).ToLower().Contains(search.Trim().ToLower()));

            return await PagedList<GetAllMIssueWithPaginationDto>.CreateAsync(issue, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> InActivateMiscellaenousIssue(MiscellaneousIssue issue)
        {
            var existing = await _context.MiscellaneousIssues.Where(x => x.Id == issue.Id)
                                                              .FirstOrDefaultAsync();
            var existingdetails = await _context.MiscellaneousIssueDetail.Where(x => x.IssuePKey == issue.Id)
                                                                        .ToListAsync();

            if (existing == null)
                return false;

            existing.IsActive = false;

            foreach (var items in existingdetails)
            {
                items.IsActive = false;
            }
            return true;
        }


        public async Task<bool> ActivateMiscellaenousIssue(MiscellaneousIssue issue)
        {
           var existing = await _context.MiscellaneousIssues.Where(x => x.Id == issue.Id)
                                                            .FirstOrDefaultAsync();

            var existingdetails = await _context.MiscellaneousIssueDetail.Where(x => x.IssuePKey == issue.Id)
                                                                          .ToListAsync();

            if (existing == null)
                return false;

            existing.IsActive = true;

            foreach (var items in existingdetails)
            {
                items.IsActive = true;
            }

            return true;
        }


          public async Task<IReadOnlyList<GetAllDetailsInMiscellaneousIssueDto>> GetAllDetailsInMiscellaneousIssue(int id)
        {


            var issues = _context.MiscellaneousIssues.Where(x => x.IsActive == true)
                                             .Select(x => new GetAllDetailsInMiscellaneousIssueDto
                                             {
                                                 Id = x.Id,
                                                 Customer = x.Customer,
                                                 CustomerCode = x.Customercode,

                                                 TransactionDate = x.TransactionDate.ToString(),
                                                 CompanyCode = x.CompanyCode,
                                                 CompanyName = x.CompanyName,
                                                 DepartmentCode = x.DepartmentCode,
                                                 DepartmentName = x.DepartmentName,
                                                 LocationCode = x.LocationCode,
                                                 LocationName = x.LocationName,
                                                 //AccountCode = x.AccountCode,
                                                 //AccountTitles = x.AccountTitles,
                                                 //EmpId = x.EmpId,
                                                 //FullName = x.FullName,
                                                 Details  = x.Details,

                                             });


            var warehouse = _context.MiscellaneousIssueDetail
                .GroupJoin(issues, misc => misc.IssuePKey , issue => issue.Id, (misc, issue) => new { misc, issue })
                .SelectMany(x => x.issue.DefaultIfEmpty(), (x, issue) => new { x.misc, issue })
                .Where(x => x.misc.IssuePKey == id)
                .Select(x => new GetAllDetailsInMiscellaneousIssueDto
                {
                    IssuePKey = x.misc.IssuePKey,
                    Customer = x.issue.Customer,
                    CustomerCode = x.issue.CustomerCode,
                    PreparedDate = x.misc.PreparedDate.ToString(),
                    PreparedBy = x.misc.PreparedBy,
                    ItemCode = x.misc.ItemCode,
                    ItemDescription = x.misc.ItemDescription,
                    TotalQuantity = x.misc.Quantity,
                    Remarks = x.misc.Remarks,

                    Uom = x.misc.Uom,

                    TransactionDate = x.issue.TransactionDate.ToString(),
                    CompanyCode = x.issue.CompanyCode,
                    CompanyName = x.issue.CompanyName,
                    DepartmentCode = x.issue.DepartmentCode,
                    DepartmentName = x.issue.DepartmentName,
                    LocationCode = x.issue.LocationCode,
                    LocationName = x.issue.LocationName,
                    AccountCode = x.misc.AccountCode,
                    AccountTitles = x.misc  .AccountTitles,
                    EmpId =x.misc.EmpId,
                    FullName = x.misc.FullName,
                    UnitCost = x.misc.UnitPrice,
                    TotalCost  = x.misc.UnitPrice * x.misc.Quantity,
                    Details = x.issue.Details
                    
                   
                });

            return await warehouse.ToListAsync();



        }


        public async Task<IReadOnlyList<GetAllAvailableIssueDto>> GetAllAvailableIssue(int empid)
        {
            var employee = await _context.Users.Where(x => x.Id == empid)
                                               .FirstOrDefaultAsync();

            var items = _context.MiscellaneousIssueDetail.Where(x => x.IsActive == true)
                                                         .Where(x => x.PreparedBy == employee.FullName)
                                                         .Where(x => x.IsTransact != true)
                                                         .Select(x => new GetAllAvailableIssueDto
                                                         {

                                                             Id = x.Id,
                                                             ItemCode = x.ItemCode,
                                                             ItemDescription = x.ItemDescription,
                                                             Uom = x.Uom,
                                                             TotalQuantity = x.Quantity,
                                                             PreparedDate = x.PreparedDate.ToString("MM/dd/yyyy"),
                                                             UnitCost = x.UnitPrice,
                                                             Barcode = x.WarehouseId,
                                                             AccountCode = x.AccountCode,
                                                             AccountTitles = x.AccountTitles,
                                                             EmpId = x.EmpId,
                                                             FullName = x.FullName



                                                         });

            return await items.ToListAsync();

        }


        public async Task<bool> CancelItemCodeInMiscellaneousIssue(MiscellaneousIssueDetails issue)
        {
            var issueDetails = await _context.MiscellaneousIssueDetail.Where(x => x.Id == issue.Id)
                                                                .FirstOrDefaultAsync();

          

            if (issueDetails == null)
                    return false;

            issueDetails.IsActive = false;
            
            return true;
        }




        // ================================================================= Validation =====================================================

        public async Task<bool> ValidateMiscellaneousInIssue(MiscellaneousReceipt receipt)
        {
            var validate = await _context.WarehouseReceived.Where(x => x.MiscellaneousReceiptId == receipt.Id)
                                                           .ToListAsync();

            foreach(var items in validate)
            {
                var issue = await _context.MiscellaneousIssueDetail.Where(x => x.WarehouseId == items.Id )
                                                                   .FirstOrDefaultAsync();

                if (issue != null)
                    return false;
              
            }
            return true;
        }

        public async Task<IReadOnlyList<MiscReceiptItemListDto>> MiscReceiptItemList()
        {

            var warehouseList = _context.WarehouseReceived
                .Where(x => x.IsActive == true)
                .Select(x => new WarehouseInventory
                { 
                    WarehouseId = x.Id,
                    ItemCode = x.ItemCode, 
                    ActualGood = x.ActualGood,
                    UnitPrice = x.UnitPrice, 
                });

            var moveOrderList = _context.MoveOrders
                .Where(x => x.IsActive == true && x.IsPrepared == true)
                .GroupBy(x => new
                {
                    x.WarehouseId,
                    x.ItemCode
                })
                //.Where(x => x.ItemCode == itemCode)
                .Select(x => new MoveOrderInventory
                {
                    WarehouseId = x.Key.WarehouseId,
                    ItemCode = x.Key.ItemCode,
                    QuantityOrdered = x.Sum(x => x.QuantityOrdered),
                });


               var issueList = _context.MiscellaneousIssueDetail
                .Where(x => x.IsActive == true)
                //.Where(x => x.ItemCode == itemCode)
                .GroupBy(x => new
                {
                    x.WarehouseId,
                    x.ItemCode
                })
                .Select(x => new DtoIssueInventory
                {
                    WarehouseId = x.Key.WarehouseId,
                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.Quantity),

                });

            var borrowedList = _context.BorrowedIssueDetails
                .Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.WarehouseId,
                    x.ItemCode,
                })
                //.Where(x => x.ItemCode == itemCode)
                .Select(x => new DtoBorrowedIssue
                {
                    WarehouseId = x.Key.WarehouseId,
                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.Quantity),

                });

            var consumed = _context.BorrowedConsumes
                .Where(x => x.IsActive)
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

            var returnedList = _context.BorrowedIssueDetails
                .Where(x => x.IsActive == true)
                .Where(x => x.IsReturned == true)
                .Where(x => x.IsApprovedReturned == true)
                //.Where(x => x.ItemCode == itemCode)
                .GroupJoin(consumed, returned => returned.Id, itemconsume => itemconsume.BorrowedItemPkey, 
                (returned, itemconsume) => new { returned, itemconsume })
                 .SelectMany(x => x.itemconsume.DefaultIfEmpty(), (x, itemconsume) => new { x.returned, itemconsume })
                 .GroupBy(x => new
                 {
                     x.returned.WarehouseId,
                     x.returned.ItemCode
                 })
                 .Select(x => new DtoBorrowedIssue
                 {
                     WarehouseId = x.Key.WarehouseId,
                     ItemCode = x.Key.ItemCode,
                     ReturnQuantity = x.Sum(x => x.returned.Quantity != null ? x.returned.Quantity : 0 - x.itemconsume.Consume) 

                 });

            var fuelRegister = _context.FuelRegisterDetails
               .Include(m => m.Material)
               .Include(m => m.Warehouse_Receiving)
               .Where(fr => fr.Is_Active == true)
                   .GroupBy(fr => new
                   {
                       fr.Material.ItemCode,
                       fr.Warehouse_ReceivingId,

                   }).Select(fr => new
                   {
                       itemCode = fr.Key.ItemCode,
                       WarehouseId = fr.Key.Warehouse_ReceivingId,
                       Quantity = fr.Sum(fr => fr.Liters.Value)

                   });


            var getUnitPrice = (from warehouse in warehouseList
                                join moveorder in moveOrderList
                                on warehouse.WarehouseId equals moveorder.WarehouseId
                                into leftJ1
                                from moveorder in leftJ1.DefaultIfEmpty()

                                join issue in issueList
                                on warehouse.WarehouseId equals issue.WarehouseId
                                into leftJ2
                                from issue in leftJ2.DefaultIfEmpty()

                                join borrow in borrowedList
                                on warehouse.WarehouseId equals borrow.WarehouseId
                                into leftJ3                                   
                                from borrow in leftJ3.DefaultIfEmpty()

                                join returned in returnedList
                                on warehouse.WarehouseId equals returned.WarehouseId
                                into leftJ4
                                from returned in leftJ4.DefaultIfEmpty()

                                join fuel in fuelRegister
                                on warehouse.WarehouseId equals fuel.WarehouseId
                                into leftJ5
                                from fuel in leftJ5.DefaultIfEmpty()


                                group new
                                {
                                    warehouse,
                                    moveorder,
                                    issue,
                                    borrow,
                                    returned,
                                    fuel,


                                }

                              by new
                              {
                                  warehouse.WarehouseId,
                                  warehouse.ItemCode,
                                  warehouse.ActualGood,

                              }

                                into x
                                select new WarehouseInventory
                                {

                                    WarehouseId = x.Key.WarehouseId,
                                    ItemCode = x.Key.ItemCode,
                                    UnitPrice =  x.First().warehouse.UnitPrice,
                                    ActualGood = x.Sum(x => x.warehouse.ActualGood)+ x.Sum(x => x.returned.ReturnQuantity) 
                                    - x.Sum(x => x.moveorder.QuantityOrdered) - x.Sum(x => x.issue.Quantity) - x.Sum(x => x.borrow.Quantity)
                                   - x.Sum(x => x.fuel.Quantity != null ? x.fuel.Quantity : 0)
                                });


            var multiplyUnitPrice = getUnitPrice.
                                 Select(x => new WarehouseInventory
                                 {
                                     WarehouseId = x.WarehouseId,
                                     ItemCode = x.ItemCode,
                                     UnitPrice = x.UnitPrice * x.ActualGood,
                                     ActualGood = x.ActualGood

                                 });



            var getUnitpriceTotal = multiplyUnitPrice
                .Where(x => x.UnitPrice > 0)
                .GroupBy(x => x.ItemCode)
            .Select(x => new WarehouseInventory
            {
                ItemCode = x.Key,
                UnitPrice = x.Sum(x => x.UnitPrice != null ? x.UnitPrice : 0) / x.Sum(x => x.ActualGood),
                ActualGood = x.Sum(x => x.ActualGood),
                TotalUnitPrice = x.Sum(x => x.UnitPrice != null ? x.UnitPrice : 0)
            });

            var result = _context.Materials
                .Where(x => x.IsActive == true)
                .GroupJoin(getUnitpriceTotal, material => material.ItemCode, unitcost => unitcost.ItemCode, (material, unitcost) => new { material, unitcost })
                .SelectMany(x => x.unitcost.DefaultIfEmpty(), (x, unitcost) => new { x.material, unitcost })
                .GroupBy(x => x.material.ItemCode)
                .Select(x => new MiscReceiptItemListDto
                {
                    ItemCode = x.Key,
                    ItemDescription = x.First().material.ItemDescription,
                    UomCode = x.First().material.Uom.UomCode,
                    UnitCost = decimal.Round(x.First().unitcost.UnitPrice != null ? x.First().unitcost.UnitPrice : 0, 2)

                }).OrderBy(x => x.ItemCode);

            return await result.ToListAsync();
            
        }
    }
}
