using ELIXIRETD.DATA.CORE.INTERFACES.REPORTS_INTERFACE;
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
            var getBorrowed = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                        .Select(x => new 
                                                        {

                                                            BorrowedPKey = x.BorrowedPKey,
                                                            ItemCode = x.ItemCode,
                                                            Quantity = x.Quantity != null ? x.Quantity : 0,
                                                            ItemDescription = x.ItemDescription,
                                                            Uom = x.Uom,
                                                            Category = x.Remarks,


                                                        });

            var getReturned = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                           .Where(x => x.IsReturned == true)
                                                           .Select(x => new
                                                           {
                                                               BorrowedPKey = x.BorrowedPKey,
                                                               ItemCode = x.ItemCode,
                                                               ReturnedQuantity = x.ReturnQuantity != null ? x.ReturnQuantity : 0

                                                           });


            var getBorrowedReturn = getBorrowed
                              .GroupJoin(getReturned, borrowed => borrowed.BorrowedPKey, returned => returned.BorrowedPKey, (borrowed, returned) => new { borrowed, returned })
                              .SelectMany(x => x.returned.DefaultIfEmpty(), (x, returned) => new { x.borrowed, returned })
                              .Select(x => new 
                              {

                                  BorrowedPKey = x.borrowed.BorrowedPKey,
                                  ItemCode = x.borrowed.ItemCode,
                                  ItemDescription = x.borrowed.ItemDescription,
                                  Quantity = x.borrowed.Quantity,
                                  ReturnedQuantity = x.returned.ReturnedQuantity,
                                  Uom = x.borrowed.Uom,
                                  Category = x.borrowed.Category,
                                                               
                              });


            var Reports = _context.BorrowedIssues
                      .GroupJoin(getBorrowedReturn, Borrowedissue => Borrowedissue.Id, borrowedreturned => borrowedreturned.BorrowedPKey, (Borrowedissue, borrowedreturned) => new { Borrowedissue, borrowedreturned })
                      .SelectMany(x => x.borrowedreturned.DefaultIfEmpty(), (x, borrowedreturned) => new { x.Borrowedissue, borrowedreturned })
                      .Where(x => x.Borrowedissue.PreparedDate >= DateTime.Parse(DateFrom) && x.Borrowedissue.PreparedDate <= DateTime.Parse(DateTo))
                      .Select(x => new DtoBorrowedAndReturned
                      {
                          BorrowedId = x.Borrowedissue.Id,
                          CustomerCode = x.Borrowedissue.CustomerCode,
                          CustomerName = x.Borrowedissue.CustomerName,
                          ItemCode = x.borrowedreturned.ItemCode,
                          ItemDescription = x.borrowedreturned.ItemDescription,
                          Remarks = x.borrowedreturned.Category,
                          Uom = x.borrowedreturned != null ? x.borrowedreturned.Uom : null,
                          BorrowedQuantity = x.borrowedreturned.Quantity,
                          ReturnQuantity = x.borrowedreturned.ReturnedQuantity,
                          Consumes = x.borrowedreturned.Quantity - x.borrowedreturned.ReturnedQuantity,
                          TransactedBy = x.Borrowedissue.PreparedBy,
                          BorrowedDate = x.Borrowedissue.PreparedDate.ToString("MM/dd/yyyy")

                      });

            return await Reports.ToListAsync();

        }

        public async Task<IReadOnlyList<DtoCancelledReports>> CancelledReports(string DateFrom, string DateTo)
        {
            var orders = _context.Orders.Where(x => x.OrderDate >= DateTime.Parse(DateFrom) && x.OrderDate <= DateTime.Parse(DateTo) && x.IsCancel == true && x.IsActive == false)
                                        .Select(x => new DtoCancelledReports
                                        {

                                            OrderId = x.Id,





                                        });

            return await orders.ToListAsync();
        }
    }

}
