using ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.SETUP_REPOSITORY
{
    public class CompanyRepository : ICompanyRepository
    {
        private new readonly StoreContext _context;
        public CompanyRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<CompanyDto>> GetAllActiveCompany()
        {
            var company = _context.Companies.Where(x => x.IsActive == true)
                                             .Select(x => new CompanyDto
                                             {
                                                 Id = x.Id, 
                                                 CompanyCode = x.CompanyCode, 
                                                 CompanyName = x.CompanyName, 
                                                 AddedBy = x.AddedBy, 
                                                 DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                 IsActive = x.IsActive
                                             });

            return await company.ToListAsync();
        }

        public async Task<IReadOnlyList<CompanyDto>> GetAllInActiveCompany()
        {
            var company = _context.Companies.Where(x => x.IsActive == false)
                                             .Select(x => new CompanyDto
                                             {
                                                 Id = x.Id,
                                                 CompanyCode = x.CompanyCode,
                                                 CompanyName = x.CompanyName,
                                                 AddedBy = x.AddedBy,
                                                 DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                 IsActive = x.IsActive
                                             });

            return await company.ToListAsync();
        }


        public async Task<bool> AddCompany(Company company)
        {
            await _context.Companies.AddAsync(company);

            return true;

        }


        public async Task<bool> UpdateCompany(Company company)
        {
            var companies = await _context.Companies.Where(x => x.Id == company.Id)
                                                    .FirstOrDefaultAsync();

            companies.CompanyName = company.CompanyName;

            return true;

        }


        public async Task<bool> InActiveCompany(Company company)
        {
            var companies = await _context.Companies.Where(x => x.Id == company.Id)
                                                    .FirstOrDefaultAsync();

            companies.IsActive = company.IsActive = false;

            return true;
        }

      


        public async Task<bool> ActivateCompany(Company company)
        {
            var companies = await _context.Companies.Where(x => x.Id == company.Id)
                                                    .FirstOrDefaultAsync();

            companies.IsActive = company.IsActive = true;

            return true;
        }



        public async Task<PagedList<CompanyDto>> GetAllCompanyWithPagination(bool status, UserParams userParams)
        {
            var companies = _context.Companies.Where(x => x.IsActive == status)
                                              .OrderByDescending(x => x.DateAdded)
                                             .Select(x => new CompanyDto
                                             {
                                                 Id = x.Id,
                                                 CompanyCode = x.CompanyCode,
                                                 CompanyName = x.CompanyName,
                                                 AddedBy = x.AddedBy,
                                                 DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                 IsActive = x.IsActive
                                             });

            return await PagedList<CompanyDto>.CreateAsync(companies, userParams.PageNumber, userParams.PageSize);
        }

       
        public async Task<PagedList<CompanyDto>> GetCompanyWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var companies = _context.Companies.Where(x => x.IsActive == status)
                                              .OrderByDescending(x => x.DateAdded)
                                             .Select(x => new CompanyDto
                                             {
                                                 Id = x.Id,
                                                 CompanyCode = x.CompanyCode,
                                                 CompanyName = x.CompanyName,
                                                 AddedBy = x.AddedBy,
                                                 DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                 IsActive = x.IsActive
                                             }).Where(x => x.CompanyName.ToLower()
                                               .Contains(search.Trim().ToLower()));


            return await PagedList<CompanyDto>.CreateAsync(companies, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> CompanyCodeExist(string company)
        {
            return await _context.Companies.AnyAsync(x => x.CompanyCode == company);
        }

    }
}
