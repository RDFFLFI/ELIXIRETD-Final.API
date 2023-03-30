using ELIXIRETD.DATA.CORE.INTERFACES.BORROWED_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
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

            var returnedborrow = _context.ReturnedBorroweds.Where(x => x.IsActive == true)
                                                   .Where(x => x.IsReturned == true)
                                                   .GroupBy(x => new
                                                   {

                                                       x.TotalbPkey,
                                                       x.CustomerCode,
                                                      

                                                   }).Select(x => new DtoReturnedpagination
                                                   {
                                                        BorrowedPKey = x.Key.TotalbPkey ,
                                                       ReturnedQuantity = x.Sum(x => x.ReturnedQuantity)


                                                   });

            var borrow = (from borrowed in _context.BorrowedIssueDetails
                          where borrowed.IsActive == status
                          join returned in returnedborrow
                          on borrowed.BorrowedPKey equals returned.BorrowedPKey
                          into leftJ
                          from returned in leftJ.DefaultIfEmpty()

                          group new

                          {
                              borrowed,
                              returned,

                          }
                          by new
                          {
                              borrowed.BorrowedPKey,
                              borrowed.CustomerCode,
                              borrowed.CustomerName,
                              borrowed.PreparedBy,
                              borrowed.BorrowedDate,
                              borrowed.IsActive,
                              returnedQuantity = returned.ReturnedQuantity != null ? returned.ReturnedQuantity : 0,

                          }

                          into total
                          select new GetAllBorrowedReceiptWithPaginationDto

                          {
                              BorrowedPKey = total.Key.BorrowedPKey,
                              CustomerCode = total.Key.CustomerCode,
                              CustomerName = total.Key.CustomerName,
                              PreparedBy = total.Key.PreparedBy,
                              TotalQuantity = total.Sum(x => x.borrowed.Quantity) - total.Key.returnedQuantity,
                              BorrowedDate = total.Key.BorrowedDate.ToString(),
                              IsActive = total.Key.IsActive,

                          }).Where(x => x.TotalQuantity != 0 && (x.TotalQuantity > 0));








            //var borrow = _context.BorrowedIssueDetails.OrderByDescending(x => x.BorrowedDate)

            //                                      .Where(x => x.IsActive == status)
            //                                      .GroupBy(x => new
            //                                      {

            //                                          x.BorrowedPKey,
            //                                          x.CustomerCode,
            //                                          x.CustomerName,
            //                                          x.PreparedBy,
            //                                          x.IsActive,
            //                                          x.BorrowedDate,

            //                                      })
            //                                      .Select(x => new GetAllBorrowedReceiptWithPaginationDto
            //                                      {

            //                                          BorrowedPKey = x.Key.BorrowedPKey,
            //                                          CustomerName = x.Key.CustomerName,
            //                                          CustomerCode = x.Key.CustomerCode,
            //                                          TotalQuantity = x.Sum(x => x.Quantity) - x.Sum(x => x.ReturnQuantity),                              
            //                                          PreparedBy = x.Key.PreparedBy,
            //                                          IsActive = x.Key.IsActive,
            //                                          BorrowedDate = x.Key.BorrowedDate.ToString(),

            //                                      }).Where(x => x.TotalQuantity != 0 && (x.TotalQuantity > 0));

            return await PagedList<GetAllBorrowedReceiptWithPaginationDto>.CreateAsync(borrow, userParams.PageNumber, userParams.PageSize);

        }


        public async Task<PagedList<GetAllBorrowedReceiptWithPaginationDto>> GetAllBorrowedIssuetWithPaginationOrig(UserParams userParams, string search, bool status)
        {

            var returnedborrow = _context.ReturnedBorroweds.Where(x => x.IsActive == true)
                                                      .Where(x => x.IsReturned == true)
                                                      .GroupBy(x => new
                                                      {

                                                          x.TotalbPkey,
                                                          x.CustomerCode,


                                                      }).Select(x => new DtoReturnedpagination
                                                      {
                                                          BorrowedPKey = x.Key.TotalbPkey,
                                                          ReturnedQuantity = x.Sum(x => x.ReturnedQuantity)


                                                      });

            var borrow = (from borrowed in _context.BorrowedIssueDetails
                          where borrowed.IsActive == status
                          join returned in returnedborrow
                          on borrowed.BorrowedPKey equals returned.BorrowedPKey
                          into leftJ
                          from returned in leftJ.DefaultIfEmpty()

                          group new

                          {
                              borrowed,
                              returned,

                          }
                          by new
                          {
                              borrowed.BorrowedPKey,
                              borrowed.CustomerCode,
                              borrowed.CustomerName,
                              borrowed.PreparedBy,
                              borrowed.BorrowedDate,
                              borrowed.IsActive,
                              returnedQuantity = returned.ReturnedQuantity != null ? returned.ReturnedQuantity : 0,

                          }

                          into total
                          select new GetAllBorrowedReceiptWithPaginationDto

                          {
                              BorrowedPKey = total.Key.BorrowedPKey,
                              CustomerCode = total.Key.CustomerCode,
                              CustomerName = total.Key.CustomerName,
                              PreparedBy = total.Key.PreparedBy,
                              TotalQuantity = total.Sum(x => x.borrowed.Quantity) - total.Key.returnedQuantity,
                              BorrowedDate = total.Key.BorrowedDate.ToString(),
                              IsActive = total.Key.IsActive,

                          }).Where(x => x.TotalQuantity != 0 && (x.TotalQuantity > 0))
                                                 .Where(x => (Convert.ToString(x.BorrowedPKey)).ToLower()
                                                 .Contains(search.Trim().ToLower()));


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


            var BorrowedReturn = _context.ReturnedBorroweds.Where(x => x.IsActive == true)
                                                             .Where(x => x.IsReturned == true)
                                                             .GroupBy(x => new
                                                             {
                                                                 x.ItemCode,
                                                                 x.WarehouseId,

                                                             }).Select(x => new ItemStocksDto
                                                             {

                                                                 ItemCode = x.Key.ItemCode,
                                                                 In = x.Sum(x => x.ReturnedQuantity),
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

            var returnedborrowed = _context.ReturnedBorroweds.Where(x => x.IsActive == true)
                                                             .Where(x => x.IsReturned == true)
                                                             .GroupBy(x => new
                                                             {

                                                                 x.BorrowedPKey,
                                                                 x.ItemCode,
                                                                

                                                             }).Select(x => new DtoReturnedpagination
                                                             {
                                                                
                                                               
                                                                 BorrowedPKey = x.Key.BorrowedPKey,
                                                                 ItemCode = x.Key.ItemCode,
                                                                 ReturnedQuantity = x.Sum(x => x.ReturnedQuantity)


                                                             });



            var warehouse = (from borrowed in _context.BorrowedIssueDetails
                             where borrowed.BorrowedPKey == id 
                             join returned in returnedborrowed
                             on borrowed.Id equals returned.BorrowedPKey
                             into leftJ
                             from returned in leftJ.DefaultIfEmpty()

                             group new

                             {
                                 borrowed,
                                 returned,

                             }
                             by new
                             {

                                 Id = borrowed.Id,
                                 WarehouseId = borrowed.WarehouseId,
                                 BorrowedPKey = borrowed.BorrowedPKey,
                                 Customer = borrowed.CustomerName,
                                 CustomerCode = borrowed.CustomerCode,
                                 ItemCode = borrowed.ItemCode,
                                 ItemDescription = borrowed.ItemDescription,
                                 PreparedBy = borrowed.PreparedBy,
                                 PreparedDate = borrowed.BorrowedDate.ToString(),
                                 Quantity = borrowed.Quantity,
                                 ReturnQuantity = returned.ReturnedQuantity != null ? returned.ReturnedQuantity : 0,


                             }

                             into total

                             select new GetAllDetailsInBorrowedIssueDto

                             {

                                 Id = total.Key.Id,
                                 WarehouseId = total.Key.WarehouseId,
                                 BorrowedPKey = total.Key.BorrowedPKey,
                                 Customer = total.Key.Customer,
                                 CustomerCode = total.Key.CustomerCode,
                                 ItemCode = total.Key.ItemCode,
                                 ItemDescription = total.Key.ItemDescription,
                                 PreparedBy = total.Key.PreparedBy,
                                 PreparedDate = total.Key.PreparedDate.ToString(),
                                 Quantity = total.Sum(x => x.borrowed.Quantity),
                                 Consumes = total.Sum(x => x.borrowed.Quantity) - total.Sum(x => x.returned.ReturnedQuantity),
                                 ReturnQuantity = total.Sum(x => x.returned.ReturnedQuantity),


                             }).Where(x => x.Consumes != 0 && (x.Consumes > 0));




            return await warehouse.ToListAsync();
        }


        public async Task<IReadOnlyList<GetAllAvailableBorrowIssueDto>> GetAllAvailableIssue(int empid)
        {
            var employee = await _context.Users.Where(x => x.Id == empid)
                                                .FirstOrDefaultAsync();

            var items = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                         .Where(x => x.PreparedBy == employee.FullName)
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


            if (items == null)
                return false;

            items.ReturnQuantity = 0;
            
            return true;

        }



        //======================================================== Validation ================================================================

        public async Task<bool> ValidateBorrowReceiptIssue(BorrowedReceipt borrowed)
        {
            var validate = await _context.WarehouseReceived.Where(x => x.BorrowedReceiptId == borrowed.Id)
                                                     .ToListAsync();


            foreach(var items in validate)
            {
                var issue = await _context.BorrowedIssueDetails.Where(x => x.WarehouseId == items.Id)
                                                                    .FirstOrDefaultAsync();

                if (issue != null)
                    return false;

            }
            return true;
        }


       public async Task<bool> EditReturnQuantity (ReturnedBorrowed borrowed)
        {
          
            borrowed.IsReturned = true;
            borrowed.ReturnedDate = DateTime.Now;
            borrowed.IsActive = true;
       
            await _context.ReturnedBorroweds.AddAsync(borrowed);

           
            return true;

        }



        public async Task<bool> SaveReturnedQuantity(BorrowedIssue borrowed)
        {
            var returned = await _context.BorrowedIssues
                .Where(x => x.Id == borrowed.Id)
                .FirstOrDefaultAsync();

            var returnedDetails = await _context.BorrowedIssueDetails
                .Where(x => x.BorrowedPKey == borrowed.Id && x.ReturnQuantity != 0)
                .ToListAsync();



            foreach (var item in returnedDetails)
            {


                item.IsReturned = true;
                item.IsActive = true;
                item.ReturnedDate = DateTime.Now;

            }

            returned.ReturnedDate = DateTime.Now;
            returned.IsReturned = true;
            returned.IsActive = true;

            return true;
        }


        public async Task<PagedList<DtoGetAllReturnedItem>> GetAllReturnedItem(UserParams userParams)
        {

            var BorrowIssue = _context.ReturnedBorroweds.Where(x => x.IsReturned == true)
                                                        .Where(x => x.IsActive == true)
                                                        .GroupBy(x => new
                                                        {

                                                            x.TotalbPkey,
                                                            x.CustomerName,
                                                            x.CustomerCode,

                                                        }).Select(x => new DtoGetAllReturnedItem
                                                        {

                                                            Id = x.Key.TotalbPkey,
                                                            CustomerCode = x.Key.CustomerCode,
                                                            CustomerName = x.Key.CustomerName,
                                                            TotalReturned = x.Sum(x => x.ReturnedQuantity),

                                                        });


            return await PagedList<DtoGetAllReturnedItem>.CreateAsync(BorrowIssue, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<PagedList<DtoGetAllReturnedItem>> GetAllReturnedItemOrig(UserParams userParams, string search)
        {

            var BorrowIssue = _context.ReturnedBorroweds.Where(x => x.IsReturned == true)
                                                       .Where(x => x.IsActive == true)
                                                       .GroupBy(x => new
                                                       {

                                                           x.TotalbPkey,
                                                           x.CustomerName,
                                                           x.CustomerCode,

                                                       }).Select(x => new DtoGetAllReturnedItem
                                                       {

                                                           Id = x.Key.TotalbPkey,
                                                           CustomerCode = x.Key.CustomerCode,
                                                           CustomerName = x.Key.CustomerName,
                                                           TotalReturned = x.Sum(x => x.ReturnedQuantity),

                                                       }).Where(x => (Convert.ToString(x.Id)).ToLower()
                                                       .Contains(search.Trim().ToLower())); 


            return await PagedList<DtoGetAllReturnedItem>.CreateAsync(BorrowIssue, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<IReadOnlyList<DtoViewBorrewedReturnedDetails>> ViewBorrewedReturnedDetails(int id)
        {
            var borrow = _context.ReturnedBorroweds.Where(x => x.TotalbPkey == id)
                                                     .Where(x => x.IsActive == true)
                                                     .Where(x => x.IsReturned == true)
                                                       .Select(x => new DtoViewBorrewedReturnedDetails
                                                      {
                                                          ItemCode = x.ItemCode,
                                                          ItemDescription = x.ItemDescription,
                                                          ReturnQuantity = x.ReturnedQuantity,
                                                          ReturnedDate = x.ReturnedDate.ToString()

                                                       }).OrderByDescending(x => x.ReturnedDate);


            return await borrow.ToListAsync();
        }

        public async Task<bool> Cancelborrowedfortransact(BorrowedIssueDetails borrowed)
        {
            var borrow = await _context.BorrowedIssueDetails.Where(x => x.Id == borrowed.Id)
                                                             .FirstOrDefaultAsync();



            if (borrow == null)
                return false;


            borrow.IsActive = false;

            return true;

        }
    }
}
