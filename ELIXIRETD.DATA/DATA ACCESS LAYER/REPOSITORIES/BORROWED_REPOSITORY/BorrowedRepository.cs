using ELIXIRETD.DATA.CORE.INTERFACES.BORROWED_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO.BorrowedNotification;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.TransactDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.BORROWED_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using EntityFramework.Functions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using System;
using System.Diagnostics.Contracts;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using NetTopologySuite.IO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.BORROWED_REPOSITORY.Excemption;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.USER_REPOSITORY.Excemption;
//using EntityFramework.FunctionsExtensions.DateDiffDay;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.BORROWED_REPOSITORY
{
    public class BorrowedRepository : IBorrowedItem
    {
        private readonly StoreContext _context;

        public BorrowedRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<PagedList<GetAllBorrowedReceiptWithPaginationDto>> GetAllBorrowedReceiptWithPagination(UserParams userParams, bool status, int empid)
        {
            //var daytoday = DateTime.Now;


            var employee = _context.Users.Where(x => x.Id == empid)
                                         .FirstOrDefault();


            var details = _context.BorrowedIssueDetails
                                                     .GroupBy(x => new
                                                     {
                                                         x.BorrowedPKey,
                                                         x.IsActive,

                                                     }).Select(x => new GetAllBorrowedReceiptWithPaginationDto
                                                     {
                                                         BorrowedPKey = x.Key.BorrowedPKey,
                                                         IsActive = x.Key.IsActive,
                                                         TotalQuantity = x.Sum(x => x.Quantity),

                                                     });

            var borrow = _context.BorrowedIssues
                .GroupJoin(details, issues => issues.Id, borrowed => borrowed.BorrowedPKey, (issues, borrowed) => new { issues, borrowed })
                .SelectMany(x => x.borrowed.DefaultIfEmpty(), (x, borrowed) => new { x.issues, borrowed })
                .Where(x => x.issues.PreparedBy == employee.FullName)
                .Where(x => x.borrowed.IsActive == true)
                .Where(x => x.issues.IsApproved == status && x.issues.IsReturned == null && x.issues.IsReject == null)
                .GroupBy(x => new
                {
                    x.borrowed.BorrowedPKey,
                    x.issues.CustomerCode,
                    x.issues.CustomerName,
                    x.borrowed.TotalQuantity,
                    x.issues.PreparedBy,
                    x.borrowed.IsActive,
                    x.issues.IsApproved,
                    x.issues.Remarks,
                    x.issues.Reason,
                    x.issues.PreparedDate,
                    x.issues.TransactionDate,
                    x.issues.IsApprovedDate,
                    x.issues.StatusApproved,

                }).Select(x => new GetAllBorrowedReceiptWithPaginationDto
                {
                    BorrowedPKey = x.Key.BorrowedPKey,
                    CustomerName = x.Key.CustomerName,
                    CustomerCode = x.Key.CustomerCode,
                    TotalQuantity = x.Key.TotalQuantity,
                    PreparedBy = x.Key.PreparedBy,
                    IsActive = x.Key.IsActive,
                    IsApproved = x.Key.IsApproved,
                    Remarks = x.Key.Remarks,
                    Reason = x.Key.Reason,
                    AgingDays = x.Key.IsApprovedDate != null ? EF.Functions.DateDiffDay(x.Key.IsApprovedDate.Value, DateTime.Now) : 0,
                    BorrowedDate = x.Key.PreparedDate.ToString(),
                    TransactionDate = x.Key.TransactionDate.ToString(),
                    ApproveDate = x.Key.IsApprovedDate.ToString(),
                    StatusApprove = x.Key.StatusApproved

                });



            //var borrow = _context.BorrowedIssues.OrderByDescending(x => x.IsApproved)
            //                                      .ThenByDescending(x => x.PreparedDate)
            //                                      .Where(x => x.PreparedBy == employee.FullName)
            //                                      .Where(x => x.IsReturned == null)
            //                                      //.Where(x => x.StatusApproved == status)
            //                                      .Where(x => x.IsApproved == status)
            //                                      //.Where(x => x.IsApproved == false)
            //                                      .Where(x => x.IsReject == null)
            //                                      .Where(x => x.IsActive == true)
            //                                      .Select(x => new GetAllBorrowedReceiptWithPaginationDto
            //                                      {

            //                                          BorrowedPKey = x.Id,
            //                                          CustomerName = x.CustomerName,
            //                                          CustomerCode = x.CustomerCode,
            //                                          TotalQuantity = x.TotalQuantity,
            //                                          PreparedBy = x.PreparedBy,
            //                                          IsActive = x.IsActive,
            //                                          IsApproved = x.IsApproved,
            //                                          Remarks = x.Remarks,
            //                                          Reason = x.Reason,
            //                                          AgingDays = x.IsApprovedDate != null ? EF.Functions.DateDiffDay(x.IsApprovedDate.Value , DateTime.Now) : 0,
            //                                          BorrowedDate = x.PreparedDate.ToString(),
            //                                          TransactionDate = x.TransactionDate.ToString("MM/dd/yyyy"),
            //                                          ApproveDate = x.IsApprovedDate.ToString(),
            //                                          StatusApprove = x.StatusApproved

            //                                      });

            return await PagedList<GetAllBorrowedReceiptWithPaginationDto>.CreateAsync(borrow, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<GetAllBorrowedReceiptWithPaginationDto>> GetAllBorrowedIssuetWithPaginationOrig(UserParams userParams, string search, bool status, int empid)
        {

            var employee = _context.Users.Where(x => x.Id == empid)
                                       .FirstOrDefault();


            var details = _context.BorrowedIssueDetails
                                                     .GroupBy(x => new
                                                     {
                                                         x.BorrowedPKey,
                                                         x.IsActive,

                                                     }).Select(x => new GetAllBorrowedReceiptWithPaginationDto
                                                     {
                                                         BorrowedPKey = x.Key.BorrowedPKey,
                                                         IsActive = x.Key.IsActive,
                                                         TotalQuantity = x.Sum(x => x.Quantity),

                                                     });

            var borrow = _context.BorrowedIssues
                .GroupJoin(details, issues => issues.Id, borrowed => borrowed.BorrowedPKey, (issues, borrowed) => new { issues, borrowed })
                .SelectMany(x => x.borrowed.DefaultIfEmpty(), (x, borrowed) => new { x.issues, borrowed })
                .Where(x => x.issues.PreparedBy == employee.FullName)
                .Where(x => x.borrowed.IsActive == true)
                .Where(x => x.issues.IsApproved == status && x.issues.IsReturned == null && x.issues.IsReject == null)
                .GroupBy(x => new
                {
                    x.borrowed.BorrowedPKey,
                    x.issues.CustomerCode,
                    x.issues.CustomerName,
                    x.borrowed.TotalQuantity,
                    x.issues.PreparedBy,
                    x.borrowed.IsActive,
                    x.issues.IsApproved,
                    x.issues.Remarks,
                    x.issues.Reason,
                    x.issues.PreparedDate,
                    x.issues.TransactionDate,
                    x.issues.IsApprovedDate,
                    x.issues.StatusApproved,

                }).Select(x => new GetAllBorrowedReceiptWithPaginationDto
                {
                    BorrowedPKey = x.Key.BorrowedPKey,
                    CustomerName = x.Key.CustomerName,
                    CustomerCode = x.Key.CustomerCode,
                    TotalQuantity = x.Key.TotalQuantity,
                    PreparedBy = x.Key.PreparedBy,
                    IsActive = x.Key.IsActive,
                    IsApproved = x.Key.IsApproved,
                    Remarks = x.Key.Remarks,
                    Reason = x.Key.Reason,
                    AgingDays = x.Key.IsApprovedDate != null ? EF.Functions.DateDiffDay(x.Key.IsApprovedDate.Value, DateTime.Now) : 0,
                    BorrowedDate = x.Key.PreparedDate.ToString(),
                    TransactionDate = x.Key.TransactionDate.ToString(),
                    ApproveDate = x.Key.IsApprovedDate.ToString(),
                    StatusApprove = x.Key.StatusApproved

                }).Where(x => (Convert.ToString(x.BorrowedPKey)).ToLower().Contains(search.Trim().ToLower()));


            //var employee = _context.Users.Where(x => x.Id == empid)
            //                          .FirstOrDefault();


            //var borrow = _context.BorrowedIssues.OrderByDescending(x => x.IsApproved)
            //                                      .ThenByDescending(x => x.PreparedDate)
            //                                      .Where(x => x.PreparedBy == employee.FullName)
            //                                       .Where(x => x.IsApproved == status)
            //                                      .Where(x => x.IsReturned == null)
            //                                      //.Where(x => x.IsApproved == false)
            //                                      .Where(x => x.IsReject == null)
            //                                      .Where(x => x.IsActive == true)
            //                                      .Select(x => new GetAllBorrowedReceiptWithPaginationDto
            //                                      {

            //                                          BorrowedPKey = x.Id,
            //                                          CustomerName = x.CustomerName,
            //                                          CustomerCode = x.CustomerCode,
            //                                          TotalQuantity = x.TotalQuantity,
            //                                          PreparedBy = x.PreparedBy,
            //                                          IsApproved = x.IsApproved,
            //                                          IsActive = x.IsActive,
            //                                          Remarks = x.Remarks,
            //                                          Reason = x.Reason,
            //                                          BorrowedDate = x.PreparedDate.ToString(),
            //                                          TransactionDate = x.TransactionDate.ToString("MM/dd/yyyy"),
            //                                          AgingDays = x.IsApprovedDate != null ? EF.Functions.DateDiffDay(x.IsApprovedDate.Value , DateTime.Now) : 0,
            //                                          ApproveDate = x.IsApprovedDate.ToString(),
            //                                          StatusApprove = x.StatusApproved

            //                                      })
                                               

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


        public async Task<IReadOnlyList<GetAvailableStocksForBorrowedIssue_Dto>> GetAvailableStocksForBorrowedIssueNoParameters()
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
                                                           .Where(x => x.IsApproved == false)
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


            var BorrowedReturn = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                             .Where(x => x.IsReturned == true)
                                                             .Where(x => x.IsApprovedReturned == true)
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
                                    WarehouseActualGood = warehouse.ActualGood != null ? warehouse.ActualGood : 0,
                                    MoveOrderOut = Moveorder.QuantityOrdered != null ? Moveorder.QuantityOrdered : 0,
                                    IssueOut = issue.Out != null ? issue.Out : 0,
                                    BorrowedOut = borrowOut.Out != null ? borrowOut.Out : 0,
                                    borrowedreturn = returned.In != null ? returned.In : 0,


                                } into total

                                select new GetAvailableStocksForBorrowedIssue_Dto
                                {

                                    WarehouseId = total.Key.WarehouseId,
                                    ItemCode = total.Key.ItemCode,
                                    RemainingStocks = total.Key.WarehouseActualGood + total.Key.borrowedreturn - total.Key.MoveOrderOut - total.Key.IssueOut - total.Key.BorrowedOut,
                                    ReceivingDate = total.Key.RecievingDate,

                                }).Where(x => x.RemainingStocks != 0)
                                   ;

            return await getAvailable.ToListAsync();
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
                                                           .Where(x => x.IsApproved == false)
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


            var BorrowedReturn = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                             .Where(x => x.IsReturned == true)
                                                             .Where(x => x.IsApprovedReturned == true)
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
                                    WarehouseActualGood = warehouse.ActualGood != null ? warehouse.ActualGood : 0,
                                    MoveOrderOut = Moveorder.QuantityOrdered != null ? Moveorder.QuantityOrdered : 0,
                                    IssueOut = issue.Out != null ? issue.Out : 0,
                                    BorrowedOut = borrowOut.Out != null ? borrowOut.Out : 0,
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
              .Where(x => x.borrowed.IsReject == null)
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

                  Uom = x.borrowed.Uom,

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

            foreach (var item in returnedDetails)
            {

                item.IsReturned = true;
                item.IsActive = true;
                item.ReturnedDate = DateTime.Now;
                item.IsApprovedReturned = false;
                


            }

            returned.IsReturned = true;
            returned.IsActive = true;
            returned.IsApprovedReturned = false;
            returned.StatusApproved = "For return approval";
            returned.DepartmentCode = borrowed.DepartmentCode;
            returned.DepartmentName = borrowed.DepartmentName;
            returned.CompanyCode = borrowed.CompanyCode;
            returned.CompanyName = borrowed.CompanyName;
            returned.LocationCode = borrowed.LocationCode;
            returned.LocationName = borrowed.LocationName;
            returned.AccountCode = borrowed.AccountCode;
            returned.AccountTitles = borrowed.AccountTitles;


            return true;
        }


        public async Task<PagedList<DtoGetAllReturnedItem>> GetAllReturnedItem(UserParams userParams, bool status, int empid)
        {
            var employee = _context.Users.Where(x => x.Id == empid)
                                     .FirstOrDefault();

            var borrowed = _context.BorrowedIssues
                                                  .Where(x => x.IsReturned == true && x.IsActive == true)
                                                  .GroupBy(x => new
                                                  {

                                                      x.Id,
                                                      x.CustomerCode,
                                                      x.CustomerName,
                                                      x.IsApprovedReturned,
                                                      x.IsApprovedReturnedDate,
                                                      x.PreparedBy,
                                                      x.StatusApproved,

                                                  }).Select(x => new DtoGetAllReturnedItem
                                                  {

                                                      Id = x.Key.Id,
                                                      CustomerCode = x.Key.CustomerCode,
                                                      CustomerName = x.Key.CustomerName,
                                                      IsApproveReturn = x.Key.IsApprovedReturned != null ? true : false,
                                                      ApproveReturnDate = x.Key.IsApprovedReturnedDate.ToString(),
                                                      PreparedBy = x.Key.PreparedBy,
                                                      StatusApprove = x.Key.StatusApproved

                                                  });


            var BorrowIssue = _context.BorrowedIssueDetails
                .GroupJoin(borrowed, borrowdetails => borrowdetails.BorrowedPKey, borrowedissue => borrowedissue.Id, (borrowdetails, borrowissue) => new { borrowdetails, borrowissue })
                .SelectMany(x => x.borrowissue.DefaultIfEmpty(), (x, borrowissue) => new { x.borrowdetails, borrowissue })
                .Where(x => x.borrowissue.PreparedBy == employee.FullName)
                .Where(x => x.borrowdetails.IsActive == true && x.borrowdetails.IsReturned == true && x.borrowdetails.IsApprovedReturned == status)
                .GroupBy(x => new
                {
                    x.borrowissue.Id,
                    x.borrowissue.CustomerCode,
                    x.borrowissue.CustomerName,
                    x.borrowdetails.PreparedBy,
                    x.borrowdetails.ReturnedDate,
                    x.borrowdetails.IsApprovedReturned,
                    x.borrowissue.ApproveReturnDate,
                    x.borrowdetails.IsApprovedReturnedDate,
                    x.borrowissue.StatusApprove,


                }).Select(x => new DtoGetAllReturnedItem
                {

                    Id = x.Key.Id,
                    CustomerCode = x.Key.CustomerCode,
                    CustomerName = x.Key.CustomerName,
                    TotalBorrowedQuantity = x.Sum(x => x.borrowdetails.Quantity),
                    Consumed = x.Sum(x => x.borrowdetails.Quantity) - x.Sum(x => x.borrowdetails.ReturnQuantity),
                    TotalReturned = x.Sum(x => x.borrowdetails.ReturnQuantity),
                    PreparedBy = x.Key.PreparedBy,
                    ReturnedDate = x.Key.ReturnedDate.ToString(),
                    IsApproveReturn = x.Key.IsApprovedReturned != null ? true : false,
                    ApproveReturnDate = x.Key.IsApprovedReturnedDate.ToString(),
                    StatusApprove = x.Key.StatusApprove,
                    

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


        public async Task<PagedList<DtoGetAllReturnedItem>> GetAllReturnedItemOrig(UserParams userParams, string search, bool status, int empid)
        {

            var employee = _context.Users.Where(x => x.Id == empid)
                                   .FirstOrDefault();

            var borrowed = _context.BorrowedIssues
                                              .Where(x => x.IsReturned == true && x.IsActive == true)
                                              .GroupBy(x => new
                                              {

                                                  x.Id,
                                                  x.CustomerCode,
                                                  x.CustomerName,
                                                  x.IsApprovedReturned,
                                                  x.IsApprovedReturnedDate,
                                                  x.PreparedBy,
                                                  x.StatusApproved


                                                  //x.IsApprovedReturned,

                                              }).Select(x => new DtoGetAllReturnedItem
                                              {

                                                  Id = x.Key.Id,
                                                  CustomerCode = x.Key.CustomerCode,
                                                  CustomerName = x.Key.CustomerName,
                                                  //IsApproveReturn = x.Key.IsApprovedReturned != null ? true : false,
                                                  IsApproveReturn = x.Key.IsApprovedReturned != null ? true : false,
                                                  ApproveReturnDate = x.Key.IsApprovedReturnedDate.ToString(),
                                                  PreparedBy = x.Key.PreparedBy,
                                                  StatusApprove = x.Key.StatusApproved


                                              });


            var BorrowIssue = _context.BorrowedIssueDetails
                .GroupJoin(borrowed, borrowdetails => borrowdetails.BorrowedPKey, borrowedissue => borrowedissue.Id, (borrowdetails, borrowissue) => new { borrowdetails, borrowissue })
                .SelectMany(x => x.borrowissue.DefaultIfEmpty(), (x, borrowissue) => new { x.borrowdetails, borrowissue })
                .Where(x => x.borrowissue.PreparedBy == employee.FullName)
                .Where(x => x.borrowdetails.IsActive == true && x.borrowdetails.IsReturned == true && x.borrowdetails.IsApprovedReturned == status)
                .GroupBy(x => new
                {
                    x.borrowissue.Id,
                    x.borrowissue.CustomerCode,
                    x.borrowissue.CustomerName,
                    x.borrowdetails.PreparedBy,
                    x.borrowdetails.ReturnedDate,
                    x.borrowdetails.IsApprovedReturned,
                    x.borrowdetails.IsApprovedReturnedDate,
                    x.borrowissue.StatusApprove


                }).Select(x => new DtoGetAllReturnedItem
                {

                    Id = x.Key.Id,
                    CustomerCode = x.Key.CustomerCode,
                    CustomerName = x.Key.CustomerName,
                    TotalBorrowedQuantity = x.Sum(x => x.borrowdetails.Quantity),
                    Consumed = x.Sum(x => x.borrowdetails.Quantity) - x.Sum(x => x.borrowdetails.ReturnQuantity),
                    TotalReturned = x.Sum(x => x.borrowdetails.ReturnQuantity),
                    PreparedBy = x.Key.PreparedBy,
                    ReturnedDate = x.Key.ReturnedDate.ToString(),
                    IsApproveReturn = x.Key.IsApprovedReturned != null ? true : false,
                    ApproveReturnDate = x.Key.IsApprovedReturnedDate.ToString(),
                    StatusApprove = x.Key.StatusApprove


                }).Where(x => (Convert.ToString(x.Id)).ToLower().Contains(search.Trim().ToLower()));


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

                                                  Remarks = x.Remarks,

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


            var borrow = _context.BorrowedIssueDetails
                .GroupJoin(issueBorrowed, borrowed => borrowed.BorrowedPKey, issues => issues.Id, (borrowed, issues) => new { borrowed, issues })
                .SelectMany(x => x.issues.DefaultIfEmpty(), (x, issues) => new { x.borrowed, issues })
              .Where(x => x.borrowed.BorrowedPKey == id)
              .Where(x => x.borrowed.IsReturned == true)
              .Where(x => x.borrowed.IsActive == true)
              .Select(x => new DtoViewBorrewedReturnedDetails
              {
                  BorrowedPKey = x.borrowed.BorrowedPKey,
                  Id = x.borrowed.Id,
                  Customer = x.issues.Customer,
                  CustomerCode = x.issues.CustomerCode,

                  ItemCode = x.borrowed.ItemCode,
                  ItemDescription = x.borrowed.ItemDescription,

                  BorrowedQuantity = x.borrowed.Quantity,
                  ReturnQuantity = x.borrowed.ReturnQuantity,
                  Consume = x.borrowed.Quantity - x.borrowed.ReturnQuantity,

                  ReturnedDate = x.borrowed.ReturnedDate.ToString(),
                  PreparedBy = x.borrowed.PreparedBy,

                  Remarks = x.issues.Remarks,

                  TransactionDate = x.issues.TransactionDate.ToString(),
                  CompanyCode = x.issues.CompanyCode,
                  CompanyName = x.issues.CompanyName,
                  DepartmentCode = x.issues.DepartmentCode,
                  DepartmentName = x.issues.DepartmentName,
                  LocationCode = x.issues.LocationCode,
                  LocationName = x.issues.LocationName,
                  AccountCode = x.issues.AccountCode,
                  AccountTitles = x.issues.AccountTitles,

                  Uom = x.borrowed.Uom

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
            borrow.Remarks = borrowed.Remarks;

            return true;

        }


        // ========================================================== Updated Borrowed ===========================================================================

        public async Task<PagedList<GetAllForApprovalBorrowedPaginationDTO>> GetAllForApprovalBorrowedWithPagination(UserParams userParams, bool status)
        {

            var details = _context.BorrowedIssueDetails
                                                   .GroupBy(x => new
                                                   {
                                                       x.BorrowedPKey,
                                                       x.IsActive,

                                                   }).Select(x => new GetAllForApprovalBorrowedPaginationDTO
                                                   {
                                                       BorrowedPKey = x.Key.BorrowedPKey,
                                                       IsActive = x.Key.IsActive,
                                                       TotalQuantity = x.Sum(x => x.Quantity),

                                                   });

            var borrow = _context.BorrowedIssues
                .GroupJoin(details, issues => issues.Id, borrowed => borrowed.BorrowedPKey, (issues, borrowed) => new { issues, borrowed })
                .SelectMany(x => x.borrowed.DefaultIfEmpty(), (x, borrowed) => new { x.issues, borrowed })
                .Where(x => x.borrowed.IsActive == true)
                .Where(x => x.issues.IsApproved == status && x.issues.IsReturned == null && x.issues.IsReject == null)
                .GroupBy(x => new
                {
                    x.borrowed.BorrowedPKey,
                    x.issues.CustomerCode,
                    x.issues.CustomerName,
                    x.borrowed.TotalQuantity,
                    x.issues.PreparedBy,
                    x.borrowed.IsActive,
                    x.issues.IsApproved,
                    x.issues.Remarks,
                    x.issues.Reason,
                    x.issues.PreparedDate,
                    x.issues.TransactionDate,
                    x.issues.IsApprovedDate,
                    x.issues.StatusApproved,

                }).Select(x => new GetAllForApprovalBorrowedPaginationDTO
                {
                    BorrowedPKey = x.Key.BorrowedPKey,
                    CustomerName = x.Key.CustomerName,
                    CustomerCode = x.Key.CustomerCode,
                    TotalQuantity = x.Key.TotalQuantity,
                    PreparedBy = x.Key.PreparedBy,
                    IsActive = x.Key.IsActive,
                 
                    Remarks = x.Key.Remarks,
                    Reason = x.Key.Reason,
                    AgingDays = x.Key.IsApprovedDate != null ? EF.Functions.DateDiffDay(x.Key.IsApprovedDate.Value, DateTime.Now) : 0,
                    BorrowedDate = x.Key.PreparedDate.ToString(),
                    TransactionDate = x.Key.TransactionDate.ToString(),
                    StatusApprove = x.Key.StatusApproved

                });

            //var borrow = _context.BorrowedIssues.OrderByDescending(x => x.PreparedDate)
            //                                   .Where(x => x.IsReturned == null)
            //                                   .Where(x => x.IsReject == null)
            //                                   .Where(x => x.IsApproved == status)
            //                                   .Where(x => x.IsActive == true)
            //                                   .Select(x => new GetAllForApprovalBorrowedPaginationDTO
            //                                   {

            //                                       BorrowedPKey = x.Id,
            //                                       CustomerName = x.CustomerName,
            //                                       CustomerCode = x.CustomerCode,
            //                                       TotalQuantity = x.TotalQuantity,
            //                                       PreparedBy = x.PreparedBy,
            //                                       Remarks = x.Remarks,
            //                                       Reason = x.Reason,
            //                                       BorrowedDate = x.PreparedDate.ToString(),
            //                                       AgingDays = x.IsApprovedDate != null ? EF.Functions.DateDiffDay( x.IsApprovedDate.Value, DateTime.Now) : 0,
            //                                       TransactionDate = x.TransactionDate.ToString("MM/dd/yyyy"),
            //                                       StatusApprove = x.StatusApproved

            //                                   });

            return await PagedList<GetAllForApprovalBorrowedPaginationDTO>.CreateAsync(borrow, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<PagedList<GetAllForApprovalBorrowedPaginationDTO>> GetAllForApprovalBorrowedWithPaginationOrig(UserParams userParams, string search, bool status)
        {
            //var borrow = _context.BorrowedIssues.OrderByDescending(x => x.PreparedDate)
            //                                .Where(x => x.IsReturned == null)
            //                                .Where(x => x.IsReject == null)
            //                                .Where(x => x.IsApproved == status)
            //                                .Where(x => x.IsActive == true)
            //                                .Select(x => new GetAllForApprovalBorrowedPaginationDTO
            //                                {

            //                                    BorrowedPKey = x.Id,
            //                                    CustomerName = x.CustomerName,
            //                                    CustomerCode = x.CustomerCode,
            //                                    TotalQuantity = x.TotalQuantity,
            //                                    PreparedBy = x.PreparedBy,
            //                                    Remarks = x.Remarks,
            //                                    Reason = x.Reason,
            //                                    BorrowedDate = x.PreparedDate.ToString(),
            //                                    AgingDays = x.IsApprovedDate != null ? EF.Functions.DateDiffDay(x.IsApprovedDate.Value ,DateTime.Now) : 0,
            //                                    TransactionDate = x.TransactionDate.ToString("MM/dd/yyyy"),
            //                                    StatusApprove = x.StatusApproved

            //                                }).


            var details = _context.BorrowedIssueDetails
                                                 .GroupBy(x => new
                                                 {
                                                     x.BorrowedPKey,
                                                     x.IsActive,

                                                 }).Select(x => new GetAllForApprovalBorrowedPaginationDTO
                                                 {
                                                     BorrowedPKey = x.Key.BorrowedPKey,
                                                     IsActive = x.Key.IsActive,
                                                     TotalQuantity = x.Sum(x => x.Quantity),

                                                 });

            var borrow = _context.BorrowedIssues
                .GroupJoin(details, issues => issues.Id, borrowed => borrowed.BorrowedPKey, (issues, borrowed) => new { issues, borrowed })
                .SelectMany(x => x.borrowed.DefaultIfEmpty(), (x, borrowed) => new { x.issues, borrowed })
                .Where(x => x.borrowed.IsActive == true)
                .Where(x => x.issues.IsApproved == status && x.issues.IsReturned == null && x.issues.IsReject == null)
                .GroupBy(x => new
                {
                    x.borrowed.BorrowedPKey,
                    x.issues.CustomerCode,
                    x.issues.CustomerName,
                    x.borrowed.TotalQuantity,
                    x.issues.PreparedBy,
                    x.borrowed.IsActive,
                    x.issues.IsApproved,
                    x.issues.Remarks,
                    x.issues.Reason,
                    x.issues.PreparedDate,
                    x.issues.TransactionDate,
                    x.issues.IsApprovedDate,
                    x.issues.StatusApproved,

                }).Select(x => new GetAllForApprovalBorrowedPaginationDTO
                {
                    BorrowedPKey = x.Key.BorrowedPKey,
                    CustomerName = x.Key.CustomerName,
                    CustomerCode = x.Key.CustomerCode,
                    TotalQuantity = x.Key.TotalQuantity,
                    PreparedBy = x.Key.PreparedBy,
                    IsActive = x.Key.IsActive,

                    Remarks = x.Key.Remarks,
                    Reason = x.Key.Reason,
                    AgingDays = x.Key.IsApprovedDate != null ? EF.Functions.DateDiffDay(x.Key.IsApprovedDate.Value, DateTime.Now) : 0,
                    BorrowedDate = x.Key.PreparedDate.ToString(),
                    TransactionDate = x.Key.TransactionDate.ToString(),
                    StatusApprove = x.Key.StatusApproved

                }).Where(x => (Convert.ToString(x.BorrowedPKey)).ToLower().Contains(search.Trim().ToLower())
                                                    || (Convert.ToString(x.CustomerCode)).ToLower().Contains(search.Trim().ToLower())
                                                      || (Convert.ToString(x.CustomerName)).ToLower().Contains(search.Trim().ToLower()));


            return await PagedList<GetAllForApprovalBorrowedPaginationDTO>.CreateAsync(borrow, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<IReadOnlyList<GetAllForApprovalDetailsInBorrowed>> GetAllForApprovalDetailsInBorrowed(int id)
        {
            var issueBorrowed = _context.BorrowedIssues.Where(x => x.IsActive == true)
                                            .Select(x => new GetAllForApprovalDetailsInBorrowed
                                            {
                                                Id = x.Id,
                                                Customer = x.CustomerName,
                                                CustomerCode = x.CustomerCode,

                                                Remarks = x.Remarks,


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
              .Where(x => x.borrowed.IsReject == null)
              .Where(x => x.borrowed.IsTransact == true)
              .Where(x => x.borrowed.IsApproved == false)
              .Where(x => x.borrowed.IsReturned == null)
              .Where(x => x.borrowed.IsActive == true)
              .Select(x => new GetAllForApprovalDetailsInBorrowed
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
                  PreparedBy = x.borrowed.PreparedBy,

                  Remarks = x.borrowed.Remarks,

                  Uom = x.borrowed.Uom,

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

            return await warehouse.ToListAsync();
        }

        public async Task<bool> ApprovedForBorrowed(BorrowedIssue borrowed)
        {
            var issue = await _context.BorrowedIssues.Where(x => x.Id == borrowed.Id)
                                                     .FirstOrDefaultAsync();


            var borrow = await _context.BorrowedIssueDetails.Where(x => x.BorrowedPKey == borrowed.Id)
                                                            .ToListAsync();


            issue.IsActive = true;
            issue.IsApproved = true;
            issue.IsApprovedDate = DateTime.Now;
            issue.ApproveBy = borrowed.ApproveBy;
            issue.StatusApproved = "Borrowed Approve";


            foreach (var item in borrow)
            {

                item.IsActive = true;
                item.IsApproved = true;
                item.IsApprovedDate = DateTime.Now;

            }


            return true;

        }







        public async Task<bool> RejectForBorrowed(BorrowedIssue borrowed)
        {

            var issue = await _context.BorrowedIssues.Where(x => x.Id == borrowed.Id)
                                                    .FirstOrDefaultAsync();


            var borrow = await _context.BorrowedIssueDetails.Where(x => x.BorrowedPKey == borrowed.Id)
                                                            .ToListAsync();


            issue.IsActive = false;
            issue.IsApproved = false;
            issue.IsReject = true;
            issue.IsApprovedDate = null;
            issue.RejectBy = borrowed.RejectBy;
            issue.Reason = borrowed.Reason;
            issue.IsRejectDate = DateTime.Now;


            foreach (var item in borrow)
            {

                item.IsActive = false;
                item.IsApproved = false;
                item.IsApprovedDate = null;
                item.IsReject = true;
                item.IsRejectDate = DateTime.Now;
                item.ReturnQuantity = 0;

            }

            return true;

        }

        public async Task<PagedList<GetRejectBorrowedPagination>> GetAllRejectBorrowedWithPaginationCustomer(UserParams userParams, int empid)
        {

            var employee = _context.Users.Where(x => x.Id == empid)
                                        .FirstOrDefault();

            var borrow = _context.BorrowedIssues.OrderByDescending(x => x.IsRejectDate)
                                         .Where(x => x.PreparedBy == employee.FullName)
                                        .Where(x => x.IsRejectDate != null)
                                        .Where(x => x.IsReturned == null)
                                        .Where(x => x.IsReject == true)
                                        .Where(x => x.IsApproved == false)
                                        .Select(x => new GetRejectBorrowedPagination
                                        {

                                            BorrowedPKey = x.Id,
                                            CustomerName = x.CustomerName,
                                            CustomerCode = x.CustomerCode,
                                            TotalQuantity = x.TotalQuantity,
                                            //PreparedBy = x.PreparedBy,
                                            RejectDate = x.IsRejectDate.ToString(),
                                            Remarks = x.Reason,
                                            RejectBy = x.RejectBy,
                                            //BorrowedDate = x.PreparedDate.ToString(),
                                            TransactionDate = x.TransactionDate.ToString("MM/dd/yyyy")

                                        });

            return await PagedList<GetRejectBorrowedPagination>.CreateAsync(borrow, userParams.PageNumber, userParams.PageSize);


        }

        public async Task<PagedList<GetRejectBorrowedPagination>> GetAllRejectBorrowedWithPaginationCustomerOrig(UserParams userParams, string search, int empid)
        {

            var employee = _context.Users.Where(x => x.Id == empid)
                                     .FirstOrDefault();


            var borrow = _context.BorrowedIssues.OrderByDescending(x => x.IsRejectDate)
                                 .Where(x => x.PreparedBy == employee.FullName)
                                 .Where(x => x.IsRejectDate != null)
                                   
                                 .Where(x => x.IsReturned == null)
                                 .Where(x => x.IsReject == true)
                                 .Where(x => x.IsApproved == false)
                                 .Select(x => new GetRejectBorrowedPagination
                                 {

                                     BorrowedPKey = x.Id,
                                     CustomerName = x.CustomerName,
                                     CustomerCode = x.CustomerCode,
                                     TotalQuantity = x.TotalQuantity,
                                     //PreparedBy = x.PreparedBy,
                                     RejectDate = x.IsRejectDate.ToString(),
                                     Remarks = x.Reason,
                                     RejectBy = x.RejectBy,
                                     //BorrowedDate = x.PreparedDate.ToString(),
                                     TransactionDate = x.TransactionDate.ToString("MM/dd/yyyy")

                                 }).Where(x => (Convert.ToString(x.BorrowedPKey)).ToLower().Contains(search.Trim().ToLower()));


            return await PagedList<GetRejectBorrowedPagination>.CreateAsync(borrow, userParams.PageNumber, userParams.PageSize);
        }



        public async Task<PagedList<GetRejectBorrowedPagination>> GetAllRejectBorrowedWithPagination(UserParams userParams)
        {
            var borrow = _context.BorrowedIssues.OrderByDescending(x => x.IsRejectDate)
                                             .Where(x => x.IsRejectDate != null)
                                             .Where(x => x.IsReturned == null)
                                             .Where(x => x.IsReject == true)
                                             .Where(x => x.IsApproved == false)
                                             .Select(x => new GetRejectBorrowedPagination
                                             {

                                                 BorrowedPKey = x.Id,
                                                 CustomerName = x.CustomerName,
                                                 CustomerCode = x.CustomerCode,
                                                 TotalQuantity = x.TotalQuantity,
                                                 //PreparedBy = x.PreparedBy,
                                                 RejectDate = x.IsRejectDate.ToString(),
                                                 Remarks = x.Reason,
                                                 RejectBy = x.RejectBy,
                                                 //BorrowedDate = x.PreparedDate.ToString(),
                                                 TransactionDate = x.TransactionDate.ToString("MM/dd/yyyy")

                                             });

            return await PagedList<GetRejectBorrowedPagination>.CreateAsync(borrow, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<GetRejectBorrowedPagination>> GetAllRejectBorrowedWithPaginationOrig(UserParams userParams, string search)
        {
            var borrow = _context.BorrowedIssues.OrderByDescending(x => x.IsRejectDate)
                                           .Where(x => x.IsRejectDate != null)
                                           .Where(x => x.IsReturned == null)
                                           .Where(x => x.IsReject == true)
                                           .Where(x => x.IsApproved == false)
                                           .Select(x => new GetRejectBorrowedPagination
                                           {

                                               BorrowedPKey = x.Id,
                                               CustomerName = x.CustomerName,
                                               CustomerCode = x.CustomerCode,
                                               TotalQuantity = x.TotalQuantity,
                                               //PreparedBy = x.PreparedBy,
                                               RejectDate = x.IsRejectDate.ToString(),
                                               Remarks = x.Remarks,
                                               RejectBy = x.RejectBy,
                                               //BorrowedDate = x.PreparedDate.ToString(),
                                               TransactionDate = x.TransactionDate.ToString("MM/dd/yyyy")

                                           }).Where(x => (Convert.ToString(x.BorrowedPKey)).ToLower().Contains(search.Trim().ToLower())
                                                    || (Convert.ToString(x.CustomerCode)).ToLower().Contains(search.Trim().ToLower())
                                                      || (Convert.ToString(x.CustomerName)).ToLower().Contains(search.Trim().ToLower()));


            return await PagedList<GetRejectBorrowedPagination>.CreateAsync(borrow, userParams.PageNumber, userParams.PageSize);
        }



        public async Task<PagedList<DtoGetAllReturnedItem>> GetAllForApproveReturnedItem(UserParams userParams, bool status)
        {

            var borrowed = _context.BorrowedIssues.Where(x => x.IsReturned == true && x.IsActive == true)
                                                  .GroupBy(x => new
                                                  {

                                                      x.Id,
                                                      x.CustomerCode,
                                                      x.CustomerName,
                                                      x.ReturnBy,
                                                      x.IsApprovedReturned,
                                                      x.StatusApproved

                                                  }).Select(x => new DtoGetAllReturnedItem
                                                  {

                                                      Id = x.Key.Id,
                                                      CustomerCode = x.Key.CustomerCode,
                                                      CustomerName = x.Key.CustomerName,
                                                      ReturnBy = x.Key.ReturnBy,
                                                      IsApproveReturn = x.Key.IsApprovedReturned != null ? true : false,
                                                      StatusApprove = x.Key.StatusApproved

                                                  });


            var BorrowIssue = _context.BorrowedIssueDetails
                .GroupJoin(borrowed, borrowdetails => borrowdetails.BorrowedPKey, borrowedissue => borrowedissue.Id, (borrowdetails, borrowissue) => new { borrowdetails, borrowissue })
                .SelectMany(x => x.borrowissue.DefaultIfEmpty(), (x, borrowissue) => new { x.borrowdetails, borrowissue })
                .Where(x => x.borrowdetails.IsActive == true && x.borrowdetails.IsReturned == true && x.borrowdetails.IsApprovedReturned == status)
                .OrderByDescending(x => x.borrowissue.IsApproveReturn)
                .GroupBy(x => new
                {
                    //x.borrowdetails.BorrowedPKey,
                    x.borrowissue.Id,
                    x.borrowissue.CustomerCode,
                    x.borrowissue.CustomerName,
                    x.borrowdetails.PreparedBy,
                    x.borrowdetails.ReturnedDate,
                    x.borrowissue.ReturnBy,
                    x.borrowdetails.IsApprovedReturned,
                    x.borrowissue.StatusApprove




                }).Select(x => new DtoGetAllReturnedItem
                {
                    //TransactionId = x.Key.BorrowedPKey,
                    Id = x.Key.Id,
                    CustomerCode = x.Key.CustomerCode,
                    CustomerName = x.Key.CustomerName,
                    PreparedBy = x.Key.PreparedBy,
                    ReturnedDate = x.Key.ReturnedDate.ToString(),
                    ReturnBy = x.Key.ReturnBy,

                    TotalBorrowedQuantity = x.Sum(x => x.borrowdetails.Quantity),
                    Consumed = x.Sum(x => x.borrowdetails.Quantity) - x.Sum(x => x.borrowdetails.ReturnQuantity),
                    TotalReturned = x.Sum(x => x.borrowdetails.ReturnQuantity),

                    IsApproveReturn = x.Key.IsApprovedReturned != null ? true : false,
                    StatusApprove = x.Key.StatusApprove


                });



            return await PagedList<DtoGetAllReturnedItem>.CreateAsync(BorrowIssue, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<DtoGetAllReturnedItem>> GetAllForApproveReturnedItemOrig(UserParams userParams, string search, bool status)
        {

            var borrowed = _context.BorrowedIssues.Where(x => x.IsReturned == true && x.IsActive == true)
                                                .GroupBy(x => new
                                                {

                                                    x.Id,
                                                    x.CustomerCode,
                                                    x.CustomerName,
                                                    x.ReturnBy,
                                                    x.IsApprovedReturned,
                                                    x.StatusApproved

                                                    //x.IsApprovedReturned

                                                }).Select(x => new DtoGetAllReturnedItem
                                                {

                                                    Id = x.Key.Id,
                                                    CustomerCode = x.Key.CustomerCode,
                                                    CustomerName = x.Key.CustomerName,
                                                    ReturnBy = x.Key.ReturnBy,
 
                                                    IsApproveReturn = x.Key.IsApprovedReturned != null ? true : false,
                                                    StatusApprove = x.Key.StatusApproved
                                                    //IsApproveReturn = x.Key.IsApprovedReturned != null ? true : false

                                                });


            var BorrowIssue = _context.BorrowedIssueDetails
                .GroupJoin(borrowed, borrowdetails => borrowdetails.BorrowedPKey, borrowedissue => borrowedissue.Id, (borrowdetails, borrowissue) => new { borrowdetails, borrowissue })
                .SelectMany(x => x.borrowissue.DefaultIfEmpty(), (x, borrowissue) => new { x.borrowdetails, borrowissue })
                .Where(x => x.borrowdetails.IsActive == true && x.borrowdetails.IsReturned == true && x.borrowdetails.IsApprovedReturned == status)
                .OrderByDescending(x => x.borrowissue.IsApproveReturn)
                .GroupBy(x => new
                {
                    //x.borrowdetails.BorrowedPKey,
                    x.borrowissue.Id,
                    x.borrowissue.CustomerCode,
                    x.borrowissue.CustomerName,
                    x.borrowdetails.PreparedBy,
                    x.borrowdetails.ReturnedDate,
                    x.borrowissue.ReturnBy,
                    x.borrowdetails.IsApprovedReturned,
                    x.borrowissue.StatusApprove




                }).Select(x => new DtoGetAllReturnedItem
                {
                    //TransactionId = x.Key.BorrowedPKey,
                    Id = x.Key.Id,
                    CustomerCode = x.Key.CustomerCode,
                    CustomerName = x.Key.CustomerName,
                    PreparedBy = x.Key.PreparedBy,
                    ReturnedDate = x.Key.ReturnedDate.ToString(),
                    ReturnBy = x.Key.ReturnBy,


                    TotalBorrowedQuantity = x.Sum(x => x.borrowdetails.Quantity),
                    Consumed = x.Sum(x => x.borrowdetails.Quantity) - x.Sum(x => x.borrowdetails.ReturnQuantity),
                    TotalReturned = x.Sum(x => x.borrowdetails.ReturnQuantity),

                    IsApproveReturn = x.Key.IsApprovedReturned != null ? true : false,
                    StatusApprove = x.Key.StatusApprove


                }).Where(x => (Convert.ToString(x.Id)).ToLower().Contains(search.Trim().ToLower())
                          || (Convert.ToString(x.CustomerCode)).ToLower().Contains(search.Trim().ToLower())
                          || (Convert.ToString(x.CustomerName)).ToLower().Contains(search.Trim().ToLower()));



            return await PagedList<DtoGetAllReturnedItem>.CreateAsync(BorrowIssue, userParams.PageNumber, userParams.PageSize);


        }

        public async Task<bool> ApproveForReturned(BorrowedIssue borrowed)
        {
            var issue = await _context.BorrowedIssues.Where(x => x.Id == borrowed.Id)
                                                     .FirstOrDefaultAsync();


            var borrow = await _context.BorrowedIssueDetails.Where(x => x.BorrowedPKey == borrowed.Id)
                                                            .ToListAsync();


            issue.IsActive = true;
            issue.IsApprovedReturned = true;
            issue.IsApprovedReturnedDate = DateTime.Now;
            issue.ApprovedReturnedBy = borrowed.ApprovedReturnedBy;
            issue.IsReturned = true;
            issue.StatusApproved = "Return approved";


            foreach (var item in borrow)
            {

                item.IsActive = true;
                item.IsApprovedReturned = true;
                item.IsApprovedReturnedDate = DateTime.Now;
                item.IsReturned = true;

            }


            var borrowWithAgingDays = await _context.BorrowedIssues.Where(x => x.Id == borrowed.Id)
                                                               .Select(x => new
                                                               {
                                                                   AgingDays = x.IsApprovedDate != null ? EF.Functions.DateDiffDay(x.IsApprovedDate.Value, DateTime.Now) : 0
                                                               }).FirstOrDefaultAsync();

            if (borrowWithAgingDays != null)
            {
                issue.AgingDays = borrowWithAgingDays.AgingDays;
            }

            await _context.SaveChangesAsync();



            return true;
        }

        public async Task<bool> CancelForReturned(BorrowedIssue borrowed)
        {
            var issue = await _context.BorrowedIssues.Where(x => x.Id == borrowed.Id)
                                                     .FirstOrDefaultAsync();


            var borrow = await _context.BorrowedIssueDetails.Where(x => x.BorrowedPKey == borrowed.Id)
                                                             .ToListAsync();


            foreach (var item in borrow)
            {

                item.IsReturned = null;
                item.ReturnedDate = null;
                item.IsApprovedReturned = null;
                item.ReturnQuantity = 0;
            }

            issue.IsReturned = null;
            issue.IsApprovedReturned = false;
            issue.Reason = borrowed.Reason;

            return true;

        }


        public async Task<PagedList<GetAllDetailsBorrowedTransactionDto>> GetAllDetailsBorrowedTransaction(UserParams userParams)
        {
          

                 var borrowed = _context.BorrowedIssueDetails
                 .GroupJoin(_context.BorrowedIssues, borrowdetails => borrowdetails.BorrowedPKey, borrowissue => borrowissue.Id, (borrowdetails, borrowissue) => new {borrowdetails, borrowissue })
                .SelectMany(x => x.borrowissue.DefaultIfEmpty(), (x, borrowissue) => new { x.borrowdetails, borrowissue })
                .Where(x => x.borrowdetails.IsActive == true)
                .GroupBy(x => new
                {

                    x.borrowissue.Id,
                    x.borrowissue.CustomerCode,
                    x.borrowissue.CustomerName,
                    x.borrowissue.TransactionDate,
                    x.borrowissue.PreparedDate,
                    x.borrowissue.IsApproved,
                    x.borrowissue.IsApprovedDate,
                    x.borrowissue.IsApprovedReturned,
                    x.borrowissue.IsApprovedReturnedDate,
                    x.borrowissue.IsReject,
                    x.borrowissue.IsRejectDate,
                    x.borrowissue.Remarks,
                    x.borrowissue.Reason,
                

                    x.borrowissue.StatusApproved,
                    x.borrowissue.PreparedBy,

                    x.borrowissue.CompanyCode,
                    x.borrowissue.CompanyName,
                    x.borrowissue.DepartmentCode,
                    x.borrowissue.DepartmentName,
                    x.borrowissue.LocationCode,
                    x.borrowissue.LocationName,
                    x.borrowissue.AccountCode,
                    x.borrowissue.AccountTitles,
                    x.borrowissue.AgingDays
                   


                })
               .Select(x => new GetAllDetailsBorrowedTransactionDto
               {

                   Id = x.Key.Id,
                   CustomerCode = x.Key.CustomerCode,
                   CustomerName = x.Key.CustomerName,
                   TransactionDate = x.Key.TransactionDate.ToString(),
                   BorrowedDate = x.Key.PreparedDate.ToString(),
                   IsApproved = x.Key.IsApproved,
                   IsApprovedDate = x.Key.IsApprovedDate.ToString(),
                   TotalBorrowed = x.Sum(x => x.borrowdetails.Quantity),
                   IsApproveReturned = x.Key.IsApprovedReturned,
                   IsApproveReturnedDate = x.Key.IsApprovedReturnedDate.ToString(),
                   Consumed = x.Sum(x => x.borrowdetails.Quantity != null ? x.borrowdetails.Quantity : 0) - x.Sum(x => x.borrowdetails.ReturnQuantity),
                   ReturnedQuantity = x.Sum(x => x.borrowdetails.ReturnQuantity != null ? x.borrowdetails.ReturnQuantity : 0),
                   IsReject = x.Key.IsReject,
                   IsRejectDate = x.Key.IsRejectDate.ToString(),
                   Remarks = x.Key.Remarks,

                   StatusApprove = x.Key.StatusApproved,
                   PreparedBy = x.Key.PreparedBy,

                   CompanyCode = x.Key.CompanyCode,
                   CompanyName = x.Key.CompanyName,
                   DepartmentCode = x.Key.DepartmentCode,
                   DepartmentName = x.Key.DepartmentName,
                   LocationCode = x.Key.LocationCode,
                   LocationName = x.Key.LocationName,
                   AccountCode = x.Key.AccountCode,
                   AccountTitles = x.Key.AccountTitles,

                   AgingDays = x.Key.IsApprovedDate != null && x.Key.IsApprovedReturnedDate == null? EF.Functions.DateDiffDay(x.Key.IsApprovedDate.Value , DateTime.Now) : 0,
                   Reason = x.Key.Reason
               });


           

            return await PagedList<GetAllDetailsBorrowedTransactionDto>.CreateAsync(borrowed, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<PagedList<GetAllDetailsBorrowedTransactionDto>> GetAllDetailsBorrowedTransactionOrig(UserParams userParams, string search)
        {
            var borrowed = _context.BorrowedIssueDetails
                 .GroupJoin(_context.BorrowedIssues, borrowdetails => borrowdetails.BorrowedPKey, borrowissue => borrowissue.Id, (borrowdetails, borrowissue) => new { borrowdetails, borrowissue })
                .SelectMany(x => x.borrowissue.DefaultIfEmpty(), (x, borrowissue) => new { x.borrowdetails, borrowissue })
                  .Where(x => x.borrowdetails.IsActive == true)
                .GroupBy(x => new
                {

                    x.borrowissue.Id,
                    x.borrowissue.CustomerCode,
                    x.borrowissue.CustomerName,
                    x.borrowissue.TransactionDate,
                    x.borrowissue.PreparedDate,
                    x.borrowissue.IsApproved,
                    x.borrowissue.IsApprovedDate,
                    x.borrowissue.IsApprovedReturned,
                    x.borrowissue.IsApprovedReturnedDate,
                    x.borrowissue.IsReject,
                    x.borrowissue.IsRejectDate,
                    x.borrowissue.Remarks,
                    x.borrowissue.Reason,


                    x.borrowissue.StatusApproved,
                    x.borrowissue.PreparedBy,

                    x.borrowissue.CompanyCode,
                    x.borrowissue.CompanyName,
                    x.borrowissue.DepartmentCode,
                    x.borrowissue.DepartmentName,
                    x.borrowissue.LocationCode,
                    x.borrowissue.LocationName,
                    x.borrowissue.AccountCode, 
                    x.borrowissue.AccountTitles,



                })
               .Select(x => new GetAllDetailsBorrowedTransactionDto
               {

                   Id = x.Key.Id,
                   CustomerCode = x.Key.CustomerCode,
                   CustomerName = x.Key.CustomerName,
                   TransactionDate = x.Key.TransactionDate.ToString(),
                   BorrowedDate = x.Key.PreparedDate.ToString(),
                   IsApproved = x.Key.IsApproved,
                   IsApprovedDate = x.Key.IsApprovedDate.ToString(),
                   TotalBorrowed = x.Sum(x => x.borrowdetails.Quantity),
                   IsApproveReturned = x.Key.IsApprovedReturned,
                   IsApproveReturnedDate = x.Key.IsApprovedReturnedDate.ToString(),
                   Consumed = x.Sum(x => x.borrowdetails.Quantity != null ? x.borrowdetails.Quantity : 0) - x.Sum(x => x.borrowdetails.ReturnQuantity),
                   ReturnedQuantity = x.Sum(x => x.borrowdetails.ReturnQuantity != null ? x.borrowdetails.ReturnQuantity : 0),
                   IsReject = x.Key.IsReject,
                   IsRejectDate = x.Key.IsRejectDate.ToString(),
                   Remarks = x.Key.Remarks,

                   StatusApprove = x.Key.StatusApproved,
                   PreparedBy = x.Key.PreparedBy,

                   CompanyCode = x.Key.CompanyCode,
                   CompanyName = x.Key.CompanyName,
                   DepartmentCode = x.Key.DepartmentCode,
                   DepartmentName = x.Key.DepartmentName,
                   LocationCode = x.Key.LocationCode,
                   LocationName = x.Key.LocationName,
                   AccountCode = x.Key.AccountCode,
                   AccountTitles = x.Key.AccountTitles,

                   AgingDays = x.Key.IsApprovedDate != null && x.Key.IsApprovedReturnedDate == null ? EF.Functions.DateDiffDay(x.Key.IsApprovedDate.Value, DateTime.Now) : 0,
                   Reason = x.Key.Reason
               }).Where(x => (Convert.ToString(x.Id)).ToLower().Contains(search.Trim().ToLower())
                          || (Convert.ToString(x.CustomerCode)).ToLower().Contains(search.Trim().ToLower())
                          || (Convert.ToString(x.CustomerName)).ToLower().Contains(search.Trim().ToLower()));


            return await PagedList<GetAllDetailsBorrowedTransactionDto>.CreateAsync(borrowed, userParams.PageNumber, userParams.PageSize);
        }




        //================================================ Borrowed Notification =============================================================






        public async Task<IReadOnlyList<GetNotificationForBorrowedApprovalDto>> GetNotificationForBorrowedApproval()
        {

            var borrowed = _context.BorrowedIssues.Where(x => x.IsActive == true)
                                                  .Where(x => x.IsApproved == false)
                                                  .Where(x => x.IsApprovedDate == null && x.IsReject == null)
                                   
                                                 .GroupBy(x => new
                                                 {
                                                     x.Id,
                                                     x.CustomerCode,
                                                     x.CustomerName,
                                                     x.IsApproved,
                                                     x.PreparedDate,
                                                     //x.IsActive

                                                 }).Select(x => new GetNotificationForBorrowedApprovalDto
                                                 {

                                                     Id = x.Key.Id,
                                                     CustomerCode = x.Key.CustomerCode,
                                                     CustomerName = x.Key.CustomerName,
                                                     IsApproved = x.Key.IsApproved,
                                                     //IsActive = x.Key.IsActive,
                                                     

                                                 });


            return await borrowed.ToListAsync();

        }

        public async Task<IReadOnlyList<GetNotificationForReturnedApprovalDto>> GetNotificationForReturnedApproval()
        {
            var returned = _context.BorrowedIssues.Where(x => x.IsActive == true)
                                                        .Where(x => x.IsReturned == true)
                                                        .Where(x => x.IsApprovedReturned == false)
                                                        .GroupBy(x => new
                                                        {

                                                            x.Id,
                                                            x.CustomerCode,
                                                            x.CustomerName,
                                                            x.IsApprovedReturned,
                                                            //x.ReturnedDate,
                                                            //x.IsActive

                                                        }).Select(x => new GetNotificationForReturnedApprovalDto
                                                        {
                                                            Id = x.Key.Id,
                                                            CustomerCode = x.Key.CustomerCode,
                                                            CustomerName = x.Key.CustomerName,
                                                            //IsActive = x.Key.IsActive


                                                        });

            return  await returned.ToListAsync();
        }


        


        public async Task<IReadOnlyList<RejectBorrowedNotificationDto>> RejectBorrowedNotification()
        {


            var reject = _context.BorrowedIssues.Where(x => x.IsActive == false && x.IsReject == true && x.IsRejectDate != null)
                                                  .Select(x => new RejectBorrowedNotificationDto
                                                  {
                                                      BorrowedPKey = x.Id,
                                                      CustomerCode = x.CustomerCode,
                                                      CustomerName = x.CustomerName,
                                                      IsActive = x.IsActive,
                                                  });

            return await reject.ToListAsync();

        }


        public async Task<IReadOnlyList<GetNotificationForBorrowedApprovalDto>> GetNotificationBorrowedApprove(int empid)
        {

            var employee = await _context.Users.Where(x => x.Id == empid)
                                          .FirstOrDefaultAsync();

            var borrowed = _context.BorrowedIssues.Where(x => x.IsActive == true)
                                                .Where(x => x.IsApproved == true)
                                                .Where(x => x.PreparedBy == employee.FullName)

                                               .GroupBy(x => new
                                               {
                                                   x.Id,
                                                   x.CustomerCode,
                                                   x.CustomerName,
                                                   x.IsApproved,
                                                   x.PreparedDate,
                                                   x.IsActive

                                               }).Select(x => new GetNotificationForBorrowedApprovalDto
                                               {

                                                   Id = x.Key.Id,
                                                   CustomerCode = x.Key.CustomerCode,
                                                   CustomerName = x.Key.CustomerName,
                                                   IsApproved = x.Key.IsApproved,
                                                   IsActive = x.Key.IsActive,

                                               });


            return await borrowed.ToListAsync();
        }

        public async Task<IReadOnlyList<GetNotificationForReturnedApprovalDto>> GetNotificationReturnedApprove(int empid)
        {

            var employee = await   _context.Users.Where(x => x.Id == empid)
                                                  .FirstOrDefaultAsync();


            var returned = _context.BorrowedIssues.Where(x => x.IsActive == true)
                                                       .Where(x => x.IsReturned == true)
                                                       .Where(x => x.IsApprovedReturned == true)
                                                       .Where(x => x.PreparedBy == employee.FullName)
                                                       .GroupBy(x => new
                                                       {

                                                           x.Id,
                                                           x.CustomerCode,
                                                           x.CustomerName,
                                                           x.IsApprovedReturned,
                                                           //x.ReturnedDate,
                                                           x.IsActive

                                                       }).Select(x => new GetNotificationForReturnedApprovalDto
                                                       {
                                                           Id = x.Key.Id,
                                                           CustomerCode = x.Key.CustomerCode,
                                                           CustomerName = x.Key.CustomerName,
                                                           IsActive = x.Key.IsActive


                                                       });

            return await returned.ToListAsync();
        }

        public async Task<IReadOnlyList<RejectBorrowedNotificationDto>> RejectBorrowedNotificationWithParameter(int empid)
        {

            var employee = await _context.Users.Where(x => x.Id == empid)
                                                .FirstOrDefaultAsync();

            var reject = _context.BorrowedIssues.Where(x => x.IsActive == false && x.IsReject == true && x.IsRejectDate != null)
                                                .Where(x => x.PreparedBy == employee.FullName)
                                                  .Select(x => new RejectBorrowedNotificationDto
                                                  {
                                                      BorrowedPKey = x.Id,
                                                      CustomerCode = x.CustomerCode,
                                                      CustomerName = x.CustomerName,
                                                      IsActive = x.IsActive,
                                                  });

            return await reject.ToListAsync();

        }




        // New Updates for borrowed


        public async Task<bool> AddPendingBorrowedItem(BorrowedIssueDetails borrow)
        {
         
            await _context.BorrowedIssueDetails.AddAsync(borrow);

            return true;

        }

        public async Task<bool> CloseSaveBorrowed(BorrowedIssueDetails borrow)
        {

            var close = await _context.BorrowedIssueDetails.Where(x => x.BorrowedPKey == borrow.BorrowedPKey)
                                                           .ToListAsync();


            foreach(var item in close)
            {

                item.ReturnQuantity = 0;
            }

            return true;
        }

        public async Task<bool> EditBorrowedQuantity(BorrowedIssueDetails borrow)
        {

            var borrowed = await _context.BorrowedIssueDetails.Where(x => x.Id == borrow.Id)
                                                              .FirstOrDefaultAsync();

            borrowed.Quantity = borrow.Quantity;
            borrowed.WarehouseId = borrow.WarehouseId;
            borrowed.ItemCode = borrow.ItemCode;
            borrowed.ItemDescription = borrow.ItemDescription;
            borrowed.Uom = borrow.Uom;

          
            return true;

        }

        public async Task<bool> EditBorrowedIssue(BorrowedIssue borrow)
        {
            
            var borrowed = await _context.BorrowedIssues.Where(x => x.Id == borrow.Id)
                                                         .FirstOrDefaultAsync();

            borrowed.CustomerCode = borrow.CustomerCode;
            borrowed.CustomerName = borrow.CustomerName;
            borrowed.TransactionDate = borrow.TransactionDate;

            borrowed.TotalQuantity = borrow.TotalQuantity;
            borrowed.DepartmentCode = borrow.DepartmentCode;
            borrowed.DepartmentName = borrow.DepartmentName;
            borrowed.CompanyCode = borrow.CompanyCode;
            borrowed.CompanyName = borrow.CompanyName;
            borrowed.LocationCode = borrow.LocationCode;
            borrowed.LocationName = borrow.LocationName;
            borrowed.AccountCode = borrow.AccountCode;
            borrowed.AccountTitles = borrow.AccountTitles;

            return true;

        }



        public async Task<IReadOnlyList<DtoViewBorrewedReturnedDetails>> ViewAllBorrowedDetails(int id)
        {
            var issueBorrowed = _context.BorrowedIssues.Where(x => x.IsActive == true)
                                            .Select(x => new GetAllDetailsInBorrowedIssueDto
                                            {
                                                Id = x.Id,
                                                Customer = x.CustomerName,
                                                CustomerCode = x.CustomerCode,

                                                Remarks = x.Remarks,

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


            var borrow = _context.BorrowedIssueDetails
                .GroupJoin(issueBorrowed, borrowed => borrowed.BorrowedPKey, issues => issues.Id, (borrowed, issues) => new { borrowed, issues })
                .SelectMany(x => x.issues.DefaultIfEmpty(), (x, issues) => new { x.borrowed, issues })
              .Where(x => x.borrowed.BorrowedPKey == id)
              .Where(x => x.borrowed.IsActive == true)
              .Select(x => new DtoViewBorrewedReturnedDetails
              {
                  BorrowedPKey = x.borrowed.BorrowedPKey,
                  Id = x.borrowed.Id,
                  Customer = x.issues.Customer,
                  CustomerCode = x.issues.CustomerCode,

                  ItemCode = x.borrowed.ItemCode,
                  ItemDescription = x.borrowed.ItemDescription,
                  ReturnQuantity = x.borrowed.ReturnQuantity,
                  Consume = x.borrowed.Quantity - x.borrowed.ReturnQuantity,
                  ReturnedDate = x.borrowed.ReturnedDate.ToString(),
                  PreparedBy = x.borrowed.PreparedBy,

                  Remarks = x.issues.Remarks,

                  TransactionDate = x.issues.TransactionDate.ToString(),
                  CompanyCode = x.issues.CompanyCode,
                  CompanyName = x.issues.CompanyName,
                  DepartmentCode = x.issues.DepartmentCode,
                  DepartmentName = x.issues.DepartmentName,
                  LocationCode = x.issues.LocationCode,
                  LocationName = x.issues.LocationName,
                  AccountCode = x.issues.AccountCode,
                  AccountTitles = x.issues.AccountTitles,

                  Uom = x.borrowed.Uom

              });

            return await borrow.ToListAsync();


        }

        public async Task<bool> CancelAllBorrowed(BorrowedIssue borrowed)
        {
           
            var issue = await _context.BorrowedIssues.Where(x => x.Id == borrowed.Id)
                                                       .FirstOrDefaultAsync();

            var details = await _context.BorrowedIssueDetails.Where(x => x.BorrowedPKey == borrowed.Id)
                                                             .ToListAsync();

            issue.ApproveBy = borrowed.ApproveBy;
            issue.IsActive = false;

            foreach(var items in details)
            {
                items.IsActive = false;
            }

            return true;

        }

        public async Task<IReadOnlyList<GetAllAvailableBorrowIssueDto>> GetTransactedBorrowedDetails(int empid)
        {
            var employee = await _context.Users.Where(x => x.Id == empid)
                                             .FirstOrDefaultAsync();

            var items = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                         .Where(x => x.PreparedBy == employee.FullName)
                                                         .Where(x => x.IsTransact == true)
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

        public async Task<bool> CancelUpdateBorrowed(BorrowedIssueDetails borrowed)
        {

            var cancel = await _context.BorrowedIssueDetails.Where(x => x.Id == borrowed.Id && x.IsActive == true)
                                                                        .FirstOrDefaultAsync();

            if (cancel == null)
                throw new NoBorrowedFoundExcemption(); 

            var activeModulesCount = await _context.BorrowedIssueDetails
                .Where(x => x.BorrowedPKey == cancel.BorrowedPKey && x.IsActive == true)
                .CountAsync();

            if (activeModulesCount > 1)
            {
                cancel.IsActive = false;
                cancel.IsTransact = false;
                return true; 
            }
            else
            {
                throw new AtLeast1ItemException(); 
            }



        }

       
    }

}
