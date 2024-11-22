﻿using ELIXIRETD.DATA.CORE.INTERFACES.BORROWED_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO.BorrowedNotification;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.BORROWED_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.BORROWED_REPOSITORY.Excemption;
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

        public async Task<IReadOnlyList<GetAvailableStocksForBorrowedIssue_Dto>> GetAvailableStocksForBorrowedIssueNoParameters()
        {
            var getWarehouseStocks = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                               .GroupBy(x => new
                                                               {

                                                                   x.ItemCode
                                                         
                                                               }).Select(x => new WarehouseInventory
                                                               {

                                                                   ItemCode = x.Key.ItemCode,
                                                                   ItemDescription = x.First().ItemDescription,
                                                                   Uom = x.First().Uom,
                                                                   ActualGood = x.Sum(x => x.ActualGood)

                                                               });

            var reserveOut = _context.Orders.Where(x => x.IsActive == true && x.PreparedDate != null)
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
                                                                Out = x.Sum(x => x.Quantity),

                                                            });




            var BorrowedOut = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
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
                                                                 In = x.Sum(x => x.returned.Quantity != null ? x.returned.Quantity : 0) -  x.Sum(x => x.itemconsume.Consume),

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

            var getAvailable = (from warehouse in getWarehouseStocks
                                join issue in issueOut
                                on warehouse.ItemCode equals issue.ItemCode
                                into leftJ1
                                from issue in leftJ1.DefaultIfEmpty()

                                join borrowOut in BorrowedOut
                                on warehouse.ItemCode equals borrowOut.ItemCode
                                into leftJ2
                                from borrowOut in leftJ2.DefaultIfEmpty()

                                join reserve in reserveOut
                                on warehouse.ItemCode equals reserve.ItemCode
                                into leftJ3
                                from reserve in leftJ3.DefaultIfEmpty()
   
                                join borrowedReturned in BorrowedReturn
                                on warehouse.ItemCode equals borrowedReturned.ItemCode
                                into leftJ4
                                from borrowedReturned in leftJ4.DefaultIfEmpty()

                                join fuel in fuelRegister
                                on warehouse.ItemCode equals fuel.itemCode
                                into leftJ5
                                from fuel in leftJ5.DefaultIfEmpty()


                                group new
                                {

                                    warehouse,
                                    issue,
                                    borrowOut,
                                    reserve,
                                    borrowedReturned,
                                    fuel,

                                }

                                by new
                                {

                                    warehouse.ItemCode,


                                } into total

                                select new GetAvailableStocksForBorrowedIssue_Dto
                                {


                                    ItemCode = total.Key.ItemCode,
                                    ItemDescription = total.First().warehouse.ItemDescription,
                                    Uom = total.First().warehouse.Uom,
                                    RemainingStocks = total.Sum(x => x.warehouse.ActualGood) + total.Sum(x => x.borrowedReturned.In) 
                                    - total.Sum(x => x.reserve.QuantityOrdered) - total.Sum(x => x.issue.Out) - total.Sum(x => x.borrowOut.Out)
                                    - total.Sum(x => x.fuel.Quantity != null ? x.fuel.Quantity : 0),

                                }).Where(x => x.RemainingStocks >= 1 );



            var GetAvailableItem =  getAvailable.GroupBy(x => new
            {
                x.ItemCode,

            }).Select(x => new GetAvailableStocksForBorrowedIssue_Dto
            {
                ItemCode = x.Key.ItemCode,
                ItemDescription = x.First().ItemDescription,
                Uom = x.First().Uom,
                RemainingStocks = x.Sum(x => x.RemainingStocks),

            }).OrderBy(x => x.ItemCode);
                                 

            return await GetAvailableItem.ToListAsync();
        }

        public async Task<IReadOnlyList<GetAvailableStocksForBorrowedIssue_Dto>> GetAvailableStocksForBorrowedIssue(string itemcode)
        {
            var getWarehouseStocks = _context.WarehouseReceived.Where(x => x.ItemCode == itemcode).Where(x => x.IsActive == true)
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

            var moveorderOut = _context.MoveOrders.Where(x => x.ItemCode == itemcode).Where(x => x.IsActive == true)
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


            var BorrowedOut = _context.BorrowedIssueDetails.Where(x => x.ItemCode == itemcode).Where(x => x.IsActive == true)
                                                          
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


            var fuelRegister = _context.FuelRegisterDetails
               .Include(m => m.Material)
               .Include(m => m.Warehouse_Receiving)
               .Where(fr => fr.Is_Active == true)
               .Where(fr => fr.Material.ItemCode == itemcode)
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

            var getRemaining = (from warehouse in getWarehouseStocks
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

                                join fuel in fuelRegister
                                on warehouse.WarehouseId equals fuel.WarehouseId
                                into leftJ4
                                from fuel in leftJ4.DefaultIfEmpty()

                                group new
                                {

                                    warehouse,
                                    Moveorder,
                                    issue,
                                    borrowOut,
                                    returned,
                                    fuel,
                                }

                                by new
                                {

                                    warehouse.WarehouseId,
                                    warehouse.ItemCode,
                                    warehouse.RecievingDate,
                                    warehouse.UnitPrice,
                                    WarehouseActualGood = warehouse.ActualGood != null ? warehouse.ActualGood : 0,
                                    MoveOrderOut = Moveorder.QuantityOrdered != null ? Moveorder.QuantityOrdered : 0,
                                    IssueOut = issue.Out != null ? issue.Out : 0,
                                    BorrowedOut = borrowOut.Out != null ? borrowOut.Out : 0,
                                    borrowedreturn = returned.In != null ? returned.In : 0,
                                    fuel = fuel.Quantity != null ? fuel.Quantity : 0,


                                } into total

                                select new GetAvailableStocksForBorrowedIssue_Dto
                                {

                                    WarehouseId = total.Key.WarehouseId,
                                    ItemCode = total.Key.ItemCode,
                                    RemainingStocks = total.Key.WarehouseActualGood + total.Key.borrowedreturn
                                    - total.Key.MoveOrderOut - total.Key.IssueOut 
                                    - total.Key.BorrowedOut
                                    - total.Key.fuel,
                                    ReceivingDate = total.Key.RecievingDate,
                                    UnitCost = total.Key.UnitPrice,


                                }).Where(x => x.RemainingStocks > 0)
                                  .Where(x => x.ItemCode == itemcode);


            return await getRemaining.ToListAsync();

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

                                                 }).Select(x => new GetNotificationForBorrowedApprovalDto
                                                 {

                                                     Id = x.Key.Id,
                                                     CustomerCode = x.Key.CustomerCode,
                                                     CustomerName = x.Key.CustomerName,
                                                     IsApproved = x.Key.IsApproved,
                                                     

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

                                                        }).Select(x => new GetNotificationForReturnedApprovalDto
                                                        {
                                                            Id = x.Key.Id,
                                                            CustomerCode = x.Key.CustomerCode,
                                                            CustomerName = x.Key.CustomerName,
                                                        });

            return  await returned.ToListAsync();
        }


        


        public async Task<IReadOnlyList<RejectBorrowedNotificationDto>> RejectBorrowedNotification()
        {


            var borrowed = _context.BorrowedIssues
                                                .Where(x => x.IsApproved == false)
                                                .Where(x => x.IsApprovedDate == null && x.IsReject == true)

                                               .GroupBy(x => new
                                               {
                                                   x.Id,
                                                   x.CustomerCode,
                                                   x.CustomerName,
                                                   x.IsApproved,

                                               }).Select(x => new RejectBorrowedNotificationDto
                                               {

                                                   Id = x.Key.Id,
                                                   CustomerCode = x.Key.CustomerCode,
                                                   CustomerName = x.Key.CustomerName,

                                               });


            return await borrowed.ToListAsync();

            

        }



       public async Task<IReadOnlyList<GetNotificationForBorrowedApprovalDto>> GetNotificationAllBorrowedNoParameters()
        {
            var borrowed = _context.BorrowedIssues
                                                .Where(x => x.IsReturned != true)
                                                 .Where(x => x.IsActive == true && x.IsApproved == false || x.IsReject != null)
                                              
                            
                                                .GroupBy(x => new
                                                {
                                                    x.Id,
                                                    x.CustomerCode,
                                                    x.CustomerName,

                                                }).Select(x => new GetNotificationForBorrowedApprovalDto
                                                {

                                                    Id = x.Key.Id,
                                                    CustomerCode = x.Key.CustomerCode,
                                                    CustomerName = x.Key.CustomerName,

                                                });


            return await borrowed.ToListAsync();
        }

        public async Task<IReadOnlyList<GetNotificationForBorrowedApprovalDto>> GetNotificationBorrowedApprove(int empid)
        {

            var employee = await _context.Users.Where(x => x.Id == empid)
                                          .FirstOrDefaultAsync();

            var borrowed = _context.BorrowedIssues.Where(x => x.IsActive == true)
                                                .Where(x => x.IsApproved == true)
                                                .Where(x => x.IsReturned != true)
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


                                                       }).Select(x => new GetNotificationForReturnedApprovalDto
                                                       {
                                                           Id = x.Key.Id,
                                                           CustomerCode = x.Key.CustomerCode,
                                                           CustomerName = x.Key.CustomerName,



                                                       });

            return await returned.ToListAsync();
        }

        public async Task<IReadOnlyList<RejectBorrowedNotificationDto>> RejectBorrowedNotificationWithParameter(int empid)
        {

            var employee = await _context.Users.Where(x => x.Id == empid)
                                                .FirstOrDefaultAsync();

            var reject = _context.BorrowedIssues.Where(x =>  x.IsReject == true && x.IsRejectDate != null)
                                                .Where(x => x.PreparedBy == employee.FullName)
                                                  .Select(x => new RejectBorrowedNotificationDto
                                                  {
                                                      Id = x.Id,
                                                      CustomerCode = x.CustomerCode,
                                                      CustomerName = x.CustomerName,
                                                      IsActive = x.IsActive,
                                                  });

            return await reject.ToListAsync();

        }


       public async Task<IReadOnlyList<GetNotificationForBorrowedApprovalDto>> GetNotificationAllBorrowed(int empid)
        {
            var employee = await _context.Users.Where(x => x.Id == empid)
                                        .FirstOrDefaultAsync();

            var borrowed = _context.BorrowedIssues
                                              
                                                .Where(x => x.IsReturned == null)
                                                .Where(x => x.IsApprovedDate != null || x.IsRejectDate != null)
                                                .Where(x => x.PreparedBy == employee.FullName)

                                               .GroupBy(x => new
                                               {
                                                   x.Id,
                                                   x.CustomerCode,
                                                   x.CustomerName,
                                                

                                               }).Select(x => new GetNotificationForBorrowedApprovalDto
                                               {

                                                   Id = x.Key.Id,
                                                   CustomerCode = x.Key.CustomerCode,
                                                   CustomerName = x.Key.CustomerName,

                                               });


            return await borrowed.ToListAsync();

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

                //item.ReturnQuantity = 0;
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

            return true;

        }



        public async Task<IReadOnlyList<DtoViewBorrewedReturnedDetails>> ViewAllBorrowedDetails(int id)
        {
            var issueBorrowed = _context.BorrowedIssues.Where(x => x.IsActive == true)
                                            .Select(x => new DtoViewBorrewedReturnedDetails
                                            {
                                                Id = x.Id,
                                                Customer = x.CustomerName,
                                                CustomerCode = x.CustomerCode,
                                                EmpId = x.EmpId,
                                                FullName = x.FullName,
                                                Remarks = x.Remarks,
                                                TransactionDate = x.TransactionDate.ToString(),
                                                Details = x.Details
                                                
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
                  EmpId = x.issues.EmpId,
                  FullName = x.issues.FullName,
                  ItemCode = x.borrowed.ItemCode,
                  ItemDescription = x.borrowed.ItemDescription,
                  Uom = x.borrowed.Uom,
                  ReturnedDate = x.borrowed.ReturnedDate.ToString(),
                  PreparedBy = x.borrowed.PreparedBy,
                  Remarks = x.issues.Remarks,
                  TransactionDate = x.issues.TransactionDate.ToString(),
                  Details = x.issues.Details,
                  BorrowedQuantity = x.borrowed.Quantity
   

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


        // ================================================== Customer ==================================================================================


        public async Task<bool> AddBorrowedIssue(BorrowedIssue borrowed)
        {

            await _context.BorrowedIssues.AddAsync(borrowed);

            return true;
        }


        public async Task<bool> AddBorrowedIssueDetails(BorrowedIssueDetails borrowed)
        {

            var unitprice = await _context.WarehouseReceived.Where(x => x.Id == borrowed.WarehouseId)
                                                            .FirstOrDefaultAsync();

            borrowed.UnitPrice = unitprice.UnitPrice;

            await _context.BorrowedIssueDetails.AddAsync(borrowed);
            return true;

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


      
        public async Task<PagedList<GetAllBorrowedReceiptWithPaginationDto>> GetAllBorrowedReceiptWithPagination(UserParams userParams, bool status, int empid)
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
                .Where(x => x.issues.IsActive == true)
                .Where(x => x.issues.IsApproved == status && x.issues.IsReturned != true && x.issues.IsReject == null)
                .GroupBy(x => new
                {
                    x.borrowed.BorrowedPKey,
                    x.issues.CustomerCode,
                    x.issues.CustomerName,
                    x.issues.EmpId,
                    x.issues.FullName,
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
                    x.issues.Details


                }).Select(x => new GetAllBorrowedReceiptWithPaginationDto
                {
                    BorrowedPKey = x.Key.BorrowedPKey,
                    CustomerName = x.Key.CustomerName,
                    CustomerCode = x.Key.CustomerCode,
                    EmpId = x.Key.EmpId,
                    FullName = x.Key.FullName,
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
                    StatusApprove = x.Key.StatusApproved,
                    Details = x.Key.Details

                }).OrderByDescending(x => x.BorrowedDate);

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
                .Where(x => x.issues.IsActive == true)
                .Where(x => x.issues.IsApproved == status && x.issues.IsReturned != true && x.issues.IsReject == null)
                .GroupBy(x => new
                {
                    x.borrowed.BorrowedPKey,
                    x.issues.CustomerCode,
                    x.issues.CustomerName,
                    x.issues.EmpId,
                    x.issues.FullName,
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
                    x.issues.Details

                }).Select(x => new GetAllBorrowedReceiptWithPaginationDto
                {
                    BorrowedPKey = x.Key.BorrowedPKey,
                    CustomerName = x.Key.CustomerName,
                    CustomerCode = x.Key.CustomerCode,
                    EmpId = x.Key.EmpId,
                    FullName = x.Key.FullName,
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
                    StatusApprove = x.Key.StatusApproved,
                    Details = x.Key.Details

                }).OrderByDescending(x => x.BorrowedDate).Where(x => (Convert.ToString(x.BorrowedPKey)).ToLower().Contains(search.Trim().ToLower()));


            return await PagedList<GetAllBorrowedReceiptWithPaginationDto>.CreateAsync(borrow, userParams.PageNumber, userParams.PageSize);
        }



        public async Task<IReadOnlyList<GetAllDetailsInBorrowedIssueDto>> GetAllDetailsInBorrowedIssue(int id)
        {

            var issueBorrowed = _context.BorrowedIssues.Where(x => x.IsActive == true)
                                               .Select(x => new DtoViewBorrewedReturnedDetails
                                               {
                                                   Id = x.Id,
                                                   Customer = x.CustomerName,
                                                   CustomerCode = x.CustomerCode,
                                                   EmpId = x.EmpId,
                                                   FullName = x.FullName,
                                                   TransactionDate = x.TransactionDate.ToString(),
                                                   Details = x.Details

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
              .Where(x => x.borrowed.IsReturned != true )
              .Where(x => x.borrowed.IsReject == null)
              .Where(x => x.borrowed.IsActive == true)

              .Select(x => new GetAllDetailsInBorrowedIssueDto
              {

                  Id = x.borrowed.Id,
                  WarehouseId = x.borrowed.WarehouseId,
                  BorrowedPKey = x.borrowed.BorrowedPKey,
                  Customer = x.issues.Customer,
                  CustomerCode = x.issues.CustomerCode,
                  EmpId = x.issues.EmpId,
                  FullName = x.issues.FullName,
                  Uom = x.borrowed.Uom,
                  PreparedDate = x.borrowed.PreparedDate.ToString(),
                  ItemCode = x.borrowed.ItemCode,
                  ItemDescription = x.borrowed.ItemDescription,
                  Quantity = x.borrowed.Quantity,
                  Remarks = x.borrowed.Remarks,
                  PreparedBy = x.borrowed.PreparedBy,
                  UnitCost = x.borrowed.UnitPrice,
                  TotalCost = x.borrowed.UnitPrice * x.borrowed.Quantity,
                  Details = x.issues.Details


         
              });


            return await warehouse.ToListAsync();
        }



        public async Task<bool> CancelAllborrowedfortransact(BorrowedIssueDetails borrowed)
        {

            var issue = await _context.BorrowedIssues.Where(x => x.Id == borrowed.BorrowedPKey)
                                                     .FirstOrDefaultAsync();

           
           
            var borrow = await _context.BorrowedIssueDetails.Where(x => x.BorrowedPKey == borrowed.BorrowedPKey)
                                                           .ToListAsync();

            foreach(var items in borrow)
            {

                items.IsActive = false;
                items.Remarks = borrowed.Remarks;
            }

            issue.IsActive = false;
            issue.Remarks = borrowed.Remarks;


            return true;
        }


        public async Task<bool> Cancelborrowedfortransact(BorrowedIssueDetails borrowed)
        {
            var borrow = await _context.BorrowedIssueDetails.Where(x => x.Id == borrowed.Id)
                                                             .FirstOrDefaultAsync();


            borrow.IsActive = false;
            borrow.Remarks = borrowed.Remarks;

            return true;

        }


        // ============================================================ Customer Returned ====================================================================

      

        public async Task<bool> AddBorrowConsume(BorrowedConsume consumes)
        {
            await _context.BorrowedConsumes.AddAsync(consumes);

            return true;
        }

        public async Task<IReadOnlyList<DTOGetItemForReturned>> GetItemForReturned(int id)
        {
            var Consume = _context.BorrowedConsumes.Where(x => x.IsActive == true)
                                                    .GroupBy(x => new
                                                    {
                                                        x.ItemCode,
                                                        x.BorrowedItemPkey,

                                                    }).Select(x => new DTOGetItemForReturned
                                                    {
                                                        BorrowedItemPkey = x.Key.BorrowedItemPkey,
                                                        ItemCode = x.Key.ItemCode,
                                                        ConsumedQuantity = x.Sum(x => x.Consume),

                                                    });

            var Borrowed = _context.BorrowedIssueDetails
            .GroupJoin(Consume, borrowed => borrowed.Id, consume => consume.BorrowedItemPkey, (borrowed, consume) => new { borrowed, consume })
            .SelectMany(x => x.consume.DefaultIfEmpty(), (x, consume) => new { x.borrowed, consume })
            .GroupJoin(_context.BorrowedIssues, borrowed => borrowed.borrowed.BorrowedPKey, issue => issue.Id, (borrowed, issue) => new { borrowed, issue })
            .SelectMany(x => x.issue.DefaultIfEmpty(), (x, issue) => new { x.borrowed, issue })
            .Where(x => x.borrowed.borrowed.IsActive == true && x.borrowed.borrowed.IsApproved == true && x.borrowed.borrowed.IsReturned != true  && x.borrowed.borrowed.BorrowedPKey == id)
            .GroupBy(x => new
            {
                x.borrowed.borrowed.Id,
                x.borrowed.borrowed.BorrowedPKey,
                x.issue.CustomerCode,
                x.issue.CustomerName,
                x.issue.EmpId,
                x.issue.FullName,
                x.issue.PreparedDate,
                x.borrowed.borrowed.ItemCode,
                x.borrowed.borrowed.ItemDescription,
                x.borrowed.borrowed.Uom,
                x.borrowed.borrowed.Quantity,
                x.borrowed.borrowed.IsReturned,
                Consumed = x.borrowed.consume.ConsumedQuantity != null ? x.borrowed.consume.ConsumedQuantity : 0,


            }).Select(x => new DTOGetItemForReturned
            {

                Id = x.Key.Id,
                BorrowedPKey = x.Key.BorrowedPKey,
                CustomerCode = x.Key.CustomerCode,
                CustomerName = x.Key.CustomerName,
                EmpId = x.Key.EmpId,
                FullName = x.Key.FullName,
                BorrowedDate = x.Key.PreparedDate.ToString(),
                ItemCode = x.Key.ItemCode,
                ItemDescription = x.Key.ItemDescription,
                Uom = x.Key.Uom,
                BorrowedQuantity = x.Key.Quantity,
                ConsumedQuantity = x.Key.Consumed,
                IsReturned = x.Key.IsReturned,
                RemainingQuantity = Math.Max(0, x.Key.Quantity - x.Key.Consumed),
                UnitCost = x.Sum(x => x.borrowed.borrowed.UnitPrice)

            });


            return await Borrowed.ToListAsync();
        }

        public async Task<bool> EditReturnQuantity(BorrowedConsume consumes)
        {

            var borrowed = await _context.BorrowedIssueDetails
                           .FirstOrDefaultAsync(x => x.Id == consumes.BorrowedItemPkey);

            var remainingItems = await GetItemForReturned(borrowed.BorrowedPKey);

            var remainingItem = remainingItems.FirstOrDefault(item => item.Id == consumes.BorrowedItemPkey);

            if (remainingItem != null && consumes.Consume <= remainingItem.RemainingQuantity && consumes.Consume >= 0)
            {
                consumes.IsActive = true;
                await _context.BorrowedConsumes.AddAsync(consumes);
                return true;
            }

            return false;
        }

        public async Task<IReadOnlyList<DtoGetConsumedItem>> GetConsumedItem(int id)
        {

            var consumed = _context.BorrowedConsumes.Where(x => x.IsActive == true)
                                                          .GroupBy(x => new
                                                          {
                                                              x.Id,
                                                              x.ItemCode,
                                                              x.Consume,
                                                              x.BorrowedItemPkey,
                                                              x.CompanyCode,
                                                              x.CompanyName,
                                                              x.EmpId,
                                                              x.FullName,
                                                              x.DepartmentCode,
                                                              x.DepartmentName,
                                                              x.LocationCode,
                                                              x.LocationName,
                                                              x.AccountCode,
                                                              x.AccountTitles,
                                                              x.ReportNumber,

                                                              x.IsActive
                                                          })
                                                          .Select(x => new DtoGetConsumedItem
                                                          {

                                                              Id = x.Key.Id,
                                                              ItemCode = x.Key.ItemCode,

                                                              ConsumedQuantity = x.Key.Consume != null ? x.Key.Consume : 0,
                                                              BorrowedItemPKey = x.Key.BorrowedItemPkey,

                                                              CompanyCode = x.Key.CompanyCode,
                                                              CompanyName = x.Key.CompanyName,

                                                              DepartmentCode = x.Key.DepartmentCode,
                                                              DepartmentName = x.Key.DepartmentName,
                                                              LocationCode = x.Key.LocationCode,
                                                              LocationName = x.Key.LocationName,
                                                              AccountCode = x.Key.AccountCode,
                                                              AccountTitles = x.Key.AccountTitles,
                                                              FullName = x.Key.FullName,
                                                              EmpId = x.Key.EmpId,
                                                              IsActive = x.Key.IsActive,
                                                              ReportNumber = x.Key.ReportNumber



                                                          });

            var borrowedconsume = _context.BorrowedIssueDetails
                                                               .GroupJoin(consumed, borrow => borrow.Id, consume => consume.BorrowedItemPKey, (borrow, consume) => new { borrow, consume })
                                                               .SelectMany(x => x.consume.DefaultIfEmpty(), (x, consume) => new { x.borrow, consume })
                                                                 .Where(x => x.borrow.IsActive == true && x.borrow.IsApproved == true && x.borrow.IsReturned != true)
                                                               .GroupBy(x => new
                                                               {
                                                                   x.borrow.Id,
                                                                   x.borrow.ItemCode,
                                                                   x.borrow.ItemDescription,
                                                                   x.borrow.Uom,
                                                                   x.borrow.Quantity,

                                                               }).Select(x => new DtoGetConsumedItem
                                                               {
                                                                   BorrowedItemPKey = x.Key.Id,
                                                                   ItemCode = x.Key.ItemCode,
                                                                   ItemDescription = x.Key.ItemDescription,
                                                                   Uom = x.Key.Uom,
                                                                   BorrowedQuantity = x.Key.Quantity != null ? x.Key.Quantity : 0,
                                                                   ItemConsumedQuantity = x.Sum(x => x.consume.ConsumedQuantity != null ? x.consume.ConsumedQuantity : 0),

                                                               });


            var borrowed = borrowedconsume
                  .GroupJoin(consumed, borrow => borrow.BorrowedItemPKey, consume => consume.BorrowedItemPKey, (borrow, consume) => new { borrow, consume })
                  .SelectMany(x => x.consume.DefaultIfEmpty(), (x, consume) => new { x.borrow, consume })
                  .Where(x => x.consume.BorrowedItemPKey == id)
                  .GroupBy(x => new
                  {
                      x.consume.Id,
                      x.borrow.BorrowedItemPKey,
                      x.borrow.ItemCode,
                      x.borrow.ItemDescription,
                      x.borrow.BorrowedQuantity,
                      x.borrow.ItemConsumedQuantity,
                      x.borrow.Uom,
                      x.consume.CompanyCode,
                      x.consume.CompanyName,
                      x.consume.DepartmentCode,
                      x.consume.DepartmentName,
                      x.consume.LocationCode,
                      x.consume.LocationName,
                      x.consume.AccountCode,
                      x.consume.AccountTitles,
                      x.consume.FullName,
                      x.consume.EmpId,
                      x.consume.ReportNumber
                  })
                  .Select(x => new DtoGetConsumedItem
                  {
                      Id = x.Key.Id,
                      BorrowedItemPKey = x.Key.BorrowedItemPKey,
                      ItemCode = x.Key.ItemCode,
                      ItemDescription = x.Key.ItemDescription,
                      Uom = x.Key.Uom,
                      BorrowedQuantity = x.Key.BorrowedQuantity,
                      ItemConsumedQuantity = x.Key.ItemConsumedQuantity,
                      ConsumedQuantity = x.Sum(x => x.consume.ConsumedQuantity != null ? x.consume.ConsumedQuantity : 0),
                      ReturnedQuantity = x.Key.BorrowedQuantity - x.Key.ItemConsumedQuantity,
                      CompanyCode = x.Key.CompanyCode,
                      CompanyName = x.Key.CompanyName,
                      DepartmentCode = x.Key.DepartmentCode,
                      DepartmentName = x.Key.DepartmentName,
                      LocationCode = x.Key.LocationCode,
                      LocationName = x.Key.LocationName,
                      AccountCode = x.Key.AccountCode,
                      AccountTitles = x.Key.AccountTitles,
                      FullName = x.Key.FullName,
                      EmpId = x.Key.EmpId,
                      ReportNumber = x.Key.ReportNumber

                  });

            return await borrowed.ToListAsync();
        }

        public async Task<bool> CancelIssuePerItemCode(BorrowedConsume consumes)
        {

            var items = await _context.BorrowedConsumes.Where(x => x.Id == consumes.Id)
                                                           .Where(x => x.IsActive == true)
                                                           .FirstOrDefaultAsync();


            items.IsActive = false;

             _context.BorrowedConsumes.Remove(items);

            return true;

        }


        public async Task<bool> EditConsumeQuantity(BorrowedConsume consumes)
        {

            var borrowedconsume = await _context.BorrowedConsumes.Where(x => x.Id == consumes.Id && x.IsActive == true)
                                                           .FirstOrDefaultAsync();

            var borrowed = await _context.BorrowedIssueDetails
                                       .FirstOrDefaultAsync(x => x.Id == consumes.BorrowedItemPkey);

            var remainingItems = await GetItemForReturned(borrowed.BorrowedPKey); 
            var remainingItem = remainingItems.FirstOrDefault(item => item.Id == consumes.BorrowedItemPkey);



          

            if (remainingItem != null && borrowedconsume.Consume + remainingItem.RemainingQuantity >= consumes.Consume && consumes.Consume >= 0)
            {

                borrowedconsume.Consume = consumes.Consume;
                borrowedconsume.DepartmentCode = consumes.DepartmentCode;
                borrowedconsume.DepartmentName = consumes.DepartmentName;
                borrowedconsume.CompanyCode = consumes.CompanyCode;
                borrowedconsume.LocationCode = consumes.LocationCode;
                borrowedconsume.LocationName = consumes.LocationName;
                borrowedconsume.AccountCode = consumes.AccountCode;
                borrowedconsume.AccountTitles = consumes.AccountTitles;
                borrowedconsume.EmpId = consumes.EmpId;
                borrowedconsume.FullName = consumes.FullName;
                borrowedconsume.ReportNumber = consumes.ReportNumber;
                return true;

            }

            return false;

        }

        public async Task<bool> ResetConsumePerItemCode(BorrowedConsume consumes)
        {
           var reset = await _context.BorrowedConsumes.Where(x => x.BorrowedItemPkey == consumes.BorrowedItemPkey && x.IsActive == true)
                                                      .ToListAsync();

            foreach(var item in reset)
            {
                item.IsActive = false;

                _context.BorrowedConsumes.Remove(item);
            }

          

            return true;
        }


        public async Task<bool> CancelAllConsumeItem(BorrowedConsume consume)
        {

           
            var consumed = await _context.BorrowedConsumes.Where(x => x.BorrowedPkey == consume.BorrowedPkey)
                                                         .ToListAsync();

            
            foreach(var items in consumed)
            {
                items.IsActive = false;

                _context.BorrowedConsumes.Remove(items);
            }

            return true;

        }

        public async Task<bool> SaveReturnedQuantity(BorrowedIssueDetails borrowed)
        {
            var returned = await _context.BorrowedIssues
                .Where(x => x.Id == borrowed.BorrowedPKey)
                .FirstOrDefaultAsync();


            var returnedDetails = await _context.BorrowedIssueDetails
                .Where(x => x.BorrowedPKey == borrowed.BorrowedPKey)
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

            return true;
        }


        public async Task<PagedList<DtoGetAllReturnedItem>> GetAllReturnedItem(UserParams userParams, bool status, int empid)
        {

            var employee = _context.Users.Where(x => x.Id == empid)
                                     .FirstOrDefault();


            var consumed = _context.BorrowedConsumes.Where(x => x.IsActive == true).GroupBy(x => new
            {
                x.BorrowedItemPkey,

            }).Select(x => new DtoGetConsumedItem
            {
                BorrowedItemPKey = x.Key.BorrowedItemPkey,
                ConsumedQuantity = x.Sum(x => x.Consume)
                

            });



            var borrowed = _context.BorrowedIssueDetails.Where(x => x.IsReturned == true && x.IsActive == true)
                             .GroupJoin(consumed, borrow => borrow.Id, consume => consume.BorrowedItemPKey, (borrow, consume) => new { borrow, consume })
                             .SelectMany(x => x.consume.DefaultIfEmpty(), (x, consume) => new { x.borrow, consume })
                              .GroupBy(x => new
                              {

                                  x.borrow.BorrowedPKey,
                                  x.borrow.PreparedBy,
                                  x.borrow.ReturnedDate,
                                  x.borrow.BorrowedDate,
                                  x.borrow.IsApprovedReturnedDate,
                                  x.borrow.ReturnBy,
      
                                

                              }).Select(x => new DtoGetAllReturnedItem
                              {

                                  Id = x.Key.BorrowedPKey,
                                  PreparedBy = x.Key.PreparedBy,
                                  ReturnedDate = x.Key.ReturnedDate.ToString(),
                                  ApproveReturnDate = x.Key.IsApprovedReturnedDate.ToString(),
                                  ReturnBy = x.Key.ReturnBy,
                                  TotalBorrowedQuantity = x.Sum(x => x.borrow.Quantity),
                                  ConsumedQuantity = x.Sum(x => x.consume.ConsumedQuantity != null ? x.consume.ConsumedQuantity : 0),
                                  BorrowedDate = x.Key.BorrowedDate.ToString(),


                              });



            var BorrowIssue = _context.BorrowedIssues
                .GroupJoin(borrowed, borrowissue => borrowissue.Id, borrowdetail => borrowdetail.Id, (borrowissue, borrowdetail) => new { borrowissue, borrowdetail })
                .SelectMany(x => x.borrowdetail.DefaultIfEmpty(), (x, borrowdetail) => new { x.borrowissue, borrowdetail })
                .Where(x => x.borrowissue.PreparedBy == employee.FullName)
                .Where(x => x.borrowissue.IsActive == true && x.borrowissue.IsReturned == true)
                .OrderByDescending(x => x.borrowissue.IsApprovedReturned)
                .GroupBy(x => new
                {

                    x.borrowissue.Id,
                    x.borrowissue.CustomerCode,
                    x.borrowissue.CustomerName,
                    x.borrowissue.EmpId,
                    x.borrowissue.FullName,
                    x.borrowissue.PreparedBy,
                    x.borrowdetail.ReturnedDate,
                    x.borrowdetail.BorrowedDate,
                    x.borrowdetail.ReturnBy,
                    x.borrowissue.IsApprovedReturned,
                    x.borrowissue.IsApprovedReturnedDate,
                    x.borrowissue.IsApprovedDate,
                    x.borrowissue.StatusApproved,
                    x.borrowdetail.ConsumedQuantity,
                    x.borrowdetail.TotalBorrowedQuantity,
                    x.borrowissue.IsActive,
                    x.borrowissue.Details


                    

                }).Select(x => new DtoGetAllReturnedItem
                {

                    Id = x.Key.Id,
                    CustomerCode = x.Key.CustomerCode,
                    CustomerName = x.Key.CustomerName,
                    EmpId = x.Key.EmpId,
                    FullName = x.Key.FullName,
                    PreparedBy = x.Key.PreparedBy,
                    ReturnedDate = x.Key.ReturnedDate,
                    BorrowedDate = x.Key.BorrowedDate,
                    ReturnBy = x.Key.ReturnBy,
                    TotalBorrowedQuantity = x.Key.TotalBorrowedQuantity,
                    ConsumedQuantity = x.Key.ConsumedQuantity,
                    ReturnedBorrow = x.Key.TotalBorrowedQuantity - x.Key.ConsumedQuantity,
                    AgingDays = x.Key.IsApprovedReturnedDate != null ? 
                    EF.Functions.DateDiffDay(x.Key.IsApprovedDate.Value, x.Key.IsApprovedReturnedDate.Value) 
                    : EF.Functions.DateDiffDay(x.Key.IsApprovedDate.Value, DateTime.Now),

                    IsApproveReturn = x.Key.IsApprovedReturned ?? false,
                    StatusApprove = x.Key.StatusApproved,
                    IsActive = x.Key.IsActive,
                    Details = x.Key.Details 


                }).Where(x => x.IsApproveReturn == status);

            return await PagedList<DtoGetAllReturnedItem>.CreateAsync(BorrowIssue, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<PagedList<DtoGetAllReturnedItem>> GetAllReturnedItemOrig(UserParams userParams, string search, bool status, int empid)
        {

            var employee = _context.Users.Where(x => x.Id == empid)
                                     .FirstOrDefault();

            var consumed = _context.BorrowedConsumes.Where(x => x.IsActive == true).GroupBy(x => new
            {
                x.BorrowedItemPkey,

            }).Select(x => new DtoGetConsumedItem
            {
                BorrowedItemPKey = x.Key.BorrowedItemPkey,
                ConsumedQuantity = x.Sum(x => x.Consume)

            });


            var borrowed = _context.BorrowedIssueDetails.Where(x => x.IsReturned == true && x.IsActive == true)
                             .GroupJoin(consumed, borrow => borrow.Id, consume => consume.BorrowedItemPKey, (borrow, consume) => new { borrow, consume })
                             .SelectMany(x => x.consume.DefaultIfEmpty(), (x, consume) => new { x.borrow, consume })
                              .GroupBy(x => new
                              {

                                  x.borrow.BorrowedPKey,
                                  x.borrow.PreparedBy,
                                  x.borrow.ReturnedDate,
                                  x.borrow.BorrowedDate,
                                  x.borrow.IsApprovedReturnedDate,
                                  x.borrow.ReturnBy,

                              }).Select(x => new DtoGetAllReturnedItem
                              {

                                  Id = x.Key.BorrowedPKey,
                                  PreparedBy = x.Key.PreparedBy,
                                  ReturnedDate = x.Key.ReturnedDate.ToString(),
                                  BorrowedDate = x.Key.BorrowedDate.ToString(),
                                  ApproveReturnDate = x.Key.IsApprovedReturnedDate.ToString(),
                                  ReturnBy = x.Key.ReturnBy,
                                  TotalBorrowedQuantity = x.Sum(x => x.borrow.Quantity),
                                  ConsumedQuantity = x.Sum(x => x.consume.ConsumedQuantity != null ? x.consume.ConsumedQuantity : 0),


                              });



            var BorrowIssue = _context.BorrowedIssues
                .GroupJoin(borrowed, borrowissue => borrowissue.Id, borrowdetail => borrowdetail.Id, (borrowissue, borrowdetail) => new { borrowissue, borrowdetail })
                .SelectMany(x => x.borrowdetail.DefaultIfEmpty(), (x, borrowdetail) => new { x.borrowissue, borrowdetail })
                .Where(x => x.borrowissue.PreparedBy == employee.FullName)
                .Where(x => x.borrowissue.IsActive == true && x.borrowissue.IsReturned == true)
                .OrderByDescending(x => x.borrowissue.IsApprovedReturned)
                .GroupBy(x => new
                {

                    x.borrowdetail.Id,
                    x.borrowissue.CustomerCode,
                    x.borrowissue.CustomerName,
                    x.borrowissue.EmpId,
                    x.borrowissue.FullName,
                    x.borrowissue.PreparedBy,
                    x.borrowissue.IsApprovedDate,
                    x.borrowdetail.ReturnedDate,
                    x.borrowdetail.BorrowedDate,
                    x.borrowissue.ReturnBy,
                    x.borrowissue.IsApprovedReturned,
                    x.borrowissue.StatusApproved,
                    x.borrowissue.IsApprovedReturnedDate,
                    x.borrowdetail.ConsumedQuantity,
                    x.borrowdetail.TotalBorrowedQuantity,
                    x.borrowissue.IsActive,
                    x.borrowissue.Details




                }).Select(x => new DtoGetAllReturnedItem
                {
                    Id = x.Key.Id,
                    CustomerCode = x.Key.CustomerCode,
                    CustomerName = x.Key.CustomerName,
                    EmpId = x.Key.EmpId,
                    FullName = x.Key.FullName,
                    PreparedBy = x.Key.PreparedBy,
                    ReturnedDate = x.Key.ReturnedDate,
                    BorrowedDate = x.Key.BorrowedDate,
                    ReturnBy = x.Key.ReturnBy,
                    TotalBorrowedQuantity = x.Key.TotalBorrowedQuantity,
                    ConsumedQuantity = x.Key.ConsumedQuantity,
                    ReturnedBorrow = x.Key.TotalBorrowedQuantity - x.Key.ConsumedQuantity,
                    AgingDays = x.Key.IsApprovedReturnedDate != null ?
                    EF.Functions.DateDiffDay(x.Key.IsApprovedDate.Value, x.Key.IsApprovedReturnedDate.Value)
                    : EF.Functions.DateDiffDay(x.Key.IsApprovedDate.Value, DateTime.Now),
                    IsApproveReturn = x.Key.IsApprovedReturned ?? false,
                    StatusApprove = x.Key.StatusApproved,
                    IsActive = x.Key.IsActive,
                    Details = x.Key.Details


                }).Where(x => x.IsApproveReturn == status)
                .Where(x => (Convert.ToString(x.Id)).ToLower().Contains(search.Trim().ToLower()));


            return await PagedList<DtoGetAllReturnedItem>.CreateAsync(BorrowIssue, userParams.PageNumber, userParams.PageSize);

        }



        public async Task<bool> CancelReturnItem(BorrowedIssueDetails borrowed)
        {

            var issue = await _context.BorrowedIssues.Where(x => x.Id == borrowed.BorrowedPKey)
                                                      .FirstOrDefaultAsync();

            var borrow = await _context.BorrowedIssueDetails.Where(x => x.BorrowedPKey == borrowed.BorrowedPKey)
                                                       .ToListAsync();

            var consumeItem = await _context.BorrowedConsumes
                .Where(x => x.BorrowedPkey == borrowed.BorrowedPKey).ToListAsync();


            foreach(var items in borrow)
            {

                items.IsReturned = null;
                items.ReturnedDate = null;
                items.IsApprovedReturned = null;
            }

            foreach(var consume in consumeItem) 
            {
                consume.IsActive = false;
                consume.IsApproveReturn = null;

            }

            issue.IsReturned = false;
            issue.IsApprovedReturned = null;
            issue.StatusApproved = "Borrow Approved";

            return true;

        }


        public async Task<IReadOnlyList<DtoViewBorrewedReturnedDetails>> ViewBorrewedReturnedDetails(int id)
        {


            var ConsumedBorrowed = _context.BorrowedConsumes.Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.BorrowedItemPkey,
                    x.ItemCode,
                   

                }).Select(x => new DtoGetConsumedItem
                {
                    BorrowedItemPKey  = x.Key.BorrowedItemPkey,
                    ItemCode = x.Key.ItemCode,
                    ConsumedQuantity = x.Sum(x => x.Consume != null ? x.Consume : 0),
                  

                });


            var borrowDetails = _context.BorrowedIssueDetails.Where(x => x.IsActive == true && x.IsReturned == true )
                                    .GroupJoin(ConsumedBorrowed, borrow => borrow.Id , consume => consume.BorrowedItemPKey, (borrow , consume) => new {borrow , consume})
                                    .SelectMany(x => x.consume.DefaultIfEmpty() , (x , consume) => new {x.borrow , consume})
                                              .GroupBy(x => new 
                                              {
                                                  x.borrow.Id,
                                                  x.borrow.BorrowedPKey,
                                                  x.borrow.ItemCode,
                                                  x.borrow.ItemDescription,
                                                  x.borrow.Uom,
                                                  x.borrow.ReturnedDate,
                                                  x.borrow.BorrowedDate
                                                  

                                              }).Select(x => new DtoViewBorrewedReturnedDetails
                                              {
                                                  Id = x.Key.Id,
                                                  BorrowedPKey = x.Key.BorrowedPKey,
                                                  ItemCode = x.Key.ItemCode,
                                                  ItemDescription = x.Key.ItemDescription,
                                                  Uom = x.Key.Uom,
                                                  BorrowedQuantity = x.Sum(x => x.borrow.Quantity),
                                                  Consume = x.Sum(x => x.consume.ConsumedQuantity != null ? x.consume.ConsumedQuantity : 0),
                                                  ReturnedDate = x.Key.ReturnedDate.ToString(),
                                                  UnitCost = x.Sum(x => x.borrow.UnitPrice != null ? x.borrow.UnitPrice : 0),
                                                  BorrowedDate = x.Key.BorrowedDate.ToString(),
                                                  

                                              });


            var borrow = _context.BorrowedIssues
                .GroupJoin(borrowDetails, issue => issue.Id, borrow => borrow.BorrowedPKey, (issue, borrow) => new { issue, borrow })
                .SelectMany(x => x.borrow.DefaultIfEmpty(), (x, borrow) => new { x.issue, borrow })
              .Where(x => x.borrow.BorrowedPKey == id)
              .Where(x => x.issue.IsReturned == true)
              .Where(x => x.issue.IsActive == true)
              .Select(x => new DtoViewBorrewedReturnedDetails
              {
                  BorrowedPKey = x.borrow.BorrowedPKey,
                  Id = x.borrow.Id,
                  CustomerCode = x.issue.CustomerCode,
                  Customer = x.issue.CustomerName,
                  EmpId = x.issue.EmpId,
                  FullName = x.issue.FullName,
                  ItemCode = x.borrow.ItemCode,
                  ItemDescription = x.borrow.ItemDescription,
                  Uom = x.borrow.Uom,
                  BorrowedQuantity = x.borrow.BorrowedQuantity,
                  ReturnQuantity = x.borrow.BorrowedQuantity - x.borrow.Consume,
                  Consume = x.borrow.Consume,
                  ReturnedDate = x.borrow.ReturnedDate,
                  PreparedBy = x.issue.PreparedBy,
                  Remarks = x.issue.Remarks,
                  BorrowedDate = x.borrow.BorrowedDate,
                  TransactionDate = x.issue.TransactionDate.ToString(),
                  Reason = x.issue.Reason,
                  UnitCost = x.borrow.UnitCost,
                  TotalCost = x.borrow.UnitCost * x.borrow.Consume,
                  Details = x.issue.Details
              });


            return await borrow.ToListAsync();
        }

        public async Task<IReadOnlyList<DtoViewConsumeForReturn>> ViewConsumeForReturn(int id)
        {
            var Consumed = _context.BorrowedConsumes.Where(x => x.BorrowedItemPkey == id && x.IsActive == true)
                .Select(x => new DtoViewConsumeForReturn
                {

                    BorrowedItemPKey = x.BorrowedItemPkey,
                    ItemCode = x.ItemCode,
                    ItemDescription = x.ItemDescription,
                    ReportNumber = x.ReportNumber,
                    Uom = x.Uom,
                    Consume = x.Consume,
                    CompanyCode = x.CompanyCode,
                    CompanyName = x.CompanyName,
                    DepartmentCode = x.DepartmentCode,
                    DepartmentName = x.DepartmentName,
                    LocationCode = x.LocationCode,
                    LocationName = x.LocationName,
                    AccountCode = x.AccountCode,
                    AccountTitles = x.AccountTitles,
                    FullName = x.FullName,
                    EmpId = x.EmpId

                });

            return await Consumed.ToListAsync();
        }

        //================================================= Customer Reject ============================================================================

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
                                            EmpId = x.EmpId,
                                            FullName = x.FullName,
                                            TotalQuantity = x.TotalQuantity,
                                            RejectDate = x.IsRejectDate.ToString(),
                                            Remarks = x.Reason,
                                            RejectBy = x.RejectBy,
                                            TransactionDate = x.TransactionDate.ToString("MM/dd/yyyy"),
                                

                                        }).OrderByDescending(x => x.RejectDate);

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
                                     EmpId = x.EmpId,
                                     FullName = x.FullName,
                                     TotalQuantity = x.TotalQuantity,
                                     RejectDate = x.IsRejectDate.ToString(),
                                     Remarks = x.Reason,
                                     RejectBy = x.RejectBy,
                                     TransactionDate = x.TransactionDate.ToString("MM/dd/yyyy")

                                 }).OrderByDescending(x => x.RejectDate).Where(x => (Convert.ToString(x.BorrowedPKey)).ToLower().Contains(search.Trim().ToLower()));


            return await PagedList<GetRejectBorrowedPagination>.CreateAsync(borrow, userParams.PageNumber, userParams.PageSize);
        }




        // ================================================ Approver ====================================================================================


        public async Task<PagedList<GetAllBorrowedReceiptWithPaginationDto>> GetAllForApprovalBorrowedWithPagination(UserParams userParams, bool status)
        {

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
                .Where(x => x.issues.IsActive == true)
                .Where(x => x.issues.IsApproved == status && x.issues.IsReturned != true && x.issues.IsReject == null)
                .GroupBy(x => new
                {
                    x.borrowed.BorrowedPKey,
                    x.issues.CustomerCode,
                    x.issues.CustomerName,
                    x.issues.EmpId,
                    x.issues.FullName,
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
                    x.issues.Details

                }).Select(x => new GetAllBorrowedReceiptWithPaginationDto
                {
                    BorrowedPKey = x.Key.BorrowedPKey,
                    CustomerName = x.Key.CustomerName,
                    CustomerCode = x.Key.CustomerCode,
                    EmpId = x.Key.EmpId,
                    FullName = x.Key.FullName,
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
                    StatusApprove = x.Key.StatusApproved,
                    Details = x.Key.Details

                }).OrderByDescending(x => x.BorrowedDate);



            return await PagedList<GetAllBorrowedReceiptWithPaginationDto>.CreateAsync(borrow, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<PagedList<GetAllBorrowedReceiptWithPaginationDto>> GetAllForApprovalBorrowedWithPaginationOrig(UserParams userParams, string search, bool status)
        {



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
                .Where(x => x.issues.IsActive == true)
                .Where(x => x.issues.IsApproved == status && x.issues.IsReturned != true && x.issues.IsReject == null)
                .GroupBy(x => new
                {
                    x.borrowed.BorrowedPKey,
                    x.issues.CustomerCode,
                    x.issues.CustomerName,
                    x.issues.EmpId,
                    x.issues.FullName,
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
                    x.issues.Details

                }).Select(x => new GetAllBorrowedReceiptWithPaginationDto
                {
                    BorrowedPKey = x.Key.BorrowedPKey,
                    CustomerName = x.Key.CustomerName,
                    CustomerCode = x.Key.CustomerCode,
                    EmpId = x.Key.EmpId,
                    FullName = x.Key.FullName,
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
                    StatusApprove = x.Key.StatusApproved,
                    Details = x.Key.Details

                }).OrderByDescending(x => x.BorrowedDate).Where(x => (Convert.ToString(x.BorrowedPKey)).ToLower().Contains(search.Trim().ToLower())
                                                    || (Convert.ToString(x.CustomerCode)).ToLower().Contains(search.Trim().ToLower())
                                                      || (Convert.ToString(x.CustomerName)).ToLower().Contains(search.Trim().ToLower()));


            return await PagedList<GetAllBorrowedReceiptWithPaginationDto>.CreateAsync(borrow, userParams.PageNumber, userParams.PageSize);
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
            issue.StatusApproved = "Borrow Approved";


            foreach (var item in borrow)
            {

                item.IsActive = true;
                item.IsApproved = true;
                item.IsApprovedDate = DateTime.Now;

            }

            return true;

        }


        public async Task<IReadOnlyList<GetAllDetailsInBorrowedIssueDto>> GetAllForApprovalDetailsInBorrowed(int id)
        {

            var issueBorrowed = _context.BorrowedIssues.Where(x => x.IsActive == true)
                                                       .Select(x => new DtoViewBorrewedReturnedDetails
                                                       {
                                                           Id = x.Id,
                                                           Customer = x.CustomerName,
                                                           CustomerCode = x.CustomerCode,
                                                           EmpId = x.EmpId,
                                                           FullName = x.FullName,
                                                           TransactionDate = x.TransactionDate.ToString(),
                                                           Details = x.Details

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
              .Where(x => x.borrowed.IsReturned != true)
              .Where(x => x.borrowed.IsReject == null)
              .Where(x => x.borrowed.IsActive == true)

              .Select(x => new GetAllDetailsInBorrowedIssueDto
              {

                  Id = x.borrowed.Id,
                  WarehouseId = x.borrowed.WarehouseId,
                  BorrowedPKey = x.borrowed.BorrowedPKey,
                  Customer = x.issues.Customer,
                  CustomerCode = x.issues.CustomerCode,
                
                  EmpId = x.issues.EmpId,
                  FullName = x.issues.FullName,
                  Uom = x.borrowed.Uom,
                  PreparedDate = x.borrowed.PreparedDate.ToString(),
                  ItemCode = x.borrowed.ItemCode,
                  ItemDescription = x.borrowed.ItemDescription,
                  Quantity = x.borrowed.Quantity,
                  Remarks = x.borrowed.Remarks,
                  PreparedBy = x.borrowed.PreparedBy,
                  UnitCost = x.borrowed.UnitPrice,
                  TotalCost = x.borrowed.UnitPrice * x.borrowed.Quantity,
                  Details = x.issues.Details
                  

              });

            return await warehouse.ToListAsync();
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

            }

            return true;

        }

        //================================================================ Approver Reject ============================================================================================

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
                                                 EmpId = x.EmpId,
                                                 FullName = x.FullName,
                                                 TotalQuantity = x.TotalQuantity,
                                                 RejectDate = x.IsRejectDate.ToString(),
                                                 Remarks = x.Reason,
                                                 RejectBy = x.RejectBy,
                                                 TransactionDate = x.TransactionDate.ToString("MM/dd/yyyy")

                                             }).OrderByDescending(x => x.RejectDate);

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
                                               RejectDate = x.IsRejectDate.ToString(),
                                               Remarks = x.Remarks,
                                               RejectBy = x.RejectBy,
                                               TransactionDate = x.TransactionDate.ToString("MM/dd/yyyy")

                                           }).OrderByDescending(x  => x.RejectDate).Where(x => (Convert.ToString(x.BorrowedPKey)).ToLower().Contains(search.Trim().ToLower())
                                                    || (Convert.ToString(x.CustomerCode)).ToLower().Contains(search.Trim().ToLower())
                                                      || (Convert.ToString(x.CustomerName)).ToLower().Contains(search.Trim().ToLower()));


            return await PagedList<GetRejectBorrowedPagination>.CreateAsync(borrow, userParams.PageNumber, userParams.PageSize);
        }



        // ============================================================ Approver for Returned =================================================================


        public async Task<PagedList<DtoGetAllReturnedItem>> GetAllForApproveReturnedItem(UserParams userParams, bool status)
        {

            var consumed = _context.BorrowedConsumes.Where(x => x.IsActive == true).GroupBy(x => new
            {
                x.BorrowedItemPkey,

            }).Select(x => new DtoGetConsumedItem
            {
                BorrowedItemPKey = x.Key.BorrowedItemPkey,
                ConsumedQuantity = x.Sum(x => x.Consume)

            });



            var borrowed = _context.BorrowedIssueDetails.Where(x => x.IsReturned == true && x.IsActive == true)
                             .GroupJoin(consumed, borrow => borrow.Id, consume => consume.BorrowedItemPKey, (borrow, consume) => new { borrow, consume })
                             .SelectMany(x => x.consume.DefaultIfEmpty(), (x, consume) => new { x.borrow, consume })
                              .GroupBy(x => new
                              {

                                  x.borrow.BorrowedPKey,
                                  x.borrow.PreparedBy,
                                  x.borrow.ReturnedDate,
                                  x.borrow.BorrowedDate,
                                  x.borrow.IsApprovedReturnedDate,
                                  x.borrow.ReturnBy,

                              }).Select(x => new DtoGetAllReturnedItem
                              {

                                  Id = x.Key.BorrowedPKey,
                                  PreparedBy = x.Key.PreparedBy,
                                  ReturnedDate = x.Key.ReturnedDate.ToString(),
                                  ApproveReturnDate = x.Key.IsApprovedReturnedDate.ToString(),
                                  BorrowedDate = x.Key.BorrowedDate.ToString(),
                                  ReturnBy = x.Key.ReturnBy,
                                  TotalBorrowedQuantity = x.Sum(x => x.borrow.Quantity),
                                  ConsumedQuantity = x.Sum(x => x.consume.ConsumedQuantity != null ? x.consume.ConsumedQuantity : 0),

                              });



            var BorrowIssue = _context.BorrowedIssues
                .GroupJoin(borrowed, borrowissue => borrowissue.Id, borrowdetail => borrowdetail.Id, (borrowissue, borrowdetail) => new { borrowissue, borrowdetail })
                .SelectMany(x => x.borrowdetail.DefaultIfEmpty(), (x, borrowdetail) => new { x.borrowissue, borrowdetail })
                .Where(x => x.borrowissue.IsActive == true && x.borrowissue.IsReturned == true)
                .OrderByDescending(x => x.borrowissue.IsApprovedReturned)
                .GroupBy(x => new
                {

                    x.borrowdetail.Id,
                    x.borrowissue.CustomerCode,
                    x.borrowissue.CustomerName,
                    x.borrowissue.PreparedBy,
                    x.borrowdetail.ReturnedDate,
                    x.borrowdetail.BorrowedDate,
                    x.borrowissue.ReturnBy,
                    x.borrowissue.IsApprovedReturned,
                    x.borrowissue.IsApprovedReturnedDate,
                    x.borrowissue.IsApprovedDate,
                    x.borrowissue.StatusApproved,
                    x.borrowdetail.ConsumedQuantity,
                    x.borrowdetail.TotalBorrowedQuantity,
                    x.borrowissue.IsActive,
                    x.borrowissue.Details




                }).Select(x => new DtoGetAllReturnedItem
                {
                    Id = x.Key.Id,
                    CustomerCode = x.Key.CustomerCode,
                    CustomerName = x.Key.CustomerName,
                    PreparedBy = x.Key.PreparedBy,
                    ReturnedDate = x.Key.ReturnedDate,
                    ReturnBy = x.Key.ReturnBy,
                    TotalBorrowedQuantity = x.Key.TotalBorrowedQuantity,
                    ConsumedQuantity = x.Key.ConsumedQuantity,
                    ReturnedBorrow = x.Key.TotalBorrowedQuantity - x.Key.ConsumedQuantity,
                    BorrowedDate = x.Key.BorrowedDate,
                    IsApproveReturn = x.Key.IsApprovedReturned ?? false,
                    StatusApprove = x.Key.StatusApproved,
                    IsActive = x.Key.IsActive,
                    AgingDays = x.Key.IsApprovedReturnedDate != null ?
                    EF.Functions.DateDiffDay(x.Key.IsApprovedDate.Value, x.Key.IsApprovedReturnedDate.Value)
                    : EF.Functions.DateDiffDay(x.Key.IsApprovedDate.Value, DateTime.Now),
                    Details = x.Key.Details


                }).OrderByDescending(x => x.ReturnedDate).Where(x => x.IsApproveReturn == status);



            return await PagedList<DtoGetAllReturnedItem>.CreateAsync(BorrowIssue, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<DtoGetAllReturnedItem>> GetAllForApproveReturnedItemOrig(UserParams userParams, string search, bool status)
        {

            var consumed = _context.BorrowedConsumes.Where(x => x.IsActive == true).GroupBy(x => new
            {
                x.BorrowedItemPkey,

            }).Select(x => new DtoGetConsumedItem
            {
                BorrowedItemPKey = x.Key.BorrowedItemPkey,
                ConsumedQuantity = x.Sum(x => x.Consume)

            });



            var borrowed = _context.BorrowedIssueDetails.Where(x => x.IsReturned == true && x.IsActive == true)
                             .GroupJoin(consumed, borrow => borrow.Id, consume => consume.BorrowedItemPKey, (borrow, consume) => new { borrow, consume })
                             .SelectMany(x => x.consume.DefaultIfEmpty(), (x, consume) => new { x.borrow, consume })
                              .GroupBy(x => new
                              {

                                  x.borrow.BorrowedPKey,
                                  x.borrow.PreparedBy,
                                  x.borrow.ReturnedDate,
                                  x.borrow.BorrowedDate,
                                  x.borrow.IsApprovedReturnedDate,
                                  x.borrow.ReturnBy,

                              }).Select(x => new DtoGetAllReturnedItem
                              {

                                  Id = x.Key.BorrowedPKey,
                                  PreparedBy = x.Key.PreparedBy,
                                  ReturnedDate = x.Key.ReturnedDate.ToString(),
                                  ApproveReturnDate = x.Key.IsApprovedReturnedDate.ToString(),
                                  ReturnBy = x.Key.ReturnBy,
                                  BorrowedDate = x.Key.BorrowedDate.ToString(),
                                  TotalBorrowedQuantity = x.Sum(x => x.borrow.Quantity),
                                  ConsumedQuantity = x.Sum(x => x.consume.ConsumedQuantity != null ? x.consume.ConsumedQuantity : 0),


                              });



            var BorrowIssue = _context.BorrowedIssues
                .GroupJoin(borrowed, borrowissue => borrowissue.Id, borrowdetail => borrowdetail.Id, (borrowissue, borrowdetail) => new { borrowissue, borrowdetail })
                .SelectMany(x => x.borrowdetail.DefaultIfEmpty(), (x, borrowdetail) => new { x.borrowissue, borrowdetail })
                .Where(x => x.borrowissue.IsActive == true && x.borrowissue.IsReturned == true)
                .OrderByDescending(x => x.borrowissue.IsApprovedReturned)
                .GroupBy(x => new
                {

                    x.borrowdetail.Id,
                    x.borrowissue.CustomerCode,
                    x.borrowissue.CustomerName,
                    x.borrowissue.EmpId,
                    x.borrowissue.FullName,
                    x.borrowissue.PreparedBy,
                    x.borrowdetail.ReturnedDate,
                    x.borrowdetail.BorrowedDate,
                    x.borrowissue.ReturnBy,
                    x.borrowissue.IsApprovedReturned,
                    x.borrowissue.IsApprovedDate,
                    x.borrowissue.IsApprovedReturnedDate,
                    x.borrowissue.StatusApproved,
                    x.borrowdetail.ConsumedQuantity,
                    x.borrowdetail.TotalBorrowedQuantity,
                    x.borrowissue.IsActive,
                    x.borrowissue.Details




                }).Select(x => new DtoGetAllReturnedItem
                {
                    Id = x.Key.Id,
                    CustomerCode = x.Key.CustomerCode,
                    CustomerName = x.Key.CustomerName,
                    EmpId = x.Key.EmpId,
                    FullName = x.Key.FullName,
                    PreparedBy = x.Key.PreparedBy,
                    ReturnedDate = x.Key.ReturnedDate,
                    ReturnBy = x.Key.ReturnBy,
                    TotalBorrowedQuantity = x.Key.TotalBorrowedQuantity,
                    ConsumedQuantity = x.Key.ConsumedQuantity,
                    ReturnedBorrow = x.Key.TotalBorrowedQuantity - x.Key.ConsumedQuantity,
                    IsApproveReturn = x.Key.IsApprovedReturned ?? false,
                    StatusApprove = x.Key.StatusApproved,
                    IsActive = x.Key.IsActive,
                    BorrowedDate = x.Key.BorrowedDate,
                    AgingDays = x.Key.IsApprovedReturnedDate != null ?
                    EF.Functions.DateDiffDay(x.Key.IsApprovedDate.Value, x.Key.IsApprovedReturnedDate.Value)
                    : EF.Functions.DateDiffDay(x.Key.IsApprovedDate.Value, DateTime.Now),
                    Details = x.Key.Details


                }).OrderByDescending(x => x.ReturnedDate).Where(x => x.IsApproveReturn == status).Where(x => (Convert.ToString(x.Id)).ToLower().Contains(search.Trim().ToLower())
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
            var borrowConsume = await _context.BorrowedConsumes.Where(x => x.BorrowedPkey == borrowed.Id).ToListAsync();


            foreach (var item in borrow)
            {

                item.IsReturned = null;
                item.ReturnedDate = null;
                item.IsApprovedReturned = null;
                item.IsReturned = false;
                
            }

            foreach (var consume in borrowConsume)
            {
                consume.IsActive = false;
                consume.IsApproveReturn = null;
            }

            issue.IsReturned = false;
            issue.IsApprovedReturned = false;
            issue.Reason = borrowed.Reason;

            return true;

        }
    }

}
