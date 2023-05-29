using ELIXIRETD.DATA.CORE.INTERFACES.BORROWED_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.TransactDto;
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
                                                  .Where(x => x.IsReject == null)
                                                  .Where(x => x.IsActive == status)                                              
                                                  .Select(x => new GetAllBorrowedReceiptWithPaginationDto
                                                  {

                                                      BorrowedPKey = x.Id,
                                                      CustomerName = x.CustomerName,
                                                      CustomerCode = x.CustomerCode,
                                                      TotalQuantity = x.TotalQuantity,
                                                      PreparedBy =  x.PreparedBy,
                                                      IsActive = x.IsActive,
                                                      IsApproved  = x.IsApproved,
                                                      Remarks = x.Remarks,
                                                      BorrowedDate = x.PreparedDate.ToString(),
                                                      TransactionDate = x.TransactionDate.ToString("MM/dd/yyyy")

                                                  });

            return await PagedList<GetAllBorrowedReceiptWithPaginationDto>.CreateAsync(borrow, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<GetAllBorrowedReceiptWithPaginationDto>> GetAllBorrowedIssuetWithPaginationOrig(UserParams userParams, string search, bool status)
        {

            var borrow = _context.BorrowedIssues.OrderByDescending(x => x.PreparedDate)
                                                  .Where(x => x.IsReturned == null)
                                                  .Where(x => x.IsReject == null)
                                                  .Where(x => x.IsActive == status)
                                                  .Select(x => new GetAllBorrowedReceiptWithPaginationDto
                                                  {

                                                      BorrowedPKey = x.Id,
                                                      CustomerName = x.CustomerName,
                                                      CustomerCode = x.CustomerCode,
                                                      TotalQuantity = x.TotalQuantity,
                                                      PreparedBy = x.PreparedBy,
                                                      IsApproved = x.IsApproved,
                                                      IsActive = x.IsActive,
                                                      Remarks = x.Remarks,
                                                      BorrowedDate = x.PreparedDate.ToString(),
                                                      TransactionDate = x.TransactionDate.ToString("MM/dd/yyyy")

                                                  })
                                                  .Where(x => (Convert.ToString(x.BorrowedPKey)).ToLower().Contains(search.Trim().ToLower())
                                                    || (Convert.ToString(x.CustomerCode)).ToLower().Contains(search.Trim().ToLower())
                                                      || (Convert.ToString(x.CustomerName)).ToLower().Contains(search.Trim().ToLower()));

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

            var issueBorrowed = _context.BorrowedIssues.Where(x => x.IsActive == true)
                                               .Select(x => new GetAllDetailsInBorrowedIssueDto
                                               {
                                                   Id = x.Id,
                                                   Customer = x.CustomerName,
                                                   CustomerCode = x.CustomerCode,

                                                   TransactionDate = x.TransactionDate.ToString(),
                                                   CompanyCode = x.CompanyCode,
                                                   CompanyName = x.CompanyName,
                                                   DepartmentCode = x.DepartmentCode,
                                                   DepartmentName = x.DepartmentName,
                                                   LocationCode = x.LocationCode,
                                                   LocationName = x.LocationName,
                                                   AccountCode = x.AccountCode,
                                                   AccountTitles = x.AccountTitles,


                                               });

            var warehouse = _context.BorrowedIssueDetails
                .GroupJoin(issueBorrowed, borrowed => borrowed.BorrowedPKey, issues => issues.Id, (borrowed, issues) => new { borrowed, issues })
                .SelectMany(x => x.issues.DefaultIfEmpty(), (x, issues) => new { x.borrowed, issues })
                .OrderBy(x => x.borrowed.WarehouseId)
              .ThenBy(x => x.borrowed.PreparedDate)
              .ThenBy(x => x.borrowed.ItemCode)
              .ThenBy(x => x.borrowed.CustomerName)
              .Where(x => x.borrowed.BorrowedPKey == id)
              .Where(x => x.borrowed.IsTransact == true)
              .Where(x => x.borrowed.IsReturned == null)
              .Where(x => x.borrowed.IsActive == true)
              .Select(x => new GetAllDetailsInBorrowedIssueDto
              {

                  Id = x.borrowed.Id,
                  WarehouseId = x.borrowed.WarehouseId,
                  BorrowedPKey = x.borrowed.BorrowedPKey,
                  Customer = x.issues.Customer,
                  CustomerCode = x.issues.CustomerCode,
                  PreparedDate = x.borrowed.PreparedDate.ToString(),
                  ItemCode = x.borrowed.ItemCode,
                  ItemDescription = x.borrowed.ItemDescription,
                  Quantity = x.borrowed.Quantity,
                  Consumes = x.borrowed.Quantity - x.borrowed.ReturnQuantity,
                  ReturnQuantity = x.borrowed.ReturnQuantity != null ? x.borrowed.ReturnQuantity : 0,
                  Remarks = x.borrowed.Remarks,
                  PreparedBy = x.borrowed.PreparedBy,

                  TransactionDate = x.issues.TransactionDate.ToString(),
                  CompanyCode = x.issues.CompanyCode,
                  CompanyName = x.issues.CompanyName,
                  DepartmentCode = x.issues.DepartmentCode,
                  DepartmentName = x.issues.DepartmentName,
                  LocationCode = x.issues.LocationCode,
                  LocationName = x.issues.LocationName,
                  AccountCode = x.issues.AccountCode,
                  AccountTitles = x.issues.AccountTitles,

              });


            //var warehouse = _context.BorrowedIssueDetails

            //  .OrderBy(x => x.WarehouseId)
            //  .ThenBy(x => x.PreparedDate)
            //  .ThenBy(x => x.ItemCode)
            //  .ThenBy(x => x.CustomerName)
            //  .Where(x => x.BorrowedPKey == id)
            //  .Where(x => x.IsTransact == true)
            //  .Where(x => x.IsReturned == null)
            //  .Where(x => x.IsActive == true)
            //   .Select(x => new GetAllDetailsInBorrowedIssueDto
            //   {

            //       Id = x.Id,
            //       WarehouseId = x.WarehouseId,
            //       BorrowedPKey = x.BorrowedPKey,
            //       Customer = x.CustomerName,
            //       CustomerCode = x.CustomerCode,
            //       PreparedDate = x.PreparedDate.ToString(),
            //       ItemCode = x.ItemCode,
            //       ItemDescription = x.ItemDescription,
            //       Quantity = x.Quantity,
            //       Consumes = x.Quantity - x.ReturnQuantity,
            //       ReturnQuantity = x.ReturnQuantity != null ? x.ReturnQuantity : 0,
            //       Remarks = x.Remarks,
            //       PreparedBy = x.PreparedBy,

            //   });

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

            //returned.DepartmentName = borrowed.DepartmentName;
            //returned.CompanyCode = borrowed.CompanyCode;
            //returned.CompanyName = borrowed.CompanyName;
            //returned.DepartmentCode = borrowed.DepartmentCode;
            //returned.DepartmentName = borrowed.DepartmentName;
            //returned.LocationCode = borrowed.LocationCode;
            //returned.LocationName = borrowed.LocationName;
            //returned.AccountCode = borrowed.AccountCode;
            //returned.AccountTitles = borrowed.AccountTitles;

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


            var borrowed = _context.BorrowedIssues.Where(x => x.IsReturned == true && x.IsActive == true)
                                                  .GroupBy(x => new
                                                  {

                                                      x.Id,
                                                      x.CustomerCode,
                                                      x.CustomerName

                                                  }).Select(x => new DtoGetAllReturnedItem
                                                  {

                                                      Id = x.Key.Id,
                                                      CustomerCode = x.Key.CustomerCode,
                                                      CustomerName = x.Key.CustomerName,
                                                      
                                                  });


            var BorrowIssue = _context.BorrowedIssueDetails
                .GroupJoin(borrowed, borrowdetails => borrowdetails.BorrowedPKey , borrowedissue => borrowedissue.Id, (borrowdetails, borrowissue) => new {borrowdetails ,borrowissue })
                .SelectMany(x => x.borrowissue.DefaultIfEmpty(), (x, borrowissue) => new { x.borrowdetails, borrowissue })
                .Where(x => x.borrowdetails.IsActive == true && x.borrowdetails.IsReturned  == true)
                .GroupBy(x => new
                {
                    x.borrowissue.Id,
                    x.borrowissue.CustomerCode,
                    x.borrowissue.CustomerName,
                    x.borrowdetails.PreparedBy,
                    x.borrowdetails.ReturnedDate,


                }).Select(x => new DtoGetAllReturnedItem
                {

                    Id = x.Key.Id,
                    CustomerCode = x.Key.CustomerCode,
                    CustomerName = x.Key.CustomerName,
                    Consumed = x.Sum(x => x.borrowdetails.Quantity) - x.Sum(x => x.borrowdetails.ReturnQuantity),
                    TotalReturned = x.Sum(x => x.borrowdetails.ReturnQuantity),
                    PreparedBy = x.Key.PreparedBy,
                    ReturnedDate = x.Key.ReturnedDate.ToString(),



                });


            //var BorrowIssue = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
            //                                         .Where(x => x.IsReturned == true)
            //                                         .GroupBy(x => new
            //                                         {

            //                                             x.BorrowedPKey,
            //                                             x.CustomerCode,
            //                                             x.CustomerName,
            //                                             x.PreparedBy,
            //                                             x.ReturnedDate,


            //                                         }).Select(total => new DtoGetAllReturnedItem
            //                                         {
            //                                             Id = total.Key.BorrowedPKey,
            //                                             CustomerCode = total.Key.CustomerCode,
            //                                             CustomerName = total.Key.CustomerName,
            //                                             Consumed = total.Sum(x => x.Quantity) - total.Sum(x => x.ReturnQuantity),
            //                                             TotalReturned = total.Sum(x => x.ReturnQuantity),
            //                                             PreparedBy = total.Key.PreparedBy,
            //                                             ReturnedDate = total.Key.ReturnedDate.ToString(),

            //                                         });



            return await PagedList<DtoGetAllReturnedItem>.CreateAsync(BorrowIssue, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<PagedList<DtoGetAllReturnedItem>> GetAllReturnedItemOrig(UserParams userParams, string search)
        {

            var borrowed = _context.BorrowedIssues.Where(x => x.IsReturned == true && x.IsActive == true)
                                                  .GroupBy(x => new
                                                  {

                                                      x.Id,
                                                      x.CustomerCode,
                                                      x.CustomerName

                                                  }).Select(x => new DtoGetAllReturnedItem
                                                  {

                                                      Id = x.Key.Id,
                                                      CustomerCode = x.Key.CustomerCode,
                                                      CustomerName = x.Key.CustomerName,

                                                  });


            var BorrowIssue = _context.BorrowedIssueDetails
                .GroupJoin(borrowed, borrowdetails => borrowdetails.BorrowedPKey, borrowedissue => borrowedissue.Id, (borrowdetails, borrowissue) => new { borrowdetails, borrowissue })
                .SelectMany(x => x.borrowissue.DefaultIfEmpty(), (x, borrowissue) => new { x.borrowdetails, borrowissue })
                .Where(x => x.borrowdetails.IsActive == true && x.borrowdetails.IsReturned == true)
                .GroupBy(x => new
                {
                    x.borrowissue.Id,
                    x.borrowissue.CustomerCode,
                    x.borrowissue.CustomerName,
                    x.borrowdetails.PreparedBy,
                    x.borrowdetails.ReturnedDate,


                }).Select(x => new DtoGetAllReturnedItem
                {

                    Id = x.Key.Id,
                    CustomerCode = x.Key.CustomerCode,
                    CustomerName = x.Key.CustomerName,
                    Consumed = x.Sum(x => x.borrowdetails.Quantity) - x.Sum(x => x.borrowdetails.ReturnQuantity),
                    TotalReturned = x.Sum(x => x.borrowdetails.ReturnQuantity),
                    PreparedBy = x.Key.PreparedBy,
                    ReturnedDate = x.Key.ReturnedDate.ToString(),


                }).Where(x => (Convert.ToString(x.Id)).ToLower().Contains(search.Trim().ToLower())
                          || (Convert.ToString(x.CustomerCode)).ToLower().Contains(search.Trim().ToLower())
                          || (Convert.ToString(x.CustomerName)).ToLower().Contains(search.Trim().ToLower()));


            return await PagedList<DtoGetAllReturnedItem>.CreateAsync(BorrowIssue, userParams.PageNumber, userParams.PageSize);

        }


        public async Task<IReadOnlyList<DtoViewBorrewedReturnedDetails>> ViewBorrewedReturnedDetails(int id)
        {




            var issueBorrowed = _context.BorrowedIssues.Where(x => x.IsActive == true)
                                              .Select(x => new GetAllDetailsInBorrowedIssueDto
                                              {
                                                  Id = x.Id,
                                                  Customer = x.CustomerName,
                                                  CustomerCode = x.CustomerCode,

                                              });

            var borrow = _context.BorrowedIssueDetails
                .GroupJoin(issueBorrowed, borrowed => borrowed.BorrowedPKey, issues => issues.Id, (borrowed, issues) => new { borrowed, issues })
                .SelectMany(x => x.issues.DefaultIfEmpty(), (x, issues) => new { x.borrowed, issues })
              .Where(x => x.borrowed.BorrowedPKey == id)
              .Where(x => x.borrowed.IsReturned == true)
              .Where(x => x.borrowed.IsActive == true)
              .Select(x => new DtoViewBorrewedReturnedDetails
              {

                  Id = x.borrowed.BorrowedPKey,
                  Customer = x.issues.Customer,
                  CustomerCode = x.issues.CustomerCode,

                  ItemCode = x.borrowed.ItemCode,
                  ItemDescription = x.borrowed.ItemDescription,
                  ReturnQuantity = x.borrowed.ReturnQuantity,
                  Consume = x.borrowed.Quantity - x.borrowed.ReturnQuantity,
                  ReturnedDate = x.borrowed.ReturnedDate.ToString(),
                  PreparedBy = x.borrowed.PreparedBy

              });



            //var borrow = _context.BorrowedIssueDetails.Where(x => x.BorrowedPKey == id)
            //                                         .Where(x => x.IsActive == true)
            //                                         .Where(x => x.IsReturned == true)
            //                                           .Select(x => new DtoViewBorrewedReturnedDetails
            //                                           {

            //                                               Id = x.BorrowedPKey,
            //                                               Customer = x.CustomerName,
            //                                               CustomerCode = x.CustomerCode,

            //                                               ItemCode = x.ItemCode,
            //                                               ItemDescription = x.ItemDescription,
            //                                               ReturnQuantity = x.ReturnQuantity,
            //                                               Consume = x.Quantity - x.ReturnQuantity,
            //                                               ReturnedDate = x.ReturnedDate.ToString(),
            //                                               PreparedBy = x.PreparedBy


            //                                           }).OrderByDescending(x => x.ReturnedDate);


            return await borrow.ToListAsync();
        }


        public async Task<bool> Cancelborrowedfortransact(BorrowedIssueDetails borrowed)
        {
            var borrow = await _context.BorrowedIssueDetails.Where(x => x.Id == borrowed.Id)
                                                             .FirstOrDefaultAsync();


            borrow.IsActive = false;

            return true;

        }


        // Updated Borrowed 

        public async Task<PagedList<GetAllBorrowedReceiptWithPaginationDto>> GetAllForApprovalBorrowedWithPagination(UserParams userParams, bool status)
        {
            var borrow = _context.BorrowedIssues.OrderByDescending(x => x.PreparedDate)
                                               .Where(x => x.IsReturned == null)
                                               .Where(x => x.IsReject == null)
                                               .Where(x => x.IsActive == status)
                                               .Select(x => new GetAllBorrowedReceiptWithPaginationDto
                                               {

                                                   BorrowedPKey = x.Id,
                                                   CustomerName = x.CustomerName,
                                                   CustomerCode = x.CustomerCode,
                                                   TotalQuantity = x.TotalQuantity,
                                                   PreparedBy = x.PreparedBy,
                                                   IsActive = x.IsActive,
                                                   IsApproved = x.IsApproved,
                                                   Remarks = x.Remarks,
                                                   BorrowedDate = x.PreparedDate.ToString(),
                                                   TransactionDate = x.TransactionDate.ToString("MM/dd/yyyy")

                                               });

            return await PagedList<GetAllBorrowedReceiptWithPaginationDto>.CreateAsync(borrow, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<GetAllBorrowedReceiptWithPaginationDto>> GetAllForApprovalBorrowedWithPaginationOrig(UserParams userParams, string search, bool status)
        {
            throw new NotImplementedException();
        }
    }
}
