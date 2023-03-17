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
    public interface ILotRepository
    {

        //--------LOT NAME---------//
        Task<IReadOnlyList<LotNameDto>> GetAllActiveLotName();
        Task<IReadOnlyList<LotNameDto>> GetAllInActiveLotName();
        Task<bool> AddLotName(LotName lotname);
        Task<bool> UpdateLotName(LotName lotname);
        Task<bool> InActiveLotName(LotName lotname);
        Task<bool> ActivateLotName(LotName lotname);
        Task<PagedList<LotNameDto>> GetAllLotNameWithPagination(bool status, UserParams userParams);
        Task<PagedList<LotNameDto>> GetLotNameWithPaginationOrig(UserParams userParams, bool status, string search);


        Task<bool> ValidateLotCategoryId(int id);
        Task<bool> SectionNameExist(string section);
        Task<bool> ValidateLotNameAndSection(LotName lot);
        Task<bool> LotCategoryNameExist(string name);




        //--------LOT cATEGORY---------//
        Task<IReadOnlyList<LotCategoryDto>> GetAllActiveLotCategories();
        Task<IReadOnlyList<LotCategoryDto>> GetAllInActiveLotCategories();
        Task<bool> AddLotCategory(LotCategory lotname);
        Task<bool> UpdateLotCategory(LotCategory lotname);
        Task<bool> InActiveLotCategory(LotCategory lotname);
        Task<bool> ActivateLotCategory(LotCategory lotname);
        Task<PagedList<LotCategoryDto>> GetAllLotCategoryWithPagination(bool status, UserParams userParams);
        Task<PagedList<LotCategoryDto>> GetLotCategoryWithPaginationOrig(UserParams userParams, bool status, string search);





    }
}
