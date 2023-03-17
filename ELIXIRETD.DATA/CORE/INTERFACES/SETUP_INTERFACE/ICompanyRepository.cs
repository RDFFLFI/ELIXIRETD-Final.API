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
    public interface ICompanyRepository
    {
        Task<IReadOnlyList<CompanyDto>> GetAllActiveCompany();
        Task<IReadOnlyList<CompanyDto>> GetAllInActiveCompany();
        Task<bool> AddCompany(Company company);
        Task<bool> UpdateCompany(Company company);
        Task<bool> InActiveCompany(Company company);
        Task<bool> ActivateCompany(Company company);
        Task<PagedList<CompanyDto>> GetAllCompanyWithPagination(bool status, UserParams userParams);
        Task<PagedList<CompanyDto>> GetCompanyWithPaginationOrig(UserParams userParams, bool status, string search);


        Task<bool> CompanyCodeExist(string company);

    }
}
