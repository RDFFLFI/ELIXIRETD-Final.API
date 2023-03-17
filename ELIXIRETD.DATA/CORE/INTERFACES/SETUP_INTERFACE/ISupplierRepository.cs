using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE
{
    public interface ISupplierRepository
    {
        Task<IReadOnlyList<SupplierDto>> GetAllActiveSupplier();
        Task<IReadOnlyList<SupplierDto>> GetAllInActiveSupplier();
        Task<bool> AddSupplier(Supplier materials);
        Task<bool> UpdateSupplier(Supplier materials);
        Task<bool> InActiveSupplier(Supplier materials);
        Task<bool> ActivateSupplier(Supplier materials);
        Task<PagedList<SupplierDto>> GetAllSupplierWithPagination(bool status, UserParams userParams);
        Task<PagedList<SupplierDto>> GetSupplierWithPaginationOrig(UserParams userParams, bool status, string search);
        Task <bool> ValidationDescritandAddress ( Supplier supplier);

        Task<bool> SupplierCodeExist(string supplier);


    }
}
