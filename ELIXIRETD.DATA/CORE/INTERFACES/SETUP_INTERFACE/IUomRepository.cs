using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.USER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE
{
    public interface IUomRepository
    {

        Task<IReadOnlyList<UomDto>> GetAllActiveUoms();
        Task<IReadOnlyList<UomDto>> GetAllInActiveUoms();
        Task<bool> AddNewUom(Uom uoms);
        Task<bool> UpdateUom(Uom uoms);
        Task<bool> InActiveUom(Uom uoms);
        Task<bool> ActivateUom(Uom uoms);
        Task<PagedList<UomDto>> GetAllUomWithPagination(bool status, UserParams userParams);
        Task<PagedList<UomDto>> GetUomWithPaginationOrig(UserParams userParams, bool status, string search);


        Task<bool> UomCodeExist(string uom);
        Task<bool> UomDescriptionExist(string uom);

        Task<bool> ValidateUomInUse(int id);

        Task<bool> validateItemUse(Uom uom);



        Task<IReadOnlyList<UomDto>> GetAllUom();


        Task<Uom> GetByCodeAsync(string uomCode);


        Task<Uom> GetByUomId(int id);

        Task<Uom> GetByUomNo (int UomNo);   

        Task UpdateSyncUom (Uom uom);


    }
}
