using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Vml;
using ELIXIRETD.DATA.CORE.INTERFACES.WAREHOUSE_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.IMPORT_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.Notification_Dto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.TransactDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.WAREHOUSE_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.WAREHOUSE_REPOSITORY
{
    public class WarehouseRepository : IWarehouseReceiveRepository
    {
        private readonly StoreContext _context;

        public WarehouseRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<bool> AddNewReceivingDetails(Warehouse_Receiving receive)
        {

            receive.ActualGood = receive.ActualDelivered;
            receive.TransactionType = "Receiving";
            receive.IsWarehouseReceived = true;
            receive.ActualReceivingDate = DateTime.Now;
            receive.ReceivingDate = DateTime.Now;

            var lotSectionExist = await _context.Materials.Include(x => x.LotSection)
                .FirstOrDefaultAsync(x => x.ItemCode == receive.ItemCode);


            if(lotSectionExist is not null)
            {
                receive.LotSection = lotSectionExist.LotSection.SectionName;
            }

            var poExist = await _context.PoSummaries.FirstOrDefaultAsync(x => x.IsReceived == false && x.Id == receive.PoSummaryId);
                
            poExist.IsReceived = true;
                



            await _context.WarehouseReceived.AddAsync(receive);

            return true;
        }

        public async Task<bool> ValidateLotSectionExist(string lotSection)
        {

            var lotSectionExist = await _context.LotSections
                .FirstOrDefaultAsync(x => x.SectionName == lotSection);

            if(lotSectionExist is  null)
            {
                return false;
            }

            return true;
        }


        public async Task<PagedList<CancelledPoDto>> GetAllCancelledPOWithPagination(UserParams userParams)
        {

            var poSummary = (from posummary in _context.PoSummaries
                             where posummary.IsActive == false
                             where posummary.IsCancelled == true
                             join warehouse in _context.WarehouseReceived
                             on posummary.Id equals warehouse.PoSummaryId into leftJ
                             from receive in leftJ.DefaultIfEmpty()

                             orderby posummary.ImportDate 

                             select new CancelledPoDto
                             {
                                 Id = posummary.Id,
                                 PO_Number = posummary.PO_Number,
                                 ItemCode = posummary.ItemCode,
                                 ItemDescription = posummary.ItemDescription,
                                 Supplier = posummary.VendorName,
                                 QuantityOrdered = posummary.Ordered,
                                 IsActive = posummary.IsActive,
                                 DateCancelled = posummary.DateCancelled.ToString(),
                                 ActualRemaining = 0,
                                 TotalReject = receive.TotalReject != null ? receive.TotalReject : 0,
                                 ActualGood = receive.ActualDelivered ,
                                 

                             }).GroupBy(x => new
                             {
                                 x.Id,
                                 x.PO_Number,
                                 x.ItemCode,
                                 x.ItemDescription,
                                 x.Supplier,
                                 x.QuantityOrdered,
                                 x.IsActive,
                                 x.DateCancelled,
                       
                             }).Select(receive => new CancelledPoDto
                                                 {
                                                     Id = receive.Key.Id,
                                                     PO_Number = receive.Key.PO_Number,
                                                     ItemCode = receive.Key.ItemCode,
                                                     ItemDescription = receive.Key.ItemDescription,
                                                     Supplier = receive.Key.Supplier,
                                                     IsActive = receive.Key.IsActive,
                                                     DateCancelled = receive.Key.DateCancelled,
                                                     ActualRemaining = receive.Key.QuantityOrdered - receive.Sum(x => x.ActualGood),

                                                 }).Where(x => x.IsActive == false);


            return await PagedList<CancelledPoDto>.CreateAsync(poSummary, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<CancelledPoDto>> GetAllCancelledPOWithPaginationOrig(UserParams userParams, string search)
        {
            

            var poSummary = (from posummary in _context.PoSummaries
                             where posummary.IsActive == false
                             where posummary.IsCancelled == true
                             join warehouse in _context.WarehouseReceived
                             on posummary.Id equals warehouse.PoSummaryId into leftJ
                             from receive in leftJ.DefaultIfEmpty()

                             orderby posummary.ImportDate

                             select new CancelledPoDto
                             {
                                 Id = posummary.Id,
                                 PO_Number = posummary.PO_Number,
                                 ItemCode = posummary.ItemCode,
                                 ItemDescription = posummary.ItemDescription,
                                 Supplier = posummary.VendorName,
                                 QuantityOrdered = posummary.Ordered,
                                 IsActive = posummary.IsActive,
                                 DateCancelled = posummary.DateCancelled.ToString(),
                                 ActualRemaining = 0,
                                 TotalReject = receive.TotalReject != null ? receive.TotalReject : 0,
                                 ActualGood = receive.ActualDelivered,

                             }).GroupBy(x => new
                             {
                                 x.Id,
                                 x.PO_Number,
                                 x.ItemCode,
                                 x.ItemDescription,
                                 x.Supplier,
                                 x.QuantityOrdered,
                                 x.IsActive,
                                 x.DateCancelled,
                                
                             }).Select(receive => new CancelledPoDto
                             {
                                 Id = receive.Key.Id,
                                 PO_Number = receive.Key.PO_Number,
                                 ItemCode = receive.Key.ItemCode,
                                 ItemDescription = receive.Key.ItemDescription,
                                 Supplier = receive.Key.Supplier,
                                 IsActive = receive.Key.IsActive,
                                 DateCancelled = receive.Key.DateCancelled,
                                 ActualRemaining = receive.Key.QuantityOrdered - receive.Sum(x => x.ActualGood),

                             }).Where(x => x.IsActive == false)
                               .Where(x => Convert.ToString(x.PO_Number).ToLower().Contains(search.Trim().ToLower())
                                                  || Convert.ToString(x.ItemCode).ToLower().Contains(search.Trim().ToLower())
                                                  || Convert.ToString(x.ItemDescription).ToLower().Contains(search.Trim().ToLower())
                                                  || Convert.ToString(x.Supplier).ToLower().Contains(search.Trim().ToLower()));

            return await PagedList<CancelledPoDto>.CreateAsync(poSummary, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<WarehouseReceivingDto>> GetAllPoSummaryWithPagination(UserParams userParams)
        {
            var poSummary = 

                              (from posummary in _context.PoSummaries
                               where posummary.IsActive == true
                               orderby posummary.ImportDate

                               join warehouse in _context.WarehouseReceived
                               on posummary.Id equals warehouse.PoSummaryId into leftJ
                               from receive in leftJ.DefaultIfEmpty()

                             join material in _context.Materials
                             on posummary.ItemCode equals material.ItemCode
                             into leftJ1
                             from material in leftJ1.DefaultIfEmpty()

                               select new WarehouseReceivingDto
                               {

                                   Id = posummary.Id,
                                   PoNumber = posummary.PO_Number,
                                   PoDate = posummary.PO_Date,
                                   PrNumber = posummary.PR_Number,
                                   PrDate = posummary.PR_Date,
                                   ItemCode = posummary.ItemCode,
                                   ItemDescription = posummary.ItemDescription,
                                   Supplier = posummary.VendorName,
                                   Uom = posummary.Uom,
                                   QuantityOrdered = posummary.Ordered,
                                   IsActive = posummary.IsActive,
                                   ActualRemaining = 0,
                                   UnitPrice = posummary.UnitPrice != null ? posummary.UnitPrice : 0 ,
                                   TotalReject = receive.TotalReject != null ? receive.TotalReject : 0,
                                   ActualGood = receive != null && receive.IsActive != false ? receive.ActualDelivered : 0,
                                   LotSectionId = material.LotSectionId,
                                   LotSection = material.LotSection.SectionName,


                               }).GroupBy(x => new
                             {
                                 x.Id,
                                 x.PoNumber,
                                 x.PoDate,
                                 x.PrNumber,
                                 x.PrDate,
                                 x.ItemCode,
                                 x.ItemDescription,
                                 x.Uom,
                                 x.Supplier,
                                 x.QuantityOrdered,
                                 x.IsActive,
                                 x.UnitPrice,
                                 x.LotSection,
                                 x.LotSectionId,

                             })

                                                     .Select(receive => new WarehouseReceivingDto
                                                     {
                                                         Id = receive.Key.Id,
                                                         PoNumber = receive.Key.PoNumber,
                                                         PoDate = receive.Key.PoDate,
                                                         PrNumber = receive.Key.PrNumber,
                                                         PrDate = receive.Key.PrDate,
                                                         ItemCode = receive.Key.ItemCode,
                                                         ItemDescription = receive.Key.ItemDescription,
                                                         Uom = receive.Key.Uom,
                                                         Supplier = receive.Key.Supplier,
                                                         TotalReject = receive.Sum(x => x.TotalReject),
                                                         QuantityOrdered = receive.Key.QuantityOrdered ,
                                                         ActualGood = receive.Sum(x => x.ActualGood),
                                                         ActualRemaining = receive.Key.QuantityOrdered - receive.Sum(x => x.ActualGood),
                                                         IsActive = receive.Key.IsActive,
                                                         UnitPrice = receive.Key.UnitPrice,
                                                         LotSection = receive.Key.LotSection,
                                                         LotSectionId = receive.Key.LotSectionId
                                                         
                                                        
                                                     })
                                                  
                                                     .Where(x => x.ActualRemaining != 0 && (x.ActualRemaining > 0) )
                                                     .Where(x => x.IsActive == true);

            return await PagedList<WarehouseReceivingDto>.CreateAsync(poSummary, userParams.PageNumber, userParams.PageSize);
        }


        //public async Task<PagedList<WarehouseReceivingDto>> GetPoSummaryByStatusWithPaginationOrig(UserParams userParams, string search)
        //{
        //    var poSummary = (from posummary in _context.PoSummaries
        //                     where posummary.IsActive == true

        //                     join warehouse in _context.WarehouseReceived
        //                     on posummary.Id equals warehouse.PoSummaryId into leftJ
        //                     from receive in leftJ.DefaultIfEmpty()
        //                     //where receive.IsActive == true

        //                     join material in _context.Materials
        //                     on posummary.ItemCode equals material.ItemCode
        //                     into leftJ1
        //                     from material in leftJ1.DefaultIfEmpty()

        //                     group new
        //                     {
        //                         posummary,
        //                         receive,
        //                         material,

        //                     }
        //                      by new
        //                      {
        //                          posummary.Id,
        //                          posummary.PO_Number,
        //                          posummary.ItemCode,
        //                      }
        //                     into receive
        //                     select new WarehouseReceivingDto
        //                     {
        //                         Id = receive.Key.Id,
        //                         PoNumber = receive.Key.PO_Number,
        //                         PoDate = receive.First().posummary.PO_Date,
        //                         RRNumber = receive.First().posummary.RRNo,
        //                         RRDate = receive.First().posummary.RRDate,
        //                         PrNumber = receive.First().posummary.PR_Number,
        //                         PrDate = receive.First().posummary.PR_Date,
        //                         PR_Year_Number = receive.First().posummary.PR_Year_Number,
        //                         ItemCode = receive.Key.ItemCode,
        //                         ItemDescription = receive.First().material.ItemDescription,
        //                         Uom = receive.First().material.Uom.UomCode,
        //                         Supplier = receive.First().posummary.VendorName,
        //                         QuantityOrdered = receive.First().posummary.Ordered,
        //                         ActualGood = receive.Sum(x => x.receive.ActualGood != null ? x.receive.ActualGood : 0),
        //                         ActualRemaining = receive.First().posummary.ActualRemaining,
        //                         IsActive = receive.First().posummary.IsActive,
        //                         TotalReject = receive.Sum(x => x.receive.TotalReject != null ? x.receive.TotalReject : 0),
        //                         UnitPrice = receive.First().posummary.UnitPrice,
        //                         LotSection = receive.First().material.LotSection.SectionName,
        //                         LotSectionId = receive.First().material.LotSectionId,
        //                         SINumber = receive.First().posummary.SINumber,
        //                         ReceiveDate = receive.First().posummary.ReceiveDate,
        //                         QuantityDelivered = receive.First().posummary.Delivered

                                 

        //                     }).Where(x => x.ActualRemaining != 0 && (x.ActualRemaining > 0));


        //    if(!string.IsNullOrEmpty(search))
        //    {
        //      poSummary = poSummary.Where(x => x.ItemDescription.ToLower().Contains(search.Trim().ToLower())
        //                             || Convert.ToString(x.PoNumber).ToLower().Contains(search.Trim().ToLower())
        //                             || x.ItemCode.ToLower().Contains(search.Trim().ToLower()));
        //    }


        //    poSummary = poSummary.OrderBy(x => x.PoNumber);


        //    return await PagedList<WarehouseReceivingDto>.CreateAsync(poSummary, userParams.PageNumber, userParams.PageSize);
        //}



        public async Task<PagedList<WarehouseReceivingDto>> GetPoSummaryByStatusWithPaginationOrig(UserParams userParams, string search)
        {
            var poSummary = (from posummary in _context.PoSummaries
                             where posummary.IsActive == true && posummary.IsReceived != true

                             join warehouse in _context.WarehouseReceived
                             on posummary.Id equals warehouse.PoSummaryId into leftJ
                             from receive in leftJ.DefaultIfEmpty()
                                 //where receive.IsActive == true

                             join material in _context.Materials
                             on posummary.ItemCode equals material.ItemCode
                             into leftJ1
                             from material in leftJ1.DefaultIfEmpty()


                             group new
                             {
                                 posummary,
                                 receive,
                                 material,

                             }
                              by new
                              {
                                  posummary.Id,
                                  posummary.PO_Number,
                                  posummary.ItemCode,
                              }
                             into receive
                             select new WarehouseReceivingDto
                             {
                                 Id = receive.Key.Id,
                                 PoNumber = receive.Key.PO_Number,
                                 PoDate = receive.First().posummary.PO_Date,
                                 RRNumber = receive.First().posummary.RRNo,
                                 RRDate = receive.First().posummary.RRDate,
                                 PrNumber = receive.First().posummary.PR_Number,
                                 PrDate = receive.First().posummary.PR_Date,
                                 PR_Year_Number = receive.First().posummary.PR_Year_Number,
                                 ItemCode = receive.Key.ItemCode,
                                 ItemDescription = receive.First().material.ItemDescription,
                                 Uom = receive.First().material.Uom.UomCode,
                                 Supplier = receive.First().posummary.VendorName,
                                 QuantityOrdered = receive.First().posummary.Ordered,
                                 ActualGood = receive.Sum(x => x.receive.ActualGood != null ? x.receive.ActualGood : 0),
                                 ActualRemaining = receive.First().posummary.ActualRemaining,
                                 IsActive = receive.First().posummary.IsActive,
                                 TotalReject = receive.Sum(x => x.receive.TotalReject != null ? x.receive.TotalReject : 0),
                                 UnitPrice = receive.First().posummary.UnitPrice,
                                 LotSection = receive.First().material.LotSection.SectionName,
                                 LotSectionId = receive.First().material.LotSectionId,
                                 SINumber = receive.First().posummary.SINumber,
                                 ReceiveDate = receive.First().posummary.ReceiveDate,
                                 QuantityDelivered = receive.First().posummary.Delivered,



                             });


            if (!string.IsNullOrEmpty(search))
            {
                poSummary = poSummary.Where(x => x.ItemDescription.ToLower().Contains(search.Trim().ToLower())
                                       || Convert.ToString(x.PoNumber).ToLower().Contains(search.Trim().ToLower())
                                       || x.ItemCode.ToLower().Contains(search.Trim().ToLower()));
            }


            poSummary = poSummary.OrderBy(x => x.PoNumber);


            return await PagedList<WarehouseReceivingDto>.CreateAsync(poSummary, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<bool> CancelPo(PoSummary summary)
        {
            var existingPo = await _context.PoSummaries.Where(x => x.Id == summary.Id)
                                                       .FirstOrDefaultAsync();

            existingPo.IsActive = false;
            existingPo.DateCancelled = DateTime.Now;
            existingPo.Reason = summary.Reason;
            existingPo.CancelBy = summary.CancelBy;
            existingPo.IsCancelled = true;
            existingPo.DateCancelled = DateTime.Now;

            return true;
        }


        public async Task<bool> ValidatePoId(int id)
        {
            var validateExisting = await _context.PoSummaries.Where(x => x.Id == id)
                                                           .Where(x => x.IsActive == true)
                                                           .FirstOrDefaultAsync();
            if (validateExisting == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateActualRemaining(Warehouse_Receiving receiving)
        {
            var validateActualRemaining = await (from posummary in _context.PoSummaries
                                                 join receive in _context.WarehouseReceived on posummary.Id equals receive.PoSummaryId into leftJ
                                                 from receive in leftJ.DefaultIfEmpty()
                                                 where posummary.IsActive == true
                                                 select new PoSummaryChecklistDto
                                                 {
                                                     Id = posummary.Id,
                                                     PO_Number = posummary.PO_Number,
                                                     ItemCode = posummary.ItemCode,
                                                     ItemDescription = posummary.ItemDescription,
                                                     Supplier = posummary.VendorName,
                                                     UOM = posummary.Uom,
                                                     QuantityOrdered = posummary.Ordered,
                                                     ActualGood = receive != null && receive.IsActive != false ? receive.ActualDelivered : 0,
                                                     IsActive = posummary.IsActive,
                                                     ActualRemaining = 0,
                                                     IsQcReceiveIsActive = receive != null ? receive.IsActive : true
                                                 })
                                                        .GroupBy(x => new
                                                        {
                                                            x.Id,
                                                            x.PO_Number,
                                                            x.ItemCode,
                                                            x.ItemDescription,
                                                            x.UOM,
                                                            x.QuantityOrdered,
                                                            x.IsActive,
                                                            x.IsQcReceiveIsActive
                                                        })
                                                   .Select(receive => new PoSummaryChecklistDto
                                                   {
                                                       Id = receive.Key.Id,
                                                       PO_Number = receive.Key.PO_Number,
                                                       ItemCode = receive.Key.ItemCode,
                                                       ItemDescription = receive.Key.ItemDescription,
                                                       UOM = receive.Key.UOM,
                                                       QuantityOrdered = receive.Key.QuantityOrdered,
                                                       ActualGood = receive.Sum(x => x.ActualGood),
                                                       ActualRemaining = ((receive.Key.QuantityOrdered /*+ (receive.Key.QuantityOrdered / 100) * 10*/) - (receive.Sum(x => x.ActualGood))), // formula for 10% allowable
                                                       IsActive = receive.Key.IsActive,
                                                       IsQcReceiveIsActive = receive.Key.IsQcReceiveIsActive
                                                   }).Where(x => x.IsQcReceiveIsActive == true)
                                                     .FirstOrDefaultAsync(x => x.Id == receiving.PoSummaryId);

            if (validateActualRemaining == null)
                return true;

            if (validateActualRemaining.ActualRemaining < receiving.ActualDelivered) 
                return false;

            return true;

        }

        public async Task<bool> ReturnPoInAvailableList(PoSummary summary)
        {
            var existingInfo = await _context.PoSummaries.Where(x => x.Id == summary.Id)
                                                       .FirstOrDefaultAsync();
            if (existingInfo == null)
                return false;

            existingInfo.IsActive = true;
            existingInfo.DateCancelled = null;
            existingInfo.Reason = null;
            existingInfo.IsCancelled = null;

            return true;
        }

        public async Task<PagedList<WarehouseReceivingDto>> ListOfWarehouseReceivingIdWithPagination(UserParams userParams)
        {

            var warehouseInventory = _context.WarehouseReceived.OrderBy(x => x.ActualReceivingDate)
                .Select(x => new WarehouseReceivingDto
                {

                    Id = x.Id,
                    PoNumber = x.PoNumber,
                    RRNumber = x.RRNo,
                    RRDate = x.RRDate,
                    PR_Year_Number = x.PR_Year_Number,
                    ItemCode = x.ItemCode,
                    ItemDescription = x.ItemDescription,
                    ActualGood = x.ActualDelivered,
                    DateReceive = x.ReceivingDate.ToString(),
                    Supplier = x.Supplier,
                    Uom = x.Uom,
                    LotSection = x.LotSection,
                    UnitPrice = x.UnitPrice,
                    TotalUnitPrice = x.UnitPrice * x.ActualDelivered,
                    SINumber = x.SINumber,
                    TransactionType = x.TransactionType
                    

                });

            return await PagedList<WarehouseReceivingDto>.CreateAsync(warehouseInventory, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<WarehouseReceivingDto>> ListOfWarehouseReceivingIdWithPaginationOrig(UserParams userParams, string search)
        {
            var warehouseInventory = _context.WarehouseReceived.OrderBy(x => x.ActualReceivingDate)
               .Select(x => new WarehouseReceivingDto
               {
                   Id = x.Id,
                   PoNumber = x.PoNumber,
                   RRNumber = x.RRNo,
                   RRDate = x.RRDate,
                   PR_Year_Number = x.PR_Year_Number,
                   ItemCode = x.ItemCode,
                   ItemDescription = x.ItemDescription,
                   ActualGood = x.ActualDelivered,
                   DateReceive = x.ReceivingDate.ToString(),
                   Supplier = x.Supplier,
                   Uom = x.Uom,
                   LotSection = x.LotSection,
                   UnitPrice = x.UnitPrice,
                   TotalUnitPrice = x.UnitPrice * x.ActualDelivered,
                   SINumber = x.SINumber,
                   TransactionType = x.TransactionType

               }).Where(x => x.ItemCode.ToLower().Contains(search.Trim().ToLower())
               || Convert.ToString(x.ItemDescription).ToLower().Contains(search.Trim().ToLower())
               || Convert.ToString(x.Supplier).ToLower().Contains(search.Trim().ToLower())
               || Convert.ToString(x.SINumber).ToLower().Contains(search.Trim().ToLower())
               || Convert.ToString(x.PoNumber).ToLower().Contains(search.Trim().ToLower())
               );

            return await PagedList<WarehouseReceivingDto>.CreateAsync(warehouseInventory, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<IReadOnlyList<ListofwarehouseReceivingIdDto>> ListOfWarehouseReceivingId(string search)
        {

            var moveorderOut = _context.MoveOrders
                //.Where(x => x.ItemCode.ToLower().Contains(search.Trim().ToLower()))
                                      .Where(x => x.ItemCode.ToLower().Contains(search.Trim().ToLower()))
                                      .Where(x => x.IsActive == true && x.IsPrepared == true)
                                      .GroupBy(x => new
                                      {
                                          x.WarehouseId,
                                          x.ItemCode,


                                      }).Select(x => new ItemStocksDto
                                      {
                                          warehouseId = x.Key.WarehouseId,
                                          ItemCode = x.Key.ItemCode,
                                          Out = x.Sum(x => x.QuantityOrdered),

                                      });


            var getIssueOut = _context.MiscellaneousIssueDetail
                .Where(x => x.ItemCode.ToLower().Contains(search.Trim().ToLower()))
                                                 .Where(x => x.IsActive == true)
                                                 .GroupBy(x => new
                                                 {
                                                     x.WarehouseId,
                                                     x.ItemCode,


                                                 }).Select(x => new ItemStocksDto
                                                 {
                                                     warehouseId = x.Key.WarehouseId,
                                                     ItemCode = x.Key.ItemCode,
                                                     Out = x.Sum(x => x.Quantity != null ? x.Quantity : 0)

                                                 }) ;


            var BorrowOut = _context.BorrowedIssueDetails
                .Where(x => x.ItemCode.ToLower().Contains(search.Trim().ToLower()))
               .Where(x => x.IsActive == true)
                                                         .GroupBy(x => new
                                                         {
                                                             x.WarehouseId,
                                                             x.ItemCode,

                                                         }).Select(x => new ItemStocksDto
                                                         {
                                                             warehouseId = x.Key.WarehouseId,
                                                             ItemCode = x.Key.ItemCode,
                                                             Out = x.Sum(x => x.Quantity),

                                                         });

            var consumed = _context.BorrowedConsumes

                .Where(x => x.IsActive)
                .Where(x => x.ItemCode.ToLower().Contains(search.Trim().ToLower()))

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
                .Where(x => x.ItemCode.ToLower().Contains(search.Trim().ToLower()))
                .Where(x => x.IsActive == true)
                                                             .Where(x => x.IsReturned == true)
                                                             .Where(x => x.IsApprovedReturned == true)
                                                             .Where(x => x.ItemCode.ToLower().Contains(search.Trim().ToLower()))
                                                             .GroupJoin(consumed, returned => returned.Id, itemconsume => itemconsume.BorrowedItemPkey, (returned, itemconsume) => new { returned, itemconsume })
                                                             .SelectMany(x => x.itemconsume.DefaultIfEmpty(), (x, itemconsume) => new { x.returned, itemconsume })
                                                             .GroupBy(x => new
                                                             {
                                                                 x.returned.WarehouseId,
                                                                 x.returned.ItemCode,

                                                                 //x.itemconsume.Consume

                                                             }).Select(x => new ItemStocksDto
                                                             {
                                                                 warehouseId = x.Key.WarehouseId,
                                                                 ItemCode = x.Key.ItemCode,
                                                                 In = x.Sum(x => x.returned.Quantity) - x.Sum(x => x.itemconsume.Consume),

                                                             });


            var warehouseInventory = _context.WarehouseReceived
                                    .Where(x => x.IsActive == true)
                                    .GroupJoin(getIssueOut, warehouse => warehouse.Id , issue => issue.warehouseId , (warehouse,issue) => new {warehouse,issue })
                                    .SelectMany(x => x.issue.DefaultIfEmpty() , (x,issue) => new {x.warehouse,issue })
                                   .GroupJoin(moveorderOut, warehouse => warehouse.warehouse.Id, moveorder => moveorder.warehouseId, (warehouse, moveorder) => new { warehouse, moveorder })
                                   .SelectMany(x => x.moveorder.DefaultIfEmpty(), (x, moveorder) => new { x.warehouse, moveorder })
                                   .GroupJoin(BorrowOut, warehouse => warehouse.warehouse.warehouse.Id, borrowed => borrowed.warehouseId, (warehouse, borrowed) => new { warehouse, borrowed })
                                   .SelectMany(x => x.borrowed.DefaultIfEmpty(), (x, borrowed) => new { x.warehouse, borrowed })
                                   .GroupJoin(BorrowedReturn, warehouse => warehouse.warehouse.warehouse.warehouse.Id, returned => returned.warehouseId, (warehouse, returned) => new { warehouse, returned })
                                   .SelectMany(x => x.returned.DefaultIfEmpty(), (x, returned) => new { x.warehouse, returned })
                                   .GroupBy(x => new
                                   {

                                       x.warehouse.warehouse.warehouse.warehouse.Id,
                                       x.warehouse.warehouse.warehouse.warehouse.ItemCode,

                                   }).
                                     Where(x => x.Key.ItemCode.ToLower().Contains(search.Trim().ToLower()))
                                    .OrderBy(x => x.Key.ItemCode)
                                    .ThenBy(x => x.First().warehouse.warehouse.warehouse.warehouse.ActualReceivingDate)
                                   .Select(total => new ListofwarehouseReceivingIdDto
                                     {
                                         Id = total.Key.Id,
                                         ItemCode = total.Key.ItemCode,
                                       ItemDescription = total.First().warehouse.warehouse.warehouse.warehouse.ItemDescription,
                                       ReceivingDate = total.First().warehouse.warehouse.warehouse.warehouse.ActualReceivingDate.ToString(),
                                       ActualGood = total.First().warehouse.warehouse.warehouse.warehouse.ActualGood
                                         + total.Sum(x => x.returned.In != null ? x.returned.In : 0)
                                         - total.Sum(x => x.warehouse.warehouse.moveorder.Out)
                                         - total.Sum(x => x.warehouse.warehouse.warehouse.issue.Out)
                                         -total.Sum(x => x.warehouse.borrowed.Out)

                                     })/*.Where(x => x.ActualGood > 0)*/;

            return await warehouseInventory.ToListAsync();
                         
        }


        public async Task<IReadOnlyList<WarehouseReceivingDto>> PoSummaryForWarehouseNotif()
        {

            var poSummary =

                              (from posummary in _context.PoSummaries
                               where posummary.IsActive == true
                               join warehouse in _context.WarehouseReceived
                               on posummary.Id equals warehouse.PoSummaryId into leftJ
                               from receive in leftJ.DefaultIfEmpty()
                               select new WarehouseReceivingDto
                               {

                                   Id = posummary.Id,
                                   PoNumber = posummary.PO_Number,
                                   PoDate = posummary.PO_Date,
                                   PrNumber = posummary.PR_Number,
                                   PrDate = posummary.PR_Date,
                                   ItemCode = posummary.ItemCode,
                                   ItemDescription = posummary.ItemDescription,
                                   Supplier = posummary.VendorName,
                                   Uom = posummary.Uom,
                                   QuantityOrdered = posummary.Ordered,
                                   IsActive = posummary.IsActive,
                                   ActualRemaining = 0,
                                   TotalReject = receive.TotalReject != null ? receive.TotalReject : 0,
                                   ActualGood = receive != null && receive.IsActive != false ? receive.ActualDelivered : 0,

                               }).GroupBy(x => new
                               {
                                   x.Id,
                                   x.PoNumber,
                                   x.PoDate,
                                   x.PrNumber,
                                   x.PrDate,
                                   x.ItemCode,
                                   x.ItemDescription,
                                   x.Uom,
                                   x.Supplier,
                                   x.QuantityOrdered,
                                   x.IsActive,

                               })
                                                     .Select(receive => new WarehouseReceivingDto
                                                     {
                                                         Id = receive.Key.Id,
                                                         PoNumber = receive.Key.PoNumber,
                                                         PoDate = receive.Key.PoDate,
                                                         PrNumber = receive.Key.PrNumber,
                                                         PrDate = receive.Key.PrDate,
                                                         ItemCode = receive.Key.ItemCode,
                                                         ItemDescription = receive.Key.ItemDescription,
                                                         Uom = receive.Key.Uom,
                                                         Supplier = receive.Key.Supplier,
                                                         TotalReject = receive.Sum(x => x.TotalReject),
                                                         QuantityOrdered = receive.Key.QuantityOrdered,
                                                         ActualGood = receive.Sum(x => x.ActualGood),
                                                         ActualRemaining = receive.Key.QuantityOrdered - receive.Sum(x => x.ActualGood),
                                                         IsActive = receive.Key.IsActive,

                                                     })
                                                     .OrderBy(x => x.PoNumber)
                                                     .Where(x => x.ActualRemaining != 0 && (x.ActualRemaining > 0))
                                                     .Where(x => x.IsActive == true);

            return await poSummary.ToListAsync();
              
        }

        public async Task<IReadOnlyList<CancelledPoDto>> CancelledPoSummaryNotif()
        {

            var poSummary = (from posummary in _context.PoSummaries
                             where posummary.IsActive == false
                             where posummary.IsCancelled == true
                             join warehouse in _context.WarehouseReceived
                             on posummary.Id equals warehouse.PoSummaryId into leftJ
                             from receive in leftJ.DefaultIfEmpty()

                             select new CancelledPoDto
                             {
                                 Id = posummary.Id,
                                 PO_Number = posummary.PO_Number,
                                 ItemCode = posummary.ItemCode,
                                 ItemDescription = posummary.ItemDescription,
                                 Supplier = posummary.VendorName,
                                 QuantityOrdered = posummary.Ordered,
                                 IsActive = posummary.IsActive,
                                 DateCancelled = posummary.DateCancelled.ToString(),
                                 ActualRemaining = 0,
                                 TotalReject = receive.TotalReject != null ? receive.TotalReject : 0,
                                 ActualGood = receive.ActualDelivered,

                             }).GroupBy(x => new
                             {
                                 x.Id,
                                 x.PO_Number,
                                 x.ItemCode,
                                 x.ItemDescription,
                                 x.Supplier,
                                 x.QuantityOrdered,
                                 x.IsActive,
                                 x.ActualGood,
                                 x.DateCancelled,

                             }).Select(receive => new CancelledPoDto
                             {
                                 Id = receive.Key.Id,
                                 PO_Number = receive.Key.PO_Number,
                                 ItemCode = receive.Key.ItemCode,
                                 ItemDescription = receive.Key.ItemDescription,
                                 Supplier = receive.Key.Supplier,
                                 ActualGood = receive.Sum(x => x.ActualGood),
                                 QuantityOrdered = receive.Key.QuantityOrdered,
                                 IsActive = receive.Key.IsActive,
                                 DateCancelled = receive.Key.DateCancelled,
                                 TotalReject = receive.Sum(x => x.TotalReject),
                                 ActualRemaining = receive.Key.QuantityOrdered - receive.Sum(x => x.ActualGood),

                             }).OrderBy(x => x.PO_Number)
                               .Where(x => x.IsActive == false);


            return await poSummary.ToListAsync();
        }

    }
}
