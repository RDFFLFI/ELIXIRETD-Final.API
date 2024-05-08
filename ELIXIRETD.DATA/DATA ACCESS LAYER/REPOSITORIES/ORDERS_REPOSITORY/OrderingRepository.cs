using ELIXIRETD.DATA.CORE.INTERFACES.Orders;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.MoveOrderDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.Notification_Dto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.PreperationDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.TransactDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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



        public async Task<IReadOnlyList<DtoOrderNotif>> GetOrdersForNotificationAll()
        {
            //var orders = _context.Orders.OrderBy(x => x.OrderDate)
            //                       .GroupBy(x => new
            //                       {
            //                           x.TrasactId,
            //                           x.CustomerName,
            //                           x.IsActive,
            //                           x.PreparedDate,
            //                           ////x.Rush

            //                       }).Where(x => x.Key.IsActive == true)
            //                         .Where(x => x.Key.PreparedDate == null)

            //                         .Select(x => new DtoOrderNotif
            //                         {
            //                             MIRId = x.Key.TrasactId,
            //                             CustomerName = x.Key.CustomerName,
            //                             IsActive = x.Key.IsActive,
            //                             //Rush = x.Key.Rush != null ? true : false,

            var orders = _context.Orders
                               .GroupBy(x => new
                               {
                                   x.TrasactId,
                                   x.CustomerName,
                                   x.IsActive,
                                   x.PreparedDate,
                                   //x.Rush

                               }).Where(x => x.Key.IsActive == true)
                                 .Where(x => x.Key.PreparedDate == null)

                                 .Select(x => new DtoOrderNotif
                                 {
                                     MIRId = x.Key.TrasactId,
                                     CustomerName = x.Key.CustomerName,
                                     IsActive = x.Key.IsActive,
                                     //Rush = x.Key.Rush != null ? true : false,

                                 })/*.Where(x => x.Rush == true)*/;

            return await orders.ToListAsync();
        }

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
                                         Rush = x.Key.Rush != null ? true : false,
                      
                                     }).Where(x => x.Rush == true);

            return await orders.ToListAsync();
        }

        public async Task<IReadOnlyList<DtoOrderNotif>> GetOrdersForNotificationNotRush()
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
                                         Rush = x.Key.Rush != null ? true : false,

                                     }).Where(x => x.Rush == false);

            return await orders.ToListAsync();
        }


        public async Task<IReadOnlyList<DtoForMoveOrderNotif>> GetMoveOrdersForNotificationAll()
        {
            var orders = _context.Orders
                                .GroupBy(x => new
                                {
                                    x.TrasactId,
                                    x.CustomerName,
                                    x.IsActive,
                                    x.IsApproved,
                                    x.IsMove,
                                    //x.Rush,
                                    x.PreparedDate

                                }).Where(x => x.Key.IsApproved == true)
                                  .Where(x => x.Key.IsActive == true)
                                  .Where(x => x.Key.PreparedDate != null)
                                  .Where(x => x.Key.IsMove == false)
                                  .Select(x => new DtoForMoveOrderNotif
                                  {
                                      MIRId = x.Key.TrasactId,
                                      CustomerName = x.Key.CustomerName,
                                      IsActive = x.Key.IsActive,
                                      //IsApproved = x.Key.IsApproved != null,
                                      //Rush = x.Key.Rush != null ? true : false,
                                      /* IsApproved = x.Key.IsApproved != null*/
                                      //Rush = x.Key.Rush != null ? true : false,

                                  })/*.Where(x => x.Rush == true)*/;

            return await orders.ToListAsync();
        }


        public async Task<IReadOnlyList<DtoForMoveOrderNotif>> GetMoveOrdersForNotification()
        {
            var orders = _context.Orders
                                  .GroupBy(x => new
                                  {
                                      x.TrasactId,
                                      x.CustomerName,
                                      x.IsActive,
                                      x.IsApproved,
                                      x.IsMove,
                                      x.Rush,
                                      x.PreparedDate

                                  }).Where(x => x.Key.IsApproved == true)
                                    .Where(x => x.Key.IsActive == true)
                                    .Where(x => x.Key.PreparedDate != null)
                                    .Where(x => x.Key.IsMove == false)
                                    .Select(x => new DtoForMoveOrderNotif
                                    {
                                        MIRId = x.Key.TrasactId,
                                        CustomerName = x.Key.CustomerName,
                                        IsActive = x.Key.IsActive,
                                        IsApproved = x.Key.IsApproved != null,
                                        Rush = x.Key.Rush != null ? true : false,

                                    }).Where(x => x.Rush == true);

            return await orders.ToListAsync();
        }

        public async Task<IReadOnlyList<DtoForMoveOrderNotif>> GetMoveOrdersForNotificationNotRush()
        {
            var orders = _context.Orders
                                 .GroupBy(x => new
                                 {
                                     x.TrasactId,
                                     x.CustomerName,
                                     x.IsActive,
                                     x.IsApproved,
                                     x.IsMove,
                                     x.Rush,
                                     x.PreparedDate

                                 }).Where(x => x.Key.IsApproved == true)
                                   .Where(x => x.Key.IsActive == true)
                                   .Where(x => x.Key.PreparedDate != null)
                                   .Where(x => x.Key.IsMove == false)
                                   .Select(x => new DtoForMoveOrderNotif
                                   {
                                       MIRId = x.Key.TrasactId,
                                       CustomerName = x.Key.CustomerName,
                                       IsActive = x.Key.IsActive,
                                       IsApproved = x.Key.IsApproved != null,
                                       Rush = x.Key.Rush != null ? true : false,

                                   }).Where(x => x.Rush == false);

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



        public async Task<IReadOnlyList<DtoForApprovalMoveOrderNotif>> GetForApprovalMoveOrdersNotificationAll()
        {


            var orderingtransaction = _context.Orders.GroupBy(x => new
            {
                x.TrasactId,
                x.IsMove,

            }).Select(x => new DtoForApprovalMoveOrderNotif
            {
                MIRId = x.Key.TrasactId,
                IsMove = x.Key.IsMove,

            });


            var orders = _context.MoveOrders
                  .GroupJoin(orderingtransaction, moveorders => moveorders.OrderNo, ordering => ordering.MIRId, (moveorders, ordering) => new { moveorders, ordering })
                .SelectMany(x => x.ordering.DefaultIfEmpty(), (x, ordering) => new { x.moveorders, ordering })
                .Where(x => x.ordering.IsMove == true && x.moveorders.IsApprove == null && x.moveorders.PreparedDate != null && x.moveorders.IsActive == true)
                .GroupBy(x => new
                {
                    x.moveorders.OrderNo,
                    x.moveorders.Customercode,
                    x.moveorders.CustomerName,
                    x.moveorders.OrderDate,
                    x.moveorders.PreparedDate,
                    x.moveorders.IsApprove,
                    x.moveorders.IsPrepared,




                }).Select(x => new DtoForApprovalMoveOrderNotif
                {
                    MIRId = x.Key.OrderNo,
                    CustomerCode = x.Key.Customercode,
                    CustomerName = x.Key.CustomerName,
                    Quantity = x.Sum(x => x.moveorders.QuantityOrdered),
                    OrderDate = x.Key.OrderDate.ToString(),
                    PreparedDate = x.Key.PreparedDate.ToString(),
                });



       //     var orders = _context.MoveOrders.Where(x => x.IsApproveReject == null)
       //     .GroupBy(x => new
       //     {

       //         x.OrderNo,
       //         x.Customercode,
       //         x.CustomerName,
       //         x.OrderDate,
       //         x.PreparedDate,
       //         x.IsApprove,
       //         x.IsPrepared,
       //         //x.Rush

       //     }).Where(x => x.Key.IsApprove != true)
       //       .Where(x => x.Key.IsPrepared == true)

       //.Select(x => new DtoForApprovalMoveOrderNotif
       //{
       //    MIRId = x.Key.OrderNo,
       //    CustomerCode = x.Key.Customercode,
       //    CustomerName = x.Key.CustomerName,
       //    Quantity = x.Sum(x => x.QuantityOrdered),
       //    OrderDate = x.Key.OrderDate.ToString(),
       //    PreparedDate = x.Key.PreparedDate.ToString(),
       //    //Rush = x.Key.Rush != null ? true : false,

       //})/*.Where(x => x.Rush == false)*/;

            return await orders.ToListAsync();
        }




        public async Task<IReadOnlyList<DtoForApprovalMoveOrderNotif>> GetForApprovalMoveOrdersNotification()
        {

            var orderingtransaction = _context.Orders.GroupBy(x => new
            {
                x.TrasactId,
                x.IsMove,

            }).Select(x => new DtoForApprovalMoveOrderNotif
            {
                MIRId = x.Key.TrasactId,
                IsMove = x.Key.IsMove,

            });




            var orders = _context.MoveOrders
                   .GroupJoin(orderingtransaction, moveorders => moveorders.OrderNo, ordering => ordering.MIRId, (moveorders, ordering) => new { moveorders, ordering })
                 .SelectMany(x => x.ordering.DefaultIfEmpty(), (x, ordering) => new { x.moveorders, ordering })
                 .Where(x => x.ordering.IsMove == true && x.moveorders.IsApprove == null && x.moveorders.PreparedDate != null && x.moveorders.IsActive == true)
                 .GroupBy(x => new
                 {
                     x.moveorders.OrderNo,
                     x.moveorders.Customercode,
                     x.moveorders.CustomerName,
                     x.moveorders.OrderDate,
                     x.moveorders.PreparedDate,
                     x.moveorders.IsApprove,
                     x.moveorders.Rush,

                 }).Select(x => new DtoForApprovalMoveOrderNotif
            {
           MIRId = x.Key.OrderNo,
           CustomerCode = x.Key.Customercode,
           CustomerName = x.Key.CustomerName,
           Quantity = x.Sum(x => x.moveorders.QuantityOrdered),
           OrderDate = x.Key.OrderDate.ToString(),
           PreparedDate = x.Key.PreparedDate.ToString(),
           Rush = x.Key.Rush != null ? true : false,

       }).Where(x => x.Rush == true);

            return await orders.ToListAsync();

        }


        public async Task<IReadOnlyList<DtoForApprovalMoveOrderNotif>> GetForApprovalMoveOrdersNotificationNotRush()
        {


            var orderingtransaction = _context.Orders.GroupBy(x => new
            {
                x.TrasactId,
                x.IsMove,

            }).Select(x => new DtoForApprovalMoveOrderNotif
            {
                MIRId = x.Key.TrasactId,
                IsMove = x.Key.IsMove,

            });




            var orders = _context.MoveOrders
                   .GroupJoin(orderingtransaction, moveorders => moveorders.OrderNo, ordering => ordering.MIRId, (moveorders, ordering) => new { moveorders, ordering })
                 .SelectMany(x => x.ordering.DefaultIfEmpty(), (x, ordering) => new { x.moveorders, ordering })
                 .Where(x => x.ordering.IsMove == true && x.moveorders.IsApprove == null && x.moveorders.PreparedDate != null && x.moveorders.IsActive == true)
                 .GroupBy(x => new
                 {
                     x.moveorders.OrderNo,
                     x.moveorders.Customercode,
                     x.moveorders.CustomerName,
                     x.moveorders.OrderDate,
                     x.moveorders.PreparedDate,
                     x.moveorders.IsApprove,
                     x.moveorders.Rush,

                 }).Select(x => new DtoForApprovalMoveOrderNotif
                 {
                     MIRId = x.Key.OrderNo,
                     CustomerCode = x.Key.Customercode,
                     CustomerName = x.Key.CustomerName,
                     Quantity = x.Sum(x => x.moveorders.QuantityOrdered),
                     OrderDate = x.Key.OrderDate.ToString(),
                     PreparedDate = x.Key.PreparedDate.ToString(),
                     Rush = x.Key.Rush != null ? true : false,

                 }).Where(x => x.Rush == false);

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



        public async Task<IReadOnlyList<GetallApproveDto>> GetAllListForApprovalOfScheduleAll()
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


                                  })



            .Select(x => new GetallApproveDto
            {
                MIRId = x.Key.TrasactId,
                Department = x.Key.Department,
                CustomerName = x.Key.CustomerName,
                CustomerCode = x.Key.Customercode,



            })/*.Where(x => x.IsRush == true)*/;

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
                                          x.Rush


                                      })

            .Select(x => new GetallApproveDto
            {
                MIRId = x.Key.TrasactId,
                Department = x.Key.Department,
                CustomerName = x.Key.CustomerName,
                CustomerCode = x.Key.Customercode,
                IsRush = x.Key.Rush != null ? true : false



            }).Where(x => x.IsRush == true);

            return await orders.ToListAsync();

        }



        public async Task<IReadOnlyList<GetallApproveDto>> GetAllListForApprovalOfScheduleNotRush()
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
                                          x.Rush



                                      })

            .Select(x => new GetallApproveDto
            {
                MIRId = x.Key.TrasactId,
                Department = x.Key.Department,
                CustomerName = x.Key.CustomerName,
                CustomerCode = x.Key.Customercode,
                IsRush = x.Key.Rush != null ? true : false


            }).Where(x => x.IsRush == false);

            return await orders.ToListAsync();

        }



    

      

     

       








        //================================= Validation =============================================================================

        public async Task<bool> ValidateDateNeeded(Ordering orders)
        {
            var dateNow = DateTime.Now/*.AddDays(-5)*/;

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

        public async Task<bool> ValidateItemCode(string ItemCode, string itemdescription , string uom)
        {
            var validate = await _context.Materials.Where(x => x.ItemCode == ItemCode)
                                                .Where(x => x.ItemDescription == itemdescription)
                                                .Where(x => x.Uom.UomCode == uom)
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
            bool status = true;



            //if(search == null)
            //{
            //   GetAllListOfMir(status);
            //}

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

        public async Task<PagedList<GetAllListOfMirDto>> GetAllListOfMirNoSearch(UserParams userParams, bool status)
        {
            var orders = _context.Orders
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
                                           
                                           //x.ItemRemarks

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
                                           Rush = x.Key.Rush,
                                           //ItemRemarks = x.Key.ItemRemarks

                                       }).Where(x => x.IsRush == status);

            return await PagedList<GetAllListOfMirDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
        }



        public async Task<PagedList<GetAllListOfMirDto>> GetAllListOfMir(UserParams userParams, bool status , string search)
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

            var orders = _context.Orders
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
                                            //x.ItemRemarks

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
                                            Rush = x.Key.Rush,
                                            //ItemRemarks = x.Key.ItemRemarks

                                        }).Where(x => x.IsRush == status)
                                          .Where(x => Convert.ToString(x.CustomerName).ToLower().Contains(search.Trim().ToLower())
                                          || Convert.ToString(x.CustomerCode).ToLower().Contains(search.Trim().ToLower())
                                           || Convert.ToString(x.CustomerType).ToLower().Contains(search.Trim().ToLower())
                                            || Convert.ToString(x.MIRId).ToLower().Contains(search.Trim().ToLower()));



            return await PagedList<GetAllListOfMirDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
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
                                            //x.ItemRemarks

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
                                            Rush = x.Key.Rush,
                                            //ItemRemarks = x.Key.ItemRemarks
                                           

                                        }).ToListAsync();

        }


        public async Task<IReadOnlyList<DtoViewListOfMirOrders>> ViewListOfMirOrders(int id)
        {

            var orders = _context.Orders.Where(x => x.IsActive == true && x.IsPrepared != true && x.TrasactId == id)
                                        .GroupBy(x => x.TrasactId)
                                        .Select(x => new DtoViewListOfMirOrders
                                        {

                                            MirId = x.Key,
                                            OrderNo = x.First().OrderNo,
                                            OrderDate = x.First().OrderDate.ToString(),
                                            DateNeeded = x.First().DateNeeded.ToString(),
                                            CustomerCode = x.First().Customercode,
                                            CustomerName = x.First().CustomerName,
                                            CompanyCode = x.First().CompanyCode,
                                            CompanyName = x.First().CompanyName,
                                            DepartmentCode = x.First().DepartmentCode,
                                            DepartmentName = x.First().Department,
                                            LocationCode = x.First().LocationCode,
                                            LocationName = x.First().LocationName,

                                            ListOrder = x.Select(x => new DtoViewListOfMirOrders.ListOrders
                                            {
                                                Id = x.Id,
                                                ItemCode = x.ItemCode,
                                                ItemDescription = x.ItemdDescription,
                                                Uom = x.Uom,
                                                ItemRemarks = x.ItemRemarks,

                                                Quantity = x.QuantityOrdered,
                                                AccountCode = x.AccountCode,
                                                AccountTitles = x.AccountTitles,
                                                EmpId = x.EmpId,
                                                FullName = x.FullName
                                                
                                            }).ToList()

                                        });

            return await orders.ToListAsync();

        }




        //private static Dictionary<string, decimal> stockOnHandDict = new Dictionary<string, decimal>();

        public async Task<IEnumerable<AllOrdersPerMIRIDsDTO>> GetAllListOfMirOrdersbyMirId(int[] listofMirIds/*, string customerName*/)
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
                                                              
                                                                .GroupBy(x => new
                                                                {

                                                                    x.ItemCode,
                                                                }).Select(x => new IssueInventoryDto
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
                                                                 In = x.Sum(x => x.returned.Quantity) - x.Sum(x => x.itemconsume.Consume),

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
            var reserveDict = new Dictionary<string, decimal>();


            var orders = _context.Orders
                .Where(ordering =>/* ordering.CustomerName == customerName &&*/ ordering.PreparedDate == null && ordering.IsActive == true)
                .Where(x => listofMirIds.Contains(x.TrasactId))
                .GroupJoin(getReserve, ordering => ordering.ItemCode, warehouse => warehouse.ItemCode, (ordering, warehouse) => new { ordering, warehouse })
                .SelectMany(x => x.warehouse.DefaultIfEmpty(), (x, warehouse) => new { x.ordering, warehouse })
                .GroupBy(x => new
                {
                    x.ordering.Id,
                    x.ordering.OrderNo,
                    x.ordering.Customercode,
                    x.ordering.CustomerName,
                    x.ordering.TrasactId,
                    x.ordering.ItemCode,
                    x.ordering.ItemdDescription,
                    x.ordering.Uom,
                    x.ordering.ItemRemarks,
                    x.ordering.StandartQuantity,
                    x.ordering.AccountCode,
                    x.ordering.AccountTitles,
                    x.ordering.EmpId,
                    x.ordering.FullName,
                    x.ordering.AssetTag,
                    Reserve = x.warehouse.Reserve != null ? x.warehouse.Reserve : 0
                    
                })
                .Select(total => new AllOrdersPerMIRIDsDTO
                {

                    Id = total.Key.Id, 
                    OrderNo = total.Key.OrderNo,
                    MIRId = total.Key.TrasactId,
                    CustomerCode = total.Key.Customercode,
                    CustomerName = total.Key.CustomerName,
                    ItemCode = total.Key.ItemCode,
                    ItemDescription = total.Key.ItemdDescription,
                    Uom = total.Key.Uom,
                    QuantityOrder = total.Sum(x => x.ordering.QuantityOrdered),
                    ItemRemarks = total.Key.ItemRemarks,
                    AccountCode = total.Key.AccountCode,
                    AccountTitles = total.Key.AccountTitles,
                    EmpId = total.Key.EmpId,
                    FullName = total.Key.FullName,
                    StandardQuantity = total.Key.StandartQuantity,
                    ActualReserve = total.Key.Reserve > 0 ? total.Key.Reserve : 0,
                    Reserve = total.Key.Reserve > 0 ? total.Key.Reserve : 0,
                    StockOnHand = total.Key.Reserve > 0 ? total.Key.Reserve : 0,
                    AssetTag = total.Key.AssetTag

                   
                }).ToList();

            foreach (var order in orders)
            {
                if (!stockOnHandDict.ContainsKey(order.ItemCode))
                {
                    stockOnHandDict[order.ItemCode] = order.StockOnHand;

                }
                if (!reserveDict.ContainsKey(order.ItemCode))
                {
                    reserveDict[order.ItemCode] = order.Reserve;

                }


                if (listofMirIds.Contains(order.MIRId))
                {
                    stockOnHandDict[order.ItemCode] -= order.QuantityOrder;
                    reserveDict[order.ItemCode] -= order.QuantityOrder;
                }

                if (stockOnHandDict[order.ItemCode] <= 0)
                {
                    stockOnHandDict[order.ItemCode] = 0;
                }

                order.StockOnHand = stockOnHandDict[order.ItemCode] ;
                order.Reserve = reserveDict[order.ItemCode];

            }


            var itemCodess = orders.Select(x => x.ItemCode).Distinct();
            foreach (var itemCode in itemCodess)
            {
                var stockOnHand = stockOnHandDict[itemCode];
                var reserve = reserveDict[itemCode];
                foreach (var order in orders.Where(x => x.ItemCode == itemCode))
                {
                    order.StockOnHand = stockOnHand;
                    order.Reserve = reserve;
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
            existingOrder.Remarks = orders.Remarks;

            existingOrder.Modified_Date = DateTime.Now;
            existingOrder.Modified_By = orders.Modified_By;
            existingOrder.AccountCode = orders.AccountCode;
            existingOrder.AccountTitles = orders.AccountTitles;
            existingOrder.EmpId = orders.EmpId;
            existingOrder.FullName = orders.FullName;

            return true;
        }

                         
        public async Task<bool> CancelOrders(Ordering orders)
        {
            var existing = await _context.Orders.Where(x => x.Id == orders.Id)
                                                 .FirstOrDefaultAsync();

            if (existing == null)
                return false;

            existing.IsActive = false;
            existing.IsCancelBy = orders.IsCancelBy;
            existing.Modified_Date = DateTime.Now;
            
            existing.IsCancel = true;
            existing.CancelDate = DateTime.Now;
            existing.Remarks = orders.Remarks;

            return true;
        }


        //=============================================== MIR Update In Ordering For Approval =======================================================================




        public async Task<IReadOnlyList<GetallApproveListDto>> GetAllListForApprovalOfSchedule(bool status)
        {

            var orders = _context.Orders.OrderBy(x => x.PreparedDate)
                                       .Where(x => x.IsApproved == null)
                                      .Where(x => x.PreparedDate != null)
                                      .Where(x => x.IsActive == true)
                                      .GroupBy(x => x.TrasactId)
                                      .Select(x => new GetallApproveListDto
                                      {
                                          MIRId = x.Key,
                                          CustomerCode = x.First().Customercode,
                                          CustomerName = x.First().CustomerName,
                                          TotalOrders = x.Sum(x => x.QuantityOrdered),
                                          PreparedDate = x.First().PreparedDate.ToString(),
                                          DateNeeded = x.First().DateNeeded.ToString(),
                                          OrderDate = x.First().OrderDate.ToString(),
                                          IsRush = x.First().Rush != null ? true : false,
                                          Status = x.First().IsApproved == null ? "For Approval" : "Approve",
                                          Order = x.Select(x => new GetallApproveListDto.Orders
                                          {

                                              OrderNo = x.OrderNo,
                                              ItemCode = x.ItemCode,
                                              ItemDescription = x.ItemdDescription,
                                              Category = x.Category,
                                              Uom = x.Uom,
                                              QuantityOrder = x.QuantityOrdered,
                                              ItemRemarks = x.ItemRemarks,
                                           
                                          }).ToList()
                                          
                                      }).Where(x => x.IsRush == status);
                                    

            return await orders.ToListAsync();

        }




        public async Task<IReadOnlyList<GetallOrderfroScheduleApproveDto>> GetAllOrdersForScheduleApproval(int Id)
        {
            var orders = _context.Orders.OrderBy(x => x.PreparedDate)
                                        .Where(x => x.TrasactId == Id)
                                        .Where(x => x.IsPrepared == true)
                                        .Where(x => x.IsApproved == null)
                                        .Where(x => x.IsActive == true)
                                        .Select(x => new GetallOrderfroScheduleApproveDto
                                        {
                                            MIRId = x.TrasactId,
                                            OrderId = x.Id, 
                                            Department = x.Department,
                                            CustomerName = x.CustomerName,
                                            CustomerCode = x.Customercode,
                                            ItemCode = x.ItemCode,
                                            ItemDescription = x.ItemdDescription,
                                            Category = x.Category,
                                            Uom = x.Uom,
                                            QuantityOrder = x.QuantityOrdered,
                                            ItemRemarks = x.ItemRemarks,
                                            AssetTag = x.AssetTag
                                            

                                        });

            return await orders.ToListAsync();

        }

        public async Task<bool> ApprovePreparedDate(Ordering orders)
        {
            var order = await _context.Orders.Where(x => x.TrasactId == orders.TrasactId)
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
                                             .ToListAsync();
            foreach (var items in order)
            {
                items.IsReject = true;
                items.RejectBy = orders.RejectBy;

                items.Remarks = orders.Remarks;
                items.RejectedDate = DateTime.Now;
                items.PreparedDate = null;
                items.IsPrepared = false;
            }


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
                x.Rush,
                x.IsActive

            }).Where(x => x.Key.IsApproved == true)
              .Where(x => x.Key.PreparedDate != null)
              .Where(x => x.Key.IsActive == true)
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

        public async Task<PagedList<TotalListOfApprovedPreparedDateDto>> TotalListOfApprovedPreparedDateNoSearch(UserParams userParams, bool status)
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
                x.IsActive,

                x.Department,
                x.DepartmentCode,
                x.CompanyName,
                x.CompanyCode,
                x.LocationCode,
                x.LocationName,
                x.Rush


            })
            .Where(x => x.Key.IsApproved == true)
            .Where(x => x.Key.IsActive == true)
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

            return await PagedList<TotalListOfApprovedPreparedDateDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<PagedList<TotalListOfApprovedPreparedDateDto>> TotalListOfApprovedPreparedDate(UserParams userParams, bool status , string search)
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
                x.IsActive,

                x.Department,
                x.DepartmentCode,
                x.CompanyName,
                x.CompanyCode,
                x.LocationCode,
                x.LocationName,

                x.Rush


            })
            .Where(x => x.Key.IsApproved == true)
            .Where(x => x.Key.IsActive == true)
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
                    
                IsRush = x.Key.Rush != null  ? true : false,
                Rush = x.Key.Rush,

            }).Where(x => x.IsRush == status)
              .Where(x => Convert.ToString(x.CustomerName).ToLower().Contains(search.Trim().ToLower())
              || Convert.ToString(x.CustomerCode).ToLower().Contains(search.Trim().ToLower())
              || Convert.ToString(x.Id).ToLower().Contains(search.Trim().ToLower()));

            return await PagedList<TotalListOfApprovedPreparedDateDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);

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
                   .Where(x => x.IsActive == true)
                   .Where(x => x.IsMove == false)
                   .GroupJoin(moveorders, ordering => ordering.Id, moveorder => moveorder.OrderPKey, (ordering, moveorder) => new { ordering, moveorder })
                   .SelectMany(x => x.moveorder.DefaultIfEmpty(), (x, moveorder) => new { x.ordering, moveorder })
                   .GroupBy(x => new
                   {
                       x.ordering.Id,        
                       x.ordering.OrderNo,
                       x.ordering.TrasactId,
                       x.ordering.OrderDate,
                       x.ordering.DateNeeded,
                       x.ordering.Department,
                       x.ordering.CustomerName,
                       x.ordering.Customercode,
                       x.ordering.ItemCode,
                       x.ordering.ItemdDescription,
                       x.ordering.Uom,
                       x.ordering.ItemRemarks,
                       x.ordering.Remarks,
                       x.ordering.AccountCode,
                       x.ordering.AccountTitles,
                       x.ordering.EmpId,
                       x.ordering.FullName,
                       x.ordering.AssetTag

                   })

                   .Select(total => new ListOfOrdersForMoveOrderDto
                   {
                       Id = total.Key.Id,
                       OrderNo = total.Key.OrderNo,
                       MIRId = total.Key.TrasactId,
                       OrderDate = total.Key.OrderDate.ToString("MM/dd/yyyy"),
                       DateNeeded = total.Key.DateNeeded.ToString("MM/dd/yyyy"),
                       ItemCode = total.Key.ItemCode,
                       ItemDescription = total.Key.ItemdDescription,
                       Uom = total.Key.Uom,
                       QuantityOrder = total.Sum(x => x.ordering.QuantityOrdered),
                       ItemRemarks = total.Key.ItemRemarks,
                       Remarks = total.Key.Remarks,
                       PreparedQuantity = total.Sum(x => x.moveorder.QuantityPrepared),
                       AccountCode = total.Key.AccountCode,
                       AccountTitles = total.Key.AccountTitles,
                       EmpId = total.Key.EmpId,
                       FullName = total.Key.FullName,
                       AssetTag = total.Key.AssetTag
                       
                       
                       

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
                                                                      //.Where(x => x.IsApproved == false)
                                                                      .SumAsync(x => x.Quantity);

            var consumed = _context.BorrowedConsumes.Where(x => x.IsActive)
                                                 .GroupBy(x => new
                                                 {
                                                   
                                                     x.BorrowedItemPkey

                                                 }).Select(x => new ItemStocksDto
                                                 {

                                                     BorrowedItemPkey = x.Key.BorrowedItemPkey,
                                                     Consume = x.Sum(x => x.Consume != null ? x.Consume : 0)

                                                 });







            var TotalBorrowReturned = await _context.BorrowedIssueDetails
                   .GroupJoin(consumed, returned => returned.Id, itemconsume => itemconsume.BorrowedItemPkey, (returned, itemconsume) => new { returned, itemconsume })
                                                             .SelectMany(x => x.itemconsume.DefaultIfEmpty(), (x, itemconsume) => new { x.returned, itemconsume })
                                                                      .Where(x => x.returned.WarehouseId == id)
                                                                      .Where(x => x.returned.IsActive == true)
                                                                      .Where(x => x.returned.IsReturned == true)
                                                                      .Where(x => x.returned.IsApprovedReturned == true)
                                                                      .SumAsync(x => x.returned.Quantity - (x.itemconsume.Consume != null ? x.itemconsume.Consume : 0));



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



            var getBorrowedReturn = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                             .Where(x => x.IsReturned == true)
                                                             .Where(x => x.IsApprovedReturned == true)
                                                             .GroupJoin(consumed, returned => returned.Id, itemconsume => itemconsume.BorrowedItemPkey, (returned, itemconsume) => new { returned, itemconsume })
                                                             .SelectMany(x => x.itemconsume.DefaultIfEmpty(), (x, itemconsume) => new { x.returned, itemconsume })
                                                             .GroupBy(x => new
                                                             {
                                                                 x.returned.ItemCode,
                                                                 x.returned.WarehouseId,


                                                             }).Select(x => new ItemStocksDto
                                                             {

                                                                 ItemCode = x.Key.ItemCode,
                                                                 Remaining = x.Sum(x => x.returned.Quantity) - x.Sum(x => x.itemconsume.Consume),
                                                                 warehouseId = x.Key.WarehouseId,

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
                                                                //.Where(x => x.IsApproved == false)
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
                                                                 x.returned.WarehouseId,
                                                                 //x.itemconsume.Consume

                                                             }).Select(x => new ItemStocksDto
                                                             {

                                                                 ItemCode = x.Key.ItemCode,
                                                                 In = x.Sum(x => x.returned.Quantity) - x.Sum(x => x.itemconsume.Consume),
                                                                 warehouseId = x.Key.WarehouseId,

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
                .Where(x => x.IsActive == true)
                .Select(x => new GetMoveOrderDetailsForMoveOrderDto
                {
                    Id = x.Id,
                    MIRId = x.TrasactId,
                    
                    OrderNoGenus = x.OrderNo,

                    Department = x.Department,
                    DepartmentCode = x.DepartmentCode,
                    CompanyName = x.CompanyName,
                    CompanyCode = x.CompanyCode,
                    LocationCode = x.LocationCode,
                    LocationName = x.LocationName,
                    AccountCode = x.AccountCode,
                    AccountTitles = x.AccountTitles,
                    EmpId = x.EmpId,
                    FullName = x.FullName,


                    Remarks = x.Remarks,

                    CustomerName = x.CustomerName,
                    CustomerCode = x.Customercode,

                    ItemCode = x.ItemCode,
                    ItemDescription = x.ItemdDescription,
                    Uom = x.Uom,
                    QuantityOrder = x.QuantityOrdered,
                    Category = x.Category,
                    OrderDate = x.OrderDate.ToString(),
                    DateNeeded = x.DateNeeded.ToString(),
                    PrepareDate = x.PreparedDate.ToString(),

                    HelpDeskNo = x.HelpdeskNo,

                    CustomerType = x.CustomerType,
                    Rush = x.Rush,                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
                    ItemRemarks = x.ItemRemarks,
                    Cip_no = x.Cip_No,
                    AssetTag = x.AssetTag,
                    DateApproved = x.DateApproved.ToString(),
                    Approver = x.Approver,
                    Requestor = x.Requestor

                    
                    



                });

            return await orders.Where(x => x.Id == orderId)
                               .FirstOrDefaultAsync();

        }



        public async Task<bool> PrepareItemForMoveOrder(MoveOrder orders)
        {

            var UnitCost = await _context.WarehouseReceived.Where(x => x.Id == orders.WarehouseId)
                                                           .Where(x => x.IsActive == orders.IsActive)
                                                           .FirstOrDefaultAsync();

            orders.UnitPrice = UnitCost.UnitPrice;

            orders.PreparedBy = orders.PreparedBy;

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
                .OrderBy(x => x.WarehouseId)
                .Where(x => x.IsPrepared == true)

                 .Select(x => new ListOfPreparedItemsForMoveOrderDto
                 {
                     Id = x.Id,
                     OrderNo = x.OrderNo,
                     BarCodes = x.WarehouseId,
                     ItemCode = x.ItemCode,
                     ItemDescription = x.ItemDescription,
                     Quantity = x.QuantityOrdered,
                     IsActive = x.IsActive,
                     Rush = x.Rush,
                     UnitCost = x.UnitPrice,
                     TotalCost = x.UnitPrice * x.QuantityOrdered

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
                //x.AccountCode = order.AccountCode;
                //x.AccountTitles = order.AccountTitles;
                //x.EmpId = order.EmpId;
                //x.FullName = order.FullName;
                x.ApprovedDate = DateTime.Now;
                x.ApproveDateTempo = DateTime.Now;
                x.IsApprove = true;
                x.DateApproved = DateTime.Now;

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
            existing.IsPrepared = false;
            existing.CancelledDate = DateTime.Now;

            return true;

        }


        // ================================================== MIR MoveOrder For Approval ========================================================

        public async Task<IReadOnlyList<ForApprovalMoveOrderListDto>> ForApprovalMoveOrderPagination(bool status)
        {

            var unitcostId = _context.MoveOrders.Where(x => x.IsActive == true)
                .GroupBy(x => new
                {

                   x.OrderNo,
                   x.UnitPrice,
                   x.ItemCode,
                   x.ItemDescription,
                   x.Uom,
                   x.QuantityOrdered,
                   x.Id
                   

                })
                .Select(x => new ViewMoveOrderForApprovalDto
                {

                    MIRId = x.Key.OrderNo,
                    Id = x.Key.Id,
                    ItemCode = x.Key.ItemCode,
                    ItemDescription = x.Key.ItemDescription,
                    Uom = x.Key.Uom,
                    UnitCost = x.Key.UnitPrice * x.Key.QuantityOrdered,
                    Quantity = x.Key.QuantityOrdered,


                });


            var UnitPriceTotal = unitcostId.GroupBy(x => new
            {
                //x.MIRId,
                x.ItemCode,
                x.ItemDescription,
                x.Uom,




            }).Select(x => new ViewMoveOrderForApprovalDto
            {
                ItemCode = x.Key.ItemCode,
                ItemDescription = x.Key.ItemDescription,
                Uom = x.Key.Uom,
                UnitCost = x.Sum(x => x.UnitCost) / x.Sum(x => x.Quantity),
                Quantity = x.Sum(x => x.Quantity)

           
            });


            var moverders = _context.Orders
            .GroupJoin(_context.MoveOrders, order => order.TrasactId, moveorder => moveorder.OrderNo, (order, moveorder) => new { order, moveorder })
            .SelectMany(x => x.moveorder.DefaultIfEmpty(), (x, moveorder) => new { x.order, moveorder })
            .GroupJoin(UnitPriceTotal, order => order.order.ItemCode , unitcost => unitcost.ItemCode , (order ,unitcost) => new { order , unitcost })
            .SelectMany(x => x.unitcost.DefaultIfEmpty() , (x , unitcost) => new {x.order , unitcost})
            .Where(x =>  x.order.moveorder.IsActive == true && x.order.moveorder.IsReject == null && x.order.moveorder.IsApprove == null && x.order.moveorder.IsPrepared == true && x.order.order.IsMove == true)
            .GroupBy(x => new
            {
                x.order.order.TrasactId,
                x.order.moveorder.Customercode,
                x.order.moveorder.CustomerName,
                x.order.moveorder.PreparedDate,
                x.order.moveorder.CompanyCode,
                x.order.moveorder.CompanyName,
                x.order.moveorder.DepartmentCode,
                x.order.moveorder.Department,
                x.order.moveorder.LocationCode,
                x.order.moveorder.LocationName,
                x.order.moveorder.AccountCode,
                x.order.moveorder.AccountTitles,
                x.order.moveorder.EmpId,
                x.order.moveorder.FullName,
                x.order.moveorder.Rush,
                x.order.moveorder.IsPrepared,
                x.order.moveorder.IsApprove,

            })
            .Select(x => new ForApprovalMoveOrderListDto
            {
                MIRId = x.Key.TrasactId,
                Customercode = x.Key.Customercode,
                CustomerName = x.Key.CustomerName,
                Quantity = x.Sum(x => x.order.moveorder.QuantityOrdered),
                PreparedDate = x.Key.PreparedDate.ToString(),
                CompanyCode = x.Key.CompanyCode,
                CompanyName = x.Key.CustomerName,
                DepartmentCode = x.Key.DepartmentCode,
                DepartmentName = x.Key.Department,
                LocationCode = x.Key.LocationCode,
                LocationName = x.Key.LocationName,
                AccountCode = x.Key.AccountCode,
                AccountTitles = x.Key.AccountTitles,
                EmpId = x.Key.EmpId,
                FullName = x.Key.FullName,
                IsRush = x.Key.Rush != null ?  true : false,
                Status = x.Key.IsPrepared == true && x.Key.IsApprove == null ? "For Approval" : "Approve",
                Order = x.GroupBy(x => new
                {
                    x.order.order.OrderNo,
                    x.unitcost.ItemCode,
                    x.unitcost.ItemDescription,
                    x.unitcost.Uom,
                    x.order.moveorder.ItemRemarks

                }).
                Select(x => new ForApprovalMoveOrderListDto.Orders
                {
                    OrderNo = x.Key.OrderNo,
                    ItemCode = x.Key.ItemCode,
                    ItemDesciption = x.Key.ItemDescription,
                    Uom = x.Key.Uom,
                    ItemRemarks = x.Key.ItemRemarks,
                    Quantity = x.Sum(x => x.unitcost.Quantity),
                    UnitCost = x.Sum(x => x.unitcost.UnitCost)


                }).ToList()



            }).Where(x => x.IsRush == status);

            return await moverders.ToListAsync();

        }


        public async Task<IReadOnlyList<ForApprovalMoveOrderListDto>> ForApprovalMoveOrderPaginationOrig(string search, bool status)
        {
            var unitcostId = _context.MoveOrders.Where(x => x.IsActive == true)
              .GroupBy(x => new
              {

                  x.OrderNo,
                  x.UnitPrice,
                  x.ItemCode,
                  x.ItemDescription,
                  x.Uom,
                  x.QuantityOrdered,
                  x.Id


              })
              .Select(x => new ViewMoveOrderForApprovalDto
              {

                  MIRId = x.Key.OrderNo,
                  Id = x.Key.Id,
                  ItemCode = x.Key.ItemCode,
                  ItemDescription = x.Key.ItemDescription,
                  Uom = x.Key.Uom,
                  UnitCost = x.Key.UnitPrice * x.Key.QuantityOrdered,
                  Quantity = x.Key.QuantityOrdered,


              });


            var UnitPriceTotal = unitcostId.GroupBy(x => new
            {

                x.ItemCode,
                x.ItemDescription,
                x.Uom,




            }).Select(x => new ViewMoveOrderForApprovalDto
            {

                ItemCode = x.Key.ItemCode,
                ItemDescription = x.Key.ItemDescription,
                Uom = x.Key.Uom,
                UnitCost = x.Sum(x => x.UnitCost) / x.Sum(x => x.Quantity),
                Quantity = x.Sum(x => x.Quantity)


            });


            var moverders = _context.Orders
            .GroupJoin(_context.MoveOrders, order => order.TrasactId, moveorder => moveorder.OrderNo, (order, moveorder) => new { order, moveorder })
            .SelectMany(x => x.moveorder.DefaultIfEmpty(), (x, moveorder) => new { x.order, moveorder })
            .GroupJoin(UnitPriceTotal, order => order.order.ItemCode, unitcost => unitcost.ItemCode, (order, unitcost) => new { order, unitcost })
            .SelectMany(x => x.unitcost.DefaultIfEmpty(), (x, unitcost) => new { x.order, unitcost })
            .Where(x => x.order.moveorder.IsActive == true && x.order.moveorder.IsReject == null && x.order.moveorder.IsApprove == null && x.order.moveorder.IsPrepared == true && x.order.order.IsMove == true)
            .GroupBy(x => new
            {
                x.order.order.TrasactId,
                x.order.moveorder.Customercode,
                x.order.moveorder.CustomerName,
                x.order.moveorder.PreparedDate,
                x.order.moveorder.CompanyCode,
                x.order.moveorder.CompanyName,
                x.order.moveorder.DepartmentCode,
                x.order.moveorder.Department,
                x.order.moveorder.LocationCode,
                x.order.moveorder.LocationName,
                x.order.moveorder.AccountCode,
                x.order.moveorder.AccountTitles,
                x.order.moveorder.EmpId,
                x.order.moveorder.FullName,
                x.order.moveorder.Rush,
                x.order.moveorder.IsPrepared,
                x.order.moveorder.IsApprove,
                //x.unitcost.ItemCode,

            })
            .Select(x => new ForApprovalMoveOrderListDto
            {
                MIRId = x.Key.TrasactId,
                Customercode = x.Key.Customercode,
                CustomerName = x.Key.CustomerName,
                Quantity = x.Sum(x => x.order.moveorder.QuantityOrdered),
                PreparedDate = x.Key.PreparedDate.ToString(),
                CompanyCode = x.Key.CompanyCode,
                CompanyName = x.Key.CustomerName,
                DepartmentCode = x.Key.DepartmentCode,
                DepartmentName = x.Key.Department,
                LocationCode = x.Key.LocationCode,
                LocationName = x.Key.LocationName,
                AccountCode = x.Key.AccountCode,
                AccountTitles = x.Key.AccountTitles,
                EmpId = x.Key.EmpId,
                FullName = x.Key.FullName,
                IsRush = x.Key.Rush != null ? true : false,
                Status = x.Key.IsPrepared == true && x.Key.IsApprove == null ? "For Approval" : "Approve",
                Order = x.GroupBy(x => new
                {
                    x.order.order.OrderNo,
                    x.unitcost.ItemCode,
                    x.unitcost.ItemDescription,
                    x.unitcost.Uom,
                    x.order.moveorder.ItemRemarks

                }).
                Select(x => new ForApprovalMoveOrderListDto.Orders
                {
                    OrderNo = x.Key.OrderNo,
                    ItemCode = x.Key.ItemCode,
                    ItemDesciption = x.Key.ItemDescription,
                    Uom = x.Key.Uom,
                    ItemRemarks = x.Key.ItemRemarks,
                    Quantity = x.Sum(x => x.unitcost.Quantity),
                    UnitCost = x.Sum(x => x.unitcost.UnitCost)


                }).ToList()



            }).Where(x => x.IsRush == status).Where(x => Convert.ToString(x.MIRId).ToLower()
                   .Contains(search.Trim().ToLower())
                  || Convert.ToString(x.CustomerName).ToLower().Contains(search.Trim().ToLower())
                  || Convert.ToString(x.Customercode).ToLower().Contains(search.Trim().ToLower()));

            return await moverders.ToListAsync();

        }







        public async Task<IReadOnlyList<ViewMoveOrderForApprovalDto>> ViewMoveOrderForApproval(int id)
        {

            var UnitPriceById = _context.MoveOrders.Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.OrderNo,
                    x.OrderNoGenus,
                    x.Id,
                    x.ItemCode,
                    x.ItemDescription,
                    x.Uom,
                    x.ItemRemarks,
                    x.AccountCode,
                    x.AccountTitles,
                    x.EmpId,
                    x.FullName,
                    x.QuantityOrdered,
                    x.UnitPrice,
                    x.AssetTag

                }).Select(x => new ViewMoveOrderForApprovalDto
                {
                    MIRId = x.Key.OrderNo,
                    OrderNoGenus = x.Key.OrderNoGenus,
                    Id = x.Key.Id,
                    ItemCode = x.Key.ItemCode,
                    ItemDescription = x.Key.ItemDescription,
                    Uom = x.Key.Uom,
                    ItemRemarks = x.Key.ItemRemarks,
                    AccountCode = x.Key.AccountCode,
                    AccountTitles = x.Key.AccountTitles,
                    EmpId = x.Key.EmpId,
                    FullName = x.Key.FullName,
                    UnitCost = x.Key.UnitPrice * x.Key.QuantityOrdered,
                    Quantity = x.Key.QuantityOrdered,
                    AssetTag = x.Key.AssetTag
                    


                });


            var UnitPriceTotal = UnitPriceById.GroupBy(x => new
            {
                x.MIRId,
                x.OrderNoGenus,
                x.ItemCode,
                x.ItemDescription,
                x.Uom,
                x.ItemRemarks,
                x.AccountCode,
                x.AccountTitles,
                x.EmpId,
                x.FullName,
                x.AssetTag


            }).Select(x => new ViewMoveOrderForApprovalDto
            {
                MIRId = x.Key.MIRId,
                OrderNoGenus = x.Key.OrderNoGenus,
                ItemCode = x.Key.ItemCode,
                ItemDescription = x.Key.ItemDescription,
                Uom = x.Key.Uom,
                ItemRemarks = x.Key.ItemRemarks,
                AccountCode = x.Key.AccountCode,
                AccountTitles = x.Key.AccountTitles,
                EmpId = x.Key.EmpId,
                FullName = x.Key.FullName,
                UnitCost = x.Sum(x => x.UnitCost) / x.Sum(x => x.Quantity),
                TotalCost = x.Sum(x => x.UnitCost),
                Quantity = x.Sum(x => x.Quantity),
                AssetTag = x.Key.AssetTag

            });




            var totalpricebyMir = UnitPriceTotal.GroupBy(x => new
            {
                x.MIRId,


            }).Select(x => new ViewMoveOrderForApprovalDto
            {
                MIRId = x.Key.MIRId,

                TQuantity = x.Sum(x => x.Quantity),
                TUnitCost = x.Sum(x => x.UnitCost),
                TTotalCost = x.Sum(x => x.TotalCost)
            });


            var orders = _context.MoveOrders
                .GroupJoin(UnitPriceTotal, Moveorders => Moveorders.OrderNo, unitprice => unitprice.MIRId, (MoveOrders, unitprice) => new { MoveOrders, unitprice })
                .SelectMany(x => x.unitprice.DefaultIfEmpty(), (x, unitprice) => new { x.MoveOrders, unitprice })
                .GroupJoin(totalpricebyMir, Moveorders => Moveorders.MoveOrders.OrderNo, tunitprice => tunitprice.MIRId, (MoveOrders, tunitprice) => new { MoveOrders, tunitprice })
                .SelectMany(x => x.tunitprice.DefaultIfEmpty(), (x, tunitprice) => new { x.MoveOrders, tunitprice })
                .Where(x => x.MoveOrders.MoveOrders.IsActive == true && x.MoveOrders.MoveOrders.IsPrepared == true)
                .Where(x => x.MoveOrders.MoveOrders.OrderNo == id)
                .GroupBy(x => new
                {
                    x.MoveOrders.MoveOrders.OrderNo,
                    x.MoveOrders.unitprice.OrderNoGenus,
                    x.MoveOrders.unitprice.ItemCode,
                    x.MoveOrders.unitprice.ItemDescription,
                    x.MoveOrders.unitprice.Uom,
                    x.MoveOrders.MoveOrders.Customercode,
                    x.MoveOrders.MoveOrders.CustomerName,
                    x.MoveOrders.MoveOrders.ApprovedDate,
                    x.MoveOrders.unitprice.UnitCost,
                    x.MoveOrders.unitprice.TotalCost,
                    x.MoveOrders.unitprice.Quantity,
                    x.tunitprice.TQuantity,
                    x.tunitprice.TUnitCost,
                    x.tunitprice.TTotalCost,
                    x.MoveOrders.MoveOrders.CompanyCode,
                    x.MoveOrders.MoveOrders.CompanyName,
                    x.MoveOrders.MoveOrders.DepartmentCode,
                    x.MoveOrders.MoveOrders.DepartmentName,
                    x.MoveOrders.MoveOrders.LocationCode,
                    x.MoveOrders.MoveOrders.LocationName,
                    x.MoveOrders.unitprice.AccountCode,
                    x.MoveOrders.unitprice.AccountTitles,
                    x.MoveOrders.unitprice.ItemRemarks,
                    x.MoveOrders.unitprice.EmpId,
                    x.MoveOrders.unitprice.FullName,
                    x.MoveOrders.unitprice.AssetTag




                }).Select(x => new ViewMoveOrderForApprovalDto
                {

                    MIRId = x.Key.OrderNo,
                    OrderNoGenus = x.Key.OrderNoGenus,
                    ItemCode = x.Key.ItemCode,
                    ItemDescription = x.Key.ItemDescription,
                    Uom = x.Key.Uom,

                    Customercode = x.Key.Customercode,
                    CustomerName = x.Key.CustomerName,
                    ApprovedDate = x.Key.ApprovedDate.ToString(),
                    Quantity = x.Key.Quantity,
                    CompanyCode = x.Key.CompanyCode,
                    CompanyName = x.Key.CompanyName,
                    DepartmentCode = x.Key.DepartmentCode,
                    DepartmentName = x.Key.DepartmentName,
                    LocationCode = x.Key.LocationCode,
                    LocationName = x.Key.LocationName,
                    AccountCode = x.Key.AccountCode,
                    AccountTitles = x.Key.AccountTitles,
                    ItemRemarks = x.Key.ItemRemarks,
                    UnitCost = x.Key.UnitCost,
                    TotalCost = x.Key.TotalCost,
                    TQuantity = x.Key.TQuantity,
                    TUnitCost = x.Key.TUnitCost,
                    TTotalCost = x.Key.TTotalCost,
                    EmpId = x.Key.EmpId,
                    FullName = x.Key.FullName,
                    AssetTag = x.Key.AssetTag

                });

            return await orders.ToListAsync();


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
                //items.IsActive = true;
                //items.IsPrepared = true;
                items.PreparedBy = null;
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

         
            var orders = _context.MoveOrders.Where(x => x.IsActive == true)
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
            var orders = _context.MoveOrders.Where(x => x.IsActive == true)
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

            var UnitPriceById = _context.MoveOrders.Where(x => x.IsActive == true)
               .GroupBy(x => new
               {
                   x.OrderNo,
                   x.Id,
                   x.ItemCode,
                   x.QuantityOrdered,
                   x.UnitPrice

               }).Select(x => new ViewMoveOrderForApprovalDto
               {
                   MIRId = x.Key.OrderNo,
                   Id = x.Key.Id,
                   ItemCode = x.Key.ItemCode,
                   UnitCost = x.Key.UnitPrice * x.Key.QuantityOrdered,
                   Quantity = x.Key.QuantityOrdered,


               });


            var UnitPriceTotal = UnitPriceById.GroupBy(x => new
            {
                x.ItemCode,


            }).Select(x => new ViewMoveOrderForApprovalDto
            {
                //MIRId = x.Key.MIRId,
                ItemCode = x.Key.ItemCode,
                UnitCost = x.Sum(x => x.UnitCost) / x.Sum(x => x.Quantity),
                TotalCost = x.Sum(x => x.UnitCost),
                Quantity = x.Sum(x => x.Quantity)

            });


            var orders = _context.MoveOrders
                .GroupJoin(UnitPriceTotal, Moveorders => Moveorders.ItemCode, unitprice => unitprice.ItemCode, (MoveOrders, unitprice) => new { MoveOrders, unitprice })
                .SelectMany(x => x.unitprice.DefaultIfEmpty(), (x, unitprice) => new { x.MoveOrders, unitprice })
                .GroupBy(x => new
                {
                    x.MoveOrders.OrderNo,
                    x.unitprice.ItemCode,
                    x.MoveOrders.ItemDescription,
                    x.MoveOrders.Uom,
                    x.MoveOrders.Department,
                    x.MoveOrders.CustomerName,
                    x.MoveOrders.Customercode,
                    x.MoveOrders.Category,
                    x.MoveOrders.OrderDate,
                    x.MoveOrders.IsApprove,
                    x.MoveOrders.IsPrepared,
                    x.MoveOrders.IsTransact,
                    x.MoveOrders.Rush,
                    x.MoveOrders.ItemRemarks,
                    x.unitprice.UnitCost,
                    x.unitprice.Quantity,
                    x.unitprice.TotalCost

                }).Where(x => x.Key.IsApprove == true).Select(x => new GetAllApprovedMoveOrderDto
                {

                    MIRId = x.Key.OrderNo,
                    ItemCode = x.Key.ItemCode,
                    ItemDescription = x.Key.ItemDescription,
                    Uom = x.Key.Uom,
                    Department = x.Key.Department,
                    CustomerName = x.Key.CustomerName,
                    CustomerCode = x.Key.Customercode,
                    Category = x.Key.Category,
                    Quantity = x.Key.Quantity,
                    OrderDate = x.Key.OrderDate.ToString(),
                    Rush = x.Key.Rush,
                    ItemRemarks = x.Key.ItemRemarks,
                    UnitCost = x.Key.UnitCost,
                    TotalCost = x.Key.TotalCost
                }).Where(x => x.MIRId == id);



          

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
                //items.IsActive = false;
                //items.IsPrepared = true;
                items.IsApprove = null;
                items.ApprovedDate = null;
                items.ApproveDateTempo = null;

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

                                            }).Where(x => x.IsRush == status);

            return await PagedList<RejectedMoveOrderPaginationDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<RejectedMoveOrderPaginationDto>> RejectedMoveOrderPaginationOrig(UserParams userParams, string search , bool status)
        {

            var orders = _context.MoveOrders.Where(x => x.IsApproveReject == true )
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
                items.IsApprove = null;
                items.IsApproveReject = null;
                
            }

            foreach (var items in existingorders)
            {
                
                items.IsReject = null;
                items.RejectBy = null;
                items.Remarks = null;
                items.RejectedDate = null;
                items.IsMove = false;
                
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


            var UnitPriceById = _context.MoveOrders.Where(x => x.IsActive == true)
                 .GroupBy(x => new
                 {
                     x.OrderNo,
                     x.OrderNoGenus,
                     x.ItemCode,
                     x.ItemDescription,
                     x.Uom,
                     x.Id,
                     //x.Category,
                     x.ItemRemarks,
                     x.QuantityOrdered,
                     x.UnitPrice,
                     x.AssetTag

                 }).Select(x => new ViewMoveOrderForApprovalDto
                 {
                     MIRId = x.Key.OrderNo,
                     OrderNoGenus = x.Key.OrderNoGenus,
                     ItemCode = x.Key.ItemCode,
                     ItemDescription = x.Key.ItemDescription,
                     Uom = x.Key.Uom,
                     //Category = x.Key.Category,
                     ItemRemarks = x.Key.ItemRemarks,
                     UnitCost = x.Key.UnitPrice * x.Key.QuantityOrdered,
                     Quantity = x.Key.QuantityOrdered,
                     AssetTag = x.Key.AssetTag


                 });



            var UnitPriceTotal = UnitPriceById.GroupBy(x => new
            {
                x.MIRId,
                x.OrderNoGenus,
                x.ItemCode,
                x.ItemDescription,
                x.Uom,
                x.ItemRemarks,
                x.AssetTag



            }).Select(x => new ViewMoveOrderForApprovalDto
            {
                MIRId = x.Key.MIRId,
                OrderNoGenus = x.Key.OrderNoGenus,
                ItemCode = x.Key.ItemCode,
                ItemDescription = x.Key.ItemDescription,
                Uom = x.Key.Uom,
                //Category = x.Key.Category,
                ItemRemarks = x.Key.ItemRemarks,
                UnitCost = x.Sum(x => x.UnitCost) / x.Sum(x => x.Quantity),
                TotalCost = x.Sum(x => x.UnitCost),
                Quantity = x.Sum(x => x.Quantity),
                AssetTag = x.Key.AssetTag


            });



            var orders = _context.MoveOrders
                .OrderBy(x => x.Rush == null)
                .ThenBy(x => x.Rush)
                 .Where(x => x.IsActive == true)
              .GroupJoin(UnitPriceTotal, Moveorders => Moveorders.OrderNo, unitprice => unitprice.MIRId, (MoveOrders, unitprice) => new { MoveOrders, unitprice })
              .SelectMany(x => x.unitprice.DefaultIfEmpty(), (x, unitprice) => new { x.MoveOrders, unitprice })
              .GroupBy(x => new
              {

                  x.MoveOrders.OrderNo,
                  x.unitprice.OrderNoGenus,
                  x.MoveOrders.OrderDate,
                  x.MoveOrders.PreparedDate,
                  x.MoveOrders.DateNeeded,
                  x.MoveOrders.Customercode,
                  x.MoveOrders.CustomerName,
                  //x.unitprice.Category,
                  x.unitprice.ItemCode,
                  x.unitprice.ItemDescription,
                  x.unitprice.Uom,
                  x.unitprice.UnitCost,
                  x.unitprice.TotalCost,
                  x.unitprice.Quantity,
                  x.MoveOrders.IsApprove,
                  x.unitprice.ItemRemarks,
                  x.unitprice.AssetTag

              }).Select(x => new ListOfMoveOrdersForTransactDto
              {

                  MIRId = x.Key.OrderNo,
                  OrderNoGenus = x.Key.OrderNoGenus,
                  OrderDate = x.Key.OrderDate.ToString(),
                  PreparedDate = x.Key.PreparedDate.ToString(),
                  DateNeeded = x.Key.DateNeeded.ToString(),
                  CustomerCode = x.Key.Customercode,
                  CustomerName = x.Key.CustomerName,
                  //Category = x.Key.Category,
                  ItemCode = x.Key.ItemCode,
                  ItemDescription = x.Key.ItemDescription,
                  Uom = x.Key.Uom,
                  Quantity = x.Key.Quantity,
                  ItemRemarks = x.Key.ItemRemarks,
                  UnitCost = x.Key.UnitCost,
                  TotalCost = x.Key.TotalCost,
                  AssetTag = x.Key.AssetTag
                  

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

            transact.PreparedBy = transact.PreparedBy;
            


            await _context.TransactOrder.AddAsync(transact);

 
            foreach (var itemss in existing)
            {
                itemss.IsTransact = true;
            }

            return true;

        }

        public async Task<IReadOnlyList<TrackingofOrderingTransactionDto>> TrackingofOrderingTransaction()
        {
            var orders = _context.Orders.Where(x => x.IsActive == true).GroupBy(x => new
            {
                x.TrasactId,
                x.Customercode,
                x.CustomerName,
                x.IsPrepared,
                x.IsApproved,
                x.IsActive

            }).Select(x => new ListofOrderDto
            {
               TransactId = x.Key.TrasactId,
                CustomerCode = x.Key.Customercode,
               CustomerName = x.Key.CustomerName,
                IsPrepared = x.Key.IsPrepared,
                IsApproved = x.Key.IsApproved,
                IsActive = x.Key.IsActive

            });


            var moveorders = orders
            .GroupJoin(_context.MoveOrders, order => order.TransactId, moveorder => moveorder.OrderNo, (order, moveorder) => new { order, moveorder })
            .SelectMany(x => x.moveorder.DefaultIfEmpty(), (x, moveorder) => new { x.order, moveorder })
            .GroupBy(x => new
            {
             
                x.order.TransactId,
                x.order.CustomerCode,
                x.order.CustomerName,
                x.order.IsPrepared,
                x.order.IsApproved,
                x.moveorder.IsApprove,
                x.order.IsActive


            }).Select(x => new ListofOrderDto
            {
                TransactId = x.Key.TransactId,
                CustomerCode = x.Key.CustomerCode,
                CustomerName = x.Key.CustomerName,
                IsPrepared = x.Key.IsPrepared,
                IsApproved = x.Key.IsApproved,
                IsServed = x.Key.IsApprove,
                IsActive = x.Key.IsActive


            });

            var List = moveorders.GroupBy(x => new
            {
                x.TransactId,
                x.CustomerCode,
                x.CustomerName,
                x.IsPrepared,
                x.IsApproved,
                x.IsServed,
                x.IsActive

            }).Select(x => new TrackingofOrderingTransactionDto
            {
               TransactId = x.Key.TransactId,
               CustomerCode = x.Key.CustomerCode,
               CustomerName = x.Key.CustomerName,
                OrderStatus = (x.Key.IsPrepared && x.Key.IsApproved.HasValue && x.Key.IsServed.HasValue && x.Key.IsServed.Value) ? "Served" :
                  (x.Key.IsPrepared && x.Key.IsApproved.HasValue && (!x.Key.IsServed.HasValue || !x.Key.IsServed.Value)) ? "Approved" :
                  (x.Key.IsPrepared && (!x.Key.IsApproved.HasValue || (!x.Key.IsServed.HasValue && !x.Key.IsServed.Value))) ? "Preparing" :
                  (x.Key.IsActive == true) ? "UnServed" : "For Preparation"
                  

            });

          
            return await List.ToListAsync();   
        }

        public async Task<IList<ListofServedDto>> ListofServedDto()
        {

            var orders = _context.Orders
                  .GroupJoin(_context.MoveOrders , ordering => ordering.TrasactId , moveorder => moveorder.OrderNo , (ordering, moveorder) => new {ordering , moveorder})
                  .SelectMany(x => x.moveorder.DefaultIfEmpty() , (x , moveorder) => new {x.ordering , moveorder})
                  .GroupBy(order => order.ordering.TrasactId)
                 .Select(group => new ListofServedDto
                 {
                     TransactId = group.Key,
                     DateNeeded = group.First().ordering.DateNeeded.ToString(),
                     OrderDate = group.First().ordering.OrderDate.ToString(),
                     CustomerCode = group.First().ordering.Customercode,
                     CustomerName = group.First().ordering.CustomerName,
                     Order = group.Select(x => new ListofServedDto.Orders
                     {
                         OrderNo = x.ordering.OrderNo,
                         CustomerType = x.ordering.CustomerType,
                         ItemCode = x.ordering.ItemCode,
                         ItemCategory =  x.ordering.Category,
                         Uom = x.ordering.Uom,
                         QuantityOrdered = x.ordering.QuantityOrdered,
                         CompanyCode = x.ordering.CompanyCode,
                         CompanyName = x.ordering.CompanyName,
                         DepartmentCode = x.ordering.DepartmentCode,
                         DepartmentName = x.ordering.Department,
                         LocationCode = x.ordering.LocationCode,
                         QuantityUnServed = x.ordering.IsActive == false ? x.ordering.QuantityOrdered : (int?)null,
                         QuantityServed = x.moveorder.IsApprove == true ? x.moveorder.QuantityOrdered : (int?)null,
                         LocationName = x.ordering.LocationName,
                         Rush = x.ordering.Rush,
                         ItemRemarks = x.ordering.ItemRemarks,
                     }).ToList()
                 });


            return await orders.ToListAsync();
        }


    }
}
