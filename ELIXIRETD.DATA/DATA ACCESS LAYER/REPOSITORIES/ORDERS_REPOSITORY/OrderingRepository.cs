using ELIXIRETD.DATA.CORE.API_RESPONSE;
using ELIXIRETD.DATA.CORE.INTERFACES.Orders;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.MoveOrderDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.Notification_Dto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.PreperationDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.TransactDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Net.WebSockets;
using System.Text.RegularExpressions;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.OrderingRepository
{
    public class OrderingRepository : IOrdering
    {
        private readonly StoreContext _context;

        public OrderingRepository(StoreContext context)
        {
            _context = context;
        }

      

        // ========================================== Schedule Prepare ===========================================================




        public async Task<IReadOnlyList<GetAllListofOrdersDto>> GetAllListofOrders(string Customer)
        {
            var datenow = DateTime.Now;

            var getWarehouseStock = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                              .GroupBy(x => new
                                                              {
                                                                  x.ItemCode,

                                                              }).Select(x => new WarehouseInventory
                                                              {
                                                                  ItemCode = x.Key.ItemCode,
                                                                  ActualGood = x.Sum(x => x.ActualGood)
                                                              });


            var getOrderingReserve = _context.Orders.Where(x => x.IsActive == true)
                                                    .Where(x => x.PreparedDate != null)
            .GroupBy(x => new
            {
                x.ItemCode,

            }).Select(x => new OrderingInventory
            {
                ItemCode = x.Key.ItemCode,
                QuantityOrdered = x.Sum(x => x.QuantityOrdered)
            });

            //var getIssueReceipt = _context.WarehouseReceived.Where(x => x.TransactionType == "MiscellaneousReceipt")
            //                                                .Where(x => x.IsWarehouseReceived == true)
            //                                                .Where(x => x.IsActive == true)
            //                                                .GroupBy(x => new
            //                                                {
            //                                                    x.ItemCode,

            //                                                }).Select(x => new IssueInventoryDto
            //                                                {

            //                                                    ItemCode = x.Key.ItemCode,
            //                                                    Quantity = x.Sum(x => x.ActualGood)
            //                                                });


            var getIssueOut = _context.MiscellaneousIssueDetail.Where(x => x.IsActive == true)
                                                               .Where(x => x.IsTransact == true)
                                                               .GroupBy(x => new
                                                               {
                                                                   x.ItemCode,

                                                               }).Select(x => new IssueInventoryDto
                                                               {

                                                                   ItemCode = x.Key.ItemCode,
                                                                   Quantity = x.Sum(x => x.Quantity)
                                                               });

            var getBorrowedIssue = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                                .Where(x => x.IsApproved == true)
                                                                .GroupBy(x => new
                                                                {

                                                                    x.ItemCode,
                                                                }).Select(x => new IssueInventoryDto
                                                                {

                                                                    ItemCode = x.Key.ItemCode,
                                                                    Quantity = x.Sum(x => x.Quantity)

                                                                });

            var BorrowedReturn = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                            .Where(x => x.IsReturned == true)
                                                            .Where(x => x.IsApprovedReturned == true)
                                                            .GroupBy(x => new
                                                            {
                                                                x.ItemCode,

                                                            }).Select(x => new ItemStocksDto
                                                            {

                                                                ItemCode = x.Key.ItemCode,
                                                                In = x.Sum(x => x.ReturnQuantity),

                                                            });



            var getReserve = getWarehouseStock
                .GroupJoin(getOrderingReserve, warehouse => warehouse.ItemCode, ordering => ordering.ItemCode, (warehouse, ordering) => new { warehouse, ordering })
                .SelectMany(x => x.ordering.DefaultIfEmpty(), (x, ordering) => new { x.warehouse, ordering })
                .GroupJoin(getIssueOut, warehouse => warehouse.warehouse.ItemCode, issue => issue.ItemCode, (warehouse, issue) => new { warehouse, issue })
                .SelectMany(x => x.issue.DefaultIfEmpty(), (x, issue) => new { x.warehouse, issue })
                .GroupJoin(BorrowedReturn, warehouse => warehouse.warehouse.warehouse.ItemCode, returned => returned.ItemCode, (warehouse, returned) => new { warehouse, returned })
                .SelectMany(x => x.returned.DefaultIfEmpty(), (x, returned) => new { x.warehouse, returned })
                .GroupJoin(getBorrowedIssue, warehouse => warehouse.warehouse.warehouse.warehouse.ItemCode, borrowed => borrowed.ItemCode, (warehouse, borrowed) => new { warehouse, borrowed })
                .SelectMany(x => x.borrowed.DefaultIfEmpty(), (x, borrowed) => new { x.warehouse, borrowed })
                //.GroupJoin(getIssueReceipt, warehouse => warehouse.warehouse.warehouse.warehouse.warehouse.ItemCode, receipts => receipts.ItemCode , (warehouse, receipts) => new {warehouse,receipts} )
                //.SelectMany(x => x.receipts.DefaultIfEmpty() , (x, receipts) => new {x.warehouse , receipts})
                .GroupBy(x => x.warehouse.warehouse.warehouse.warehouse.ItemCode)
                .Select(total => new ReserveInventory
                {

                    ItemCode = total.Key,
                    Reserve = total.Sum(x => x.warehouse.warehouse.warehouse.warehouse.ActualGood != null ? x.warehouse.warehouse.warehouse.warehouse.ActualGood : 0) +
                              total.Sum(x => x.warehouse.returned.In != null ? x.warehouse.returned.In : 0) /*+*/
                             /*  total.Sum(x => x.receipts.Quantity != null ? x.receipts.Quantity : 0)*/ -
                               total.Sum(x => x.warehouse.warehouse.issue.Quantity != null ? x.warehouse.warehouse.issue.Quantity : 0) -
                                total.Sum(x => x.warehouse.warehouse.warehouse.ordering.QuantityOrdered != null ? x.warehouse.warehouse.warehouse.ordering.QuantityOrdered : 0) -
                               total.Sum(x => x.borrowed.Quantity != null ? x.borrowed.Quantity : 0),

                });


            var orders = _context.Orders
                .Where(ordering => ordering.CustomerName == Customer && ordering.PreparedDate == null && ordering.IsActive == true)
                .GroupJoin(getReserve, ordering => ordering.ItemCode, warehouse => warehouse.ItemCode, (ordering, warehouse) => new { ordering, warehouse })
                .SelectMany(x => x.warehouse.DefaultIfEmpty(), (x, warehouse) => new { x.ordering, warehouse })
                .GroupBy(x => new
                {
                    x.ordering.Id,
                    x.ordering.OrderDate,
                    x.ordering.DateNeeded,
                    x.ordering.Department,
                    x.ordering.CustomerName,
                    x.ordering.Customercode,
                    x.ordering.Category,
                    x.ordering.ItemCode,
                    x.ordering.ItemdDescription,
                    x.ordering.Uom,
                    x.ordering.IsActive,
                    x.ordering.IsPrepared,
                    x.ordering.Rush,
                    x.ordering.StandartQuantity,

                    Reserve = x.warehouse.Reserve != null ? x.warehouse.Reserve : 0

                }).OrderBy(x => x.Key.Rush == null)
                 .ThenBy(x => x.Key.Rush)
                 .ThenBy(x => x.Key.DateNeeded)
                .Select(total => new GetAllListofOrdersDto
                {
                    Id = total.Key.Id,
                    OrderDate = total.Key.OrderDate.ToString("MM/dd/yyyy"),
                    DateNeeded = total.Key.DateNeeded.ToString("MM/dd/yyyy"),
                    Department = total.Key.Department,
                    CustomerCode = total.Key.Customercode,
                    CustomerName = total.Key.CustomerName,
                    Category = total.Key.Category,
                    ItemCode = total.Key.ItemCode,
                    ItemDescription = total.Key.ItemdDescription,
                    Uom = total.Key.Uom,
                    QuantityOrder = total.Sum(x => x.ordering.QuantityOrdered),
                    IsActive = total.Key.IsActive,
                    IsPrepared = total.Key.IsPrepared,
                    StockOnHand = total.Key.Reserve != null ? total.Key.Reserve : 0,
                    Rush = total.Key.Rush,
                    StandardQuantity = total.Key.StandartQuantity

                });

            return await orders.ToListAsync();

        }

        public async Task<bool> GenerateNumber(GenerateOrderNo generate)
        {

            await _context.GenerateOrders.AddAsync(generate);

            return true;
        }


        public async Task<bool> SchedulePreparedDate(Ordering orders)
        {
            var existingOrder = await _context.Orders.Where(x => x.Id == orders.Id)
                                                     .FirstOrDefaultAsync();



            if (existingOrder == null)
                return false;


            existingOrder.IsPrepared = true;
            existingOrder.PreparedDate = orders.PreparedDate;
            existingOrder.OrderNoPKey = orders.OrderNoPKey;

            var hasRushOrders = await _context.Orders
               .AnyAsync(x => x.Id == orders.Id && !string.IsNullOrEmpty(x.Rush));

            existingOrder.IsRush = hasRushOrders;

            if (hasRushOrders)
            {
                var generateOrder = await _context.GenerateOrders
                    .FirstOrDefaultAsync(x => x.Id == existingOrder.OrderNoPKey);

                if (generateOrder != null)
                {
                    generateOrder.Rush = true;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<IReadOnlyList<GetAllListCancelOrdersDto>> GetAllListOfCancelOrders()
        {
            var cancelled = _context.Orders.Where(x => x.CancelDate != null)
                                           .Where(x => x.IsActive == false)

                                           .Select(x => new GetAllListCancelOrdersDto
                                           {
                                               Id = x.Id,
                                               Department = x.Department,
                                               CustomerName = x.CustomerName,
                                               Category = x.Category,
                                               QuantityOrder = x.QuantityOrdered,
                                               OrderDate = x.OrderDate.ToString("MM/dd/yyyy"),
                                               DateNeeded = x.DateNeeded.ToString("MM/dd/yyyy"),
                                               PreparedDate = x.PreparedDate.ToString(),
                                               CancelDate = x.CancelDate.ToString(),
                                               CancelBy = x.IsCancelBy,
                                               IsActive = x.IsActive == true

                                           });
            return await cancelled.ToListAsync();
        }
        public async Task<bool> ReturnCancelOrdersInList(Ordering orders)
        {
            var existing = await _context.Orders.Where(x => x.Id == orders.Id)
                                                .Where(x => x.IsActive == false)
                                                .FirstOrDefaultAsync();

            if (existing == null)
                return false;

            existing.IsActive = true;
            existing.IsCancelBy = null;
            existing.IsCancel = null;
            existing.Remarks = null;
            existing.CancelDate = null;

            return true;
        }

        //========================================== Preparation =======================================================================


        public async Task<IReadOnlyList<OrderSummaryDto>> OrderSummary(string DateFrom, string DateTo)
        {

            CultureInfo usCulture = new CultureInfo("en-US");
            CultureInfo.CurrentCulture = usCulture;

            var dateFrom = DateTime.ParseExact(DateFrom, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            var dateTo = DateTime.ParseExact(DateTo, "MM/dd/yyyy", CultureInfo.InvariantCulture);

            if (dateFrom >= dateTo)
            {
                return new List<OrderSummaryDto>();
            }

            var Totalramaining = _context.WarehouseReceived.GroupBy(x => new
            {
                x.ItemCode,
                x.ItemDescription,


            }).Select(x => new ItemStocksDto
            {
                ItemCode = x.Key.ItemCode,
                ItemDescription = x.Key.ItemDescription,
                Remaining = x.Sum(x => x.ActualDelivered)
            });


            var totalOrders = _context.Orders.GroupBy(x => new
            {
                x.ItemCode,
                x.IsPrepared,
                x.IsActive

            }).Select(x => new OrderSummaryDto
            {
                ItemCode = x.Key.ItemCode,
                TotalOrders = x.Sum(x => x.QuantityOrdered),
                IsPrepared = x.Key.IsPrepared


            }).Where(x => x.IsPrepared == false);

            var orders = _context.Orders
                .Where(ordering => ordering.OrderDate >= DateTime.Parse(DateFrom) && ordering.OrderDate <= DateTime.Parse(DateTo))
                .GroupJoin(totalOrders, ordering => ordering.ItemCode, total => total.ItemCode, (ordering, total) => new { ordering, total })
                .SelectMany(x => x.total.DefaultIfEmpty(), (x, total) => new { x.ordering, total })
                .GroupJoin(Totalramaining, ordering => ordering.ordering.ItemCode, remaining => remaining.ItemCode, (ordering, remaining) => new { ordering, remaining })
                .SelectMany(x => x.remaining.DefaultIfEmpty(), (x, remaining) => new { x.ordering, remaining })
                .GroupBy(x => new
                {
                    x.ordering.ordering.Id,
                    x.ordering.ordering.OrderDate,
                    x.ordering.ordering.DateNeeded,
                    x.ordering.ordering.CustomerName,
                    x.ordering.ordering.Customercode,
                    x.ordering.ordering.Category,
                    x.ordering.ordering.ItemCode,
                    x.ordering.ordering.ItemdDescription,
                    x.ordering.ordering.Uom,
                    x.ordering.ordering.QuantityOrdered,
                    x.ordering.ordering.IsActive,
                    x.ordering.ordering.IsPrepared,
                    x.ordering.ordering.PreparedDate,
                    x.ordering.ordering.IsApproved

                }).Select(total => new OrderSummaryDto
                {
                    Id = total.Key.Id,
                    OrderDate = total.Key.OrderDate.ToString("MM/dd/yyyy"),
                    DateNeeded = total.Key.DateNeeded.ToString("MM/dd/yyyy"),
                    CustomerName = total.Key.CustomerName,
                    CustomerCode = total.Key.Customercode,
                    Category = total.Key.Category,
                    ItemCode = total.Key.ItemCode,
                    ItemDescription = total.Key.ItemdDescription,
                    Uom = total.Key.Uom,
                    QuantityOrder = total.Key.QuantityOrdered,
                    IsActive = total.Key.IsActive,
                    IsPrepared = total.Key.IsPrepared,
                    StockOnHand = total.Sum(x => x.remaining.Remaining),
                    Difference = total.Sum(x => x.remaining.Remaining) - total.Key.QuantityOrdered,
                    PreparedDate = total.Key.PreparedDate.ToString(),
                    IsApproved = total.Key.IsApproved != null

                });


            return await orders.ToListAsync();
        }


        public async Task<IReadOnlyList<DetailedListofOrdersDto>> DetailedListOfOrders(string customer)
        {
            var orders = _context.Orders.OrderBy(x => x.Rush == null)
                                         .ThenBy(x => x.Rush)
                                          .Where(x => x.CustomerName == customer)
                                        .Select(x => new DetailedListofOrdersDto
                                        {
                                            OrderDate = x.OrderDate.ToString("MM/dd/yyyy"),
                                            DateNeeded = x.DateNeeded.ToString("MM/dd/yyyy"),
                                            Department = x.Department,
                                            Category = x.Category,
                                            CustomerName = x.CustomerName,
                                            CustomerCode = x.Customercode,
                                            ItemCode = x.ItemCode,
                                            ItemDescription = x.ItemdDescription,
                                            Uom = x.Uom,
                                            QuantityOrder = x.QuantityOrdered,
                                            Rush = x.Rush


                                        });
            return await orders.ToListAsync();

        }


        // =========================================== MoveOrder ==============================================================================

       





        // ======================================== Move Order Approval ==============================================================

        
      

     
        // =========================================== Transact Order ============================================================

      
     

        // Notification

        public async Task<IReadOnlyList<DtoOrderNotif>> GetOrdersForNotification()
        {
            var orders = _context.Orders.OrderBy(x => x.OrderDate)
                                   .GroupBy(x => new
                                   {
                                       x.TrasactId,
                                       x.CustomerName,
                                       x.IsActive,
                                       x.PreparedDate,
                                       x.Rush

                                   }).Where(x => x.Key.IsActive == true)
                                     .Where(x => x.Key.PreparedDate == null)

                                     .Select(x => new DtoOrderNotif
                                     {
                                         MIRId = x.Key.TrasactId,
                                         CustomerName = x.Key.CustomerName,
                                         IsActive = x.Key.IsActive,
                                         Rush = x.Key.Rush
                                     });

            return await orders.ToListAsync();
        }

        public async Task<IReadOnlyList<DtoForMoveOrderNotif>> GetMoveOrdersForNotification()
        {
            var orders = _context.Orders
                                  .GroupBy(x => new
                                  {
                                      x.CustomerName,
                                      x.IsActive,
                                      x.IsApproved,
                                      x.IsMove,
                                      x.Rush

                                  }).Where(x => x.Key.IsActive == true)
                                    .Where(x => x.Key.IsApproved == true)
                                    .Where(x => x.Key.IsMove == false)
                                    .Select(x => new DtoForMoveOrderNotif
                                    {
                                        CustomerName = x.Key.CustomerName,
                                        IsActive = x.Key.IsActive,
                                        IsApproved = x.Key.IsApproved != null,
                                        Rush = x.Key.Rush
                                    });

            return await orders.ToListAsync();
        }


        public async Task<IReadOnlyList<DtoForTransactNotif>> GetAllForTransactMoveOrderNotification()
        {
            var orders = _context.MoveOrders.Where(x => x.IsActive == true)
                                         .Where(x => x.IsTransact == false)
             .GroupBy(x => new
             {
                 x.OrderNo,
                 x.Customercode,
                 x.CustomerName,
                 x.OrderDate,
                 x.DateNeeded,
                 x.PreparedDate,
                 x.IsApprove,
                 x.IsTransact,
                 x.Rush

             }).Where(x => x.Key.IsApprove == true)

        .Select(x => new DtoForTransactNotif
        {
            MIRId = x.Key.OrderNo,
            CustomerCode = x.Key.Customercode,
            CustomerName = x.Key.CustomerName,
            TotalOrder = x.Sum(x => x.QuantityOrdered),
            OrderDate = x.Key.OrderDate.ToString("MM/dd/yyyy"),
            DateNeeded = x.Key.DateNeeded.ToString("MM/dd/yyyy"),
            PrepareDate = x.Key.PreparedDate.ToString(),
            IsApproved = x.Key.IsApprove != null,
            Rush = x.Key.Rush


        });

            return await orders.ToListAsync();

        }

        public async Task<IReadOnlyList<DtoForApprovalMoveOrderNotif>> GetForApprovalMoveOrdersNotification()
        {
            var orders = _context.MoveOrders.Where(x => x.IsApproveReject == null)
            .GroupBy(x => new
            {

                x.OrderNo,
                x.Customercode,
                x.CustomerName,
                x.OrderDate,
                x.PreparedDate,
                x.IsApprove,
                x.IsPrepared,
                x.Rush

            }).Where(x => x.Key.IsApprove != true)
              .Where(x => x.Key.IsPrepared == true)

       .Select(x => new DtoForApprovalMoveOrderNotif
       {
           MIRId = x.Key.OrderNo,
           CustomerCode = x.Key.Customercode,
           CustomerName = x.Key.CustomerName,
           Quantity = x.Sum(x => x.QuantityOrdered),
           OrderDate = x.Key.OrderDate.ToString(),
           PreparedDate = x.Key.PreparedDate.ToString(),
           Rush = x.Key.Rush

       });

            return await orders.ToListAsync();

        }

        public async Task<IReadOnlyList<DtoRejectMoveOrderNotif>> GetRejectMoveOrderNotification()
        {
            var orders = _context.MoveOrders.Where(x => x.IsApproveReject == true)
           .GroupBy(x => new
           {

               x.OrderNo,
               x.Customercode,
               x.CustomerName,
               x.OrderDate,
               x.PreparedDate,
               x.IsApprove,
               x.IsReject,
               x.RejectedDateTempo,
               x.Remarks,
               x.Rush

           }).Select(x => new DtoRejectMoveOrderNotif
           {
               MIRId = x.Key.OrderNo,
               CustomerCode = x.Key.Customercode,
               CustomerName = x.Key.CustomerName,
               Quantity = x.Sum(x => x.QuantityOrdered),
               OrderDate = x.Key.OrderDate.ToString(),
               PreparedDate = x.Key.PreparedDate.ToString(),
               IsReject = x.Key.IsReject != null,
               RejectedDate = x.Key.RejectedDateTempo.ToString(),
               Remarks = x.Key.Remarks,
               Rush = x.Key.Rush

           });


            return await orders.ToListAsync();
        }


        public async Task<IReadOnlyList<GetallApproveDto>> GetAllListForApprovalOfSchedule()
        {

            var orders = _context.Orders
                                      .Where(x => x.IsApproved == null)
                                      .Where(x => x.PreparedDate != null)
                                      .OrderBy(x => x.PreparedDate)
                                      .Where(x => x.IsActive == true)
                                      .GroupBy(x => new
                                      {
                                          x.TrasactId,
                                          x.Department,
                                          x.CustomerName,
                                          x.Customercode,
                                          x.Category,
                                          x.PreparedDate,
                                          x.IsApproved,
                                          x.IsActive,
                                          x.Rush,


                                      })//}).Where(x => x.Key.IsApproved == null)
                                        //    .Where(x => x.Key.PreparedDate != null)
                                        //    .Where(x => x.Key.IsActive == true)

            .Select(x => new GetallApproveDto
            {
                MIRId = x.Key.TrasactId,
                Department = x.Key.Department,
                CustomerName = x.Key.CustomerName,
                CustomerCode = x.Key.Customercode,
                Category = x.Key.Category,
                TotalOrders = x.Sum(x => x.QuantityOrdered),
                PreparedDate = x.Key.PreparedDate.ToString(),
                IsRush = x.Key.Rush != null ? true : false,
                Rush = x.Key.Rush


            });

            return await orders.ToListAsync();

        }








        //================================= Validation =============================================================================

        public async Task<bool> ValidateDateNeeded(Ordering orders)
        {
            var dateNow = DateTime.Now.AddDays(-5);

            if (Convert.ToDateTime(orders.DateNeeded).Date < dateNow.Date)
                return false;
            return true;

        }

        public async Task<bool> ValidateExistOrderandItemCode(int TransactId, string ItemCode, string customertype, string itemdescription, string customercode)
        {
            var validate = await _context.Orders.Where(x => x.TrasactId == TransactId)
                                                    .Where(x => x.Customercode == customercode)
                                                    .Where(x => x.CustomerType == customertype)
                                                    .Where(x => x.ItemCode == ItemCode)
                                                    //.Where(x => x.CustomerType == CustomerType)
                                                    .Where(x => x.ItemdDescription == itemdescription)
                                                    .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateUom(string Uom)
        {
            var validate = await _context.Uoms.Where(x => x.UomCode == Uom)
                                                  .Where(x => x.IsActive == true)
                                                  .FirstOrDefaultAsync();
            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateItemCode(string ItemCode, string itemdescription)
        {
            var validate = await _context.Materials.Where(x => x.ItemCode == ItemCode)
                                                .Where(x => x.ItemDescription == itemdescription)
                                                .Where(x => x.IsActive == true)
                                                .FirstOrDefaultAsync();
            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateItemDescription(string ItemDescription)
        {
            var validate = await _context.Materials.Where(x => x.ItemDescription == ItemDescription)
                                                   .Where(x => x.IsActive == true)
                                                   .FirstOrDefaultAsync();


            if (validate == null)
                return false;



            return true;
        }



        public async Task<bool> ValidateCustomerCode(string Customer)
        {
            var validate = await _context.Customers.Where(x => x.CustomerCode == Customer)
                                                 .Where(x => x.IsActive == true)
                                                 .FirstOrDefaultAsync();
            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateCustomerName(string Customer, string CustomerName, string CustomerType)
        {
            var validate = await _context.Customers.Where(x => x.CustomerCode == Customer)
                                                 .Where(x => x.CustomerName == CustomerName)
                                                 .Where(x => x.CustomerType == CustomerType)
                                                 .Where(x => x.IsActive == true)
                                                 .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;
        }



        public async Task<bool> ValidatePrepareDate(Ordering orders)
        {
            var dateNow = DateTime.Now;

            if (Convert.ToDateTime(orders.PreparedDate).Date < dateNow.Date)
                return false;
            return true;

        }

        public async Task<bool> ValidateWarehouseId(int id, string itemcode)
        {
            var validate = await _context.WarehouseReceived.Where(x => x.Id == id)
                                                           .Where(x => x.ItemCode == itemcode)
                                                           .Where(x => x.IsActive == true)
                                                            .FirstOrDefaultAsync();

            if (validate == null)
                return false;
            return true;
        }

        public async Task<bool> ValidateQuantity(decimal quantity)
        {
            var existingQuantity = await _context.Orders
                                 .Where(x => x.QuantityOrdered == quantity)
                                 .FirstOrDefaultAsync();

            if (existingQuantity == null)
                return false;

            return true;
        }




        //=========================================== MIR Update In Ordering Preparation Schedule============================================================

        //Customer
        public async Task<bool> AddNewOrders(Ordering Orders)
        {

            var existing = await _context.Customers.Where(x => x.CustomerName == Orders.CustomerName)
                                                   .FirstOrDefaultAsync();
            if (existing == null)
                return false;


            Orders.IsActive = true;
            Orders.StandartQuantity = Orders.QuantityOrdered;


            await _context.Orders.AddAsync(Orders);
            return true;

        }




        public async Task<PagedList<GetAllListofOrdersPaginationDto>> GetAllListofOrdersPagination(UserParams userParams/*, bool status*/)
        {

            var orders = _context.Orders.OrderBy(x => x.SyncDate)

                     .GroupBy(x => new
                     {
                         x.CustomerName,
                         x.IsActive,
                         x.PreparedDate,
                         //x.TrasactId,
                         //x.Rush,


                     })
                       .Where(x => x.Key.IsActive == true)
                       .Where(x => x.Key.PreparedDate == null)
                       .Select(x => new GetAllListofOrdersPaginationDto
                       {
                           CustomerName = x.Key.CustomerName,
                           IsActive = x.Key.IsActive,
                           //Rush = x.Key.Rush != null ? true : false,
                           ////MIRId = x.Key.TrasactId

                       })/*.Where(x => x.Rush == status)*/;

            return await PagedList<GetAllListofOrdersPaginationDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);


        }


        public async Task<PagedList<GetAllListofOrdersPaginationDto>> GetAllListofOrdersPaginationOrig(UserParams userParams, string search /*, bool status*/)
        {

            var orders = _context.Orders.OrderBy(x => x.SyncDate)  /*.OrderBy(x => x.Rush == null ? 1 : 0)*/
                                   //                              //.ThenBy(x => x.Rush)
                                   .OrderBy(x => x.TrasactId)


                                 .GroupBy(x => new
                                 {
                                     x.CustomerName,
                                     x.IsActive,
                                     x.PreparedDate,
                                     //x.TrasactId,
                                     //x.Rush,

                                 })/*.OrderByDescending(x => x.Key.Dat)*/
                                   .Where(x => x.Key.IsActive == true)
                                   .Where(x => x.Key.PreparedDate == null)

                                   .Select(x => new GetAllListofOrdersPaginationDto
                                   {
                                       CustomerName = x.Key.CustomerName,
                                       IsActive = x.Key.IsActive,
                                       //Rush = x.Key.Rush != null ? true : false,
                                       //MIRId = x.Key.TrasactId

                                   })/*.Where(x => x.Rush == status)*/
                                    .Where(x => Convert.ToString(x.CustomerName).ToLower().Contains(search.Trim().ToLower()));

            return await PagedList<GetAllListofOrdersPaginationDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);

        }


        //Viewing
        public async Task<IReadOnlyList<GetAllListOfMirDto>> GetAllListOfMir(string Customer, bool status)
        {
            //bool status;

            // if(IsRush != null)
            //{
            //    status = true;
            //}
            //else
            //{
            //    status = false;
            //}

            var orders = _context.Orders.Where(x => x.CustomerName == Customer)
                                        .Where(x => x.IsActive == true)
                                        .Where(x => x.IsPrepared != true)
                                        .GroupBy(x => new
                                        {

                                            x.Customercode,
                                            x.CustomerName,
                                            x.CustomerType,
                                            x.TrasactId,
                                            x.DateNeeded,
                                            x.OrderDate,
                                            x.Rush,

                                        }).Select(x => new GetAllListOfMirDto
                                        {

                                            MIRId = x.Key.TrasactId,
                                            CustomerCode = x.Key.Customercode,
                                            CustomerName = x.Key.CustomerName,
                                            CustomerType = x.Key.CustomerType,
                                            TotalQuantity = x.Sum(x => x.QuantityOrdered),
                                            DateNeeded = x.Key.DateNeeded.ToString(),
                                            OrderedDate = x.Key.OrderDate.ToString(),
                                            IsRush = x.Key.Rush != null ? true : false,
                                            Rush = x.Key.Rush

                                        }).Where(x => x.IsRush == status);

            return await orders.ToListAsync();
        }

        public async Task<IReadOnlyList<GetAllListOfMirDto>> GetAllListOfMirOrders(string Customer)
        {

            return await _context.Orders.Where(x => x.CustomerName == Customer)
                                        .Where(x => x.IsActive == true)
                                        .Where(x => x.IsPrepared != true)
                                        .GroupBy(x => new
                                        {

                                            x.Customercode,
                                            x.CustomerName,
                                            x.CustomerType,
                                            x.TrasactId,
                                            x.DateNeeded,
                                            x.OrderDate,
                                            x.Rush,

                                        }).Select(x => new GetAllListOfMirDto
                                        {

                                            MIRId = x.Key.TrasactId,
                                            CustomerCode = x.Key.Customercode,
                                            CustomerName = x.Key.CustomerName,
                                            CustomerType = x.Key.CustomerType,
                                            TotalQuantity = x.Sum(x => x.QuantityOrdered),
                                            DateNeeded = x.Key.DateNeeded.ToString(),
                                            OrderedDate = x.Key.OrderDate.ToString(),
                                            IsRush = x.Key.Rush != null ? true : false,
                                            Rush = x.Key.Rush

                                        }).ToListAsync();

        }




        //private static Dictionary<string, decimal> stockOnHandDict = new Dictionary<string, decimal>();

        public async Task<IEnumerable<AllOrdersPerMIRIDsDTO>> GetAllListOfMirOrdersbyMirId(int[] listofMirIds, string customerName)
        {
            var result = new List<AllOrdersPerMIRIDsDTO>();

            var datenow = DateTime.Now;

            var getWarehouseStock = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                              .GroupBy(x => new
                                                              {
                                                                  x.ItemCode,

                                                              }).Select(x => new WarehouseInventory
                                                              {
                                                                  ItemCode = x.Key.ItemCode,
                                                                  ActualGood = x.Sum(x => x.ActualGood)
                                                              });


            var getOrderingReserve = _context.Orders.Where(x => x.IsActive == true)
                                                    .Where(x => x.PreparedDate != null)
            .GroupBy(x => new
            {
                x.ItemCode,

            }).Select(x => new OrderingInventory
            {
                ItemCode = x.Key.ItemCode,
                QuantityOrdered = x.Sum(x => x.QuantityOrdered)
            });


            var getIssueOut = _context.MiscellaneousIssueDetail.Where(x => x.IsActive == true)
                                                               .Where(x => x.IsTransact == true)
                                                               .GroupBy(x => new
                                                               {
                                                                   x.ItemCode,

                                                               }).Select(x => new IssueInventoryDto
                                                               {

                                                                   ItemCode = x.Key.ItemCode,
                                                                   Quantity = x.Sum(x => x.Quantity)
                                                               });

            var getBorrowedIssue = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                                .Where(x => x.IsApproved == true)
                                                                .GroupBy(x => new
                                                                {

                                                                    x.ItemCode,
                                                                }).Select(x => new IssueInventoryDto
                                                                {

                                                                    ItemCode = x.Key.ItemCode,
                                                                    Quantity = x.Sum(x => x.Quantity)

                                                                });

            var BorrowedReturn = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                            .Where(x => x.IsReturned == true)
                                                            .Where(x => x.IsApprovedReturned == true)
                                                            .GroupBy(x => new
                                                            {
                                                                x.ItemCode,

                                                            }).Select(x => new ItemStocksDto
                                                            {

                                                                ItemCode = x.Key.ItemCode,
                                                                In = x.Sum(x => x.ReturnQuantity),

                                                            });

            var getReserve = getWarehouseStock
              .GroupJoin(getOrderingReserve, warehouse => warehouse.ItemCode, ordering => ordering.ItemCode, (warehouse, ordering) => new { warehouse, ordering })
              .SelectMany(x => x.ordering.DefaultIfEmpty(), (x, ordering) => new { x.warehouse, ordering })
              .GroupJoin(getIssueOut, warehouse => warehouse.warehouse.ItemCode, issue => issue.ItemCode, (warehouse, issue) => new { warehouse, issue })
              .SelectMany(x => x.issue.DefaultIfEmpty(), (x, issue) => new { x.warehouse, issue })
              .GroupJoin(BorrowedReturn, warehouse => warehouse.warehouse.warehouse.ItemCode, returned => returned.ItemCode, (warehouse, returned) => new { warehouse, returned })
              .SelectMany(x => x.returned.DefaultIfEmpty(), (x, returned) => new { x.warehouse, returned })
              .GroupJoin(getBorrowedIssue, warehouse => warehouse.warehouse.warehouse.warehouse.ItemCode, borrowed => borrowed.ItemCode, (warehouse, borrowed) => new { warehouse, borrowed })
              .SelectMany(x => x.borrowed.DefaultIfEmpty(), (x, borrowed) => new { x.warehouse, borrowed })

              .GroupBy(x => x.warehouse.warehouse.warehouse.warehouse.ItemCode)
              .Select(total => new ReserveInventory
              {

                  ItemCode = total.Key,
                  Reserve = total.Sum(x => x.warehouse.warehouse.warehouse.warehouse.ActualGood != null ? x.warehouse.warehouse.warehouse.warehouse.ActualGood : 0) +
                            total.Sum(x => x.warehouse.returned.In != null ? x.warehouse.returned.In : 0) -
                             total.Sum(x => x.warehouse.warehouse.issue.Quantity != null ? x.warehouse.warehouse.issue.Quantity : 0) -
                              total.Sum(x => x.warehouse.warehouse.warehouse.ordering.QuantityOrdered != null ? x.warehouse.warehouse.warehouse.ordering.QuantityOrdered : 0) -
                             total.Sum(x => x.borrowed.Quantity != null ? x.borrowed.Quantity : 0),

              });


            var itemCodes = new HashSet<string>(listofMirIds.Select(id => id.ToString()));
            var stockOnHandDict = new Dictionary<string, decimal>();

            var orders = _context.Orders
                .Where(ordering => ordering.CustomerName == customerName && ordering.PreparedDate == null && ordering.IsActive == true)
                .Where(x => listofMirIds.Contains(x.TrasactId))
                .GroupJoin(getReserve, ordering => ordering.ItemCode, warehouse => warehouse.ItemCode, (ordering, warehouse) => new { ordering, warehouse })
                .SelectMany(x => x.warehouse.DefaultIfEmpty(), (x, warehouse) => new { x.ordering, warehouse })
                .GroupBy(x => new
                {
                    x.ordering.Id,
                    x.ordering.TrasactId,
                    x.ordering.ItemCode,
                    x.ordering.ItemdDescription,
                    x.ordering.Uom,
                    //x.ordering.IsActive,
                    //x.ordering.IsPrepared,
                    x.ordering.StandartQuantity,

                    Reserve = x.warehouse.Reserve != null ? x.warehouse.Reserve : 0
                    
                })
                .Select(total => new AllOrdersPerMIRIDsDTO
                {

                    Id = total.Key.Id,
                    MIRId = total.Key.TrasactId,
                    ItemCode = total.Key.ItemCode,
                    ItemDescription = total.Key.ItemdDescription,
                    Uom = total.Key.Uom,
                    QuantityOrder = total.Sum(x => x.ordering.QuantityOrdered),
                    //IsActive = total.Key.IsActive,
                    ////IsPrepared = total.Key.IsPrepared,
                    StockOnHand = total.Key.Reserve != null ? total.Key.Reserve : 0,
                    StandardQuantity = total.Key.StandartQuantity

                }).ToList();


            foreach (var order in orders)
            {
                if (!stockOnHandDict.ContainsKey(order.ItemCode))
                {
                    stockOnHandDict[order.ItemCode] = order.StockOnHand;
                }

                if (listofMirIds.Contains(order.MIRId))
                {
                    stockOnHandDict[order.ItemCode] -= order.QuantityOrder;
                }

                order.StockOnHand = stockOnHandDict[order.ItemCode];
            }


            var itemCodess = orders.Select(x => x.ItemCode).Distinct();
            foreach (var itemCode in itemCodess)
            {
                var stockOnHand = stockOnHandDict[itemCode];
                foreach (var order in orders.Where(x => x.ItemCode == itemCode))
                {
                    order.StockOnHand = stockOnHand;
                }
            }


            result.AddRange(orders);
            return result;

        }

        public async Task<bool> PreparationOfSchedule(Ordering orders)
        {

            var Ordertransact = await _context.Orders.Where(x => x.TrasactId == orders.TrasactId)
                                             .ToListAsync();


            foreach (var items in Ordertransact)
            {

                items.IsPrepared = true;
                items.PreparedDate = orders.PreparedDate;
            }


            return true;
        }



        public async Task<bool> EditQuantityOrder(Ordering orders)
        {
            var existingOrder = await _context.Orders.Where(x => x.Id == orders.Id)
                                                     .FirstOrDefaultAsync();

            existingOrder.QuantityOrdered = orders.QuantityOrdered;

            return true;
        }

        public async Task<bool> CancelOrders(Ordering orders)
        {
            var existing = await _context.Orders.Where(x => x.Id == orders.Id)
                                                 .Where(x => x.IsActive == true)
                                                 .FirstOrDefaultAsync();

            if (existing == null)
                return false;

            existing.IsActive = false;
            existing.IsCancelBy = orders.IsCancelBy;
            existing.IsCancel = true;
            existing.CancelDate = DateTime.Now;
            existing.Remarks = orders.Remarks;

            return true;
        }


        //=============================================== MIR Update In Ordering For Approval =======================================================================


        public async Task<IReadOnlyList<GetallApproveDto>> GetAllListForApprovalOfSchedule(bool status)
        {

            var orders = _context.Orders
                                      .Where(x => x.IsApproved == null)
                                      .Where(x => x.PreparedDate != null)
                                      .OrderBy(x => x.PreparedDate)
                                      .Where(x => x.IsActive == true)
                                      .GroupBy(x => new
                                      {
                                          x.TrasactId,
                                          x.Department,
                                          x.CustomerName,
                                          x.Customercode,
                                          x.PreparedDate,
                                          x.IsApproved,
                                          x.IsActive,
                                          x.Rush,


                                      })//}).Where(x => x.Key.IsApproved == null)
                                        //    .Where(x => x.Key.PreparedDate != null)
                                        //    .Where(x => x.Key.IsActive == true)

            .Select(x => new GetallApproveDto
            {
                MIRId = x.Key.TrasactId,
                Department = x.Key.Department,
                CustomerName = x.Key.CustomerName,
                CustomerCode = x.Key.Customercode,
                TotalOrders = x.Sum(x => x.QuantityOrdered),
                PreparedDate = x.Key.PreparedDate.ToString(),
                IsRush = x.Key.Rush != null ? true : false,
                Rush = x.Key.Rush


            }).Where(x => x.IsRush == status);

            return await orders.ToListAsync();

        }

        public async Task<IReadOnlyList<GetallOrderfroScheduleApproveDto>> GetAllOrdersForScheduleApproval(int Id)
        {
            var orders = _context.Orders.OrderBy(x => x.PreparedDate)
                                        .Where(x => x.TrasactId == Id)
                                        .Where(x => x.IsPrepared == true)
                                        .Where(x => x.IsApproved == null)
                                        .Select(x => new GetallOrderfroScheduleApproveDto
                                        {
                                            MIRId = x.TrasactId,
                                            OrderDate = x.OrderDate.ToString("MM/dd/yyyy"),
                                            DateNeeded = x.DateNeeded.ToString("MM/dd/yyyy"),
                                            Department = x.Department,
                                            CustomerName = x.CustomerName,
                                            CustomerCode = x.Customercode,
                                            ItemCode = x.ItemCode,
                                            ItemDescription = x.ItemdDescription,
                                            Category = x.Category,
                                            Uom = x.Uom,
                                            QuantityOrder = x.QuantityOrdered,
                                            //IsApproved = x.IsApproved != null,
                                            //Rush = x.Rush

                                        });

            return await orders.ToListAsync();

        }

        public async Task<bool> ApprovePreparedDate(Ordering orders)
        {
            var order = await _context.Orders.Where(x => x.TrasactId == orders.TrasactId)
                                             .Where(x => x.IsActive == true)
                                             .ToListAsync();

            foreach (var items in order)
            {
                items.IsApproved = true;
                items.ApprovedDate = DateTime.Now;
                items.RejectBy = null;
                items.RejectedDate = null;
                items.Remarks = null;
            }
            return true;
        }


        public async Task<bool> RejectPreparedDate(Ordering orders)
        {
            var order = await _context.Orders.Where(x => x.TrasactId == orders.TrasactId)
                                             .Where(x => x.IsActive == true)
                                             .ToListAsync();
            foreach (var items in order)
            {
                items.IsReject = true;
                items.RejectBy = orders.RejectBy;
                items.IsActive = true;

                items.Remarks = orders.Remarks;
                items.RejectedDate = DateTime.Now;
                items.PreparedDate = null;
                items.IsPrepared = false;
                //items.OrderNoPKey = 0;
                //items.IsRush = null;
            }

            //var orderPkey = await _context.GenerateOrders.Where(x => x.Id == orders.OrderNoPKey)
            //                                .FirstOrDefaultAsync();

            //orderPkey.IsActive = false;

            return true;
        }


        public async Task<IReadOnlyList<GetAllCalendarApproveDto>> GetAllApprovedOrdersForCalendar(bool status)
        {

            var orders = _context.Orders.GroupBy(x => new
            {
                x.TrasactId,
                x.Department,
                x.CustomerName,
                x.Customercode,
                x.PreparedDate,
                x.IsApproved,
                x.IsMove,
                x.IsReject,
                x.Remarks,
                x.Rush

            }).Where(x => x.Key.IsApproved == true)
              .Where(x => x.Key.PreparedDate != null)
              .Where(x => x.Key.IsMove == false)

              .Select(x => new GetAllCalendarApproveDto
              {

                  MIRId = x.Key.TrasactId,
                  Department = x.Key.Department,
                  CustomerName = x.Key.CustomerName,
                  CustomerCode = x.Key.Customercode,
                  TotalOrders = x.Sum(x => x.QuantityOrdered),
                  PreparedDate = x.Key.PreparedDate.ToString(),
                  IsMove = x.Key.IsMove,
                  IsReject = x.Key.IsReject != null,
                  Remarks = x.Key.Remarks,
                  IsRush = x.Key.Rush != null ? true : false,
                  Rush = x.Key.Rush
              }).Where(x => x.IsRush == status);

            return await orders.ToListAsync();

        }

        //=============================================== MIR Update in MoveOrder For Preparation =======================================================================

        public async Task<PagedList<GetAllListForMoveOrderPaginationDto>> GetAllListForMoveOrderPagination(UserParams userParams)
        {

            var orders = _context.Orders
                                 .GroupBy(x => new
                                 {
                                     x.CustomerName,
                                     x.IsActive,
                                     x.IsApproved,
                                     x.IsMove

                                 }).Where(x => x.Key.IsActive == true)
                                   .Where(x => x.Key.IsApproved == true)
                                    .Where(x => x.Key.IsMove == false)
                                   .Select(x => new GetAllListForMoveOrderPaginationDto
                                   {
                                       CustomerName = x.Key.CustomerName,
                                       IsActive = x.Key.IsActive,
                                       IsApproved = x.Key.IsApproved != null

                                   });

            return await PagedList<GetAllListForMoveOrderPaginationDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);

        }


        public async Task<PagedList<GetAllListForMoveOrderPaginationDto>> GetAllListForMoveOrderPaginatioOrig(UserParams userParams, string search)
        {

            var orders = _context.Orders
                                 .GroupBy(x => new
                                 {
                                     x.CustomerName,
                                     x.IsActive,
                                     x.IsApproved,
                                     x.IsMove

                                 }).Where(x => x.Key.IsActive == true)
                                   .Where(x => x.Key.IsApproved == true)
                                    .Where(x => x.Key.IsMove == false)
                                   .Select(x => new GetAllListForMoveOrderPaginationDto
                                   {
                                       CustomerName = x.Key.CustomerName,
                                       IsActive = x.Key.IsActive,
                                       IsApproved = x.Key.IsApproved != null

                                   }).Where(x => Convert.ToString(x.CustomerName).ToLower().Contains(search.Trim().ToLower()));


            return await PagedList<GetAllListForMoveOrderPaginationDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<IReadOnlyList<TotalListOfApprovedPreparedDateDto>> TotalListOfApprovedPreparedDate(string customername , bool status)
        {

            var orders = _context.Orders.GroupBy(x => new
            {
                x.TrasactId,
                x.CustomerName,
                x.Customercode,
              
                x.PreparedDate,
                x.IsApproved,
                x.IsMove,
                x.IsReject,

                x.Department,
                x.DepartmentCode,
                x.CompanyName,
                x.CompanyCode,
                x.LocationCode,
                x.LocationName,
                x.Rush


            }).Where(x => x.Key.CustomerName == customername)
            .Where(x => x.Key.IsApproved == true)
            .Where(x => x.Key.PreparedDate != null)
            .Where(x => x.Key.IsMove == false)

            .Select(x => new TotalListOfApprovedPreparedDateDto
            {
                Id = x.Key.TrasactId,
                CustomerName = x.Key.CustomerName,
                CustomerCode = x.Key.Customercode,
               
                TotalOrders = x.Sum(x => x.QuantityOrdered),
                PreparedDate = x.Key.PreparedDate.ToString(),
                IsMove = x.Key.IsMove,
                IsReject = x.Key.IsReject != null,

                Department = x.Key.Department,
                DepartmentCode = x.Key.DepartmentCode,
                CompanyName = x.Key.CompanyName,
                CompanyCode = x.Key.CompanyCode,
                LocationCode = x.Key.LocationCode,
                LocationName = x.Key.LocationName,
                    
                IsRush = x.Key.Rush != null ? true : false,
                Rush = x.Key.Rush,

            }).Where(x => x.IsRush == status);

            return await orders.ToListAsync();

        }


        public async Task<IReadOnlyList<ListOfOrdersForMoveOrderDto>> ListOfOrdersForMoveOrder(int id)
        {
            var moveorders = _context.MoveOrders.Where(x => x.IsActive == true)
                                                .GroupBy(x => new
                                                {
                                                    x.OrderNo,
                                                    x.OrderNoPkey
                                                }).Select(x => new MoveOrderItemDto
                                                {
                                                    OrderNo = x.Key.OrderNo,
                                                    OrderPKey = x.Key.OrderNoPkey,
                                                    QuantityPrepared = x.Sum(x => x.QuantityOrdered),


                                                });


            var orders = _context.Orders
                   .Where(x => x.TrasactId == id)
                   .Where(x => x.IsMove == false)
                   .GroupJoin(moveorders, ordering => ordering.Id, moveorder => moveorder.OrderPKey, (ordering, moveorder) => new { ordering, moveorder })
                   .SelectMany(x => x.moveorder.DefaultIfEmpty(), (x, moveorder) => new { x.ordering, moveorder })
                   .GroupBy(x => new
                   {
                       x.ordering.Id,
                       x.ordering.TrasactId,
                       x.ordering.OrderDate,
                       x.ordering.DateNeeded,
                       x.ordering.Department,
                       x.ordering.CustomerName,
                       x.ordering.Customercode,
                       //x.ordering.Category,
                       x.ordering.ItemCode,
                       x.ordering.ItemdDescription,
                       x.ordering.Uom,
                       //x.ordering.Rush
                       //x.ordering.IsApproved,

                   })
                   //}).OrderBy(x => x.Key.Rush == null)
                   // .ThenBy(x => x.Key.Rush)
                   .Select(total => new ListOfOrdersForMoveOrderDto
                   {
                       Id = total.Key.Id,
                       MIRId = total.Key.TrasactId,
                       OrderDate = total.Key.OrderDate.ToString("MM/dd/yyyy"),
                       DateNeeded = total.Key.DateNeeded.ToString("MM/dd/yyyy"),

                       //Department = total.Key.Department,
                       //CustomerName = total.Key.CustomerName,
                       //CustomerCode = total.Key.Customercode,
                       //Category = total.Key.Category,

                       ItemCode = total.Key.ItemCode,
                       ItemDescription = total.Key.ItemdDescription,
                       Uom = total.Key.Uom,
                       QuantityOrder = total.Sum(x => x.ordering.QuantityOrdered),

                       //IsApproved = total.Key.IsApproved != null,

                       PreparedQuantity = total.Sum(x => x.moveorder.QuantityPrepared),
                       //Rush = total.Key.Rush


                   });

            return await orders.ToListAsync();


        }

        public async Task<ItemStocksDto> GetActualItemQuantityInWarehouse(int id, string itemcode)
        {
            var TotaloutMoveOrder = await _context.MoveOrders.Where(x => x.WarehouseId == id)
                                                             .Where(x => x.IsActive == true)
                                                             .Where(x => x.IsPrepared == true)
                                                             .Where(x => x.ItemCode == itemcode)
                                                             .SumAsync(x => x.QuantityOrdered);

            var TotalIssue = await _context.MiscellaneousIssueDetail.Where(x => x.WarehouseId == id)
                                                                    .Where(x => x.IsActive == true)
                                                                    .Where(x => x.IsTransact == true)
                                                                    .SumAsync(x => x.Quantity);

            var TotalBorrowIssue = await _context.BorrowedIssueDetails.Where(x => x.WarehouseId == id)
                                                                      .Where(x => x.IsActive == true)
                                                                      .Where(x => x.IsApproved == true)
                                                                      .SumAsync(x => x.Quantity);

            var TotalBorrowReturned = await _context.BorrowedIssueDetails.Where(x => x.WarehouseId == id)
                                                                      .Where(x => x.IsActive == true)
                                                                      .Where(x => x.IsReturned == true)
                                                                      .Where(x => x.IsApprovedReturned == true)
                                                                      .SumAsync(x => x.ReturnQuantity);



            var totalRemaining = _context.WarehouseReceived
                              .OrderBy(x => x.ReceivingDate)
                              .Where(totalin => totalin.Id == id && totalin.ItemCode == itemcode && totalin.IsActive == true)
                              .GroupBy(x => new
                              {
                                  x.Id,
                                  x.ItemCode,
                                  x.ItemDescription,
                                  x.ActualDelivered,
                                  x.ReceivingDate

                              }).Select(total => new ItemStocksDto
                              {
                                  warehouseId = total.Key.Id,
                                  ItemCode = total.Key.ItemCode,
                                  ItemDescription = total.Key.ItemDescription,
                                  ActualGood = total.Key.ActualDelivered,
                                  DateReceived = total.Key.ReceivingDate.ToString("MM/dd/yyyy"),
                                  Remaining = total.Key.ActualDelivered + TotalBorrowReturned - TotaloutMoveOrder - TotalIssue - TotalBorrowIssue
                              });

            return await totalRemaining.Where(x => x.Remaining != 0)
                                       .FirstOrDefaultAsync();

        }


        public async Task<ItemStocksDto> GetFirstNeeded(string itemCode)
        {
            var getwarehouseIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                           .GroupBy(x => new
                                                           {
                                                               x.Id,
                                                               x.ItemCode,
                                                               x.ReceivingDate,
                                                           }).Select(x => new WarehouseInventory
                                                           {
                                                               WarehouseId = x.Key.Id,
                                                               ItemCode = x.Key.ItemCode,
                                                               ActualGood = x.Sum(x => x.ActualGood),
                                                               RecievingDate = x.Key.ReceivingDate.ToString()
                                                           });


            var getMoveOrder = _context.MoveOrders.Where(x => x.IsActive == true)
                                                  .Where(x => x.IsPrepared == true)
                                                  .GroupBy(x => new
                                                  {
                                                      x.ItemCode,
                                                      x.WarehouseId

                                                  }).Select(x => new ItemStocksDto
                                                  {
                                                      ItemCode = x.Key.ItemCode,
                                                      Remaining = x.Sum(x => x.QuantityOrdered),
                                                      warehouseId = x.Key.WarehouseId

                                                  });

            //var getMiscReceipts = _context.WarehouseReceived.Where(x => x.TransactionType == "MiscellaneousReceipt")
            //                                                .Where(x => x.IsWarehouseReceived == true)
            //                                                .Where(x => x.IsActive == true)
            //                                                .GroupBy(x => new
            //                                                {
            //                                                    x.Id,
            //                                                    x.ItemCode,

            //                                                }).Select(x => new ItemStocksDto
            //                                                {
            //                                                    warehouseId = x.Key.Id,
            //                                                    ItemCode = x.Key.ItemCode,
            //                                                });


            var getMiscIssue = _context.MiscellaneousIssueDetail.Where(x => x.IsActive == true)
                                                                .Where(x => x.IsTransact == true)
                                                                .GroupBy(x => new
                                                                {
                                                                    x.ItemCode,
                                                                    x.WarehouseId,

                                                                }).Select(x => new ItemStocksDto
                                                                {
                                                                    ItemCode = x.Key.ItemCode,
                                                                    Remaining = x.Sum(x => x.Quantity),
                                                                    warehouseId = x.Key.WarehouseId,

                                                                });

            var getBorrowedIssue = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                                .Where(x => x.IsApproved == true)
                                                                .GroupBy(x => new
                                                                {

                                                                    x.ItemCode,
                                                                    x.WarehouseId,

                                                                }).Select(x => new ItemStocksDto
                                                                {

                                                                    ItemCode = x.Key.ItemCode,
                                                                    Remaining = x.Sum(x => x.Quantity),
                                                                    warehouseId = x.Key.WarehouseId,

                                                                });

            var getBorrowedReturn = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                                 .Where(x => x.IsReturned == true)
                                                                 .Where(x => x.IsApprovedReturned == true)
                                                                  .GroupBy(x => new
                                                                  {

                                                                      x.ItemCode,
                                                                      x.WarehouseId,

                                                                  }).Select(x => new ItemStocksDto
                                                                  {
                                                                      ItemCode = x.Key.ItemCode,
                                                                      Remaining = x.Sum(x => x.ReturnQuantity),
                                                                      warehouseId = x.Key.WarehouseId

                                                                  });



            var totalremaining = getwarehouseIn
                              .OrderBy(x => x.RecievingDate)
                              .ThenBy(x => x.ItemCode)
                              .GroupJoin(getMoveOrder, warehouse => warehouse.WarehouseId, moveorder => moveorder.warehouseId, (warehouse, moveorder) => new { warehouse, moveorder })
                              .SelectMany(x => x.moveorder.DefaultIfEmpty(), (x, moveorder) => new { x.warehouse, moveorder })
                              .GroupJoin(getMiscIssue, warehouse => warehouse.warehouse.WarehouseId, issue => issue.warehouseId, (warehouse, issue) => new { warehouse, issue })
                              .SelectMany(x => x.issue.DefaultIfEmpty(), (x, issue) => new { x.warehouse, issue })
                              .GroupJoin(getBorrowedIssue, warehouse => warehouse.warehouse.warehouse.WarehouseId, borrow => borrow.warehouseId, (warehouse, borrow) => new { warehouse, borrow })
                              .SelectMany(x => x.borrow.DefaultIfEmpty(), (x, borrow) => new { x.warehouse, borrow })
                              .GroupJoin(getBorrowedReturn, warehouse => warehouse.warehouse.warehouse.warehouse.WarehouseId, returned => returned.warehouseId, (warehouse, returned) => new { warehouse, returned })
                              .SelectMany(x => x.returned.DefaultIfEmpty(), (x, returned) => new { x.warehouse, returned })
                              .GroupBy(x => new
                              {
                                  x.warehouse.warehouse.warehouse.warehouse.WarehouseId,
                                  x.warehouse.warehouse.warehouse.warehouse.ItemCode,
                                  x.warehouse.warehouse.warehouse.warehouse.RecievingDate

                              })
                              .Select(x => new ItemStocksDto
                              {
                                  warehouseId = x.Key.WarehouseId,
                                  ItemCode = x.Key.ItemCode,
                                  DateReceived = x.Key.RecievingDate.ToString(),
                                  Remaining = x.Sum(x => x.warehouse.warehouse.warehouse.warehouse.ActualGood == null ? 0 : x.warehouse.warehouse.warehouse.warehouse.ActualGood) +
                                              x.Sum(x => x.returned.Remaining == null ? 0 : x.returned.Remaining) -
                                              x.Sum(x => x.warehouse.warehouse.warehouse.moveorder.Remaining == null ? 0 : x.warehouse.warehouse.warehouse.moveorder.Remaining) -
                                              x.Sum(x => x.warehouse.warehouse.issue.Remaining == null ? 0 : x.warehouse.warehouse.issue.Remaining) -
                                              x.Sum(x => x.warehouse.borrow.Remaining == null ? 0 : x.warehouse.borrow.Remaining)

                              });


            return await totalremaining.Where(x => x.Remaining != 0)
                                       .Where(x => x.ItemCode == itemCode)
                                       .FirstOrDefaultAsync();


        }


        public async Task<IReadOnlyList<GetAllOutOfStockByItemCodeAndOrderDateDto>> GetAllOutOfStockByItemCodeAndOrderDate(string itemcode, string orderdate)
        {

            CultureInfo usCulture = new CultureInfo("en-US");
            CultureInfo.CurrentCulture = usCulture;

            DateTime.ParseExact(orderdate, "MM/dd/yyyy", CultureInfo.InvariantCulture);


            var moveOrderOut = _context.MoveOrders.Where(x => x.IsActive == true)
                                                 .Where(x => x.IsPrepared == true)
                                                 .GroupBy(x => new
                                                 {
                                                     x.ItemCode,
                                                     x.WarehouseId,

                                                 }).Select(x => new OrderingInventory
                                                 {
                                                     ItemCode = x.Key.ItemCode,
                                                     QuantityOrdered = x.Sum(x => x.QuantityOrdered),
                                                     warehouseId = x.Key.WarehouseId

                                                 });

            var getIssueOut = _context.MiscellaneousIssueDetail.Where(x => x.IsActive == true)
                                                               .Where(x => x.IsTransact == true)
                                                               .GroupBy(x => new
                                                               {
                                                                   x.ItemCode,
                                                                   x.WarehouseId

                                                               }).Select(x => new IssueInventoryDto
                                                               {

                                                                   ItemCode = x.Key.ItemCode,
                                                                   Quantity = x.Sum(x => x.Quantity),
                                                                   warehouseId = x.Key.WarehouseId
                                                               });

            var getBorrowedIssue = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                                .Where(x => x.IsApproved == true)
                                                                .GroupBy(x => new
                                                                {

                                                                    x.ItemCode,
                                                                    x.WarehouseId,

                                                                }).Select(x => new IssueInventoryDto
                                                                {

                                                                    ItemCode = x.Key.ItemCode,
                                                                    Quantity = x.Sum(x => x.Quantity),
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
                                                                warehouseId = x.Key.WarehouseId

                                                            });


            var totalRemaining = _context.WarehouseReceived
                           .GroupJoin(moveOrderOut, warehouse => warehouse.Id, moveorder => moveorder.warehouseId, (warehouse, moveorder) => new { warehouse, moveorder })
                           .SelectMany(x => x.moveorder.DefaultIfEmpty(), (x, moveorder) => new { x.warehouse, moveorder })
                           .GroupJoin(getIssueOut, warehouse => warehouse.warehouse.Id, issue => issue.warehouseId, (warehouse, issue) => new { warehouse, issue })
                           .SelectMany(x => x.issue.DefaultIfEmpty(), (x, issue) => new { x.warehouse, issue })
                           .GroupJoin(getBorrowedIssue, warehouse => warehouse.warehouse.warehouse.Id, borrow => borrow.warehouseId, (warehouse, borrow) => new { warehouse, borrow })
                           .SelectMany(x => x.borrow.DefaultIfEmpty(), (x, borrow) => new { x.warehouse, borrow })
                           .GroupJoin(BorrowedReturn, warehouse => warehouse.warehouse.warehouse.warehouse.Id, returned => returned.warehouseId, (warehouse, returned) => new { warehouse, returned })
                           .SelectMany(x => x.returned.DefaultIfEmpty(), (x, returned) => new { x.warehouse, returned })
                           .GroupBy(x => new
                           {
                               x.warehouse.warehouse.warehouse.warehouse.Id,
                               x.warehouse.warehouse.warehouse.warehouse.ItemCode,
                               x.warehouse.warehouse.warehouse.warehouse.ItemDescription,
                               x.warehouse.warehouse.warehouse.warehouse.ReceivingDate,
                               x.warehouse.warehouse.warehouse.warehouse.ActualGood,

                           }).OrderBy(x => x.Key.ReceivingDate)
                           .Select(total => new ItemStocksDto
                           {
                               warehouseId = total.Key.Id,
                               ItemCode = total.Key.ItemCode,
                               ItemDescription = total.Key.ItemDescription,
                               DateReceived = total.Key.ReceivingDate.ToString(),
                               Remaining = total.Key.ActualGood + total.Sum(x => x.returned.In) - total.Sum(x => x.warehouse.warehouse.warehouse.moveorder.QuantityOrdered) - total.Sum(x => x.warehouse.warehouse.issue.Quantity) - total.Sum(x => x.warehouse.borrow.Quantity),

                           });

            var totalOrders = _context.Orders
                      .GroupBy(x => new
                      {
                          x.ItemCode,
                          x.IsPrepared,
                          x.IsActive

                      }).Select(x => new GetAllOutOfStockByItemCodeAndOrderDateDto
                      {

                          ItemCode = x.Key.ItemCode,
                          TotalOrders = x.Sum(x => x.QuantityOrdered),
                          IsPrepared = x.Key.IsPrepared

                      }).Where(x => x.IsPrepared == false);


            var orders = _context.Orders
                .Where(ordering => ordering.ItemCode == itemcode)
                 .Where(x => x.OrderDate == DateTime.Parse(orderdate))
                  .GroupJoin(totalRemaining, ordering => ordering.ItemCode, warehouse => warehouse.ItemCode, (ordering, warehouse) => new { ordering, warehouse })
                  .SelectMany(x => x.warehouse.DefaultIfEmpty(), (x, warehouse) => new { x.ordering, warehouse })
                  .GroupBy(x => new
                  {
                      x.ordering.Id,
                      x.ordering.OrderDate,
                      x.ordering.DateNeeded,
                      x.ordering.Department,
                      x.ordering.CustomerName,
                      x.ordering.Customercode,
                      x.ordering.Category,
                      x.ordering.ItemCode,
                      x.ordering.ItemdDescription,
                      x.ordering.Uom,
                      x.ordering.IsActive,
                      x.ordering.IsPrepared,
                      x.ordering.PreparedDate,
                      x.ordering.IsApproved

                  }).Select(total => new GetAllOutOfStockByItemCodeAndOrderDateDto
                  {

                      Id = total.Key.Id,
                      OrderDate = total.Key.OrderDate.ToString("MM/dd/yyyy"),
                      DateNeeded = total.Key.DateNeeded.ToString("MM/dd/yyyy"),
                      Department = total.Key.Department,
                      CustomerName = total.Key.CustomerName,
                      CustomerCode = total.Key.Customercode,
                      Category = total.Key.Category,
                      ItemCode = total.Key.ItemCode,
                      ItemDescription = total.Key.ItemdDescription,
                      Uom = total.Key.Uom,
                      QuantityOrder = total.Sum(x => x.ordering.QuantityOrdered),
                      IsActive = total.Key.IsActive,
                      IsPrepared = total.Key.IsPrepared,
                      StockOnHand = total.Sum(x => x.warehouse.Remaining),
                      Difference = total.Sum(x => x.warehouse.Remaining) - total.Sum(x => x.ordering.QuantityOrdered),
                      PreparedDate = total.Key.PreparedDate.ToString(),
                      IsApproved = total.Key.IsApproved != null

                  });

            return await orders.ToListAsync();

        }



        public async Task<GetMoveOrderDetailsForMoveOrderDto> GetMoveOrderDetailsForMoveOrder(int orderId)
        {
            var orders = _context.Orders.Where(x => x.IsMove == false)
                //.OrderBy(x => x.Rush == null)
                // .ThenBy(x => x.Rush)
                .Select(x => new GetMoveOrderDetailsForMoveOrderDto
                {
                    Id = x.Id,
                    MIRId = x.TrasactId,

                    Department = x.Department,
                    DepartmentCode = x.DepartmentCode,
                    CompanyName = x.CompanyName,
                    CompanyCode = x.CompanyCode,
                    LocationCode = x.LocationCode,
                    LocationName = x.LocationName,
                    //AccountCode = x.AccountCode,
                    //AccountTitles = x.AccountTitles,

                    CustomerName = x.CustomerName,
                    CustomerCode = x.Customercode,
                    //Address = x.AddressOrder,
                    ItemCode = x.ItemCode,
                    ItemDescription = x.ItemdDescription,
                    Uom = x.Uom,
                    QuantityOrder = x.QuantityOrdered,
                    Category = x.Category,
                    OrderDate = x.OrderDate.ToString(),
                    DateNeeded = x.DateNeeded.ToString(),
                    PrepareDate = x.PreparedDate.ToString(),

                    CustomerType = x.CustomerType,
                    Rush = x.Rush

                });

            return await orders.Where(x => x.Id == orderId)
                               .FirstOrDefaultAsync();

        }



        public async Task<bool> PrepareItemForMoveOrder(MoveOrder orders)
        {
            await _context.MoveOrders.AddAsync(orders);
            return true;

        }


        public async Task<bool> CancelControlInMoveOrder(Ordering orders)
        {
            var cancelorder = await _context.Orders.Where(x => x.TrasactId == orders.TrasactId)
                                                   .ToListAsync();

            var existMOveOrders = await _context.MoveOrders.Where(x => x.OrderNo == orders.TrasactId)
                                                            .ToListAsync();

            foreach (var items in cancelorder)
            {
                items.IsApproved = null;
                items.ApprovedDate = null;

            }

            if (existMOveOrders != null)
            {
                foreach (var items in existMOveOrders)
                {
                    items.IsActive = false;
                }
            }
            return true;

        }



        public async Task<IReadOnlyList<ListOfPreparedItemsForMoveOrderDto>> ListOfPreparedItemsForMoveOrder(int id)
        {
            var orders = _context.MoveOrders
                //.OrderBy(x => x.Rush == null)
                // .ThenBy(x => x.Rush)
                 .Select(x => new ListOfPreparedItemsForMoveOrderDto
                 {
                     Id = x.Id,
                     OrderNo = x.OrderNo,
                     BarCodes = x.WarehouseId,
                     ItemCode = x.ItemCode,
                     ItemDescription = x.ItemDescription,
                     Quantity = x.QuantityOrdered,
                     IsActive = x.IsActive,
                     Rush = x.Rush


                 });

            return await orders.Where(x => x.OrderNo == id)
                               .Where(x => x.IsActive == true)
                               .ToListAsync();
        }



        public async Task<bool> SavePreparedMoveOrder(MoveOrder order)
        {
            var existing = await _context.Orders.Where(x => x.TrasactId == order.OrderNo)
                                                .ToListAsync();

            var existingsMoveOrders = await _context.MoveOrders.Where(x => x.OrderNo == order.OrderNo)
                                                              .Where(x => x.IsPrepared == true)
                                                              .ToListAsync();

            if (!existingsMoveOrders.Any())
                return false;

            foreach (var x in existing)
            {
                x.IsMove = true;

            }

            foreach (var x in existingsMoveOrders)
            {
                x.Department = order.Department;
                x.CompanyCode = order.CompanyCode;
                x.CompanyName = order.CompanyName;
                x.DepartmentCode = order.DepartmentCode;
                x.DepartmentName = order.DepartmentName;
                x.LocationCode = order.LocationCode;
                x.LocationName = order.LocationName;
                x.AccountCode = order.AccountCode;
                x.AccountTitles = order.AccountTitles;
                x.RejectBy = null;
                x.RejectedDate = null;
                x.RejectedDateTempo = null;
                x.IsReject = null;

            }

            return true;

        }


        public async Task<bool> CancelMoveOrder(MoveOrder moveOrder)
        {
            var existing = await _context.MoveOrders.Where(x => x.Id == moveOrder.Id)
                                                    .FirstOrDefaultAsync();

            if (existing == null)
            {
                return false;
            }

            existing.IsActive = false;
            existing.CancelledDate = DateTime.Now;

            return true;

        }


        // ================================================== MIR MoveOrder For Approval ========================================================



        public async Task<PagedList<ForApprovalMoveOrderPaginationDto>> ForApprovalMoveOrderPagination(UserParams userParams, bool status)
        {

            var order = _context.MoveOrders.Where(x => x.IsActive == true)
                                           .Where(x => x.IsReject == null)
                                           .Where(x => x.IsApprove == null)
                                           .Where(x => x.IsPrepared == true)
                                           .GroupBy(x => new
                                           {

                                               x.OrderNo,
                                               x.CustomerName,
                                               x.Customercode,
                                              
                                               x.PreparedDate,
                                               x.Rush


                                           })

                                              .Select(x => new ForApprovalMoveOrderPaginationDto
                                              {

                                                  MIRId = x.Key.OrderNo,
                                                  CustomerName = x.Key.CustomerName,
                                                  Customercode = x.Key.Customercode,
                                                 
                                                  Quantity = x.Sum(x => x.QuantityOrdered),
                                                  PreparedDate = x.Key.PreparedDate.ToString(),
                                                  IsRush = x.Key.Rush != null ? true : false,
                                                  Rush = x.Key.Rush,

                                              }).Where(x => x.IsRush == status);

            return await PagedList<ForApprovalMoveOrderPaginationDto>.CreateAsync(order, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<PagedList<ForApprovalMoveOrderPaginationDto>> ForApprovalMoveOrderPaginationOrig(UserParams userParams, string search , bool status)
        {
            var order = _context.MoveOrders.Where(x => x.IsActive == true)
                                           .Where(x => x.IsReject == null)
                                           .Where(x => x.IsApprove == null)
                                           .Where(x => x.IsPrepared == true)
                                           .GroupBy(x => new
                                           {

                                               x.OrderNo,
                                               x.CustomerName,
                                               x.Customercode,
                                              
                                               x.PreparedDate,
                                               x.Rush


                                           })

                                              .Select(x => new ForApprovalMoveOrderPaginationDto
                                              {

                                                  MIRId = x.Key.OrderNo,
                                                  CustomerName = x.Key.CustomerName,
                                                  Customercode = x.Key.Customercode,
                                                
                                                  Quantity = x.Sum(x => x.QuantityOrdered),
                                                  PreparedDate = x.Key.PreparedDate.ToString(),
                                                  IsRush = x.Key.Rush != null ? true : false,
                                                  Rush = x.Key.Rush,

                                              }).Where(x => x.IsRush == status)
                                                 .Where(x => Convert.ToString(x.MIRId).ToLower().Contains(search.Trim().ToLower())
                                                  || Convert.ToString(x.CustomerName).ToLower().Contains(search.Trim().ToLower())
                                                     || Convert.ToString(x.Customercode).ToLower().Contains(search.Trim().ToLower()));




            return await PagedList<ForApprovalMoveOrderPaginationDto>.CreateAsync(order, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<IReadOnlyList<ViewMoveOrderForApprovalDto>> ViewMoveOrderForApproval(int id)
        {
            var orders = _context.MoveOrders
                 .Where(x => x.IsActive == true)
                                            .Select(x => new ViewMoveOrderForApprovalDto
                                            {
                                                Id = x.Id,
                                                MIRId = x.OrderNo,
                                                BarcodeNo = x.WarehouseId,
                                                ItemCode = x.ItemCode,
                                                ItemDescription = x.ItemDescription,
                                                Uom = x.Uom,
                                                Customercode = x.Customercode,
                                                CustomerName = x.CustomerName,
                                                Address = x.AddressOrder,
                                                ApprovedDate = x.ApprovedDate.ToString(),
                                                Quantity = x.QuantityOrdered,

                                                CompanyCode = x.CompanyCode,
                                                CompanyName = x.CompanyName,
                                                DepartmentCode = x.DepartmentCode,
                                                DepartmentName = x.DepartmentName,
                                                LocationCode = x.LocationCode,
                                                LocationName = x.LocationName,
                                                AccountCode = x.AccountCode,
                                                AccountTitles = x.AccountTitles,


                                            });

            return await orders.Where(x => x.MIRId == id)
                                  .ToListAsync();

        }


        public async Task<bool> ApprovalForMoveOrders(MoveOrder moveorder)
        {
            var existing = await _context.MoveOrders.Where(x => x.OrderNo == moveorder.OrderNo)
                                                    .ToListAsync();

            if (existing == null)
                return false;

            foreach (var items in existing)
            {

                items.ApprovedDate = DateTime.Now;
                items.ApproveDateTempo = DateTime.Now;
                items.IsApprove = true;

            }

            return true;
        }


        public async Task<bool> UpdatePrintStatus(MoveOrder moveorder)
        {
            var existing = await _context.MoveOrders.Where(x => x.OrderNo == moveorder.OrderNo)
                                                     .ToListAsync();

            if (existing == null)
                return false;

            foreach (var items in existing)
            {
                items.IsPrint = true;
            }
            return true;

        }


        public async Task<bool> RejectForMoveOrder(MoveOrder moveOrder)
        {
            var existing = await _context.MoveOrders.Where(x => x.OrderNo == moveOrder.OrderNo)
                                                      .ToListAsync();


            var existingOrders = await _context.Orders.Where(x => x.TrasactId == moveOrder.OrderNo)
                                                      .ToListAsync();

            if (existing == null)
                return false;

            foreach (var items in existing)
            {
                items.RejectBy = moveOrder.RejectBy;
                items.RejectedDate = DateTime.Now;
                items.RejectedDateTempo = DateTime.Now;
                items.Remarks = moveOrder.Remarks;
                items.IsReject = true;
                items.IsActive = true;
                items.IsPrepared = true;
                items.IsApproveReject = null;

            }

            foreach (var items in existingOrders)
            {
                items.IsMove = false;
                items.IsReject = true;
                items.RejectBy = moveOrder.RejectBy;
                items.Remarks = moveOrder.Remarks;
            }

            return true;
        }

        // ================================================== MIR MoveOrder Approval ========================================================

        public async Task<PagedList<ApprovedMoveOrderPaginationDto>> ApprovedMoveOrderPagination(UserParams userParams , bool status)
        {

         
            var orders = _context.MoveOrders
                     .GroupBy(x => new
                     {
                         x.OrderNo,
                         x.Department,
                         x.CustomerName,
                         x.Customercode,
                         x.AddressOrder,
                      
                         x.PreparedDate,
                         x.IsApprove,
                         x.IsPrepared,
                         x.IsReject,
                         x.ApproveDateTempo,
                         x.IsPrint,
                         x.IsTransact,
                         x.Rush


                     }).Where(x => x.Key.IsApprove == true)


              .Select(x => new ApprovedMoveOrderPaginationDto
              {

                  MIRId = x.Key.OrderNo,
                  Department = x.Key.Department,
                  CustomerName = x.Key.CustomerName,
                  CustomerCode = x.Key.Customercode,
                  Address = x.Key.AddressOrder,
                
                  Quantity = x.Sum(x => x.QuantityOrdered),
                  PreparedDate = x.Key.PreparedDate.ToString(),
                  IsApprove = x.Key.IsApprove != null,
                  IsPrepared = x.Key.IsPrepared,
                  ApprovedDate = x.Key.ApproveDateTempo.ToString(),
                  IsPrint = x.Key.IsPrint != null,
                  IsTransact = x.Key.IsTransact,
                  IsRush = x.Key.Rush != null ? true : false,
                  Rush = x.Key.Rush,

              }).Where(x => x.IsRush == status);

            return await PagedList<ApprovedMoveOrderPaginationDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);


        }

        public async Task<PagedList<ApprovedMoveOrderPaginationDto>> ApprovedMoveOrderPaginationOrig(UserParams userParams, string search , bool status)
        {
            var orders = _context.MoveOrders
                    .GroupBy(x => new
                    {
                        x.OrderNo,
                        x.Department,
                        x.CustomerName,
                        x.Customercode,
                        x.AddressOrder,
                      
                        x.PreparedDate,
                        x.IsApprove,
                        x.IsPrepared,
                        x.IsReject,
                        x.ApproveDateTempo,
                        x.IsPrint,
                        x.IsTransact,
                        x.Rush


                    }).Where(x => x.Key.IsApprove == true)


             .Select(x => new ApprovedMoveOrderPaginationDto
             {

                 MIRId = x.Key.OrderNo,
                 Department = x.Key.Department,
                 CustomerName = x.Key.CustomerName,
                 CustomerCode = x.Key.Customercode,
                 Address = x.Key.AddressOrder,
              
                 Quantity = x.Sum(x => x.QuantityOrdered),
                 PreparedDate = x.Key.PreparedDate.ToString(),
                 IsApprove = x.Key.IsApprove != null,
                 IsPrepared = x.Key.IsPrepared,
                 ApprovedDate = x.Key.ApproveDateTempo.ToString(),
                 IsPrint = x.Key.IsPrint != null,
                 IsTransact = x.Key.IsTransact,
                 IsRush = x.Key.Rush != null ? true : false,
                 Rush = x.Key.Rush,

             }).Where(x => x.IsRush == status)
             .Where(x => Convert.ToString(x.MIRId).ToLower().Contains(search.Trim().ToLower())
                 || Convert.ToString(x.CustomerName).ToLower().Contains(search.Trim().ToLower())
                  || Convert.ToString(x.CustomerCode).ToLower().Contains(search.Trim().ToLower()));

            return await PagedList<ApprovedMoveOrderPaginationDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<GetAllApprovedMoveOrderDto> GetAllApprovedMoveOrder(int id)
        {

            var orders = _context.MoveOrders.Where(x => x.OrderNoPkey == id)
                                            .GroupBy(x => new
                                            {
                                                x.OrderNo,
                                                x.WarehouseId,
                                                x.ItemCode,
                                                x.ItemDescription,
                                                x.Uom,
                                                x.Department,
                                                x.CustomerName,
                                                x.Customercode,
                                                x.Category,
                                                x.OrderDate,
                                                x.PreparedDate,
                                                x.IsApprove,
                                                x.IsPrepared,
                                                x.IsReject,
                                                x.ApproveDateTempo,
                                                x.IsPrint,
                                                x.IsTransact,
                                                x.Rush


                                            }).Where(x => x.Key.IsApprove == true)

                                             .Select(x => new GetAllApprovedMoveOrderDto
                                             {
                                                 MIRId = x.Key.OrderNo,
                                                 BarcodeNo = x.Key.WarehouseId,
                                                 ItemCode = x.Key.ItemCode,
                                                 ItemDescription = x.Key.ItemDescription,
                                                 Uom = x.Key.Uom,
                                                 Department = x.Key.Department,
                                                 CustomerName = x.Key.CustomerName,
                                                 CustomerCode = x.Key.Customercode,
                                                 Category = x.Key.Category,
                                                 Quantity = x.Sum(x => x.QuantityOrdered),
                                                 OrderDate = x.Key.OrderDate.ToString(),
                                                 PreparedDate = x.Key.PreparedDate.ToString(),
                                                 IsApprove = x.Key.IsApprove != null,
                                                 IsPrepared = x.Key.IsPrepared,
                                                 ApprovedDate = x.Key.ApproveDateTempo.ToString(),
                                                 IsPrint = x.Key.IsPrint != null,
                                                 IsTransact = x.Key.IsTransact,
                                                 Rush = x.Key.Rush

                                             });

            return await orders.FirstOrDefaultAsync();

        }

        public async Task<bool> RejectApproveMoveOrder(MoveOrder moveOrder)
        {
            var existing = await _context.MoveOrders.Where(x => x.OrderNo == moveOrder.OrderNo)
                                                    .ToListAsync();

            if (existing == null)
                return false;

            foreach (var items in existing)
            {
                items.RejectBy = moveOrder.RejectBy;
                items.RejectedDate = DateTime.Now;
                items.RejectedDateTempo = DateTime.Now;
                items.Remarks = moveOrder.Remarks;
                items.IsReject = null;
                items.IsApproveReject = true;
                items.IsActive = false;
                items.IsPrepared = true;
                items.IsApprove = false;

            }
            return true;
        }


        public async Task<PagedList<RejectedMoveOrderPaginationDto>> RejectedMoveOrderPagination(UserParams userParams , bool status)
        {


            var orders = _context.MoveOrders.Where(x => x.IsApproveReject == true)
                                            .GroupBy(x => new
                                            {

                                                x.OrderNo,
                                                x.Department,
                                                x.CustomerName,
                                                x.Customercode,

                                                x.OrderDate,
                                                x.PreparedDate,
                                                x.IsApprove,
                                                x.IsReject,
                                                x.RejectedDateTempo,
                                                x.Rush
                                                //x.Remarks,


                                            }).Select(x => new RejectedMoveOrderPaginationDto
                                            {
                                                MIRId = x.Key.OrderNo,
                                                Department = x.Key.Department,
                                                CustomerName = x.Key.CustomerName,
                                                CustomerCode = x.Key.Customercode,

                                                Quantity = x.Sum(x => x.QuantityOrdered),
                                                OrderDate = x.Key.OrderDate.ToString(),
                                                PreparedDate = x.Key.PreparedDate.ToString(),
                                                IsReject = x.Key.IsReject != null,
                                                RejectedDate = x.Key.RejectedDateTempo.ToString(),
                                                IsRush = x.Key.Rush != null ? true : false,
                                                Rush = x.Key.Rush,

                                                //Remarks = x.Key.Remarks,


                                            }).Where(x => x.IsRush == status);

            return await PagedList<RejectedMoveOrderPaginationDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<RejectedMoveOrderPaginationDto>> RejectedMoveOrderPaginationOrig(UserParams userParams, string search , bool status)
        {

            var orders = _context.MoveOrders.Where(x => x.IsApproveReject == true)
                                          .GroupBy(x => new
                                          {

                                              x.OrderNo,
                                              x.Department,
                                              x.CustomerName,
                                              x.Customercode,

                                              x.OrderDate,
                                              x.PreparedDate,
                                              x.IsApprove,
                                              x.IsReject,
                                              x.RejectedDateTempo,
                                              x.Rush
                                              //x.Remarks,


                                          }).Select(x => new RejectedMoveOrderPaginationDto
                                          {
                                              MIRId = x.Key.OrderNo,
                                              Department = x.Key.Department,
                                              CustomerName = x.Key.CustomerName,
                                              CustomerCode = x.Key.Customercode,

                                              Quantity = x.Sum(x => x.QuantityOrdered),
                                              OrderDate = x.Key.OrderDate.ToString(),
                                              PreparedDate = x.Key.PreparedDate.ToString(),
                                              IsReject = x.Key.IsReject != null,
                                              RejectedDate = x.Key.RejectedDateTempo.ToString(),
                                              IsRush = x.Key.Rush != null ? true : false,
                                              Rush = x.Key.Rush,

                                              //Remarks = x.Key.Remarks,


                                          }).Where(x => x.IsRush == status)
                                          .Where(x => Convert.ToString(x.MIRId).ToLower().Contains(search.Trim().ToLower())
                                            || Convert.ToString(x.CustomerName).ToLower().Contains(search.Trim().ToLower())
                                            || Convert.ToString(x.CustomerCode).ToLower().Contains(search.Trim().ToLower()));


            return await PagedList<RejectedMoveOrderPaginationDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<bool> ReturnMoveOrderForApproval(MoveOrder moveorder)
        {
            var existing = await _context.MoveOrders.Where(x => x.OrderNo == moveorder.OrderNo)
                                                    .ToListAsync();

            var existingorders = await _context.Orders.Where(x => x.TrasactId == moveorder.OrderNo)
                                                      .ToListAsync();

            foreach (var items in existing)
            {
                items.RejectBy = null;
                items.RejectedDate = null;
                items.Remarks = null;
                items.IsReject = null;
                items.IsActive = true;
                items.IsPrepared = true;
                items.IsApprove = null;
                items.IsApproveReject = null;
            }

            foreach (var items in existingorders)
            {

                items.IsReject = null;
                items.RejectBy = null;
                items.Remarks = null;
                items.RejectedDate = null;
            }

            return true;
        }

        //=================================================================== MIR Transact MoveOrder =======================================================


        public async Task<IReadOnlyList<TotalListForTransactMoveOrderDto>> TotalListForTransactMoveOrder(bool status)
        {


            var orders = _context.MoveOrders.Where(x => x.IsActive == true)
                                            .Where(x => x.IsTransact == status)
                                            .GroupBy(x => new
                                            {
                                                x.OrderNo,
                                                x.Department,
                                                x.CustomerName,
                                                x.Customercode,

                                                x.DateNeeded,
                                                x.PreparedDate,
                                                x.IsApprove,
                                                x.IsTransact,
                                                x.Rush

                                            }).Where(x => x.Key.IsApprove == true)

                                            .Select(x => new TotalListForTransactMoveOrderDto
                                            {
                                                MIRId = x.Key.OrderNo,
                                                Department = x.Key.Department,
                                                CustomerName = x.Key.CustomerName,
                                                CustomerCode = x.Key.Customercode,

                                                TotalOrders = x.Sum(x => x.QuantityOrdered),
                                                DateNeeded = x.Key.DateNeeded.ToString("MM/dd/yyyy"),
                                                PreparedDate = x.Key.PreparedDate.ToString(),
                                                IsApproved = x.Key.IsApprove != null,
                                                IsRush = x.Key.Rush != null ? true: false,
                                                Rush = x.Key.Rush

                                            });

            return await orders.ToListAsync();

        }

        public async Task<IReadOnlyList<ListOfMoveOrdersForTransactDto>> ListOfMoveOrdersForTransact(int orderid)
        {
       
            var orders = _context.MoveOrders.OrderBy(x => x.Rush == null)
                                            .ThenBy(x => x.Rush)
                                            .Where(x => x.IsActive == true)
                                           .Where(x => x.IsApprove == true)

                                           .Select(x => new ListOfMoveOrdersForTransactDto
                                           {
                                               OrderNoPkey = x.OrderNoPkey,
                                               MIRId = x.OrderNo,
                                               BarcodeNo = x.WarehouseId,
                                               OrderDate = x.OrderDate.ToString(),
                                               PreparedDate = x.PreparedDate.ToString(),
                                               DateNeeded = x.DateNeeded.ToString(),
                                               Department = x.Department,
                                               CustomerCode = x.Customercode,
                                               CustomerName = x.CustomerName,
                                               Category = x.Category,
                                               ItemCode = x.ItemCode,
                                               ItemDescription = x.ItemDescription,
                                               Uom = x.Uom,
                                               Quantity = x.QuantityOrdered,
                                               IsApprove = x.IsApprove != null,
                          

                                           });

            return await orders.Where(x => x.MIRId == orderid)
                               .ToListAsync();

        }


        public async Task<bool> TransanctListOfMoveOrders(TransactMoveOrder transact)
        {
            var existing = await _context.MoveOrders.Where(x => x.OrderNo == transact.OrderNo)
                                                    .Where(x => x.IsApprove == true)
                                                    .ToListAsync();


            var existingtransact = await _context.TransactOrder.Where(x => x.OrderNo == transact.OrderNo)
                                                       .ToListAsync();



            foreach (var x in existingtransact)
            {

                x.IsActive = true;
                x.IsTransact = true;
                x.PreparedDate = DateTime.Now;

            }


            await _context.TransactOrder.AddAsync(transact);

            if (!existing.Any())
                return false;


            foreach (var itemss in existing)
            {
                itemss.IsTransact = true;
            }

            return true;

        }





    }
}
