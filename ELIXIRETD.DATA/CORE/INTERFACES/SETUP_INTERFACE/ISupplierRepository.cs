using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using Microsoft.EntityFrameworkCore.Storage;

namespace ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE
{
    public interface ISupplierRepository
    {
        Task<IReadOnlyList<SupplierDto>> GetAllActiveSupplier();
        Task<IReadOnlyList<SupplierDto>> GetAllInActiveSupplier();
        Task<bool> AddSupplier(Supplier supplier);
        Task<PagedList<SupplierDto>> GetAllSupplierWithPagination(bool status, UserParams userParams);
        Task<PagedList<SupplierDto>> GetSupplierWithPaginationOrig(UserParams userParams, bool status, string search);
        Task<bool> ValidationDescritandAddress(Supplier supplier);

        Task<bool> SupplierCodeExist(string supplier);

        Task<bool> UpdateManualSupplier(UpdateManualSupplierDto supplier);


        // validation

        Task<Supplier> GetBySupplierNo(int ? supplierNo);
        Task<Supplier> GetById(int id);
        Task Update(Supplier supplier);


      
    }
}
