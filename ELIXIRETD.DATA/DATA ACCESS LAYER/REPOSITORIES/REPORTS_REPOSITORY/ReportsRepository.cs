using ELIXIRETD.DATA.CORE.INTERFACES.REPORTS_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORY_DTO.MRP;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORTS_REPOSITORY
{
    public class ReportsRepository : IReports
    {
        private readonly StoreContext _context;

    

        public ReportsRepository(StoreContext storeContext)
        {
            _context = storeContext;
        }

       

        public async Task<PagedList<DtoWarehouseReceivingReports>> WarehouseReceivingReports(UserParams userParams, string DateFrom, string DateTo)
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
                                                          Category = x.LotSection,
                                                          Quantity = x.ActualDelivered,
                                                          TotalReject = x.TotalReject,
                                                          SupplierName = x.Supplier,
                                                          TransactionType = x.TransactionType,
                                                          ReceivedBy = x.AddedBy

                                                      });

           
            return await PagedList<DtoWarehouseReceivingReports>.CreateAsync(warehouse, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<PagedList<DtoMoveOrderReports>> WarehouseMoveOrderReports(UserParams userParams, string DateFrom, string DateTo)
        {
            var orders = _context.MoveOrders
                        .Where(moveorder => moveorder.PreparedDate >= DateTime.Parse(DateFrom) && moveorder.PreparedDate <= DateTime.Parse(DateTo) && moveorder.IsActive == true && moveorder.IsPrepared == true)
                        .GroupJoin(_context.TransactOrder, moveorder => moveorder.OrderNo, transact => transact.OrderNo, (moveorder, transact) => new { moveorder, transact })
                        .SelectMany(x => x.transact.DefaultIfEmpty(), (x, transact) => new { x.moveorder, transact })
                         .Select(x => new DtoMoveOrderReports
                         {

                             MIRId = x.moveorder.OrderNo,
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
                             IsActive = x.moveorder.IsActive,
                             CompanyCode = x.moveorder.CompanyCode,
                             CompanyName = x.moveorder.CompanyName,
                             DepartmentCode = x.moveorder.DepartmentCode,
                             DepartmentName = x.moveorder.DepartmentName,
                             LocationCode = x.moveorder.LocationCode,
                             LocationName = x.moveorder.LocationName,
                             AccountCode = x.moveorder.AccountCode,
                             AccountTitles = x.moveorder.AccountTitles,
                             CustomerType = x.moveorder.CustomerType



                         });

            return await PagedList<DtoMoveOrderReports>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<DtoTransactReports>> TransactedMoveOrderReport(UserParams userParams, string DateFrom, string DateTo)
        {
            var orders = (from transact in _context.TransactOrder
                          where transact.IsActive == true && transact.IsTransact == true && transact.DeliveryDate >= DateTime.Parse(DateFrom) && transact.DeliveryDate <= DateTime.Parse(DateTo)
                          join moveorder in _context.MoveOrders
                          on transact.OrderNo equals moveorder.OrderNo
                          into leftJ
                          from moveorder in leftJ.DefaultIfEmpty()

                          join customer in _context.Customers
                          on moveorder.Customercode equals customer.CustomerCode
                          into leftJ2
                          from customer in leftJ2.DefaultIfEmpty()

                          group new
                          {
                              transact,
                              moveorder,
                              customer

                          }

                          by new
                          {
                              moveorder.OrderNo,
                              customer.CustomerName,
                              customer.CustomerCode,
                              moveorder.ItemCode,
                              moveorder.ItemDescription,
                              moveorder.Uom,
                              moveorder.QuantityOrdered,
                              MoveOrderDate = moveorder.ApprovedDate.ToString(),
                              transact.PreparedBy,
                              TransactionDate = transact.PreparedDate.ToString(),
                              DeliveryDate = transact.DeliveryDate.ToString(),
                              transact.IsActive,

                             moveorder.CompanyCode,
                             moveorder.CompanyName,
                             moveorder.DepartmentCode,
                             moveorder.DepartmentName,
                             moveorder.LocationCode,
                             moveorder.LocationName,
                             moveorder.AccountCode,
                             moveorder.AccountTitles,

                              customer.CustomerType,



                          } into total

                          select new DtoTransactReports
                          {

                              MIRId = total.Key.OrderNo,
                              CustomerName = total.Key.CustomerName,
                              CustomerCode = total.Key.CustomerCode,
                              ItemCode = total.Key.ItemCode,
                              ItemDescription = total.Key.ItemDescription,
                              Uom = total.Key.Uom,
                              Quantity = total.Key.QuantityOrdered,
                              MoveOrderDate = total.Key.MoveOrderDate,
                              TransactedBy = total.Key.PreparedBy,
                              TransactionType = total.Key.IsActive,
                              TransactedDate = total.Key.TransactionDate,
                              DeliveryDate = total.Key.DeliveryDate,


                              CustomerType = total.Key.CustomerType,

                               CompanyCode = total.Key.CompanyCode,
                              CompanyName = total.Key.CompanyName,
                              DepartmentCode = total.Key.DepartmentCode,
                              DepartmentName = total.Key.DepartmentName,
                              LocationCode = total.Key.LocationCode,
                              LocationName = total.Key.LocationName,
                              AccountCode = total.Key.AccountCode,
                              AccountTitles = total.Key.AccountTitles,

                          });

            return await PagedList<DtoTransactReports>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);

        }


        public async Task<PagedList<DtoMiscReports>> MiscReports(UserParams userParams, string DateFrom, string DateTo)
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
                                Category = receipt.LotSection,
                                Quantity = receipt.ActualGood,
                                TransactBy = receiptHeader.PreparedBy,
                                TransactDate = receiptHeader.TransactionDate.ToString(),
                                ReceivingDate = receipt.ReceivingDate.ToString(),

                                CompanyCode = receiptHeader.CompanyCode,
                                CompanyName = receiptHeader.CompanyName,
                                DepartmentCode = receiptHeader.DepartmentCode,
                                DepartmentName = receiptHeader.DepartmentName,
                                LocationCode = receiptHeader.LocationCode,
                                LocationName = receiptHeader.LocationName,
                                AccountCode = receiptHeader.AccountCode,
                                AccountTitles = receiptHeader.AccountTitles,


                            });

            return await PagedList<DtoMiscReports>.CreateAsync(receipts, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<DtoMiscIssue>> MiscIssue(UserParams userParams, string DateFrom , string DateTo)
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
                           TransactDate = x.issue.PreparedDate.ToString() ,

                           PreparedDate = x.issue.PreparedDate.ToString(),

                           CompanyCode = x.receipt.CompanyCode,
                           CompanyName = x.receipt.CompanyName,
                           DepartmentCode = x.receipt.DepartmentCode,
                           DepartmentName = x.receipt.DepartmentName,
                           LocationCode = x.receipt.LocationCode,
                           LocationName = x.receipt.LocationName,
                           AccountCode = x.receipt.AccountCode,
                           AccountTitles = x.receipt.AccountTitles,

                       });


            return await PagedList<DtoMiscIssue>.CreateAsync(issues, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<DtoBorrowedAndReturned>> ReturnBorrowedReports(UserParams userParams ,string DateFrom, string DateTo)
        {


            var Reports = (from borrowed in _context.BorrowedIssues
                           where borrowed.PreparedDate >= DateTime.Parse(DateFrom) && borrowed.PreparedDate <= DateTime.Parse(DateTo) && borrowed.IsActive == true
                           join returned in _context.BorrowedIssueDetails
                           on borrowed.Id equals returned.BorrowedPKey
                           into leftJ
                           from returned in leftJ.DefaultIfEmpty()

                           select new DtoBorrowedAndReturned
                           {

                               BorrowedId = borrowed.Id,
                               CustomerCode = borrowed.CustomerCode,
                               CustomerName = borrowed.CustomerName,
                               ItemCode = returned.ItemCode,
                               ItemDescription = returned.ItemDescription,
                               BorrowedQuantity = returned.Quantity != null ? returned.Quantity : 0,
                               Consumed = (returned.Quantity != null ? returned.Quantity : 0) - (returned.ReturnQuantity != null ? returned.ReturnQuantity : 0),
                               ReturnedQuantity = returned.ReturnQuantity != null ? returned.ReturnQuantity : 0,
                               ReturnedDate = returned.ReturnedDate.ToString(),
                               Uom = returned.Uom,
                               TransactedBy = borrowed.PreparedBy,
                               BorrowedDate = borrowed.PreparedDate.ToString(),

                               StatusApprove = borrowed.StatusApproved,
                               AgingDays = borrowed.AgingDays,

                               //IsApprove = borrowed.IsApproved,
                               IsApproveDate = borrowed.IsApprovedDate.ToString(),
                               IsApproveBy = borrowed.ApproveBy,
                               //IsApproveReturned = borrowed.IsApprovedReturned != null ? true : false,
                               IsApproveReturnDate = borrowed.IsApprovedReturnedDate.ToString(),
                               IsApproveReturnedBy = borrowed.ApprovedReturnedBy,

                               CompanyCode = borrowed.CompanyCode,
                               CompanyName = borrowed.CompanyName,
                               DepartmentCode = borrowed.DepartmentCode,
                               DepartmentName = borrowed.DepartmentName,
                               LocationCode = borrowed.LocationCode,
                               LocationName = borrowed.LocationName,
                               AccountCode = borrowed.AccountCode,
                               AccountTitles = borrowed.AccountTitles,

                           });

            return await PagedList<DtoBorrowedAndReturned>.CreateAsync(Reports, userParams.PageNumber, userParams.PageSize);

        }



        public async Task<PagedList<DtoCancelledReports>> CancelledReports(UserParams userParams , string DateFrom, string DateTo )
        {
            var orders = _context.Orders.Where(x => x.CancelDate >= DateTime.Parse(DateFrom) && x.CancelDate <= DateTime.Parse(DateTo) && x.IsCancel == true && x.IsActive == false)
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

            return await PagedList<DtoCancelledReports>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<DtoInventoryMovement>> InventoryMovementReports(UserParams userParams , string DateFrom, string DateTo , string PlusOne )
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
                                                            .Where(x => x.PreparedDate >= DateTime.Parse(DateFrom) && x.PreparedDate <= DateTime.Parse(DateTo) && x.ApprovedDate == null)
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
                                                                    .Where(x => x.IsApproved == true)
                                                                    .Where(x => x.BorrowedDate >= DateTime.Parse(DateFrom) && x.BorrowedDate <= DateTime.Parse(DateTo))
                                                                    .GroupBy(x => new
                                                                    {
                                                                        x.ItemCode,

                                                                    }).Select(x => new DtoBorrowedIssue
                                                                    {
                                                                        ItemCode = x.Key.ItemCode,
                                                                        Quantity = x.Sum(x => x.Quantity != null ? x.Quantity : 0) - x.Sum(x => x.ReturnQuantity)

                                                                    });


            var getBorrowedOutByDatePlus = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                                  .Where(x => x.IsApproved == true)
                                                                  .Where(x => x.BorrowedDate >= DateTime.Parse(PlusOne) && x.BorrowedDate <= (DateToday))
                                                                  .GroupBy(x => new
                                                                  {
                                                                      x.ItemCode,

                                                                  }).Select(x => new DtoBorrowedIssue
                                                                  {
                                                                      ItemCode = x.Key.ItemCode,
                                                                      Quantity = x.Sum(x => x.Quantity != null ? x.Quantity : 0) - x.Sum(x => x.ReturnQuantity)

                                                                  });




            var getReturnedOutByDate = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                                       .Where(x => x.IsApprovedReturned == true)
                   
                                                                       .Where(x => x.IsReturned == true)
                                                                       .Where(x => x.ReturnedDate >= DateTime.Parse(DateFrom) && x.ReturnedDate <= DateTime.Parse(DateTo))
                                                                       .GroupBy(x => new
                                                                       {
                                                                           x.ItemCode,

                                                                       }).Select(x => new DtoBorrowedIssue

                                                                       {

                                                                           ItemCode = x.Key.ItemCode,
                                                                           ReturnQuantity = x.Sum(x => x.ReturnQuantity)

                                                                       });



            var getReturnedOutByDatePlus = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                                        .Where(x => x.IsApprovedReturned == true)
                                                                        .Where(x => x.IsReturned == true)
                                                                        .Where(x => x.ReturnedDate >= DateTime.Parse(PlusOne) && x.ReturnedDate <= (DateToday))
                                                                        .GroupBy(x => new
                                                                        {
                                                                            x.ItemCode,

                                                                        }).Select(x => new DtoBorrowedIssue

                                                                        {

                                                                           ItemCode = x.Key.ItemCode,
                                                                           ReturnQuantity = x.Sum(x => x.ReturnQuantity)

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

            var getIssueOut = _context.MiscellaneousIssueDetail.Where(x => x.IsActive == true)
                                                 .Where(x => x.IsTransact == true)
                                                 .GroupBy(x => new
                                                 {

                                                     x.ItemCode

                                                 }).Select(x => new DtoMiscIssue
                                                 {

                                                     ItemCode = x.Key.ItemCode,
                                                     Quantity = x.Sum(x => x.Quantity)

                                                 });




            var getBorrowedOut = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                              .Where(x => x.IsApproved == true)
                                                              .GroupBy(x => new
                                                              {

                                                                 x.ItemCode,

                                                              }).Select(x => new DtoBorrowedIssue
                                                              {

                                                                  ItemCode = x.Key.ItemCode,
                                                                  Quantity = x.Sum(x => x.Quantity)

                                                              });


            var getReturned = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                            .Where(x => x.IsReturned == true)
                                                               .Where(x => x.IsApprovedReturned == true)
                                                             .GroupBy(x => new
                                                             {

                                                                 x.ItemCode,

                                                             }).Select(x => new DtoBorrowedIssue
                                                             {

                                                                 ItemCode = x.Key.ItemCode,
                                                                 ReturnQuantity = x.Sum(x => x.ReturnQuantity)

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
                             SOH= total.Sum(x => x.warehouse.ActualGood != null ? x.warehouse.ActualGood : 0) +
                             total.Sum(x => x.returned.ReturnQuantity != null ? x.returned.ReturnQuantity : 0) -
                             total.Sum(x => x.issue.Quantity != null ? x.issue.Quantity : 0) -
                             total.Sum(x => x.borrowed.Quantity != null ? x.borrowed.Quantity : 0) -
                             total.Sum(x => x.moveorder.QuantityOrdered != null ? x.moveorder.QuantityOrdered : 0)

                             

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
                                         //material.ItemCategory.ItemCategoryName,
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
                                         //ItemCategory = total.Key.ItemCategoryName,
                                         TotalOut = total.Key.borrowed + total.Key.Moveorder + total.Key.Issue,
                                         TotalIn = total.Key.receipt + total.Key.receiveIn + total.Key.returned,
                                         Ending = (total.Key.receipt + total.Key.receiveIn + total.Key.returned) - (total.Key.Moveorder + total.Key.Issue + total.Key.borrowed),
                                         CurrentStock = total.Key.SOH,
                                         PurchaseOrder = total.Key.receiveInPlus + total.Key.receiptInPlus + total.Key.returnedPlus,
                                         OtherPlus = total.Key.moverorderPlus + total.Key.issuePlus + total.Key.borrowedPlus,

                                     });

            return await PagedList<DtoInventoryMovement>.CreateAsync(movementInventory, userParams.PageNumber, userParams.PageSize);


        }

       
    }

}
