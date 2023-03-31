﻿using ELIXIRETD.DATA.CORE.INTERFACES.REPORTS_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORY_DTO.MRP;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORTS_REPOSITORY
{
    public class ReportsRepository : IReports
    {
        private readonly StoreContext _context;

    

        public ReportsRepository(StoreContext storeContext)
        {
            _context = storeContext;
        }

       

        public async Task<IReadOnlyList<DtoWarehouseReceivingReports>> WarehouseReceivingReports(string DateFrom, string DateTo)
        {

            var warehouse = _context.WarehouseReceived.Where(x => x.ReceivingDate >= DateTime.Parse(DateFrom) && x.ReceivingDate <= DateTime.Parse(DateTo))
                                                      .Where(x => x.IsActive == true)
                                                      .Select(x => new DtoWarehouseReceivingReports
                                                      {
                                                          WarehouseId = x.Id,
                                                          PoNumber = x.PoNumber,
                                                          ReceiveDate = x.ReceivingDate.ToString(),
                                                          ItemCode = x.ItemCode,
                                                          ItemDescrption = x.ItemDescription,
                                                          Uom = x.Uom,
                                                          Category = x.LotCategory,
                                                          Quantity = x.ActualGood,
                                                          TotalReject = x.TotalReject,
                                                          SupplierName = x.Supplier,
                                                          TransactionType = x.TransactionType,

                                                      });

            return await warehouse.ToListAsync();
                                               
        }

        public async Task<IReadOnlyList<DtoMoveOrderReports>> WarehouseMoveOrderReports(string DateFrom, string DateTo)
        {
            var orders = _context.MoveOrders
                        .Where(moveorder => moveorder.PreparedDate >= DateTime.Parse(DateFrom) && moveorder.PreparedDate <= DateTime.Parse(DateTo) && moveorder.IsActive == true)
                        .GroupJoin(_context.TransactOrder, moveorder => moveorder.OrderNo, transact => transact.OrderNo, (moveorder, transact) => new { moveorder, transact })
                        .SelectMany(x => x.transact.DefaultIfEmpty(), (x, transact) => new { x.moveorder, transact })
                         .Select(x => new DtoMoveOrderReports
                         {

                             MoveOrderId = x.moveorder.OrderNo,
                             CustomerCode = x.moveorder.Customercode,
                             CustomerName = x.moveorder.CustomerName,
                             ItemCode = x.moveorder.ItemCode,
                             ItemDescription = x.moveorder.ItemDescription, 
                             Uom = x.moveorder.Uom,
                             Category = x.moveorder.Category,
                             Quantity = x.moveorder.QuantityOrdered,
                             MoveOrderBy = x.moveorder.PreparedBy,
                             MoveOrderDate = x.moveorder.PreparedDate.ToString(),
                             TransactedBy = x.transact.PreparedBy,
                             TransactedDate = x.transact.PreparedDate.ToString(),

                         });

            return await orders.ToListAsync();

        }

        public async Task<IReadOnlyList<DtoMiscReports>> MiscReports(string DateFrom, string DateTo)
        {
            var receipts = (from receiptHeader in _context.MiscellaneousReceipts
                            join receipt in _context.WarehouseReceived
                            on receiptHeader.Id equals receipt.MiscellaneousReceiptId
                            into leftJ
                            from receipt in leftJ.DefaultIfEmpty()

                            where receipt.ReceivingDate >= DateTime.Parse(DateFrom) && receipt.ReceivingDate <= DateTime.Parse(DateTo) && receipt.IsActive == true && receipt.TransactionType == "MiscellaneousReceipt"

                            select new DtoMiscReports
                            {

                                ReceiptId = receiptHeader.Id,
                                SupplierCode = receiptHeader.SupplierCode,
                                SupplierName = receiptHeader.supplier,
                                Details = receiptHeader.Remarks,
                                ItemCode = receipt.ItemCode,
                                ItemDescription = receipt.ItemDescription,
                                Uom = receipt.Uom,
                                Category = receipt.LotCategory,
                                Quantity = receipt.ActualGood,
                                TransactBy = receiptHeader.PreparedBy,
                                TransactDate = receipt.ReceivingDate.ToString()


                            });

            return await receipts.ToListAsync();

        }

        public async Task<IReadOnlyList<DtoMiscIssue>> MiscIssue(string DateFrom, string DateTo)
        {
            var issues = _context.MiscellaneousIssues
                       .GroupJoin(_context.MiscellaneousIssueDetail, receipt => receipt.Id, issue => issue.IssuePKey, (receipt, issue) => new { receipt, issue })
                       .SelectMany(x => x.issue.DefaultIfEmpty(), (x, issue) => new { x.receipt, issue })
                       .Where(x => x.issue.PreparedDate >= DateTime.Parse(DateFrom) && x.issue.PreparedDate <= DateTime.Parse(DateTo) && x.issue.IsActive == true && x.issue.IsTransact == true)
                       .Select(x => new DtoMiscIssue
                       {

                           IssueId = x.receipt.Id,
                           CustomerCode = x.receipt.Customercode,
                           CustomerName = x.receipt.Customer,
                           Details = x.issue.Remarks,
                           ItemCode = x.issue !=null ? x.issue.ItemCode : null ,
                           ItemDescription = x.issue !=null ? x.issue.ItemDescription : null ,
                           Uom = x.issue != null    ? x.issue.Uom : null ,  
                           Quantity = x.issue != null ? x.issue.Quantity : 0,
                           TransactBy = x.receipt.PreparedBy ,
                           TransactDate = x.issue.PreparedDate.ToString() 

                       });


            return await issues.ToListAsync();
        }



        public async Task<IReadOnlyList<DtoBorrowedAndReturned>> ReturnBorrowedReports(string DateFrom, string DateTo)
        {

            var getReturned = _context.ReturnedBorroweds.Where(x => x.IsActive == true)
                                                           .Where(x => x.IsReturned == true)
                                                           .GroupBy(x => new
                                                           {
                                                               x.TotalbPkey,
                                                               x.ItemCode,

                                                           })
                                                           .Select(x => new
                                                           {
                                                               TotalbPkey = x.Key.TotalbPkey,
                                                               ItemCode = x.Key.ItemCode,
                                                               ReturnedQuantity = x.Sum(x => x.ReturnedQuantity != null ? x.ReturnedQuantity : 0)

                                                           });


            var getBorrowedReturn = _context.BorrowedIssueDetails
                              .Where(x => x.IsActive == true)
                              .GroupJoin(getReturned, borrowed => borrowed.BorrowedPKey, returned => returned.TotalbPkey, (borrowed, returned) => new { borrowed, returned })
                              .SelectMany(x => x.returned.DefaultIfEmpty(), (x, returned) => new { x.borrowed, returned })
                              .GroupBy(x => new
                              {
                                  x.borrowed.BorrowedPKey,
                                  x.borrowed.ItemCode,
                                  x.borrowed.ItemDescription,
                                  x.borrowed.Uom,


                              })

                              .Select(x => new 
                              {

                                  BorrowedPKey = x.Key.BorrowedPKey,
                                  ItemCode = x.Key.ItemCode,
                                  ItemDescription = x.Key.ItemDescription,
                                  Quantity = x.Sum(x => x.borrowed.Quantity != null ? x.borrowed.Quantity : 0),
                                  ReturnedQuantity = x.Sum(x => x.returned.ReturnedQuantity != null ? x.returned.ReturnedQuantity : 0),
                                  Uom = x.Key.Uom,
                                                               
                              });


            var Reports = _context.BorrowedIssues
                      .GroupJoin(getBorrowedReturn, Borrowedissue => Borrowedissue.Id, borrowedreturned => borrowedreturned.BorrowedPKey, (Borrowedissue, borrowedreturned) => new { Borrowedissue, borrowedreturned })
                      .SelectMany(x => x.borrowedreturned.DefaultIfEmpty(), (x, borrowedreturned) => new { x.Borrowedissue, borrowedreturned })
                      .Where(x => x.Borrowedissue.PreparedDate >= DateTime.Parse(DateFrom) && x.Borrowedissue.PreparedDate <= DateTime.Parse(DateTo) && x.Borrowedissue.IsActive == true)
                      .GroupBy(x => new
                      {
                          x.Borrowedissue.Id,
                          x.Borrowedissue.CustomerCode,
                          x.Borrowedissue.CustomerName,
                          x.borrowedreturned.ItemCode,
                          x.borrowedreturned.ItemDescription,
                          x.borrowedreturned.Uom,
                          x.Borrowedissue.PreparedBy,
                          x.Borrowedissue.PreparedDate,

                      }).Select(x => new DtoBorrowedAndReturned
                      {
                          BorrowedId = x.Key.Id,
                          CustomerCode = x.Key.CustomerCode,
                          CustomerName = x.Key.CustomerName,
                          ItemCode = x.Key.ItemCode,
                          ItemDescription = x.Key.ItemDescription,
                          Uom = x.Key.Uom,
                          BorrowedQuantity = x.Sum(x => x.borrowedreturned.Quantity != null ? x.borrowedreturned.Quantity : 0),
                          ReturnQuantity = x.Sum(x => x.borrowedreturned.ReturnedQuantity != null ? x.borrowedreturned.ReturnedQuantity : 0),
                          Consumes = x.Sum(x => x.borrowedreturned.Quantity != null ? x.borrowedreturned.Quantity : 0) - x.Sum(x => x.borrowedreturned.ReturnedQuantity != null ? x.borrowedreturned.ReturnedQuantity : 0),
                          TransactedBy = x.Key.PreparedBy,
                          BorrowedDate = x.Key.PreparedDate.ToString()

                      });

            return await Reports.ToListAsync();

        }

        public async Task<IReadOnlyList<DtoCancelledReports>> CancelledReports(string DateFrom, string DateTo)
        {
            var orders = _context.Orders.Where(x => x.OrderDate >= DateTime.Parse(DateFrom) && x.OrderDate <= DateTime.Parse(DateTo) && x.IsCancel == true && x.IsActive == false)
                                        .Select(x => new DtoCancelledReports
                                        {

                                            OrderId = x.Id,
                                            DateNeeded = x.DateNeeded.ToString(),
                                            DateOrdered = x.OrderDate.ToString(),
                                            CustomerCode = x.Customercode,
                                            CustomerName = x.CustomerName,
                                            ItemCode = x.ItemCode,
                                            ItemDescription = x.ItemdDescription,
                                            QuantityOrdered = x.QuantityOrdered,
                                            CancelledDate = x.CancelDate.ToString(),
                                            CancelledBy = x.IsCancelBy,
                                            Reason = x.Remarks


                                        });

            return await orders.ToListAsync();
        }

        public async Task<IReadOnlyList<DtoInventoryMovement>> InventoryMovementReports(string DateFrom, string DateTo , string PlusOne)
        {
            var DateToday = DateTime.Parse(DateTime.Now.ToString());

          


            var getWarehouseStock = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                              .GroupBy(x => new
                                                              {

                                                                  x.ItemCode,

                                                              }).Select(x => new WarehouseInventory
                                                              {

                                                                  ItemCode = x.Key.ItemCode,
                                                                  ActualGood = x.Sum(x=> x.ActualGood)

                                                              });

            var getMoveOrdersOutByDate = _context.MoveOrders.Where(x => x.IsActive == true)
                                                            .Where(x => x.IsPrepared == true)
                                                            .Where(x => x.PreparedDate >= DateTime.Parse(DateFrom) && x.PreparedDate <= DateTime.Parse(DateTo) && x.ApprovedDate != null)
                                                            .GroupBy(x => new
                                                            {

                                                                x.ItemCode,
                                                            }).Select(x => new MoveOrderInventory
                                                            {

                                                                ItemCode = x.Key.ItemCode,
                                                                QuantityOrdered = x.Sum(x=> x.QuantityOrdered)


                                                            });

       

            var getMoveOrdersOutbyDatePlus = _context.MoveOrders.Where(x => x.IsActive == true)
                                                                .Where(x => x.IsPrepared == true)
                                                                .Where(x => x.PreparedDate >= DateTime.Parse(PlusOne) && x.PreparedDate <= DateToday && x.ApprovedDate != null)
                                                                .GroupBy(x => new
                                                                {

                                                                    x.ItemCode,

                                                                }).Select(x => new MoveOrderInventory
                                                                {


                                                                    ItemCode = x.Key.ItemCode,
                                                                    QuantityOrdered = x.Sum(x => x.QuantityOrdered)

                                                                });


            var getIssueOutByDate = _context.MiscellaneousIssueDetail.Where(x => x.IsActive == true)
                                                                     .Where(x => x.IsTransact == true)
                                                                     .Where(x => x.PreparedDate >= DateTime.Parse(DateFrom) && x.PreparedDate <= DateTime.Parse(DateTo))
                                                                     .GroupBy(x => new
                                                                     {
                                                                         x.ItemCode,

                                                                     }).Select(x => new DtoMiscIssue
                                                                     {

                                                                         ItemCode = x.Key.ItemCode,
                                                                         Quantity = x.Sum(x => x.Quantity)

                                                                     });

            var getIssueOutByDatePlus = _context.MiscellaneousIssueDetail.Where(x => x.IsActive == true && x.IsTransact == true)
                                                                         .Where(x => x.PreparedDate >= DateTime.Parse(PlusOne) && x.PreparedDate <= (DateToday))
                                                                         .GroupBy(x => new
                                                                         {

                                                                             x.ItemCode,

                                                                         }).Select(x => new DtoMiscIssue
                                                                         {

                                                                             ItemCode = x.Key.ItemCode,
                                                                             Quantity = x.Sum(x => x.Quantity)

                                                                         });

            var getBorrowedOutByDate = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                                    .Where(x => x.BorrowedDate >= DateTime.Parse(DateTo) && x.BorrowedDate <= DateTime.Parse(DateFrom))
                                                                    .GroupBy(x => new
                                                                    {
                                                                        x.ItemCode,

                                                                    }).Select(x => new DtoBorrowedIssue
                                                                    {
                                                                        ItemCode = x.Key.ItemCode,
                                                                        Quantity = x.Sum(x => x.Quantity)

                                                                    });

            var getBorrowedOutByDatePlus = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                                  .Where(x => x.BorrowedDate >= DateTime.Parse(PlusOne) && x.BorrowedDate <= (DateToday))
                                                                  .GroupBy(x => new
                                                                  {
                                                                      x.ItemCode,

                                                                  }).Select(x => new DtoBorrowedIssue
                                                                  {
                                                                      ItemCode = x.Key.ItemCode,
                                                                      Quantity = x.Sum(x => x.Quantity)

                                                                  });




            var getReturnedOutByDate = _context.ReturnedBorroweds.Where(x => x.IsActive == true)
                                                                       .Where(x => x.IsReturned == true)
                                                                       .Where(x => x.BorrowedDate >= DateTime.Parse(DateFrom) && x.BorrowedDate <= DateTime.Parse(DateTo))
                                                                       .GroupBy(x => new
                                                                       {
                                                                           x.ItemCode,

                                                                       }).Select(x => new DtoBorrowedIssue

                                                                       {

                                                                           ItemCode = x.Key.ItemCode,
                                                                           ReturnQuantity = x.Sum(x => x.ReturnedQuantity)

                                                                       });



            var getReturnedOutByDatePlus = _context.ReturnedBorroweds.Where(x => x.IsActive == true)
                                                                        .Where(x => x.IsReturned == true)
                                                                        .Where(x => x.BorrowedDate >= DateTime.Parse(PlusOne) && x.BorrowedDate <= (DateToday))
                                                                        .GroupBy(x => new
                                                                        {
                                                                            x.ItemCode,

                                                                        }).Select(x => new DtoBorrowedIssue

                                                                        {

                                                                           ItemCode = x.Key.ItemCode,
                                                                           ReturnQuantity = x.Sum(x => x.ReturnedQuantity)

                                                                        });


            var getReceiveIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                         .Where(x => x.TransactionType == "Receiving")
                                                         .Where(x => x.ReceivingDate >= DateTime.Parse(DateFrom) && x.ReceivingDate <= DateTime.Parse(DateTo))
                                                         .GroupBy(x => new
                                                         {
                                                             x.ItemCode,

                                                         }).Select(x => new DtoReceiveIn
                                                         {

                                                             ItemCode = x.Key.ItemCode,
                                                             Quantity = x.Sum(x => x.ActualGood)

                                                         });

            var getReceiveInPlus = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                        .Where(x => x.TransactionType == "Receiving")
                                                        .Where(x => x.ReceivingDate >= DateTime.Parse(PlusOne) && x.ReceivingDate <= (DateToday))
                                                        .GroupBy(x => new
                                                        {
                                                            x.ItemCode,

                                                        }).Select(x => new DtoReceiveIn
                                                        {

                                                            ItemCode = x.Key.ItemCode,
                                                            Quantity = x.Sum(x => x.ActualGood)

                                                        });


            var getReceiptIn = _context.WarehouseReceived.Where(x => x.IsActive == true && x.TransactionType == "MiscellaneousReceipt")
                                                             .Where(x => x.ReceivingDate >= DateTime.Parse(DateFrom) && x.ReceivingDate <= DateTime.Parse(DateTo))
                                                             .GroupBy(x => new
                                                             {
                                                                 x.ItemCode,

                                                             }).Select(x => new DtoRecieptIn
                                                             {

                                                                 ItemCode = x.Key.ItemCode,
                                                                 Quantity = x.Sum(x => x.ActualGood)

                                                             });


            var getReceiptInPlus = _context.WarehouseReceived.Where(x => x.IsActive == true && x.TransactionType == "MiscellaneousReceipt")
                                                            .Where(x => x.ReceivingDate >= DateTime.Parse(PlusOne) && x.ReceivingDate <= (DateToday))
                                                            .GroupBy(x => new
                                                            {
                                                                x.ItemCode,

                                                            }).Select(x => new DtoRecieptIn
                                                            {

                                                                ItemCode = x.Key.ItemCode,
                                                                Quantity = x.Sum(x => x.ActualGood)

                                                            });


            var getMoveOrderOut = _context.MoveOrders.Where(x => x.IsActive == true)
                                                     .Where(x => x.IsPrepared == true)
                                                     .GroupBy(x => new
                                                     {

                                                         x.ItemCode,

                                                     }).Select(x => new MoveOrderInventory
                                                     {
                                                         ItemCode = x.Key.ItemCode,
                                                         QuantityOrdered = x.Sum(x => x.QuantityOrdered)

                                                     });

            var getIssueOut = _context.MoveOrders.Where(x => x.IsActive == true)
                                                 .Where(x => x.IsTransact == true)
                                                 .GroupBy(x => new
                                                 {

                                                     x.ItemCode,

                                                 }).Select(x => new DtoMiscIssue
                                                 {

                                                     ItemCode = x.Key.ItemCode,
                                                     Quantity = x.Sum(x => x.QuantityOrdered)

                                                 });




            var getBorrowedOut = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                              .GroupBy(x => new
                                                              {

                                                                 x.ItemCode,

                                                              }).Select(x => new DtoBorrowedIssue
                                                              {

                                                                  ItemCode = x.Key.ItemCode,
                                                                  Quantity = x.Sum(x => x.Quantity)

                                                              });


            var getReturned = _context.ReturnedBorroweds.Where(x => x.IsActive == true)
                                                            .Where(x => x.IsReturned == true)
                                                             .GroupBy(x => new
                                                             {

                                                                 x.ItemCode,

                                                             }).Select(x => new DtoBorrowedIssue
                                                             {

                                                                 ItemCode = x.Key.ItemCode,
                                                                 ReturnQuantity = x.Sum(x => x.ReturnedQuantity)

                                                             });


            var getSOH = (from warehouse in getWarehouseStock
                          join moveorder in getMoveOrderOut
                          on warehouse.ItemCode equals moveorder.ItemCode
                          into leftJ1
                          from moveorder in leftJ1.DefaultIfEmpty()

                          join issue in getIssueOut
                          on warehouse.ItemCode equals issue.ItemCode
                          into leftJ2
                          from issue in leftJ2.DefaultIfEmpty()

                          join borrowed in getBorrowedOut
                          on warehouse.ItemCode equals borrowed.ItemCode
                          into leftJ3
                          from borrowed in leftJ3.DefaultIfEmpty()

                          join returned in getReturned
                          on warehouse.ItemCode equals returned.ItemCode
                          into leftJ4
                          from returned in leftJ4.DefaultIfEmpty()

                          group new
                          {

                              warehouse,
                              moveorder,
                              issue,
                              borrowed,
                              returned,

                          }

                          by new
                          {
                              warehouse.ItemCode
                          }

                          into total

                          select new DtoSOH
                          {

                              ItemCode = total.Key.ItemCode,
                              SOH  = total.Sum(x => x.warehouse.ActualGood != null ? x.warehouse.ActualGood : 0 ) + 
                                     total.Sum(x => x.returned.ReturnQuantity != null ? x.returned.ReturnQuantity : 0 ) -
                                     total.Sum(x => x.moveorder.QuantityOrdered != null ? x.moveorder.QuantityOrdered : 0) -
                                     total.Sum(x => x.issue.Quantity != null ? x.issue.Quantity : 0 ) -
                                     total.Sum(x => x.borrowed.Quantity != null ? x.borrowed.Quantity : 0)

                          });

            var movementInventory = (from material in _context.Materials
                                     join moveorder in getMoveOrdersOutByDate
                                     on material.ItemCode equals moveorder.ItemCode
                                     into leftJ1
                                     from moveorder in leftJ1.DefaultIfEmpty()

                                     join issue in getIssueOutByDate
                                     on material.ItemCode equals issue.ItemCode
                                     into leftJ2
                                     from issue in leftJ2.DefaultIfEmpty()

                                     join borrowed in getBorrowedOutByDate
                                     on material.ItemCode equals borrowed.ItemCode
                                     into leftJ3
                                     from borrowed in leftJ3.DefaultIfEmpty()

                                     join returned in getReturnedOutByDate
                                     on material.ItemCode equals returned.ItemCode
                                     into leftJ4
                                     from returned in leftJ4.DefaultIfEmpty()

                                     join receiveIn in getReceiveIn
                                     on material.ItemCode equals receiveIn.ItemCode
                                     into leftJ5
                                     from receiveIn in leftJ5.DefaultIfEmpty()

                                     join receipt in getReceiptIn
                                     on material.ItemCode equals receipt.ItemCode
                                     into leftJ6
                                     from receipt in leftJ6.DefaultIfEmpty()

                                     join SOH in getSOH
                                     on material.ItemCode equals SOH.ItemCode
                                     into leftJ7
                                     from SOH in leftJ7.DefaultIfEmpty()

                                     join moveorderPlus in getMoveOrdersOutbyDatePlus
                                     on material.ItemCode equals moveorderPlus.ItemCode
                                     into leftJ8
                                     from moverorderPlus in leftJ8.DefaultIfEmpty()

                                     join issuePlus in getIssueOutByDatePlus
                                     on material.ItemCode equals issuePlus.ItemCode
                                     into leftJ9
                                     from issuePlus in leftJ9.DefaultIfEmpty()

                                     join borrowedPlus in getBorrowedOutByDatePlus
                                     on material.ItemCode equals borrowedPlus.ItemCode
                                     into leftJ10
                                     from borrowedPlus in leftJ10.DefaultIfEmpty()

                                     join returnedPlus in getReturnedOutByDatePlus
                                     on material.ItemCode equals returnedPlus.ItemCode
                                     into leftJ11
                                     from returnedPlus in leftJ11.DefaultIfEmpty()

                                     join receiveInPlus in getReceiptInPlus
                                     on material.ItemCode equals receiveInPlus.ItemCode
                                     into leftJ12
                                     from receiveInPlus in leftJ12.DefaultIfEmpty()

                                     join receiptInPlus in getReceiptInPlus
                                     on material.ItemCode equals receiptInPlus.ItemCode
                                     into leftJ13
                                     from receiptInPlus in leftJ13.DefaultIfEmpty()

                                     group new
                                     {

                                         material,
                                         moveorder,
                                         issue,
                                         borrowed,
                                         returned,
                                         receiveIn,
                                         receipt,
                                         SOH,
                                         moverorderPlus,
                                         issuePlus,
                                         borrowedPlus,
                                         returnedPlus,
                                         receiveInPlus,
                                         receiptInPlus,

                                     }

                                     by new
                                     {
                                         material.ItemCode,
                                         material.ItemDescription,
                                         material.SubCategory.SubCategoryName,
                                         Moveorder = moveorder.QuantityOrdered != null ? moveorder.QuantityOrdered : 0,
                                         Issue = issue.Quantity != null ? issue.Quantity : 0,
                                         borrowed = borrowed.Quantity != null ? borrowed.Quantity : 0,
                                         returned = returned.ReturnQuantity != null ? returned.ReturnQuantity : 0,
                                         receiveIn = receiveIn.Quantity != null ? receiveIn.Quantity : 0,
                                         receipt = receipt.Quantity != null ? receipt.Quantity : 0,
                                         SOH = SOH.SOH != null ? SOH.SOH : 0,
                                         moverorderPlus = moverorderPlus.QuantityOrdered != null ? moverorderPlus.QuantityOrdered : 0,
                                         issuePlus = issuePlus.Quantity != null ? issuePlus.Quantity : 0,
                                         borrowedPlus = borrowedPlus.Quantity != null ? borrowedPlus.Quantity : 0,
                                         returnedPlus = returnedPlus.ReturnQuantity != null ? returnedPlus.ReturnQuantity : 0,
                                         receiptInPlus = receiptInPlus.Quantity != null ? receiptInPlus.Quantity : 0,
                                         receiveInPlus = receiveInPlus.Quantity != null ? receiveInPlus.Quantity : 0,

                                     }
                                     into total

                                     select new DtoInventoryMovement
                                     {

                                         ItemCode = total.Key.ItemCode,
                                         ItemDescription = total.Key.ItemDescription,
                                         ItemCategory = total.Key.SubCategoryName,
                                         TotalOut = total.Key.Moveorder + total.Key.Issue + total.Key.borrowed,
                                         TotalIn = total.Key.receipt + total.Key.receiveIn + total.Key.returned,
                                         Ending = (total.Key.receipt + total.Key.receiveIn + total.Key.returned) - (total.Key.Moveorder + total.Key.Issue + total.Key.borrowed),
                                         CurrentStock = total.Key.SOH,
                                         PurchaseOrder = total.Key.receiveInPlus + total.Key.receiptInPlus + total.Key.returnedPlus,
                                         OtherPlus = total.Key.moverorderPlus + total.Key.issuePlus + total.Key.borrowedPlus,

                                     });

            return await movementInventory.ToListAsync();


        }

       
    }

}
