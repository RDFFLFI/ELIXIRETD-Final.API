using ELIXIRETD.DATA.CORE.INTERFACES.BORROWED_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.BORROWED_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.BORROWED_REPOSITORY
{
    public class BorrowedRepository : IBorrowedItem
    {
        private readonly StoreContext _context;

        public BorrowedRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<PagedList<GetAllBorrowedReceiptWithPaginationDto>> GetAllBorrowedReceiptWithPagination(UserParams userParams, bool status)
        {
            var borrow = _context.BorrowedIssues.OrderByDescending(x => x.PreparedDate)
                                                  .Where(x => x.IsReturned == null)
                                                  .Where(x => x.IsActive == status)                                              
                                                  .Select(x => new GetAllBorrowedReceiptWithPaginationDto
                                                  {

                                                      BorrowedPKey = x.Id,
                                                      CustomerName = x.CustomerName,
                                                      CustomerCode = x.CustomerCode,
                                                      TotalQuantity = x.TotalQuantity,
                                                      PreparedBy =  x.PreparedBy,
                                                      IsActive = x.IsActive,
                                                      Remarks = x.Remarks,
                                                      BorrowedDate = x.PreparedDate.ToString(),

                                                  });

            return await PagedList<GetAllBorrowedReceiptWithPaginationDto>.CreateAsync(borrow, userParams.PageNumber, userParams.PageSize);

        }


        public async Task<PagedList<GetAllBorrowedReceiptWithPaginationDto>> GetAllBorrowedIssuetWithPaginationOrig(UserParams userParams, string search, bool status)
        {

            var borrow = _context.BorrowedIssues.OrderByDescending(x => x.PreparedDate)
                                                  .Where(x => x.IsReturned == null)
                                                  .Where(x => x.IsActive == status)
                                                  .Select(x => new GetAllBorrowedReceiptWithPaginationDto
                                                  {

                                                      BorrowedPKey = x.Id,
                                                      CustomerName = x.CustomerName,
                                                      CustomerCode = x.CustomerCode,
                                                      TotalQuantity = x.TotalQuantity,
                                                      PreparedBy = x.PreparedBy,
                                                      IsActive = x.IsActive,
                                                      Remarks = x.Remarks,
                                                      BorrowedDate = x.PreparedDate.ToString(),

                                                  })
                                                  .Where(x => (Convert.ToString(x.BorrowedPKey)).ToLower()
                                                  .Contains(search.Trim().ToLower()));

            return await PagedList<GetAllBorrowedReceiptWithPaginationDto>.CreateAsync(borrow, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> AddBorrowedIssue(BorrowedIssue borrowed)
        {
            await _context.BorrowedIssues.AddAsync(borrowed);

            return true;
        }


        public async Task<bool> AddBorrowedIssueDetails(BorrowedIssueDetails borrowed)
        {

            await _context.BorrowedIssueDetails.AddAsync(borrowed);
            return true;

        }


        public async Task<IReadOnlyList<GetAvailableStocksForBorrowedIssue_Dto>> GetAvailableStocksForBorrowedIssue(string itemcode)
        {
            var getWarehouseStocks = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                               .GroupBy(x => new
                                                               {

                                                                   x.Id,
                                                                   x.ItemCode,
                                                                   x.ActualGood,
                                                                   x.ActualReceivingDate,

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

                                                      x.WarehouseId,
                                                      x.ItemCode

                                                  }).Select(x => new MoveOrderInventory
                                                  {

                                                      WarehouseId = x.Key.WarehouseId,
                                                      ItemCode = x.Key.ItemCode,
                                                      QuantityOrdered = x.Sum(x => x.QuantityOrdered)

                                                  });


            var issueOut = _context.MiscellaneousIssueDetail.Where(x => x.IsActive == true)
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

            var BorrowedOut = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                           .GroupBy(x => new
                                                           {
                                                               x.ItemCode,
                                                               x.WarehouseId

                                                           }).Select(x => new ItemStocksDto
                                                           {
                                                               ItemCode = x.Key.ItemCode,
                                                               Out =  x.Sum(x => x.Quantity),                                                   
                                                               warehouseId = x.Key.WarehouseId

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


            var getAvailable = (from warehouse in getWarehouseStocks
                                join Moveorder in moveorderOut
                                on warehouse.WarehouseId equals Moveorder.WarehouseId
                                into leftJ1
                                from Moveorder in leftJ1.DefaultIfEmpty()

                                join issue in issueOut
                                on warehouse.WarehouseId equals issue.warehouseId
                                into leftJ2
                                from issue in leftJ2.DefaultIfEmpty()

                                join borrowOut in BorrowedOut
                                on warehouse.WarehouseId equals borrowOut.warehouseId
                                into leftJ3
                                from borrowOut in leftJ3.DefaultIfEmpty()

                                join returned in BorrowedReturn
                                on warehouse.WarehouseId equals returned.warehouseId
                                into LeftJ4
                                from returned in LeftJ4.DefaultIfEmpty()

                                group new
                                {

                                    warehouse,
                                    Moveorder,
                                    issue,
                                    borrowOut,
                                    returned,
                                }

                                by new
                                {

                                    warehouse.WarehouseId,
                                    warehouse.ItemCode,
                                    warehouse.RecievingDate,
                                    WarehouseActualGood = warehouse.ActualGood != null ? warehouse.ActualGood : 0 ,
                                    MoveOrderOut = Moveorder.QuantityOrdered != null ? Moveorder.QuantityOrdered : 0,
                                    IssueOut = issue.Out != null ? issue.Out : 0,
                                    BorrowedOut = borrowOut.Out != null ? borrowOut.Out : 0 ,
                                    borrowedreturn = returned.In != null ? returned.In : 0,


                                } into total

                                select new GetAvailableStocksForBorrowedIssue_Dto
                                {

                                    WarehouseId = total.Key.WarehouseId,
                                    ItemCode = total.Key.ItemCode,
                                    RemainingStocks = total.Key.WarehouseActualGood + total.Key.borrowedreturn - total.Key.MoveOrderOut - total.Key.IssueOut - total.Key.BorrowedOut,
                                    ReceivingDate = total.Key.RecievingDate,

                                }).Where(x => x.RemainingStocks != 0)
                                   .Where(x => x.ItemCode == itemcode);

            return await getAvailable.ToListAsync();

        }



        public async Task<bool> UpdateIssuePKey(BorrowedIssueDetails borowed)
        {

            var existing = await _context.BorrowedIssueDetails.Where(x => x.Id == borowed.Id)
                                                               .FirstOrDefaultAsync();
            if (existing == null)
                return false;

            existing.BorrowedPKey = borowed.BorrowedPKey;
            existing.IsActive = borowed.IsActive;
            existing.IsTransact = true;
          

            return true;
        }


        public async Task<bool> InActiveBorrowedIssues(BorrowedIssue borrowed)
        {

            var existing = await _context.BorrowedIssues.Where(x => x.Id == borrowed.Id)
                                                        .FirstOrDefaultAsync();


            var existingdetails = await _context.BorrowedIssueDetails.Where(x => x.BorrowedPKey == borrowed.Id)
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

        public async Task<bool> ActiveBorrowedIssues(BorrowedIssue borrowed)
        {
            var existing = await _context.BorrowedIssues.Where(x => x.Id == borrowed.Id)
                                                       .FirstOrDefaultAsync();


            var existingdetails = await _context.BorrowedIssueDetails.Where(x => x.BorrowedPKey == borrowed.Id)
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


        public async Task<IReadOnlyList<GetAllDetailsInBorrowedIssueDto>> GetAllDetailsInBorrowedIssue(int id)
        {
            var warehouse = _context.BorrowedIssueDetails

              .OrderBy(x => x.WarehouseId)
              .ThenBy(x => x.PreparedDate)
              .ThenBy(x => x.ItemCode)
              .ThenBy(x => x.CustomerName)
              .Where(x => x.BorrowedPKey == id)
              .Where(x => x.IsTransact == true)
              .Where(x => x.IsReturned == null)
              .Where(x => x.IsActive == true)
               .Select(x => new GetAllDetailsInBorrowedIssueDto
               {

                   Id = x.Id,
                   WarehouseId = x.WarehouseId,
                   BorrowedPKey = x.BorrowedPKey,
                   Customer = x.CustomerName,
                   CustomerCode = x.CustomerCode,
                   PreparedDate = x.PreparedDate.ToString(),
                   ItemCode = x.ItemCode,
                   ItemDescription = x.ItemDescription,
                   Quantity = x.Quantity,
                   Consumes = x.Quantity - x.ReturnQuantity,
                   ReturnQuantity = x.ReturnQuantity != null ? x.ReturnQuantity : 0,
                   Remarks = x.Remarks

               });

            return await warehouse.ToListAsync();
        }


        public async Task<IReadOnlyList<GetAllAvailableBorrowIssueDto>> GetAllAvailableIssue(int empid)
        {
            var employee = await _context.Users.Where(x => x.Id == empid)
                                                .FirstOrDefaultAsync();

            var items = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                         .Where(x => x.PreparedBy == employee.FullName)
                                                         .Where(x => x.IsTransact != true)
                                                         .Select(x => new GetAllAvailableBorrowIssueDto
                                                         {

                                                             Id = x.Id,
                                                             ItemCode = x.ItemCode,
                                                             ItemDescription = x.ItemDescription,
                                                             Uom = x.Uom,
                                                             TotalQuantity = x.Quantity,
                                                             BorrowDate = x.BorrowedDate.ToString()

                                                         });

            return await items.ToListAsync();
        }


        public async Task<bool> CancelIssuePerItemCode(BorrowedIssueDetails borrowed)
        {

            var items = await _context.BorrowedIssueDetails.Where(x => x.Id == borrowed.Id)
                                                           .Where(x => x.IsActive == true)
                                                           .Where(x => x.IsTransact == true)
                                                           .FirstOrDefaultAsync();


            items.ReturnQuantity = 0;
            
            return true;

        }

        //====================================================== Returned ====================================================//

        public async Task<bool> EditReturnQuantity(BorrowedIssueDetails borrowed)
        {

            var editquantity = await _context.BorrowedIssueDetails.Where(x => x.Id == borrowed.Id)
                                                                  .FirstOrDefaultAsync();

            if (editquantity == null)
                return false;

            editquantity.ReturnQuantity = borrowed.ReturnQuantity;

            if (editquantity.Quantity < editquantity.ReturnQuantity || editquantity.ReturnQuantity < 0)
                return false;

            return true;

        }



        public async Task<bool> SaveReturnedQuantity(BorrowedIssue borrowed)
        {
            var returned = await _context.BorrowedIssues
                .Where(x => x.Id == borrowed.Id)
                .FirstOrDefaultAsync();

            var returnedDetails = await _context.BorrowedIssueDetails
                .Where(x => x.BorrowedPKey == borrowed.Id)
                .ToListAsync();



            foreach (var item in returnedDetails)
            {


                item.IsReturned = true;
                item.IsActive = true;
                item.ReturnedDate = DateTime.Now;

            }

            returned.IsReturned = true;
            returned.IsActive = true;

            return true;
        }


        public async Task<PagedList<DtoGetAllReturnedItem>> GetAllReturnedItem(UserParams userParams)
        {
            var BorrowIssue = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                     .Where(x => x.IsReturned == true)
                                                     .GroupBy(x => new
                                                     {

                                                         x.BorrowedPKey,
                                                         x.CustomerCode,
                                                         x.CustomerName,
                                                         x.PreparedBy,
                                                         x.ReturnedDate,


                                                     }).Select(total => new DtoGetAllReturnedItem
                                                     {
                                                         Id = total.Key.BorrowedPKey,
                                                         CustomerCode = total.Key.CustomerCode,
                                                         CustomerName = total.Key.CustomerName,
                                                         Consumed = total.Sum(x => x.Quantity) - total.Sum(x => x.ReturnQuantity),
                                                         TotalReturned = total.Sum(x => x.ReturnQuantity),
                                                         PreparedBy = total.Key.PreparedBy,
                                                         ReturnedDate = total.Key.ReturnedDate.ToString(),

                                                     });

            return await PagedList<DtoGetAllReturnedItem>.CreateAsync(BorrowIssue, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<PagedList<DtoGetAllReturnedItem>> GetAllReturnedItemOrig(UserParams userParams, string search)
        {

            var BorrowIssue = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                     .Where(x => x.IsReturned == true)
                                                     .Where(x => x.IsTransact == false)
                                                     .GroupBy(x => new
                                                     {

                                                         x.BorrowedPKey,
                                                         x.CustomerCode,
                                                         x.CustomerName,
                                                         x.PreparedBy,
                                                         x.ReturnedDate,


                                                     }).Select(total => new DtoGetAllReturnedItem
                                                     {
                                                         Id = total.Key.BorrowedPKey,
                                                         CustomerCode = total.Key.CustomerCode,
                                                         CustomerName = total.Key.CustomerName,
                                                         TotalReturned = total.Sum(x => x.ReturnQuantity),
                                                         Consumed = total.Sum(x => x.Quantity) - total.Sum(x => x.ReturnQuantity),
                                                         PreparedBy = total.Key.PreparedBy,
                                                         ReturnedDate = total.Key.ReturnedDate.ToString(),

                                                     }).Where(x => (Convert.ToString(x.Id)).ToLower()
                                                       .Contains(search.Trim().ToLower())); ;


            return await PagedList<DtoGetAllReturnedItem>.CreateAsync(BorrowIssue, userParams.PageNumber, userParams.PageSize);

        }


        public async Task<IReadOnlyList<DtoViewBorrewedReturnedDetails>> ViewBorrewedReturnedDetails(int id)
        {
            var borrow = _context.BorrowedIssueDetails.Where(x => x.BorrowedPKey == id)
                                                     .Where(x => x.IsActive == true)
                                                     .Where(x => x.IsReturned == true)
                                                       .Select(x => new DtoViewBorrewedReturnedDetails
                                                       {
                                                           ItemCode = x.ItemCode,
                                                           ItemDescription = x.ItemDescription,
                                                           ReturnQuantity = x.ReturnQuantity,
                                                           Consume = x.Quantity - x.ReturnQuantity,
                                                           ReturnedDate = x.ReturnedDate.ToString()

                                                       }).OrderByDescending(x => x.ReturnedDate);


            return await borrow.ToListAsync();
        }


        public async Task<bool> Cancelborrowedfortransact(BorrowedIssueDetails borrowed)
        {
            var borrow = await _context.BorrowedIssueDetails.Where(x => x.Id == borrowed.Id)
                                                             .FirstOrDefaultAsync();


            borrow.IsActive = false;

            return true;

        }

      
    }
}
