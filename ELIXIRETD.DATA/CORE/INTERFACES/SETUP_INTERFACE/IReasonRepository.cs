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
    public interface IReasonRepository
    {
        Task<IReadOnlyList<ReasonDto>> GetAllActiveReason();
        Task<IReadOnlyList<ReasonDto>> GetAllInActiveReason();
        Task<bool> AddReason(Reason reason);
        Task<bool> UpdateReason(Reason reason);
        Task<bool> InActiveReason(Reason reason);
        Task<bool> ActivateReason(Reason reason);
        Task<PagedList<ReasonDto>> GetAllReasonWithPagination(bool status, UserParams userParams);
        Task<PagedList<ReasonDto>> GetReasonWithPaginationOrig(UserParams userParams, bool status, string search);


        Task<bool> ValidateModuleId(int id);
        Task<bool> ValidateReasonEntry(Reason reason);

        Task<bool> validateReasonSame(Reason reason);
    }
}
