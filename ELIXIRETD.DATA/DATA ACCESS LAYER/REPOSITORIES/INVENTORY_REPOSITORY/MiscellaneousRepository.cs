using ELIXIRETD.DATA.CORE.INTERFACES.INVENTORY_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
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
            await _context.WarehouseReceived.AddAsync(receive);
            return true;
        }

        public async Task<bool> InActiveMiscellaneousReceipt(MiscellaneousReceipt receipt)
        {
            var existing = await _context.MiscellaneousReceipts.Where(x => x.Id == receipt.Id)
                                                               .FirstOrDefaultAsync();

            var existingWH = await _context.WarehouseReceived.Where(x => x.MiscellanousReceiptId == receipt.Id)
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

            var existingWH = await _context.WarehouseReceived.Where(x => x.MiscellanousReceiptId == receipt.Id)
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
                                                            Remarks = x.Remarks,
                                                            IsActive= x.IsActive
                                                            
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
                                                            Remarks = x.Remarks,
                                                            IsActive = x.IsActive

                                                        }).Where(x => (Convert.ToString(x.Id)).ToLower()
                                                          .Contains(search.Trim().ToLower()));

            return await PagedList<GetAllMReceiptWithPaginationdDto>.CreateAsync(receipt , userParams.PageNumber , userParams.PageSize);

        }


        public async Task<IReadOnlyList<GetWarehouseDetailsByMReceiptDto>> GetWarehouseDetailsByMReceipt(int id)
        {

            var receipt = _context.WarehouseReceived
                      .Where(x => x.MiscellanousReceiptId == id && x.IsActive == true)
                      .GroupJoin(_context.MiscellaneousReceipts, warehouse => warehouse.MiscellanousReceiptId, receiptparents => receiptparents.Id, (warehouse, receiptparents) => new { warehouse, receiptparents })
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
                          PreparedDate = x.receiptparents.PreparedBy,
                          Remarks = x.receiptparents.Remarks

                      });

            return await receipt.ToListAsync();

        }


        //================================================= Miscellaneous Issue ===================================================================

        public async Task<bool> AddMiscellaneousIssueDetails(MiscellaneousIssueDetails details)
        {
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

        public async Task<IReadOnlyList<GetAvailableStocksForIssueDto>> GetAvailableStocksForIssue(string itemcode)
        {
            var getWarehouseStocks = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                               .GroupBy(x => new
                                                               {
                                                                   x.Id,
                                                                   x.ItemCode,
                                                                   x.ActualGood,
                                                                   x.ActualReceivingDate
                                                               }).Select(x => new WarehouseInventory
                                                               {
                                                                   WarehouseId = x.Key.Id,
                                                                   ItemCode = x.Key.ItemCode,
                                                                   ActualGood = x.Key.ActualGood,
                                                                   RecievingDate = x.Key.ActualReceivingDate.ToString()

                                                               });

            var moveorderOut = _context.MoveOrders.Where(x => x.IsActive == true)
                                                  .Where(x => x.IsPrepared == true)
                                                  .GroupBy(x => new
                                                  {
                                                      x.warehouseId,
                                                      x.ItemCode
                                                  }).Select(x => new MoveOrderInventory
                                                  {
                                                      WarehouseId = x.Key.warehouseId,
                                                      ItemCode = x.Key.ItemCode,
                                                      QuantityOrdered = x.Sum(x => x.QuantityOrdered)

                                                  });

            var issueOut = _context.MiscellaneousIssueDetail.Where(x => x.IsActive == true)
                                                            .GroupBy(x => new
                                                            {
                                                                x.ItemCode,
                                                                x.WareHouseId

                                                            }).Select(x => new ItemStocksDto
                                                            {
                                                                ItemCode = x.Key.ItemCode,
                                                                warehouseId = x.Key.WareHouseId,
                                                                Out = x.Sum(x => x.Quantity)

                                                            });

            var borrowedOut = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                           .GroupBy(x => new
                                                           {

                                                               x.ItemCode,
                                                               x.WarehouseId

                                                           }).Select(x => new ItemStocksDto
                                                           {
                                                               ItemCode = x.Key.ItemCode,
                                                               warehouseId = x.Key.WarehouseId,
                                                               Out = x.Sum(x => x.Quantity)

                                                           });

            var BorrowedReturn = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                             .Where(x => x.IsReturned == true)
                                                             .GroupBy(x => new
                                                             {
                                                                 x.ItemCode,
                                                                 x.WarehouseId,

                                                             }).Select(x => new ItemStocksDto
                                                             {

                                                                 ItemCode = x.Key.ItemCode,
                                                                 In = x.Sum(x => x.ReturnQuantity),
                                                                 warehouseId = x.Key.WarehouseId,

                                                             });


            var getAvailable = getWarehouseStocks
                              .GroupJoin(moveorderOut, warehouse => warehouse.WarehouseId, moveorder => moveorder.WarehouseId, (warehouse, moveorder) => new { warehouse, moveorder })
                              .SelectMany(x => x.moveorder.DefaultIfEmpty(), (x, moveorder) => new { x.warehouse, moveorder })
                              .GroupJoin(borrowedOut , warehouse => warehouse.warehouse.WarehouseId , borrowed => borrowed.warehouseId , ( warehouse , borrowed) => new { warehouse, borrowed })
                              .SelectMany(x => x.borrowed.DefaultIfEmpty() , (x , borrowed) => new {x.warehouse , borrowed })
                              .GroupJoin(BorrowedReturn, warehouse => warehouse.warehouse.warehouse.WarehouseId, returned => returned.warehouseId , (warehouse , returned) => new {warehouse , returned })
                              .SelectMany(x => x.returned.DefaultIfEmpty(), (x, returned) => new {x.warehouse, returned})
                              .GroupJoin(issueOut, warehouse => warehouse.warehouse.warehouse.warehouse.WarehouseId, issue => issue.warehouseId, (warehouse, issue) => new { warehouse, issue })
                              .SelectMany(x => x.issue.DefaultIfEmpty(), (x, issue) => new
                              {
                                  warehouseId = x.warehouse.warehouse.warehouse.warehouse.WarehouseId,
                                  itemcode = x.warehouse.warehouse.warehouse.warehouse.ItemCode,
                                  ReceivingDate = x.warehouse.warehouse.warehouse.warehouse.RecievingDate,
                                  WarehouseActualGood = x.warehouse.warehouse.warehouse.warehouse.ActualGood != null ? x.warehouse.warehouse.warehouse.warehouse.ActualGood : 0,
                                  MoveOrderOut = x.warehouse.warehouse.warehouse.moveorder.QuantityOrdered != null ? x.warehouse.warehouse.warehouse.moveorder.QuantityOrdered : 0,
                                  IssueOut = issue.Out != null ? issue.Out : 0,
                                  BorrowedOut = x.warehouse.warehouse.borrowed.Out != null ? x.warehouse.warehouse.borrowed.Out : 0,
                                  Borrowedreturn = x.warehouse.returned.In != null ? x.warehouse.returned.In : 0,   

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

                              }
                              //,
                              //x => x
                              ).Select(total => new GetAvailableStocksForIssueDto
                              {
                                  WarehouseId = total.Key.warehouseId,
                                  ItemCode = total.Key.itemcode,
                                  RemainingStocks = total.Key.WarehouseActualGood + total.Key.Borrowedreturn - total.Key.MoveOrderOut - total.Key.IssueOut - total.Key.BorrowedOut,
                                  ReceivingDate = total.Key.ReceivingDate

                              }).Where(x => x.RemainingStocks != 0)
                              .Where(x => x.ItemCode == itemcode);

            return await getAvailable.ToListAsync();

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
                                                        IsActive = x.IsActive

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
                                                        IsActive = x.IsActive

                                                    }).Where(x => (Convert.ToString(x.IssuePKey)).ToLower()
                                                      .Contains(search.Trim().ToLower()));

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

            var warehouse = _context.MiscellaneousIssueDetail.Where(x => x.IssuePKey == id)
                                                             .Select(x => new GetAllDetailsInMiscellaneousIssueDto
                                                             {
                                                                 IssuePKey = x.IssuePKey,
                                                                 Customer = x.Customer,
                                                                 CustomerCode = x.CustomerCode,
                                                                 PreparedDate = x.PreparedDate.ToString(),
                                                                 PreparedBy = x.PreparedBy,
                                                                 ItemCode = x.ItemCode,
                                                                 ItemDescription = x.ItemDescription,
                                                                 TotalQuantity = x.Quantity,
                                                                 Remarks = x.Remarks

                                                             });

            return await warehouse.ToListAsync();
        }

        public async Task<IReadOnlyList<GetAllAvailableIssueDto>> GetAllAvailableIssue(int empid)
        {
            var employee = await _context.Users.Where(x => x.Id == empid)
                                               .FirstOrDefaultAsync();

            var items = _context.MiscellaneousIssueDetail.Where(x => x.IsActive == true)
                                                         .Where(x => x.PreparedBy == employee.FullName)
                                                         .Select(x => new GetAllAvailableIssueDto
                                                         {

                                                             Id = x.Id,
                                                             ItemCode = x.ItemCode,
                                                             ItemDescription= x.ItemDescription,
                                                             Uom = x.Uom,
                                                             TotalQuantity = x.Quantity,
                                                             PreparedDate = x.PreparedDate.ToString("MM/dd/yyyy")

                                                         });

            return await items.ToListAsync();

        }

        public async Task<bool> CancelItemCodeInMiscellaneousIssue(MiscellaneousIssueDetails issue)
        {
            var validate = await _context.WarehouseReceived.Where(x => x.MiscellanousReceiptId == issue.Id)
                                                           .ToListAsync();

            foreach (var items in validate)
            {
                var issueDetails = await _context.MiscellaneousIssueDetail.Where(x => x.WareHouseId == items.Id)
                                                                    .FirstOrDefaultAsync();

                if (issueDetails != null)
                    return false;
            }

            return true;
        }




        // ================================================================= Validation =====================================================

        public async Task<bool> ValidateMiscellaneousInIssue(MiscellaneousReceipt receipt)
        {
            var validate = await _context.WarehouseReceived.Where(x => x.MiscellanousReceiptId == receipt.Id)
                                                           .ToListAsync();

            foreach(var items in validate)
            {
                var issue = await _context.MiscellaneousIssueDetail.Where(x => x.WareHouseId == items.Id )
                                                                   .FirstOrDefaultAsync();

                if (issue != null)
                    return false;
              
            }
            return true;
        }

      
    }
}
