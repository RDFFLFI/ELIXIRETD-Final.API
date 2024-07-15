using DocumentFormat.OpenXml.Vml;
using ELIXIRETD.DATA.CORE.INTERFACES.REPORTS_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORY_DTO.MRP;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO.ConsolidationDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.BORROWED_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORTS_REPOSITORY
{
    public class ReportsRepository : IReports
    {
        private readonly  StoreContext _context;

    

        public ReportsRepository(StoreContext storeContext)
        {
            _context = storeContext;
        }

       

        public async Task<PagedList<DtoWarehouseReceivingReports>> WarehouseReceivingReports(UserParams userParams, string DateFrom, string DateTo , string Search)
        {

            var warehouse = _context.WarehouseReceived.Where(x => x.ReceivingDate.Date >= DateTime.Parse(DateFrom).Date && x.ReceivingDate.Date <= DateTime.Parse(DateTo).Date)
                                                      .Where(x => x.IsActive == true)
                                                      .Where(x => x.TransactionType == "Receiving")
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
                                                          ReceivedBy = x.AddedBy,
                                                          UnitPrice = x.UnitPrice,
                                                          Amount = x.UnitPrice * x.ActualDelivered,
                                                          SINumber = x.SINumber
                                                          

                                                      });

            if(!string.IsNullOrEmpty(Search))
            {
                warehouse = warehouse.Where(x => Convert.ToString(x.PoNumber).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemCode).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.TransactionType).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemDescrption).ToLower().Contains(Search.Trim().ToLower()));
            }


           
            return await PagedList<DtoWarehouseReceivingReports>.CreateAsync(warehouse, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<PagedList<DtoMoveOrderReports>> WarehouseMoveOrderReports(UserParams userParams, string DateFrom, string DateTo, string Search)
        {
            var orders = _context.MoveOrders
                        .Where(moveorder => moveorder.PreparedDate.Value.Date >= DateTime.Parse(DateFrom).Date && moveorder.PreparedDate.Value.Date <= DateTime.Parse(DateTo).Date && moveorder.IsActive == true && moveorder.IsPrepared == true && moveorder.IsTransact == false)
                        .GroupJoin(_context.TransactOrder, moveorder => moveorder.OrderNo, transact => transact.OrderNo, (moveorder, transact) => new { moveorder, transact })
                        .SelectMany(x => x.transact.DefaultIfEmpty(), (x, transact) => new { x.moveorder, transact })
                         .Select(x => new DtoMoveOrderReports
                         {

                             MIRId = x.moveorder.OrderNo,           
                             Requestor = x.moveorder.Requestor,
                             Approver = x.moveorder.Approver,
                             CustomerCode = x.moveorder.Customercode,
                             CustomerName = x.moveorder.CustomerName,
                             ItemCode = x.moveorder.ItemCode,
                             ItemDescription = x.moveorder.ItemDescription,
                             Uom = x.moveorder.Uom,
                             Category = x.moveorder.Category,
                             Quantity = x.moveorder.QuantityOrdered,
                             MoveOrderBy = x.moveorder.PreparedBy,
                             MoveOrderDate = x.moveorder.PreparedDate.ToString(),
                             TransactedDate = x.transact.PreparedDate.ToString(),
                             OrderDate = x.moveorder.OrderDate.ToString(),
                             DateNeeded = x.moveorder.DateNeeded.ToString(),
                             IsActive = x.moveorder.IsActive,
                             CompanyCode = x.moveorder.CompanyCode,
                             CompanyName = x.moveorder.CompanyName,
                             DepartmentCode = x.moveorder.DepartmentCode,
                             DepartmentName = x.moveorder.DepartmentName,
                             LocationCode = x.moveorder.LocationCode,
                             LocationName = x.moveorder.LocationName,
                             AccountCode = x.moveorder.AccountCode,
                             AccountTitles = x.moveorder.AccountTitles,
                             Empid = x.moveorder.EmpId,
                             FullName = x.moveorder.FullName,            
                             CustomerType = x.moveorder.CustomerType,
                             ItemRemarks = x.moveorder.ItemRemarks,
                             UnitCost = x.moveorder.UnitPrice,
                             LineAmount = x.moveorder.UnitPrice * x.moveorder.QuantityOrdered,
                             Cip_No = x.moveorder.Cip_No,
                             AssetTag = x.moveorder.AssetTag,
                             HelpdeskNo = x.moveorder.HelpdeskNo,
                             DateApproved = x.moveorder.DateApproved.ToString(),
                             Rush = x.moveorder.Rush,
                             Remarks = x.moveorder.Remarks

                         });

            if (!string.IsNullOrEmpty(Search))
            {
                orders = orders.Where(x => Convert.ToString(x.MIRId).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemCode).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.CustomerName).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemDescription).ToLower().Contains(Search.Trim().ToLower()));
            }

            return await PagedList<DtoMoveOrderReports>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<DtoTransactReports>> TransactedMoveOrderReport(UserParams userParams, string DateFrom, string DateTo, string Search)
        {
            var orders = _context.MoveOrders
                .Where(x => x.IsActive == true)
                .GroupJoin(_context.TransactOrder, moveorder => moveorder.OrderNo, transact => transact.OrderNo, (moveorder, transact) => new { moveorder, transact })
                .SelectMany(x => x.transact.DefaultIfEmpty(), (x , transact) => new {x.moveorder, transact })
                .GroupJoin(_context.Materials , moveorder => moveorder.moveorder.ItemCode , material => material.ItemCode, (moveorder, material) => new {moveorder,material})
                .SelectMany(x => x.material.DefaultIfEmpty(), (x, material) => new {x.moveorder, material})
                .Where(x => x.moveorder.transact.IsActive == true && x.moveorder.transact.IsTransact == true 
                && x.moveorder.transact.DeliveryDate.Value.Date >= DateTime.Parse(DateFrom).Date 
                && x.moveorder.transact.DeliveryDate.Value.Date <= DateTime.Parse(DateTo).Date)
   
                .Select(x => new DtoTransactReports
                {
                    MIRId = x.moveorder.moveorder.OrderNo,
                    Id = x.moveorder.moveorder.Id,
                    Requestor = x.moveorder.moveorder.Requestor,
                    Approver = x.moveorder.moveorder.Approver,
                    CustomerName = x.moveorder.moveorder.CustomerName,
                    CustomerCode = x.moveorder.moveorder.Customercode,
                    ItemCode = x.material.ItemCode,
                    ItemDescription = x.material.ItemDescription,
                    Uom = x.material.Uom.UomCode,
                    Quantity = x.moveorder.moveorder.QuantityOrdered,
                    MoveOrderDate = x.moveorder.moveorder.PreparedDate.ToString(),
                    MoveOrderBy = x.moveorder.moveorder.PreparedBy,
                    TransactedBy = x.moveorder.moveorder.PreparedBy,
                    TransactionType = x.moveorder.moveorder.IsActive,
                    TransactedDate = x.moveorder.moveorder.DateApproved.ToString(),
                    DeliveryDate = x.moveorder.transact.DeliveryDate.ToString(),
                    CustomerType = x.moveorder.moveorder.CustomerType,
                    DateNeeded = x.moveorder.moveorder.DateNeeded.ToString(),
                    OrderDate = x.moveorder.moveorder.OrderDate.ToString(),
                    CompanyCode = x.moveorder.moveorder.CompanyCode,
                    CompanyName = x.moveorder.moveorder.CompanyName,
                    DepartmentCode = x.moveorder.moveorder.DepartmentCode,
                    DepartmentName = x.moveorder.moveorder.DepartmentName,
                    LocationCode = x.moveorder.moveorder.LocationCode,
                    LocationName = x.moveorder.moveorder.LocationName,
                    AccountCode = x.moveorder.moveorder.AccountCode,
                    AccountTitles = x.moveorder.moveorder.AccountTitles,
                    EmpId = x.moveorder.moveorder.EmpId,
                    FullName = x.moveorder.moveorder.FullName,
                    ItemRemarks = x.moveorder.moveorder.ItemRemarks,
                    UnitCost = x.moveorder.moveorder.UnitPrice,
                    LineAmount = x.moveorder.moveorder.UnitPrice * x.moveorder.moveorder.QuantityOrdered,
                    Cip_No = x.moveorder.moveorder.Cip_No,
                    HelpDesk = x.moveorder.moveorder.HelpdeskNo,
                    Remarks = x.moveorder.moveorder.Remarks,
                    Status = x.moveorder.moveorder.IsTransact == true ? "Transacted" : "For Approval",
                    Category = x.moveorder.moveorder.Category,
                    AssetTag = x.moveorder.moveorder.AssetTag,
                    DateApproved = x.moveorder.moveorder.ApprovedDate.ToString(),
                    Rush = x.moveorder.moveorder.Rush
                });

            if (!string.IsNullOrEmpty(Search))
            {
                orders = orders.Where(x => Convert.ToString(x.MIRId).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemCode).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.CustomerName).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemDescription).ToLower().Contains(Search.Trim().ToLower()));
            }

            return await PagedList<DtoTransactReports>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);

        }


        public async Task<PagedList<DtoMiscReports>> MiscReports(UserParams userParams, string DateFrom, string DateTo, string Search)
        {
            var receipts = (from receiptHeader in _context.MiscellaneousReceipts
                            join receipt in _context.WarehouseReceived
                            on receiptHeader.Id equals receipt.MiscellaneousReceiptId
                            into leftJ
                            from receipt in leftJ.DefaultIfEmpty()

                            where receipt.ReceivingDate.Date >= DateTime.Parse(DateFrom).Date && receipt.ReceivingDate.Date <= DateTime.Parse(DateTo).Date && receipt.IsActive == true && receipt.TransactionType == "MiscellaneousReceipt"

                            select new DtoMiscReports
                            {

                                ReceiptId = receiptHeader.Id,
                                SupplierCode = receiptHeader.SupplierCode,
                                SupplierName = receiptHeader.supplier,
                                Details = receiptHeader.Details,
                                Remarks = receiptHeader.Remarks,
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
                                AccountCode = receipt.AccountCode,
                                AccountTitles = receipt.AccountTitles,
                                EmpId = receipt.EmpId,
                                FullName = receipt.FullName,

                                UnitCost = receipt.UnitPrice,
                                TotalCost = receipt.UnitPrice * receipt.ActualDelivered


                            });

            if (!string.IsNullOrEmpty(Search))
            {
                receipts = receipts.Where(x => Convert.ToString(x.ReceiptId).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemCode).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.SupplierName).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemDescription).ToLower().Contains(Search.Trim().ToLower()));
            }

            return await PagedList<DtoMiscReports>.CreateAsync(receipts, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<DtoMiscIssue>> MiscIssue(UserParams userParams, string DateFrom , string DateTo, string Search)
        {
            var issues = _context.MiscellaneousIssues
                       .GroupJoin(_context.MiscellaneousIssueDetail, receipt => receipt.Id, issue => issue.IssuePKey, (receipt, issue) => new { receipt, issue })
                       .SelectMany(x => x.issue.DefaultIfEmpty(), (x, issue) => new { x.receipt, issue })
                       .Where(x => x.issue.PreparedDate >= DateTime.Parse(DateFrom) && x.issue.PreparedDate <= DateTime.Parse(DateTo) && x.issue.IsActive == true )
                       .Select(x => new DtoMiscIssue
                       {

                           IssueId = x.receipt.Id,
                           CustomerCode = x.receipt.Customercode,
                           CustomerName = x.receipt.Customer,
                           Details = x.receipt.Details,
                           Remarks = x.receipt.Remarks,
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
                           AccountCode = x.issue.AccountCode,
                           AccountTitles = x.issue.AccountTitles,
                           EmpId = x.issue.EmpId,
                           FullName = x.issue.FullName,
                           UnitCost = x.issue.UnitPrice,
                           TotalCost = x.issue.UnitPrice * x.issue.Quantity

                       });

            if (!string.IsNullOrEmpty(Search))
            {
                issues = issues.Where(x => Convert.ToString(x.IssueId).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemCode).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.CustomerName).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemDescription).ToLower().Contains(Search.Trim().ToLower()));
            }

            return await PagedList<DtoMiscIssue>.CreateAsync(issues, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<BorrowedTransactionReportsDto>> BorrowedTransactionReports(UserParams userParams, string DateFrom, string DateTo, string Search)
        {

            var borrowed = _context.BorrowedIssues
                .GroupJoin(_context.BorrowedIssueDetails, issue => issue.Id, borrow => borrow.BorrowedPKey, (issue, borrow) => new { issue, borrow })
                .SelectMany(x => x.borrow.DefaultIfEmpty(), (x, borrow) => new { x.issue, borrow })
                .Where(x => x.issue.PreparedDate.Date >= DateTime.Parse(DateFrom).Date && x.issue.PreparedDate.Date <= DateTime.Parse(DateTo).Date)
                .Where(x => x.issue.IsActive == true)
                .GroupBy(x => x.issue.Id)
                .Select(x => new BorrowedTransactionReportsDto
                {
                    BorrowedId = x.Key,
                    CustomerCode = x.First().issue.CustomerCode,
                    CustomerName = x.First().issue.CustomerName,
                    EmpId = x.First().issue.EmpId,
                    FullName = x.First().issue.FullName,
                    TransactedBy = x.First().issue.PreparedBy,
                    BorrowedDate = x.First().issue.PreparedDate.ToString(),
                    Details = x.First().issue.Details,
                    AgingDays = x.First().issue.IsApprovedReturnedDate != null ? 
                    EF.Functions.DateDiffDay(x.First().issue.IsApprovedDate.Value , x.First().issue.IsApprovedReturnedDate)
                    : x.First().issue.IsApprovedDate == null ? 0 : EF.Functions.DateDiffDay(x.First().issue.IsApprovedDate , DateTime.Now) ,
                    Remarks = x.First().issue.Remarks,
                    Status = (x.First().issue.IsApproved == true) ? "Approved" :
                            (x.First().issue.IsApproved == false) ? "For Approval" : "Unknown",
                        BorrowedItemPkey = x.First().borrow.Id,
                        ItemCode = x.First().borrow.ItemCode,
                        ItemDescription = x.First().borrow.ItemDescription,
                        Uom = x.First().borrow.Uom,
                        BorrowedQuantity = x.First().borrow.Quantity != null ? x.First().borrow.Quantity : 0,

                });

            if (!string.IsNullOrEmpty(Search))
            {
                borrowed = borrowed.Where(x => Convert.ToString(x.BorrowedId).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemCode).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.CustomerName).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemDescription).ToLower().Contains(Search.Trim().ToLower()));
            }

            return await PagedList<BorrowedTransactionReportsDto>.CreateAsync(borrowed, userParams.PageNumber, userParams.PageSize);

        }


        public async Task<PagedList<DtoBorrowedAndReturned>> ReturnBorrowedReports(UserParams userParams, string DateFrom, string DateTo, string Search)
        {


            var ConsumeQuantity = _context.BorrowedConsumes.Where(x => x.IsActive == true)
                                   .Select(x => new DtoBorrowedAndReturned
                                   {
                                       Id = x.Id,
                                       BorrowedId = x.BorrowedItemPkey,
                                       ItemCode = x.ItemCode,
                                       ItemDescription = x.ItemDescription,
                                       Consumed = x.Consume,
                                       CompanyCode = x.CompanyCode,
                                       CompanyName = x.CompanyName,
                                       DepartmentCode = x.DepartmentCode,
                                       DepartmentName = x.DepartmentName,
                                       LocationCode = x.LocationCode,
                                       LocationName = x.LocationName,
                                       AccountCode = x.AccountCode,
                                       AccountTitles = x.AccountTitles,
                                       EmpId = x.EmpId,
                                       FullName = x.FullName,
                                       ReportNumber = x.ReportNumber

                                   });

            var details = _context.BorrowedIssueDetails
                         .Where(x => x.IsActive == true && x.IsApprovedReturned == true)
                         .GroupJoin(ConsumeQuantity, borrow => borrow.Id, consume => consume.BorrowedId, (borrow, consume) => new { borrow, consume })
                         .SelectMany(x => x.consume.DefaultIfEmpty(), (x, consume) => new { x.borrow, consume })
                         .Select(x => new DtoBorrowedAndReturned
                         {

                             Id = x.borrow.Id,
                             BorrowedId = x.borrow.BorrowedPKey,
                             ItemCode = x.borrow.ItemCode,
                             ItemDescription = x.borrow.ItemDescription,
                             Uom = x.borrow.Uom,
                             BorrowedQuantity = x.borrow.Quantity != null ? x.borrow.Quantity : 0,
                             Consumed = x.consume.Consumed != null ? x.consume.Consumed : 0,
                             CompanyCode = x.consume.CompanyCode,
                             CompanyName = x.consume.CompanyName,
                             DepartmentCode = x.consume.DepartmentCode,
                             DepartmentName = x.consume.DepartmentName,
                             LocationCode = x.consume.LocationCode,
                             LocationName = x.consume.LocationName,
                             AccountCode = x.consume.AccountCode,
                             AccountTitles = x.consume.AccountTitles,
                             EmpId = x.consume.EmpId,
                             FullName = x.consume.FullName,
                             ReportNumber = x.consume.ReportNumber

                         });


            var borrowIssue =  _context.BorrowedIssues
                      .Where(x => x.IsActive == true);

              var Reports = details
                           .GroupJoin(borrowIssue, returned => returned.BorrowedId , borrowed => borrowed.Id , (returned, borrowed) => new { returned, borrowed})
                           .SelectMany(x => x.borrowed.DefaultIfEmpty() , (x , borrowed) => new {x.returned , borrowed })
                           .Where(x => x.borrowed.PreparedDate.Date >= DateTime.Parse(DateFrom).Date && x.borrowed.PreparedDate.Date <= DateTime.Parse(DateTo).Date)
                           .Select(x => new DtoBorrowedAndReturned
                           {

                               BorrowedId = x.borrowed.Id,
                               CustomerCode = x.borrowed.CustomerCode,
                               CustomerName = x.borrowed.CustomerName,
                               EmpIdByIssue = x.borrowed.EmpId,
                               FullNameByIssue = x.borrowed.FullName,
                               ItemCode = x.returned.ItemCode,
                               ItemDescription = x.returned.ItemDescription,
                               Uom = x.returned.Uom,
                               BorrowedQuantity = x.returned.BorrowedQuantity,
                               Consumed = x.returned.Consumed,
                               ReturnedQuantity = x.returned.BorrowedQuantity - x.returned.Consumed,
                               TransactedBy = x.borrowed.PreparedBy,
                               BorrowedDate = x.borrowed.PreparedDate.ToString(),
                               Remarks = x.borrowed.Remarks,
                               Details = x.borrowed.Details,
                               Status = x.borrowed.StatusApproved,
                               IsApproveReturnDate = x.borrowed.IsApprovedReturnedDate.ToString(),
                               CompanyCode = x.returned.CompanyCode,
                               CompanyName = x.returned.CompanyName,
                               DepartmentCode = x.returned.DepartmentCode,
                               DepartmentName = x.returned.DepartmentName,
                               LocationCode = x.returned.LocationCode,
                               LocationName = x.returned.LocationName,
                               AccountCode = x.returned.AccountCode,
                               AccountTitles = x.returned.AccountTitles,
                               EmpId = x.returned.EmpId,
                               FullName = x.returned.FullName,
                               ReportNumber = x.returned.ReportNumber,
                               AgingDays = x.borrowed.IsApprovedReturnedDate != null ? EF.Functions.DateDiffDay(x.borrowed.IsApprovedDate.Value , x.borrowed.IsApprovedReturnedDate.Value)
                               : x.borrowed.IsApprovedDate == null ? 0 : EF.Functions.DateDiffDay(x.borrowed.IsApprovedDate, DateTime.Now),
                               IsActive = x.borrowed.IsActive
                             
                               
                           });

            if (!string.IsNullOrEmpty(Search))
            {
                Reports = Reports.Where(x => Convert.ToString(x.BorrowedId).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemCode).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.CustomerName).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemDescription).ToLower().Contains(Search.Trim().ToLower()));
            }

            return await PagedList<DtoBorrowedAndReturned>.CreateAsync(Reports, userParams.PageNumber, userParams.PageSize);

        }

        

        public async Task<PagedList<DtoCancelledReports>> CancelledReports(UserParams userParams , string DateFrom, string DateTo, string Search )
        {

            var cancelled = _context.Orders.Where(x => x.IsActive == false || x.QuantityOrdered != x.StandartQuantity)
                                        .GroupBy(x => new
                                        {
                                            x.TrasactId,
                                            x.Id,
                                            x.OrderNo,
                                            x.DateNeeded,
                                            x.OrderDate,
                                            x.Customercode,
                                            x.CustomerName,
                                            x.ItemCode,
                                            x.ItemdDescription,
                                            Remarks = x.Remarks,
                                           




                                        }).Select(x => new DtoCancelledReports
                                        {
                                            MIRId = x.Key.TrasactId,
                                            OrderId = x.Key.Id,
                                            OrderNo = x.Key.OrderNo,
                                            DateNeeded = x.Key.DateNeeded.ToString(/*"yyyy-MM-dd"*/),
                                            DateOrdered = x.Key.OrderDate.ToString(/*"yyyy-MM-dd"*/),
                                            CustomerCode = x.Key.Customercode,
                                            CustomerName = x.Key.CustomerName,
                                            ItemCode = x.Key.ItemCode,
                                            //ItemRemarks = x.First().ItemRemarks,

                                            CancelledDate = x.First().CancelDate != null 
                                            ? x.First().CancelDate: x.First().Modified_Date == null
                                            ? x.First().OrderDate : x.First().Modified_Date,

                                            CancelledBy = x.First().IsCancelBy != null ? x.First().IsCancelBy : x.First().Modified_By,
                                            ItemDescription = x.Key.ItemdDescription,
                                            QuantityOrdered = x.Sum(x => x.QuantityOrdered),
                                            Reason = x.Key.Remarks,
                                            //DepartmentCode = x.First().DepartmentCode,
                                            //Department = x.First().Department,
                                            //CompanyCode = x.First().CompanyCode,
                                            //CompanyName = x.First().CompanyName,
                                            //LocationCode = x.First().LocationCode,
                                            //LocationName = x.First().LocationName,
                                            //AccountCode = x.First().AccountCode,
                                            //AccountTitles = x.First().AccountTitles,

                                        });


            var orders = _context.Orders
                       .GroupJoin(cancelled, order => order.Id, cancel => cancel.OrderId, (order, cancel) => new { order, cancel })
                       .SelectMany(x => x.cancel.DefaultIfEmpty(), (x, cancel) => new { x.order, cancel })

                       .Where(x => x.cancel.CancelledDate.Value.Date >= DateTime.Parse(DateFrom).Date 
                       && x.cancel.CancelledDate.Value.Date <= DateTime.Parse(DateTo).Date)

                       .GroupBy(x => new
                       {
                           x.order.TrasactId,
                           x.order.Id,
                           x.order.OrderNo,
                           x.order.OrderDate,
                           x.order.DateNeeded,
                           x.order.Customercode,
                           x.order.CustomerName,
                           x.order.ItemCode,
                           x.order.ItemdDescription,

                       }).Select(x => new DtoCancelledReports
                       {
                           MIRId = x.Key.TrasactId,
                           OrderId = x.Key.Id,
                           OrderNo = x.Key.OrderNo,
                           DateNeeded = x.Key.DateNeeded.ToString(/*"yyyy-MM-dd hh:mm tt"*/),
                           DateOrdered = x.Key.OrderDate.ToString(/*"yyyy-MM-dd hh:mm tt"*/),
                           CustomerCode = x.Key.Customercode,
                           CustomerName = x.Key.CustomerName,
                           ItemCode = x.Key.ItemCode,
                           ItemDescription = x.Key.ItemdDescription,
                           CancelledBy = x.First().cancel.CancelledBy,
                           CancelledDate = x.First().cancel.CancelledDate,
                           Reason = x.First().cancel.Reason != null ? x.First().cancel.Reason : x.First().order.Remarks,
                           QuantityOrdered = x.Sum(x => x.cancel.QuantityOrdered) != null && x.Sum(x => x.order.StandartQuantity) - x.Sum(x => x.order.QuantityOrdered) != 0 ?
                           x.Sum(x => x.order.StandartQuantity) - x.Sum(x => x.order.QuantityOrdered) : x.Sum(x => x.cancel.QuantityOrdered),

                           DepartmentCode = x.First().order.DepartmentCode,
                           Department = x.First().order.Department,
                           CompanyCode = x.First().order.CompanyCode,
                           CompanyName = x.First().order.CompanyName,
                           LocationCode = x.First().order.LocationCode,
                           LocationName = x.First().order.LocationName,
                           AccountCode = x.First().order.AccountCode,
                           AccountTitles = x.First().order.AccountTitles,
                           ItemRemarks = x.First().order.ItemRemarks
                       });


            if (!string.IsNullOrEmpty(Search))
            {
                orders = orders.Where(x => Convert.ToString(x.OrderId).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.MIRId).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemCode).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.CustomerName).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemDescription).ToLower().Contains(Search.Trim().ToLower()));
            }

            return await PagedList<DtoCancelledReports>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<DtoInventoryMovement>> InventoryMovementReports(UserParams userParams , string DateFrom , string PlusOne , string Search)
        {
            var DateToday = DateTime.Today;

         
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
                                                            .Where(x => x.PreparedDate.Value >= DateTime.Parse(DateFrom) && x.PreparedDate.Value <= DateTime.Parse(PlusOne))
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
                                                                .Where(x => x.PreparedDate.Value >= DateTime.Parse(PlusOne).AddDays(1) && x.PreparedDate.Value <= DateToday)
                                                                .GroupBy(x => new
                                                                {

                                                                    x.ItemCode,

                                                                }).Select(x => new MoveOrderInventory
                                                                {

                                                                    ItemCode = x.Key.ItemCode,
                                                                    QuantityOrdered = x.Sum(x => x.QuantityOrdered)

                                                                });


            var getIssueOutByDate = _context.MiscellaneousIssueDetail.Where(x => x.IsActive == true)
                                                                     .Where(x => x.PreparedDate >= DateTime.Parse(DateFrom) && x.PreparedDate <= DateTime.Parse(PlusOne))
                                                                     .GroupBy(x => new
                                                                     {
                                                                         x.ItemCode,

                                                                     }).Select(x => new DtoMiscIssue
                                                                     {

                                                                         ItemCode = x.Key.ItemCode,
                                                                         Quantity = x.Sum(x => x.Quantity)

                                                                     });

            var getIssueOutByDatePlus = _context.MiscellaneousIssueDetail.Where(x => x.IsActive == true)
                                                                         .Where(x => x.PreparedDate >= DateTime.Parse(PlusOne).AddDays(1) && x.PreparedDate <= (DateToday))
                                                                         .GroupBy(x => new
                                                                         {

                                                                             x.ItemCode,

                                                                         }).Select(x => new DtoMiscIssue
                                                                         {

                                                                             ItemCode = x.Key.ItemCode,
                                                                             Quantity = x.Sum(x => x.Quantity)

                                                                         });

            var getBorrowedOutByDate = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                                    .Where(x => x.BorrowedDate >= DateTime.Parse(DateFrom) && x.BorrowedDate <= DateTime.Parse(PlusOne))
                                                                    .GroupBy(x => new
                                                                    {
                                                                        x.ItemCode,

                                                                    }).Select(x => new DtoBorrowedIssue
                                                                    {
                                                                        ItemCode = x.Key.ItemCode,
                                                                        Quantity = x.Sum(x => x.Quantity)


                                                                    });


            var getBorrowedOutByDatePlus = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                                  .Where(x => x.BorrowedDate >= DateTime.Parse(PlusOne).AddDays(1)
                                                                  && x.BorrowedDate <= (DateToday))
                                                                  .GroupBy(x => new
                                                                  {
                                                                      x.ItemCode,

                                                                  }).Select(x => new DtoBorrowedIssue
                                                                  {
                                                                      ItemCode = x.Key.ItemCode,
                                                                      Quantity = x.Sum(x => x.Quantity)

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




            var getReturnedOutByDate = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                                       .Where(x => x.IsReturned == true)
                                                                       .Where(x => x.IsApprovedReturned == true)
                                                                       .GroupJoin(consumed, returned => returned.Id , consume => consume.BorrowedItemPkey , (returned , consume) => new { returned, consume})
                                                                       .SelectMany(x => x.consume.DefaultIfEmpty() , (x , consume) => new {x.returned , consume })
                                                                       .Where(x => x.returned.IsApprovedReturnedDate.Value >= DateTime.Parse(DateFrom) 
                                                                       && x.returned.IsApprovedReturnedDate.Value <= DateTime.Parse(PlusOne))
                                                                       .GroupBy(x => new
                                                                       {
                                                                           x.returned.ItemCode,

                                                                       }).Select(x => new DtoBorrowedIssue
                                                                       {

                                                                           ItemCode = x.Key.ItemCode,
                                                                           ReturnQuantity =x.Sum(x => x.returned.Quantity) - x.Sum(x => x.consume.Consume)
                                                                           
                                                                       });



            var getReturnedOutByDatePlus = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                                        .Where(x => x.IsReturned == true)
                                                                        .Where(x => x.IsApprovedReturned == true)
                                                                        .GroupJoin(consumed, returned => returned.Id , consume => consume.BorrowedItemPkey , (returned , consume) => new { returned, consume})
                                                                        .SelectMany(x => x.consume.DefaultIfEmpty() , (x , consume) => new {x.returned , consume })
                                                                        .Where(x => x.returned.IsApprovedReturnedDate.Value >= DateTime.Parse(PlusOne).AddDays(1) 
                                                                        && x.returned.IsApprovedReturnedDate.Value <= (DateToday))
                                                                        .GroupBy(x => new
                                                                        {
                                                                            x.returned.ItemCode,

                                                                        }).Select(x => new DtoBorrowedIssue
                                                                        {

                                                                           ItemCode = x.Key.ItemCode,
                                                                           ReturnQuantity = x.Sum(x => x.returned.Quantity) - x.Sum(x => x.consume.Consume)

                                                                        });


            var getReceiveIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                         .Where(x => x.TransactionType == "Receiving")
                                                         .Where(x => x.ActualReceivingDate >= DateTime.Parse(DateFrom) && x.ActualReceivingDate <= DateTime.Parse(PlusOne))
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
                                                        .Where(x => x.ActualReceivingDate >= DateTime.Parse(PlusOne).AddDays(1) && x.ActualReceivingDate <= (DateToday))
                                                        .GroupBy(x => new
                                                        {
                                                            x.ItemCode,

                                                        }).Select(x => new DtoReceiveIn
                                                        {

                                                            ItemCode = x.Key.ItemCode,
                                                            Quantity = x.Sum(x => x.ActualGood)

                                                        });

            var getReceiptIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                         .Where(x => x.TransactionType == "MiscellaneousReceipt")
                                                             .Where(x => x.ActualReceivingDate >= DateTime.Parse(DateFrom) && x.ActualReceivingDate <= DateTime.Parse(PlusOne))
                                                             .GroupBy(x => new
                                                             {
                                                                 x.ItemCode,

                                                             }).Select(x => new DtoRecieptIn
                                                             {

                                                                 ItemCode = x.Key.ItemCode,
                                                                 Quantity = x.Sum(x => x.ActualGood)

                                                             });


            var getReceiptInPlus = _context.WarehouseReceived.Where(x => x.IsActive == true  )
                                                            .Where(x => x.TransactionType == "MiscellaneousReceipt")
                                                            .Where(x => x.ActualReceivingDate >= DateTime.Parse(PlusOne).AddDays(1) && x.ActualReceivingDate <= (DateToday))
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
                                                 .GroupBy(x => new
                                                 {

                                                     x.ItemCode

                                                 }).Select(x => new DtoMiscIssue
                                                 {

                                                     ItemCode = x.Key.ItemCode,
                                                     Quantity = x.Sum(x => x.Quantity)

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

            var getReturned = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                            .Where(x => x.IsReturned == true)
                                                            .Where(x => x.IsApprovedReturned == true)
                                                            .GroupJoin(consumed, returned => returned.Id , consume => consume.BorrowedItemPkey , (returned , consume) => new { returned, consume})
                                                            .SelectMany(x => x.consume.DefaultIfEmpty() , (x , consume) => new {x.returned , consume })
                                                             .GroupBy(x => new
                                                             {

                                                                 x.returned.ItemCode,

                                                             }).Select(x => new DtoBorrowedIssue
                                                             {

                                                                 ItemCode = x.Key.ItemCode,
                                                                 ReturnQuantity = x.Sum(x => x.returned.Quantity) - x.Sum(x => x.consume.Consume)

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
                                         material.ItemDescription

                                     }
                                     into total

                                     select new DtoInventoryMovement
                                     {

                                         ItemCode = total.Key.ItemCode,
                                         ItemDescription = total.Key.ItemDescription,
                                         TotalOut = total.Sum(x => x.borrowed.Quantity) + total.Sum(x => x.moveorder.QuantityOrdered) + total.Sum(x => x.issue.Quantity),
                                         TotalIn = total.Sum(x => x.receipt.Quantity) + total.Sum(x => x.receiveIn.Quantity) + total.Sum(x => x.returned.ReturnQuantity),
                                         Ending = (total.Sum(x => x.receipt.Quantity) + total.Sum(x => x.receiveIn.Quantity) + total.Sum(x => x.returned.ReturnQuantity)) - 
                                         (total.Sum(x => x.borrowed.Quantity) + total.Sum(x => x.moveorder.QuantityOrdered) + total.Sum(x => x.issue.Quantity)),


                                         CurrentStock = total.Sum(x => x.SOH.SOH),
                                         PurchaseOrder = total.Sum(x => x.receiveInPlus.Quantity) + total.Sum(x => x.receiptInPlus.Quantity) + total.Sum(x => x.returnedPlus.ReturnQuantity),
                                         OtherPlus = total.Sum(x => x.moverorderPlus.QuantityOrdered) + total.Sum(x => x.issuePlus.Quantity) + total.Sum(x => x.borrowedPlus.Quantity),

                                     });

            if(!string.IsNullOrEmpty(Search))
            {
                movementInventory = movementInventory
                    .Where(x => Convert.ToString(x.ItemCode).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemDescription).ToLower().Contains(Search.Trim().ToLower()));
            }

            movementInventory = movementInventory.OrderBy(x => x.ItemCode);
 
            return await PagedList<DtoInventoryMovement>.CreateAsync(movementInventory, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<IReadOnlyList<ConsolidateFinanceReportDto>> ConsolidateFinanceReport(string DateFrom, string DateTo, string Search)
        {
            var dateFrom = DateTime.Parse(DateFrom).Date;
            var dateTo = DateTime.Parse(DateTo).Date;

            var receivingConsol = _context.WarehouseReceived
                .Where(x => x.TransactionType == "Receiving" && x.IsActive == true)
                .Where(x => x.ActualReceivingDate.Date >= dateFrom && x.ActualReceivingDate.Date <= dateTo)
                .Select(x => new ConsolidateFinanceReportDto
                { 
                    Id = x.Id,
                    TransactionDate = x.ActualReceivingDate.Date,
                    ItemCode = x.ItemCode,
                    ItemDescription = x.ItemDescription,
                    Uom = x.Uom,
                    Category = "",
                    Quantity = x.ActualGood,
                    UnitCost = x.UnitPrice,
                    LineAmount = Math.Round(x.UnitPrice * x.ActualGood, 2),
                    Source = x.PoNumber,
                    TransactionType = "Receiving",
                    Reason = "",
                    Reference = x.SINumber,
                    SupplierName = x.Supplier,
                    EncodedBy = x.AddedBy,
                    CompanyCode = "10",
                    CompanyName = "RDF Corporate Services",
                    DepartmentCode = "0010",
                    DepartmentName = "Corporate Common",
                    LocationCode = "0001",
                    LocationName = "Head Office",
                    AccountTitleCode = "117701",
                    AccountTitle = "Materials & Supplies Inventory",
                    EmpId= "",
                    Fullname = "",
                    AssetTag = "",
                    CIPNo = "",
                    Helpdesk = 0,
                    Rush = ""

                }).ToList();

            var moveOrderConsol = _context.TransactOrder
                .Join(_context.MoveOrders, transact => transact.OrderNo,
                moveOrder => moveOrder.OrderNo, (transact, moveOrder) => new { transact, moveOrder})       
               .Where(x => x.transact.IsTransact == true && x.transact.IsActive == true && x.moveOrder.IsActive == true)
               .Where(x => x.transact.DeliveryDate.Value >= dateFrom.Date && x.transact.DeliveryDate.Value <= dateTo)
                .Select(x => new ConsolidateFinanceReportDto
                {
                    Id = x.transact.Id,
                    TransactionDate = x.transact.DeliveryDate.Value,
                    ItemCode = x.moveOrder.ItemCode,
                    ItemDescription = x.moveOrder.ItemDescription,
                    Uom = x.moveOrder.Uom,
                    Category = x.moveOrder.Category,
                    Quantity = Math.Round(x.moveOrder.QuantityOrdered,2),
                    UnitCost = x.moveOrder.UnitPrice,
                    LineAmount = Math.Round(x.moveOrder.UnitPrice * x.moveOrder.QuantityOrdered , 2),
                    Source = x.transact.OrderNo,
                    TransactionType = "MoveOrder",
                    Reason = "",
                    Reference = x.moveOrder.ItemRemarks,
                    SupplierName = "",
                    EncodedBy = x.transact.PreparedBy,
                    CompanyCode = x.moveOrder.CompanyCode,
                    CompanyName = x.moveOrder.CompanyName,
                    DepartmentCode = x.moveOrder.DepartmentCode,
                    DepartmentName = x.moveOrder.DepartmentName,
                    LocationCode = x.moveOrder.LocationCode,
                    LocationName = x.moveOrder.LocationName,
                    AccountTitleCode = x.moveOrder.AccountCode,
                    AccountTitle = x.moveOrder.AccountTitles,
                    EmpId = x.moveOrder.EmpId,
                    Fullname = x.moveOrder.FullName,
                    AssetTag = x.moveOrder.AssetTag,
                    CIPNo = x.moveOrder.Cip_No,
                    Helpdesk = x.moveOrder.HelpdeskNo,
                    //Remarks = x.moveOrder.Remarks,
                    Rush = x.moveOrder.Rush

                }).ToList();

            var receiptConsol = _context.MiscellaneousReceipts
                .GroupJoin(_context.WarehouseReceived, receipt => receipt.Id, warehouse => warehouse.MiscellaneousReceiptId, (receipt, warehouse) => new { receipt, warehouse })
                .SelectMany(x => x.warehouse.DefaultIfEmpty(), (x, warehouse) => new { x.receipt, warehouse })
                .Where(x => x.warehouse.ReceivingDate.Date >= dateFrom && x.warehouse.ReceivingDate.Date <= dateTo)
                .Where(x => x.warehouse.IsActive == true && x.warehouse.TransactionType == "MiscellaneousReceipt") 
                .Select(x => new ConsolidateFinanceReportDto
                {
                    Id = x.warehouse.Id,
                    TransactionDate = x.receipt.TransactionDate.Date,
                    ItemCode = x.warehouse.ItemCode,
                    ItemDescription = x.warehouse.ItemDescription,
                    Uom = x.warehouse.Uom,
                    Category = "",
                    Quantity = x.warehouse.ActualGood,
                    UnitCost = x.warehouse.UnitPrice,
                    LineAmount = Math.Round(x.warehouse.UnitPrice * x.warehouse.ActualGood, 2),
                    Source = x.receipt.Id,
                    TransactionType = "Receipt",
                    Reason = x.receipt.Remarks,
                    Reference = x.receipt.Details,
                    SupplierName = "",
                    EncodedBy = x.receipt.PreparedBy,
                    CompanyCode = x.receipt.CompanyCode,
                    CompanyName = x.receipt.CompanyName,
                    DepartmentCode = x.receipt.DepartmentCode,
                    DepartmentName = x.receipt.DepartmentName,
                    LocationCode = x.receipt.LocationCode,
                    LocationName = x.receipt.LocationName,
                    AccountTitleCode = x.warehouse.AccountCode,
                    AccountTitle = x.warehouse.AccountTitles,
                    EmpId = x.warehouse.EmpId,
                    Fullname = x.warehouse.FullName,
                    AssetTag = "",
                    CIPNo = "",
                    Helpdesk = 0,
                    //Remarks = x.receipt.Remarks,
                    Rush = ""
                }).ToList();

            var issueConsol = _context.MiscellaneousIssues
                .Join(_context.MiscellaneousIssueDetail, miscDatail => miscDatail.Id, issue => issue.IssuePKey,
                (miscDetail, issue) => new { miscDetail, issue })
                .Where(x => x.issue.IsActive == true)
                .Where(x => x.issue.PreparedDate.Date >= dateFrom && x.issue.PreparedDate.Date <= dateTo)
                .Select(x => new ConsolidateFinanceReportDto
                {
                   Id = x.issue.Id,
                    TransactionDate = x.miscDetail.TransactionDate.Date,
                    ItemCode = x.issue.ItemCode,
                    ItemDescription = x.issue.ItemDescription,
                    Uom = x.issue.Uom,
                    Category = "",
                    Quantity = Math.Round(x.issue.Quantity, 2),
                    UnitCost = x.issue.UnitPrice,
                    LineAmount = Math.Round(x.issue.UnitPrice * x.issue.Quantity, 2),
                    Source = x.miscDetail.Id,
                    TransactionType = "Issue",
                    Reason = x.issue.Remarks,
                    Reference = x.miscDetail.Details,
                    SupplierName = "",
                    EncodedBy = x.issue.PreparedBy,
                    CompanyCode = x.miscDetail.CompanyCode,
                    CompanyName = x.miscDetail.CompanyName,
                    DepartmentCode = x.miscDetail.DepartmentCode,
                    DepartmentName = x.miscDetail.DepartmentName,
                    LocationCode = x.miscDetail.LocationCode,
                    LocationName = x.miscDetail.LocationName,
                    AccountTitleCode = x.issue.AccountCode,
                    AccountTitle = x.issue.AccountTitles,
                    EmpId = x.issue.EmpId,
                    Fullname =x.issue.FullName,
                    AssetTag = "",
                    CIPNo = "",
                    Helpdesk = 0,
                    //Remarks = x.issue.Remarks,
                    Rush = ""


                }).ToList();

            var borrowedConsol = _context.BorrowedIssues
                .Join(_context.BorrowedIssueDetails, borrow => borrow.Id, borrowDetail => borrowDetail.BorrowedPKey,
                (borrow, borrowDetail) => new { borrow, borrowDetail })
                .Where(x => x.borrowDetail.IsActive == true)
                .Where(x => x.borrowDetail.PreparedDate.Date >= dateFrom && x.borrowDetail.PreparedDate.Date <= dateTo)
                .Select(x => new ConsolidateFinanceReportDto
                {
                    Id = x.borrowDetail.Id,
                    TransactionDate = x.borrowDetail.PreparedDate.Date,
                    ItemCode = x.borrowDetail.ItemCode,
                    ItemDescription = x.borrowDetail.ItemDescription,
                    Uom = x.borrowDetail.Uom,
                    Category = "",
                    Quantity = Math.Round(x.borrowDetail.Quantity, 2),
                    UnitCost = x.borrowDetail.UnitPrice,
                    LineAmount = Math.Round(x.borrowDetail.UnitPrice * x.borrowDetail.Quantity, 2),
                    Source = x.borrow.Id,
                    TransactionType = "Borrow",
                    Reason = x.borrow.Remarks,
                    Reference = x.borrow.Details,
                    SupplierName = "" ,
                    EncodedBy = x.borrow.PreparedBy,
                    CompanyCode = "",
                    CompanyName = "",
                    DepartmentCode = "",
                    DepartmentName = "",
                    LocationCode = "",
                    LocationName = "",
                    AccountTitleCode = "",
                    AccountTitle = "",
                    EmpId = "",
                    Fullname = "",
                    AssetTag = "",
                    CIPNo = "",
                    Helpdesk = 0,
                    //Remarks = x.borrow.Remarks,
                    Rush = ""

                }).ToList();

            var consumeList = _context.BorrowedConsumes
                .Where(x => x.IsActive == true)
                .Select(x => new BorrowedConsolidatedDto
                {
                    Id = x.Id,
                    BorrowedId = x.BorrowedItemPkey,
                    ItemCode = x.ItemCode,
                    ItemDescription = x.ItemDescription,
                    Uom = x.Uom,
                    Consumed = x.Consume,
                    CompanyCode = x.CompanyCode,
                    CompanyName = x.CompanyName,
                    DepartmentCode = x.DepartmentCode,
                    DepartmentName = x.DepartmentName,
                    LocationCode = x.LocationCode,
                    LocationName = x.LocationName,
                    AccountCode = x.AccountCode,
                    AccountTitles = x.AccountTitles,
                    EmpId = x.EmpId,
                    FullName = x.FullName,
                    ReportNumber = x.ReportNumber,

                });

            var returnList = _context.BorrowedIssueDetails
                .Where(x => x.IsActive == true && x.IsApprovedReturned == true)
                .GroupJoin(consumeList, borrowDetails => borrowDetails.Id, consume => consume.BorrowedId
                , (borrowDetails, consume) => new { borrowDetails, consume })
                .SelectMany(x => x.consume.DefaultIfEmpty(), (x , consume) => new {x.borrowDetails, consume} )
                .Select(x => new BorrowedConsolidatedDto
                {
                    Id = x.borrowDetails.Id,
                    BorrowedId = x.borrowDetails.BorrowedPKey,
                    ItemCode = x.borrowDetails.ItemCode,
                    ItemDescription = x.borrowDetails.ItemDescription,
                    Uom = x.borrowDetails.Uom,
                    BorrowedQuantity = x.borrowDetails.Quantity != null ? x.borrowDetails.Quantity : 0,
                    Consumed = x.consume.Consumed != null ? x.consume.Consumed : 0,
                    CompanyCode = x.consume.CompanyCode,
                    CompanyName = x.consume.CompanyName,
                    DepartmentCode = x.consume.DepartmentCode,
                    DepartmentName = x.consume.DepartmentName,
                    LocationCode = x.consume.LocationCode,
                    LocationName = x.consume.LocationName,
                    AccountCode = x.consume.AccountCode,
                    AccountTitles = x.consume.AccountTitles,
                    EmpId = x.consume.EmpId,
                    FullName = x.consume.FullName,
                    ReportNumber = x.consume.ReportNumber,
                    UnitPrice = x.borrowDetails.UnitPrice
                    
                });

            var borrowedIssueList =  _context.BorrowedIssues
                .Where(x => x.IsActive == true);

            var returnedConsol = returnList
                .GroupJoin(borrowedIssueList, borrowDetail => borrowDetail.BorrowedId, borrow => borrow.Id,
                (borrowDetail, borrow) => new { borrowDetail, borrow })
                .SelectMany(x => x.borrow.DefaultIfEmpty(), (x, borrow) => new { x.borrowDetail, borrow })
                .Where(x => x.borrow.PreparedDate.Date >= dateFrom && x.borrow.PreparedDate.Date <= dateTo)
                .Select(x => new ConsolidateFinanceReportDto
                {

                    Id = x.borrowDetail.Id,
                    TransactionDate = x.borrow.PreparedDate.Date,
                    ItemCode = x.borrowDetail.ItemCode,
                    ItemDescription = x.borrowDetail.ItemDescription,
                    Uom = x.borrowDetail.Uom,
                    Category = "",
                    Quantity = x.borrowDetail.BorrowedQuantity - x.borrowDetail.Consumed,
                    UnitCost = x.borrowDetail.UnitPrice,
                    LineAmount = Math.Round(x.borrowDetail.UnitPrice.Value * x.borrowDetail.BorrowedQuantity - x.borrowDetail.Consumed, 2),
                    Source = x.borrow.Id,
                    TransactionType = "Returned",
                    Reason = x.borrowDetail.Remarks,
                    Reference = x.borrowDetail.Details,
                    SupplierName= "",
                    EncodedBy = x.borrow.PreparedBy,
                    CompanyCode = x.borrowDetail.CompanyCode,
                    CompanyName = x.borrowDetail.CompanyName,
                    DepartmentCode = x.borrowDetail.DepartmentCode,
                    DepartmentName = x.borrowDetail.DepartmentName,
                    LocationCode = x.borrowDetail.LocationCode,
                    LocationName = x.borrowDetail.LocationName,
                    AccountTitleCode = x.borrowDetail.AccountCode,
                    AccountTitle = x.borrowDetail.AccountTitles,
                    EmpId = x.borrowDetail.EmpId,
                    Fullname = x.borrowDetail.FullName,
                    AssetTag = "",
                    CIPNo = "",
                    Helpdesk = 0,
                    //Remarks = x.borrowDetail.Remarks,
                    Rush = ""

                }).ToList();

            var consolidateList = receivingConsol
                .Concat(moveOrderConsol)
                .Concat(receiptConsol) 
                .Concat(issueConsol)
                .Concat(borrowedConsol)
                .Concat(returnedConsol)
                .ToList();

            var materials = await _context.Materials
                .Include(x => x.Uom)
                .Include(x => x.ItemCategory)
                .ToListAsync();

            var reports = consolidateList
                .Join(materials, 
                 consol => consol.ItemCode, material => material.ItemCode,
                 (consol, material) =>  new ConsolidateFinanceReportDto
                 {
                     Id = consol.Id,
                     TransactionDate = consol.TransactionDate,
                     ItemCode = material.ItemCode,
                     ItemDescription = material.ItemDescription,
                     Uom = material.Uom.UomCode,
                     Category = material.ItemCategory.ItemCategoryName,
                     Quantity = consol.Quantity,
                     UnitCost = consol.UnitCost,
                     LineAmount = consol.LineAmount,
                     Source = consol.Source,
                      TransactionType = consol.TransactionType,
                     Reason = consol.Reason,
                     Reference = consol.Reference,
                     SupplierName = consol.SupplierName,
                     EncodedBy = consol.EncodedBy,
                     CompanyCode = consol.CompanyCode,
                     CompanyName = consol.CompanyName,
                     DepartmentCode = consol.DepartmentCode,
                     DepartmentName = consol.DepartmentName,
                     LocationCode = consol.LocationCode,
                     LocationName = consol.LocationName,
                     AccountTitleCode = consol.AccountTitleCode,
                     AccountTitle = consol.AccountTitle,
                     EmpId = consol.EmpId,
                     Fullname = consol.Fullname,
                     AssetTag = consol.AssetTag,
                     CIPNo = consol.CIPNo,
                     Helpdesk = consol.Helpdesk,
                     //Remarks = consol.Remarks,
                     Rush = consol.Rush

                 }).ToList();

            if(!string.IsNullOrEmpty(Search))
            {
               reports = reports.Where(x => x. ItemCode.ToLower().Contains(Search.ToLower())
               || x.ItemDescription.ToLower().Contains(Search.ToLower()) 
               || x.Source.ToString().Contains(Search)
               || x.TransactionType.ToLower().Contains(Search.ToLower())).ToList();
            }

            return reports;
        }
    }

}
