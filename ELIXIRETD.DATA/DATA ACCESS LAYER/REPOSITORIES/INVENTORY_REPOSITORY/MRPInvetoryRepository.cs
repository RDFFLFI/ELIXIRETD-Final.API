using ELIXIRETD.DATA.CORE.INTERFACES.INVENTORY_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.IMPORT_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORY_DTO.MRP;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.INVENTORY_REPOSITORY
{
    public class MRPInvetoryRepository : IMRPInventory
    {
        private readonly StoreContext _context;

        public MRPInvetoryRepository(StoreContext context)
        {
            _context= context;
        }

        public async Task<IReadOnlyList<DtoGetAllAvailableInRawmaterialInventory>> GetAllAvailableInRawmaterialInventory()
        {
            return await _context.WarehouseReceived
              .GroupBy(x => new
            {
                  x.ItemCode,
                  x.ItemDescription,
                  x.LotCategory,
                  x.Uom,
                  x.IsWarehouseReceived,

            }).Select(inventory => new DtoGetAllAvailableInRawmaterialInventory
            {
                ItemCode = inventory.Key.ItemCode,
                ItemDescription = inventory.Key.ItemDescription,
                LotCategory = inventory.Key.LotCategory,
                Uom = inventory.Key.Uom,
                SOH = inventory.Sum(x => x.ActualGood),
                ReceiveIn = inventory.Sum(x => x.ActualGood),
                RejectOrder= inventory.Sum(x => x.TotalReject),
                IsWarehouseReceived = inventory.Key.IsWarehouseReceived

            }).OrderBy(x => x.ItemCode)
            .Where(x => x.IsWarehouseReceived == true)
            .ToListAsync();

        }

        public Task<IReadOnlyList<DtoMRP>> MRPInventory()
        {
            throw new NotImplementedException();
        }

        //public async Task<IReadOnlyList<DtoMRP>> MRPInventory()
        //{
        //    var EndDate = DateTime.Now;
        //    var StartDate = EndDate.AddDays(-30);

        //    var getPoSummarry = _context.PoSummaries.GroupBy(x => new
        //    {

        //        x.ItemCode,

        //    }).Select(x => new PoSummaryDto
        //    {
        //        ItemCode = x.Key.ItemCode,
        //        UnitPrice = x.Sum(x => x.UnitPrice),
        //        Ordered = x.Sum(x => x.Ordered),
        //        TotalPrice = x.Average(x => x.UnitPrice)

        //    });


        //    var getWarehouseIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
        //                                                   .OrderBy(x => x.ActualReceivingDate)
        //                                                   .GroupBy(x => new
        //                                                   {

        //                                                       x.ItemCode,
        //                                                   }).Select(x => new WarehouseInventory
        //                                                   {

        //                                                       ItemCode = x.Key.ItemCode,
        //                                                       ActualGood = x.Sum(x => x.ActualGood)
        //                                                   });

        //    var getMoveOrderOut = _context.MoveOrders.Where(x => x.IsActive == true)
        //                                            .Where(x => x.IsPrepared == true)
        //                                            .GroupBy(x => new
        //                                            {
        //                                                x.ItemCode,

        //                                            }).Select(x => new MoveOrderInventory
        //                                            {

        //                                             ItemCode = x.Key.ItemCode,
        //                                             QuantityOrdered = x.Sum(x => x.QuantityOrdered),

        //                                            });

        //    var getReceiptIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
        //                                                 .Where(x => x.TransactionType == "MiscellaneousReceipt")
        //                                                 .GroupBy(x => new
        //                                                 {

        //                                                     x.ItemCode,

        //                                                 }).Select(x => new DtoRecieptIn
        //                                                 {

        //                                                     ItemCode = x.Key.ItemCode,
        //                                                     Quantity = x.Sum(x => x.ActualGood),

        //                                                 });


        //    var getReceitOut = _context.WarehouseReceived.Where(x => x.IsActive == true)
        //                                                 .Where(x => x.TransactionType == "MiscellaneousReceipt")
        //                                                 .GroupBy(x => new
        //                                                 {

        //                                                     x.ItemCode,
        //                                                 }).Select(x => new DtoReceiptOut
        //                                                 {

        //                                                     ItemCode = x.Key.ItemCode,
        //                                                     QuantityReject = x.Sum(x => x.TotalReject),
        //                                                 });

        //    var getIssueOut = _context.MiscellaneousIssueDetail.Where(x => x.IsActive == true)
        //                                                        .Where(x => x.IsTransact == true)
        //                                                        .GroupBy(x => new
        //                                                        {

        //                                                            x.ItemCode,

        //                                                        }).Select(x => new DtoIssueInventory
        //                                                        {
        //                                                            ItemCode = x.Key.ItemCode,
        //                                                            Quantity = x.Sum(x => x.Quantity),

        //                                                        });

        //    var getBorrowedIssue = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
        //                                               .GroupBy(x => new
        //                                               {

        //                                                   x.ItemCode,

        //                                               }).Select(x => new DtoBorrowedIssue
        //                                               {

        //                                                   ItemCode = x.Key.ItemCode,
        //                                                   Quantity = x.Sum(x => x.Quantity),

        //                                               });

        //    var getReturnedBorrow = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
        //                                                         .Where(x => x.IsReturned == true)
        //                                                         .GroupBy(x => new
        //                                                         {

        //                                                             x.ItemCode,

        //                                                         }).Select(x => new DtoBorrowedIssue
        //                                                         {

        //                                                             ItemCode = x.Key.ItemCode,
        //                                                             ReturnQuantity = x.Sum(x => x.ReturnQuantity)

        //                                                         });

        //    var getWarehouseStock = _context.WarehouseReceived.Where(x => x.IsActive == true)
        //                                                      .GroupBy(x => new
        //                                                      {
        //                                                          x.ItemCode,

        //                                                      }).Select(x => new WarehouseInventory
        //                                                      {

        //                                                          ItemCode = x.Key.ItemCode,
        //                                                          ActualGood = x.Sum(x => x.ActualGood)

        //                                                      });


        //    var getOrderingReserve = _context.Orders.Where(x => x.IsActive == true)
        //                                            .Where(x => x.PreparedDate != null)
        //                                            .GroupBy(x => new
        //                                            {
        //                                                x.ItemCode,

        //                                            }).Select(x => new OrderingInventory
        //                                            {
        //                                                ItemCode = x.Key.ItemCode,
        //                                                QuantityOrdered = x.Sum(x => x.QuantityOrdered),

        //                                            });

        //    var getSOH = (from warehouse in getWarehouseStock
        //                  join issue in getIssueOut
        //                  on warehouse.ItemCode equals issue.ItemCode
        //                  into leftJ1
        //                  from issue in leftJ1.DefaultIfEmpty()

        //                  join moveorder in getMoveOrderOut
        //                  on warehouse.ItemCode equals moveorder.ItemCode
        //                  into leftJ2
        //                  from moveorder in leftJ2.DefaultIfEmpty()

        //                  join borrowed in getBorrowedIssue
        //                  on warehouse.ItemCode equals borrowed.ItemCode
        //                  into leftJ3
        //                  from borrowed in leftJ3.DefaultIfEmpty()

        //                  join returned in getReturnedBorrow
        //                  on warehouse.ItemCode equals returned.ItemCode
        //                  into leftJ4
        //                  from returned in leftJ4.DefaultIfEmpty()

        //                  join receipt in getReceiptIn
        //                  on warehouse.ItemCode equals receipt.ItemCode
        //                  into leftJ5
        //                  from receipt in leftJ5.DefaultIfEmpty()

        //                  group new
        //                  {

        //                      warehouse,
        //                      moveorder,
        //                      issue,
        //                      borrowed,
        //                      returned,
        //                      receipt,


        //                  }
        //                  by new
        //                  {
        //                      warehouse.ItemCode

        //                  } into total

        //                  select new DtoSOH
        //                  {

        //                      ItemCode = total.Key.ItemCode,
        //                      SOH = total.Sum(x => x.warehouse.ActualGood == null ? x.warehouse.ActualGood : 0) +
        //                     total.Sum(x => x.returned.ReturnQuantity == null ? x.returned.ReturnQuantity : 0) -
        //                     total.Sum(x => x.issue.Quantity == null ? x.issue.Quantity : 0) - 
        //                     total.Sum(x => x.borrowed.Quantity == null ? x.borrowed.Quantity : 0)

        //                  });

        //    var getReserve = (from warehouse in getWarehouseStock
        //                      join ordering in getOrderingReserve
        //                      on warehouse.ItemCode equals ordering.ItemCode
        //                      into leftJ1
        //                      from ordering in leftJ1.DefaultIfEmpty()

        //                      group new
        //                      {

        //                          warehouse,
        //                          ordering,

        //                      } by new
        //                      {

        //                          warehouse.ItemCode

        //                      } into total

        //                      select new ReserveInventory
        //                      {

        //                          ItemCode = total.Key.ItemCode,
        //                          Reserve = total.Sum(x => x.warehouse.ActualGood == null ? x.warehouse.ActualGood : 0) - 
        //                          total.Sum(x => x.ordering.QuantityOrdered == null ? x.ordering.QuantityOrdered : 0)

        //                      });


        //    var getSuggestedPo = (from posummary in getPoSummarry
        //                          join receive in getWarehouseIn
        //                          on posummary.ItemCode equals receive.ItemCode
        //                          into leftJ1
        //                          from receive in leftJ1.DefaultIfEmpty()

        //                          group new
        //                          {
        //                              posummary,
        //                              receive,

        //                          }
        //                          by new
        //                          {

        //                              posummary.ItemCode,

        //                          } into total
        //                          select new DtoPoSummaryInventory
        //                          {

        //                              ItemCode = total.Key.ItemCode,
        //                              Ordered = total.Sum(x => x.posummary.Ordered == null ? x.posummary.Ordered : 0 ) -
        //                                        total.Sum(x => x.receive.ActualGood == null ? x.receive.ActualGood : 0)

        //                          });

        //    var getMoveOrderoutPerMonth = _context.MoveOrders.Where(x => x.PreparedDate >= StartDate && x.PreparedDate <= EndDate)
        //                                                      .Where(x => x.IsActive == true)
        //                                                      .Where(x => x.IsPrepared == true)
        //                                                      .GroupBy(x => new
        //                                                      {
        //                                                          x.ItemCode,

        //                                                      }).Select(x => new MoveOrderInventory
        //                                                      {
        //                                                          ItemCode = x.Key.ItemCode,
        //                                                          QuantityOrdered = x.Sum(x => x.QuantityOrdered)

        //                                                      });

        //    var getBorrowedOutPerMonth = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
        //                                                              .GroupBy(x => new
        //                                                              {

        //                                                                  x.ItemCode,

        //                                                              }).Select(x => new DtoBorrowedIssue
        //                                                              {

        //                                                                  ItemCode = x.Key.ItemCode,
        //                                                                  Quantity = x.Sum(x => x.Quantity),

        //                                                              });

        //    var getReserveUsage = (from warehouse in getWarehouseStock
        //                           join moveorder in getMoveOrderoutPerMonth
        //                           on warehouse.ItemCode equals moveorder.ItemCode
        //                           into leftJ1
        //                           from moveorder in leftJ1.DefaultIfEmpty()

        //                           join borrowed in getBorrowedOutPerMonth
        //                           on warehouse.ItemCode equals borrowed.ItemCode
        //                           into leftJ2
        //                           from borrowed in leftJ2.DefaultIfEmpty()

        //                           group new
        //                           {
        //                               warehouse,
        //                               borrowed,
        //                               moveorder,
        //                           }
        //                           by new 























    
    }
}
