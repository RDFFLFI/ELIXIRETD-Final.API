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
        Task<bool> AddLotName(LotSection lotname);
        Task<bool> UpdateLotName(LotSection lotname);
        Task<bool> InActiveLotName(LotSection lotname);
        Task<bool> ActivateLotName(LotSection lotname);
        Task<PagedList<LotNameDto>> GetAllLotNameWithPagination(bool status, UserParams userParams);
        Task<PagedList<LotNameDto>> GetLotNameWithPaginationOrig(UserParams userParams, bool status, string search);


        Task<bool> ValidateLotCategoryId(int id);
        Task<bool> SectionNameExist(string section);
        Task<bool> ValidateLotNameAndSection(LotSection lot);
        Task<bool> LotCategoryNameExist(string name);




        //--------LOT cATEGORY---------//
        Task<IReadOnlyList<LotCategoryDto>> GetAllActiveLotCategories();
        Task<IReadOnlyList<LotCategoryDto>> GetAllInActiveLotCategories();
        Task<bool> AddLotCategory(LotNames lotname);
        Task<bool> UpdateLotCategory(LotNames lotname);
        Task<bool> InActiveLotCategory(LotNames lotname);
        Task<bool> ActivateLotCategory(LotNames lotname);
        Task<PagedList<LotCategoryDto>> GetAllLotCategoryWithPagination(bool status, UserParams userParams);
        Task<PagedList<LotCategoryDto>> GetLotCategoryWithPaginationOrig(UserParams userParams, bool status, string search);


        Task<bool> ValidateLotCode(string lotcode);


        Task<bool> ValidateLotInUse(int id);

        Task<bool> ValidateLotNameSame(LotNames lotname);

        Task<bool> ValidateLotSectionSame(LotSection section);



    }
}
