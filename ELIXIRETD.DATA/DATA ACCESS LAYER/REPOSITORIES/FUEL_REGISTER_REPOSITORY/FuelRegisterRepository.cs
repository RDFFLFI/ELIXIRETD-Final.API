using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using ELIXIRETD.DATA.CORE.INTERFACES.FUEL_REGISTER_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.FUEL_REGISTER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.FUEL_REGISTER_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.FUEL_REGISTER_REPOSITORY
{
    public class FuelRegisterRepository : IFuelRegisterRepository
    {
        private readonly StoreContext _context;

        public FuelRegisterRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<bool> MaterialNotExist(int id)
        {
            var fuelNotExist = await _context.Materials
                .FirstOrDefaultAsync(f => f.Id == id);

            if(fuelNotExist is null) 
                return false;

            return true;
        }

        public async Task<bool> WarehouseNotExist(int id)
        {
            var warehouseNotExist = await _context.WarehouseReceived
                .FirstOrDefaultAsync(f => f.Id == id);

            if (warehouseNotExist is null)
                return false;

            return true;
        }

        public async Task<bool> CreateFuelRegister(CreateFuelRegisterDto fuel)
        {


            var fuelRegisterExist = await _context.FuelRegisters
                .FirstOrDefaultAsync(fr => fr.Id == fuel.Id);

            var material = await _context.Materials
                .FirstOrDefaultAsync(m => m.ItemCode.ToUpper() == fuel.Item_Code);

            if (fuelRegisterExist is not null)
            {
                fuelRegisterExist.UserId = fuel.UserId;
                fuelRegisterExist.MaterialId = material.Id;
                fuelRegisterExist.Warehouse_ReceivingId = fuel.Warehouse_ReceivingId;
                fuelRegisterExist.Liters = fuel.Liters;
                fuelRegisterExist.Asset = fuel.Asset;
                fuelRegisterExist.Odometer = fuel.Odometer;
                fuelRegisterExist.Modified_By = fuel.Modified_By;
                fuelRegisterExist.Updated_At = DateTime.Now;
                fuelRegisterExist.Remarks = fuel.Remarks;

            }
            else
            {

                var newFuelRegister = new FuelRegister
                {
                    Source = "ELIXIR ETD",
                    UserId = fuel.UserId,
                    MaterialId = material.Id,
                    Warehouse_ReceivingId = fuel.Warehouse_ReceivingId,
                    Liters = fuel.Liters,
                    Asset = fuel.Asset,
                    Odometer = fuel.Odometer,
                    Added_By = fuel.Added_By,
                    Remarks = fuel.Remarks,
                    Is_Approve = false,
                    
                };

                await _context.FuelRegisters.AddAsync(newFuelRegister);

            }
          
            return true;
        }

        public async Task<IReadOnlyList<GetMaterialByStocksDto>> GetMaterialByStocks()
        {

            string diesel = "DIESEL";

            var getWarehouseStocks = _context.WarehouseReceived
                .Where(x => x.IsActive == true)
                .Where(x => x.ItemCode.Contains(diesel))
                .Where(x => x.ItemCode.Contains(diesel))
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
                                                                 In = x.Sum(x => x.returned.Quantity != null ? x.returned.Quantity : 0) - x.Sum(x => x.itemconsume.Consume),

                                                             });

            var fuelRegister = _context.FuelRegisters
                .Include(x => x.Material)
                .Where(fr => fr.Is_Approve == false)
                .Where(fr => fr.Is_Active == true)
                .GroupBy(fr => fr.Material.ItemCode)
                .Select(fr => new
                {
                    ItemCode = fr.Key,
                    Quantity = fr.Sum(q => q.Liters)

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
                                on warehouse.ItemCode equals fuel.ItemCode
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

                                select new GetMaterialByStocksDto
                                {

                                    Item_Code = total.Key.ItemCode,
                                    Item_Description = total.First().warehouse.ItemDescription,
                                    Uom = total.First().warehouse.Uom,
                                    RemainingStocks = total.Sum(x => x.warehouse.ActualGood) + total.Sum(x => x.borrowedReturned.In)
                                    - total.Sum(x => x.reserve.QuantityOrdered) - total.Sum(x => x.issue.Out) - total.Sum(x => x.borrowOut.Out) - total.Sum(x => x.fuel.Quantity.Value),

                                }).Where(x => x.RemainingStocks >= 1);


            var GetAvailableItem = getAvailable
                .Join(_context.Materials
                .Where(f => f.IsActive),
                available => available.Item_Code, fuel => fuel.ItemCode , (available,fuel) => new {available,fuel})
                .GroupBy(x => new
            {
                x.available.Item_Code,

            }).Select(x => new GetMaterialByStocksDto
            {
                Id = x.First().fuel.Id,
                Item_Code = x.Key.Item_Code,
                Item_Description = x.First().available.Item_Description,
                Uom = x.First().available.Uom,
                RemainingStocks = x.Sum(x => x.available.RemainingStocks),

            }).OrderBy(x => x.Item_Code);


            return await GetAvailableItem.ToListAsync();

        }

        public async Task<IReadOnlyList<GetMaterialStockByWarehouseDto>> GetMaterialStockByWarehouse()
        {

           var itemCode = "DIESEL";

               var getWarehouseStocks = _context.WarehouseReceived
                .Where(x => x.ItemCode == itemCode)
                .Where(x => x.IsActive == true)
                .Select(x => new WarehouseInventory
                {

                    WarehouseId = x.Id,
                    ItemCode = x.ItemCode,
                    ActualGood = x.ActualDelivered,
                    RecievingDate = x.ActualReceivingDate.ToString(),
                    UnitPrice = x.UnitPrice

                });


            var moveorderOut = _context.MoveOrders
                .Where(x => x.ItemCode == itemCode)
                .Where(x => x.IsActive == true)
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


            var issueOut = _context.MiscellaneousIssueDetail
                .Where(x => x.ItemCode == itemCode)
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


            var issueSumOut = _context.MiscellaneousIssueDetail
                .Where(x => x.ItemCode == itemCode)
                .Where(x => x.IsActive == true)
                .GroupBy(x => new
                {

                    x.ItemCode,


                }).Select(x => new ItemStocksDto
                {
                    ItemCode = x.Key.ItemCode,
                    Out = x.Sum(x => x.Quantity),

                });


            var BorrowedOut = _context.BorrowedIssueDetails
                .Where(x => x.ItemCode == itemCode)
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


            var BorrowedSumOut = _context.BorrowedIssueDetails
                .Where(x => x.ItemCode == itemCode)
                .Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new ItemStocksDto
                {
                    ItemCode = x.Key.ItemCode,
                    Out = x.Sum(x => x.Quantity),

                });

            var consumed = _context.BorrowedConsumes
                .Where(x => x.IsActive)
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
                .Where(x => x.ItemCode == itemCode)
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


            var fuelRegister = _context.FuelRegisters
                .Include(m => m.Material)
                .Where(fr => fr.Is_Active == true)
                .Where(fr => fr.Is_Approve == false && fr.Material.ItemCode == itemCode)
                .GroupBy(fr => new
                {
                    fr.Material.ItemCode,
                    fr.Warehouse_ReceivingId,

                }).Select(fr => new
                {
                    itemCode = fr.Key.ItemCode,
                    WarehouseId = fr.Key.Warehouse_ReceivingId,
                    Quantity = fr.Sum(fr => fr.Liters)

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
                                into LeftJ5
                                from fuel in LeftJ5.DefaultIfEmpty()

                                group new
                                {

                                    warehouse,
                                    Moveorder,
                                    issue,
                                    borrowOut,
                                    returned,
                                    fuel,
                                    //reserve
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
                                    FuelRegister = fuel.Quantity != null ? fuel.Quantity : 0

                                } into total

                                select new GetMaterialStockByWarehouseDto
                                {

                                    WarehouseId = total.Key.WarehouseId,
                                    ItemCode = total.Key.ItemCode,
                                    Remaining_Stocks = total.Key.WarehouseActualGood + total.Key.borrowedreturn - total.Key.MoveOrderOut - total.Key.IssueOut - total.Key.BorrowedOut - total.Key.FuelRegister ,
                                    Receiving_Date = total.Key.RecievingDate,
                                    Unit_Cost = total.Key.UnitPrice,

                                }).Where(x => x.Remaining_Stocks > 0);


            return await getRemaining.ToListAsync();


        }

        public async Task<PagedList<GetFuelRegisterDto>> GetFuelRegister(UserParams userParams, string Search, string Status, int ? UserId)
        {
            const string forApproval = "For Approval";
            const string approved = "Approved";
            const string forTransacted = "For Transaction";
            const string transacted = "Transacted";
            const string rejected = "Rejected";


            var results =  _context.FuelRegisters
                .Include(r => r.Material)
                .ThenInclude(r => r.Uom)
                .Include(x => x.User)
                .Include(r => r.Material)
                .ThenInclude(r => r.ItemCategory)
                .Where(f => f.Is_Active)
                .Select(f => new GetFuelRegisterDto
                {
                    Id = f.Id,
                    Source = f.Source,
                    //Plate_No = f.Plate_No,
                    UserId = f.UserId.Value,
                    Driver = f.User.FullName,
                    MaterialId = f.MaterialId,
                    Item_Code = f.Material.ItemCode,
                    Item_Description = f.Material.ItemDescription,
                    Uom = f.Material.Uom.UomCode,
                    Item_Categories = f.Material.ItemCategory.ItemCategoryName,
                    Warehouse_ReceivingId = f.Warehouse_ReceivingId,
                    Unit_Cost = f.Warehouse_Receiving.UnitPrice,
                    Liters = f.Liters.Value,
                    Asset = f.Asset,
                    Odometer = f.Odometer,
                    Company_Code = f.Company_Code,
                    Company_Name = f.Company_Name,
                    Department_Code = f.Department_Code,
                    Department_Name = f.Department_Name,
                    Location_Code = f.Location_Code,
                    Location_Name = f.Location_Name,
                    Account_Title_Code = f.Account_Title_Code,
                    Account_Title_Name = f.Account_Title_Name,
                    EmpId = f.EmpId,
                    Fullname = f.Fullname,
                    Added_By = f.Added_By,
                    Created_At = f.Created_At,
                    Modified_By = f.Modified_By,
                    Updated_At = f.Updated_At,
                    Is_Reject = f.Is_Reject,
                    Reject_Remarks = f.Reject_Remarks,
                    Reject_By = f.Reject_By,
                    Is_Approve = f.Is_Approve,
                    Approve_At = f.Approve_At,
                    Approve_By = f.Approve_By,
                    Is_Transact = f.Is_Transact,
                    Transact_At = f.Transact_At,
                    Transact_By = f.Transact_By,
                    Remarks = f.Remarks

                });


            if (!string.IsNullOrEmpty(Search))
                results = results.Where(r => r.Driver.Contains(Search)
                || r.Id.ToString().Contains(Search));


            if (UserId is not null)
            {
                results = results.Where(r => r.UserId == UserId);

            }

            if (!string.IsNullOrEmpty(Status))
            {
                switch (Status)
                {
                    case forApproval:
                        results = results.Where(r => r.Is_Approve == false && r.Is_Reject == false);
                        break;

                    case approved:
                        results = results.Where(r => r.Is_Approve);
                        break;

                    case forTransacted:
                        results = results.Where(r => r.Is_Approve == true && r.Is_Transact == null);
                        break;

                    case transacted:
                        results = results.Where(r => r.Is_Approve == true && r.Is_Transact == true);
                        break;

                    case rejected:
                        results = results.Where(r => r.Is_Reject);
                        break;

                    default:
                        return new PagedList<GetFuelRegisterDto>(new List<GetFuelRegisterDto>(), 0, userParams.PageNumber, userParams.PageSize);

                }

            }



            results = results.OrderBy(r => r.Id);


            return await PagedList<GetFuelRegisterDto>.CreateAsync(results,userParams.PageNumber, userParams.PageSize);
        }


        public async Task<bool> FuelRegisterNotExist(int id)
        {
            var fuelExist = await _context.FuelRegisters
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fuelExist is null)
                return false;


            return true;
        }
 
        public async Task<bool> ApproveFuel(ApproveFuelDto fuel)
        {
            var fuelExist = await _context.FuelRegisters
                .FirstOrDefaultAsync(f => f.Id == fuel.Id);

            fuelExist.Transact_At = DateTime.Now;
            fuelExist.Transact_By = fuel.Transact_By;
            fuelExist.Approve_At = DateTime.Now;
            fuelExist.Approve_By = fuel.Approve_By;
            fuelExist.Is_Approve = true;
            fuelExist.Company_Code = fuel.Company_Code; 
            fuelExist.Company_Name = fuel.Company_Name;
            fuelExist.Department_Code = fuel.Department_Code;
            fuelExist.Department_Name  = fuel.Department_Name;
            fuelExist.Location_Code = fuel.Location_Code;
            fuelExist.Location_Name = fuel.Location_Name;
            fuelExist.Account_Title_Code = fuel.Account_Title_Code;
            fuelExist.Account_Title_Name = fuelExist.Account_Title_Name;
            fuelExist.EmpId = fuel.EmpId;
            fuelExist.Fullname = fuel.Fullname;

            return true;

        }

        public async Task<bool> RejectFuel(RejectFuelDto fuel)
        {
            var fuelExist = await _context.FuelRegisters
                .FirstOrDefaultAsync(f => f.Id == fuel.Id);

            fuelExist.Reject_Remarks = fuel.Reject_Remarks;
            fuelExist.Reject_By = fuel.Reject_By;
            fuelExist.Is_Reject = true;

            return true;
        }

        public async Task<bool> TransactFuel(TransactedFuelDto fuel)
        {
            var fuelExist = await _context.FuelRegisters
                .FirstOrDefaultAsync(f => f.Id == fuel.Id && f.Is_Approve);

            if(fuelExist == null)
            {
                return false;
            }
        

            fuelExist.Transact_At = DateTime.Now;
            fuelExist.Transact_By = fuel.Transacted_By;
            fuelExist.Is_Transact = true;

            return true;
        }

        public async Task<bool> CancelFuel(int id)
        {
            var fuelExist = await _context.FuelRegisters
                .FirstOrDefaultAsync(f => f.Id == id);

            fuelExist.Is_Active = false;

            return true;
        }

        public async Task<IReadOnlyList<GetDriverUserDto>> GetDriverUser()
        {
            string roleName = "DRIVER";

            var role = _context.Roles
                .AsNoTrackingWithIdentityResolution()
                .Where(r => r.RoleName.ToUpper() == roleName)
                .Select(x => x.Id);

            var user = await _context.Users
                .AsNoTrackingWithIdentityResolution()
                .Include(u => u.UserRole)
                .AsSplitQuery()
                .Where(u => u.IsActive == true &&
                role.Contains(u.UserRoleId))
                .Select(u => new GetDriverUserDto
                {
                    Id = u.Id,
                    EmpId = u.EmpId,
                    Fullname  = u.FullName,
                    Username = u.UserName,
                    Role = u.UserRole.RoleName

                }).ToListAsync();

            return user;

        }

        public async Task<IReadOnlyList<GetForApprovalFuelDto>> GetForApprovalFuel(int id)
        {
            var fuel = await _context.FuelRegisters
                .Where(f => f.Id == id && f.Is_Approve == false)
                .Select(f => new GetForApprovalFuelDto
                {
                    Id = f.Id,
                    Source = f.Source,
                    UserId = f.UserId.Value,
                    Driver = f.User.FullName,
                    MaterialId = f.MaterialId,
                    Item_Code = f.Material.ItemCode,
                    Item_Description = f.Material.ItemDescription,
                    Uom = f.Material.Uom.UomCode,
                    Item_Categories = f.Material.ItemCategory.ItemCategoryName,
                    Warehouse_ReceivingId = f.Warehouse_ReceivingId,
                    Unit_Cost = f.Warehouse_Receiving.UnitPrice,
                    Liters = f.Liters.Value,
                    Asset = f.Asset,
                    Odometer = f.Odometer,
                    Added_By = f.Added_By,
                    Created_At = f.Created_At,
                    Modified_By = f.Modified_By,
                    Updated_At = f.Updated_At,
                    Is_Approved = f.Is_Approve,
                    


                }).ToListAsync();

            return fuel;
        }
    }
}
