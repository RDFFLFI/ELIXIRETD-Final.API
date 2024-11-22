using ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.SETUP_REPOSITORY
{
    public class SupplierRepository : ISupplierRepository
    {
        private new readonly StoreContext _context;

        public SupplierRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<SupplierDto>> GetAllActiveSupplier()
        {
            var supplier = _context.Suppliers.Where(x => x.IsActive == true)
                                             .Select(x => new SupplierDto
                                             {
                                                 Id = x.Id,
                                                 SupplierCode = x.SupplierCode,
                                                 SupplierName = x.SupplierName,

                                                 ModifyBy = x.ModifyBy,
                                                 ModifyDate = x.ModifyDate.ToString(),

                                                 SyncDate = x.SyncDate.ToString(),
                                                 SyncStatus = x.StatusSync,

                                                 DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                 AddedBy = x.AddedBy,
                                                 IsActive = x.IsActive
                                             });

            return await supplier.ToListAsync();

        }

        public async Task<IReadOnlyList<SupplierDto>> GetAllInActiveSupplier()
        {
            var supplier = _context.Suppliers.Where(x => x.IsActive == false)
                                           .Select(x => new SupplierDto
                                           {
                                               Id = x.Id,
                                               SupplierCode = x.SupplierCode,
                                               SupplierName = x.SupplierName,
                                               ModifyBy = x.ModifyBy,
                                               ModifyDate = x.ModifyDate.ToString(),

                                               SyncStatus = x.StatusSync,

                                             
                                               SyncDate = x.SyncDate.ToString(),

                                               DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                               AddedBy = x.AddedBy,
                                               IsActive = x.IsActive
                                           });

            return await supplier.ToListAsync();
        }

        public async Task<bool> AddSupplier(Supplier supplier)
        {
            supplier.Id = 0;

            var addSupplier = new Supplier
            {
                Supplier_No = supplier.Supplier_No,
                SupplierCode = supplier.SupplierCode,   
                SupplierName = supplier.SupplierName,
                AddedBy = supplier.AddedBy,
                DateAdded = supplier.DateAdded,
                ModifyBy = supplier.AddedBy,
                SyncDate = DateTime.Now,
                StatusSync = "New Added"
                

            };


            await _context.Suppliers.AddAsync(addSupplier);

            return true;
        }

        public async Task<PagedList<SupplierDto>> GetAllSupplierWithPagination(bool status, UserParams userParams)
        {
            var supplier = _context.Suppliers.Where(x => x.IsActive == status)
                                             .OrderByDescending(x => x.DateAdded)
                                         .Select(x => new SupplierDto
                                         {
                                             Id = x.Id,
                                             SupplierCode = x.SupplierCode,
                                             SupplierName = x.SupplierName,

                                             SyncDate = x.SyncDate.ToString(),
                                             SyncStatus = x.StatusSync,

                                             ModifyBy = x.ModifyBy,
                                             ModifyDate = x.ModifyDate.ToString(),
                                             DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                             AddedBy = x.AddedBy,
                                             IsActive = x.IsActive
                                         });

            return await PagedList<SupplierDto>.CreateAsync(supplier, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<SupplierDto>> GetSupplierWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var supplier = _context.Suppliers.Where(x => x.IsActive == status)
                                             .OrderByDescending(x => x.DateAdded)
                                      .Select(x => new SupplierDto
                                      {
                                          Id = x.Id,
                                          SupplierCode = x.SupplierCode,
                                          SupplierName = x.SupplierName,


                                          SyncStatus = x.StatusSync,
                                          SyncDate = x.SyncDate.ToString(),

                                          ModifyBy = x.ModifyBy,
                                          ModifyDate = x.ModifyDate.ToString(),
                                          DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                          AddedBy = x.AddedBy,
                                          IsActive = x.IsActive

                                      }).Where(x => x.SupplierName.ToLower().Contains(search.Trim().ToLower())
                                      || x.SupplierCode.ToLower().Contains(search.Trim().ToLower()));

            return await PagedList<SupplierDto>.CreateAsync(supplier, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SupplierCodeExist(string supplier)
        {
            return await _context.Suppliers.AnyAsync(x => x.SupplierCode == supplier);
        }

        public async  Task<bool> ValidationDescritandAddress(Supplier supplier)
        {
            var valid = await _context.Suppliers.Where(x => x.SupplierName == supplier.SupplierName)
                                                .Where(x => x.SupplierAddress == supplier.SupplierAddress)
                                                .FirstOrDefaultAsync();

            if(valid == null)
            {
                return false;
            }
            return true;
                                                
        }

        public async Task<Supplier> GetBySupplierNo(int ? supplierNo)
        {
            return await _context.Suppliers.FirstOrDefaultAsync(x => x.Supplier_No == supplierNo);
        }

        public  async Task<Supplier> GetById(int id)
        {
            return await _context.Suppliers.FindAsync(id);
        }

        public async Task Update(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            await Task.CompletedTask;
        }

        public async Task<bool> UpdateManualSupplier(UpdateManualSupplierDto supplier)
        {
           var existingSupplier  = await _context.Suppliers.FirstOrDefaultAsync(x => x.Id == supplier.Id);
            if(existingSupplier == null)
            {
                return false;
            }

            existingSupplier.SupplierName = supplier.SupplierName;
            existingSupplier.SupplierCode = supplier.SupplierCode;
            existingSupplier.ModifyBy = supplier.ModifiedBy;
            

            return true;
            
        }
    }
}
