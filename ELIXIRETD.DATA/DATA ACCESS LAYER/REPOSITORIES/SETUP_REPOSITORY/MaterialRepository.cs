using ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORY_DTO.MRP;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Security.Cryptography;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.SETUP_REPOSITORY
{
    public class MaterialRepository : IMaterialRepository
    {
        private new readonly StoreContext _context;

        public MaterialRepository(StoreContext context)
        {
            _context = context;
        }


        public async Task<IReadOnlyList<MaterialDto>> GetAllActiveMaterials()
        {
            var materials = _context.Materials.Where(x => x.IsActive == true)
                                              .OrderBy(x => x.ItemCode)
                                            .Select(x => new MaterialDto
                                            {
                                                Id = x.Id,
                                                ItemCode = x.ItemCode,
                                                ItemCategoryId = x.AccountTitle.ItemCategoryId,
                                                ItemCategoryName = x.AccountTitle.ItemCategory.ItemCategoryName,
                                                ItemDescription = x.ItemDescription,
                                                //AccountTitleId = x.AccountTitleId,
                                                //AccountPName = x.AccountTitle.AccountPName,
                                                BufferLevel = x.BufferLevel,
                                                Uom = x.Uom.UomCode,
                                                UomId = x.UomId,
                                                DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                AddedBy = x.AddedBy,
                                                IsActive = x.IsActive
                                            });

            return await materials.ToListAsync();
        }

        public async Task<IReadOnlyList<MaterialDto>> GetAllInActiveMaterials()
        {
            var materials = _context.Materials.Where(x => x.IsActive == false)
                                             .Select(x => new MaterialDto
                                             {
                                                 Id = x.Id,
                                                 ItemCode = x.ItemCode,
                                                 ItemCategoryId = x.AccountTitle.ItemCategoryId,
                                                 ItemCategoryName = x.AccountTitle.ItemCategory.ItemCategoryName,
                                                 ItemDescription = x.ItemDescription,
                                                 //AccountTitleId = x.AccountTitleId,
                                                 //AccountPName = x.AccountTitle.AccountPName,
                                                 BufferLevel = x.BufferLevel,
                                                 Uom = x.Uom.UomCode,
                                                 UomId = x.UomId,
                                                 DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                 AddedBy = x.AddedBy,
                                                 IsActive = x.IsActive

                                             });

            return await materials.ToListAsync();
        }





        public async Task<bool> AddMaterial(Material materials)
        {


            await _context.AddAsync(materials);

            return true;
        }


        public async Task<bool> UpdateMaterial(Material materials)
        {

            var existingMaterial = await _context.Materials.Where(x => x.Id == materials.Id)
                                                           .FirstOrDefaultAsync();



            existingMaterial.ItemDescription = materials.ItemDescription;
            existingMaterial.AccountTitleId = materials.AccountTitleId;
            existingMaterial.UomId = materials.UomId;
            existingMaterial.BufferLevel = materials.BufferLevel;

            return true;
        }

        public async Task<bool> ActivateMaterial(Material materials)
        {


            var material = await _context.Materials.Where(x => x.Id == materials.Id)

                                          .FirstOrDefaultAsync();


            material.IsActive = true;


            return true;


        }

        public async Task<bool> InActiveMaterial(Material materials)
        {
            var existingMaterial = await _context.Materials.Where(x => x.Id == materials.Id)
                                                           .FirstOrDefaultAsync();


            if(existingMaterial == null)
                return false;

            existingMaterial.IsActive = false;

            return true;
        }



        public async Task<PagedList<MaterialDto>> GetAllMaterialWithPagination(bool status, UserParams userParams)
        {
            var materials = _context.Materials.Where(x => x.IsActive == status)
                                              .OrderBy(x => x.ItemCode)
                                              .Select(x => new MaterialDto
                                              {

                                                  Id = x.Id,
                                                  ItemCode = x.ItemCode,
                                                  ItemCategoryId = x.AccountTitle.ItemCategoryId,
                                                  ItemCategoryName = x.AccountTitle.ItemCategory.ItemCategoryName,
                                                  ItemDescription = x.ItemDescription,
                                                  //AccountTitleId = x.AccountTitleId,
                                                  //AccountPName = x.AccountTitle.AccountPName,
                                                  BufferLevel = x.BufferLevel,
                                                  Uom = x.Uom.UomCode,
                                                  UomId = x.UomId,
                                                  DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                  AddedBy = x.AddedBy,
                                                  IsActive = x.IsActive

                                              });

            return await PagedList<MaterialDto>.CreateAsync(materials, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<MaterialDto>> GetMaterialWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var materials = _context.Materials.Where(x => x.IsActive == status)
                                              .OrderBy(x => x.ItemCode)
                                            .Select(x => new MaterialDto
                                            {
                                                Id = x.Id,
                                                ItemCode = x.ItemCode,
                                                ItemCategoryId = x.AccountTitle.ItemCategoryId,
                                                ItemCategoryName = x.AccountTitle.ItemCategory.ItemCategoryName,
                                                ItemDescription = x.ItemDescription,
                                                //AccountTitleId = x.AccountTitleId,
                                                //AccountPName = x.AccountTitle.AccountPName,
                                                BufferLevel = x.BufferLevel,
                                                Uom = x.Uom.UomCode,
                                                UomId = x.UomId,
                                                DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                AddedBy = x.AddedBy,
                                                IsActive = x.IsActive

                                            }).Where(x => x.ItemCode.ToLower().Contains(search.Trim().ToLower())
                                             || x.ItemDescription.ToLower().Contains(search.Trim().ToLower())
                                             || x.ItemCategoryName.ToLower().Contains(search.Trim().ToLower())
                                             || x.Uom.ToLower().Contains(search.Trim().ToLower()));

            return await PagedList<MaterialDto>.CreateAsync(materials, userParams.PageNumber, userParams.PageSize);

        }


        //---------------ITEM CATEGORY---------------


        public async Task<IReadOnlyList<ItemCategoryDto>> GetAllActiveItemCategory()
        {
            var categories = _context.ItemCategories.Where(x => x.IsActive == true)
                                        .Select(x => new ItemCategoryDto
                                        {
                                            Id = x.Id,
                                            ItemCategoryName = x.ItemCategoryName,
                                            AddedBy = x.AddedBy,
                                            DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                            IsActive = x.IsActive


                                        });
            return await categories.ToListAsync();
        }

        public async Task<IReadOnlyList<ItemCategoryDto>> GetAllInActiveItemCategory()
        {
            var categories = _context.ItemCategories.Where(x => x.IsActive == false)
                                      .Select(x => new ItemCategoryDto
                                      {
                                          Id = x.Id,
                                          ItemCategoryName = x.ItemCategoryName,
                                          AddedBy = x.AddedBy,
                                          DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                          IsActive = x.IsActive

                                      });
            return await categories.ToListAsync();
        }


        public async Task<bool> AddNewItemCategory(ItemCategory category)
        {
            await _context.AddAsync(category);

            return true;
        }


        public async Task<bool> UpdateItemCategory(ItemCategory category)
        {
            var existingCategory = await _context.ItemCategories.Where(x => x.Id == category.Id)
                                                                .FirstOrDefaultAsync();
            if (existingCategory == null)
            {
                return false;
            }

            existingCategory.ItemCategoryName = category.ItemCategoryName;

            return true;
        }

        public async Task<bool> InActiveItemCategory(ItemCategory category)
        {
            var existingCategory = await _context.ItemCategories.Where(x => x.Id == category.Id)
                                                        .FirstOrDefaultAsync();

            existingCategory.IsActive = false;

            return true;
        }

        public async Task<bool> ActivateItemCategory(ItemCategory category)
        {
            var existingCategory = await _context.ItemCategories.Where(x => x.Id == category.Id)
                                                          .FirstOrDefaultAsync();

            existingCategory.IsActive = true;

            return true;
        }


        public async Task<PagedList<ItemCategoryDto>> GetAllItemCategoryWithPagination(bool status, UserParams userParams)
        {
            var categories = _context.ItemCategories.Where(x => x.IsActive == status)
                                                    .OrderByDescending(x => x.DateAdded)
                                                     .Select(x => new ItemCategoryDto
                                                     {
                                                         Id = x.Id,
                                                         ItemCategoryName = x.ItemCategoryName,
                                                         AddedBy = x.AddedBy,
                                                         DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                         IsActive = x.IsActive
                                                     });

            return await PagedList<ItemCategoryDto>.CreateAsync(categories, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<ItemCategoryDto>> GetItemCategoryWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var categories = _context.ItemCategories.Where(x => x.IsActive == status)
                                                    .OrderByDescending(x => x.DateAdded)
                                                     .Select(x => new ItemCategoryDto
                                                     {
                                                         Id = x.Id,
                                                         ItemCategoryName = x.ItemCategoryName,
                                                         AddedBy = x.AddedBy,
                                                         DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                         IsActive = x.IsActive

                                                     }).Where(x => x.ItemCategoryName.ToLower().Contains(search.Trim().ToLower()));

            return await PagedList<ItemCategoryDto>.CreateAsync(categories, userParams.PageNumber, userParams.PageSize);
        }

        //==================================================== Sub Category ===================================================


        public async Task<IReadOnlyList<AccountTitlesDto>> GetAllActiveAccountTitles()
        {
            var category = _context.AccountTitles.Where(x => x.IsActive == true)
                                                 .Select(x => new AccountTitlesDto
                                                 {
                                                     Id = x.Id,
                                                     AccountPName = x.AccountPName,
                                                     ItemCategoryId = x.ItemCategoryId,
                                                     ItemCategoryName = x.ItemCategory.ItemCategoryName,
                                                     AddedBy = x.AddedBy,
                                                     DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                     IsActive = x.IsActive

                                                 });

            return await category.ToListAsync();
        }

        public async Task<IReadOnlyList<AccountTitlesDto>> GetInActiveAccountTitles()
        {
            var category = _context.AccountTitles.Where(x => x.IsActive == false)
                                                 .Select(x => new AccountTitlesDto
                                                 {
                                                     Id = x.Id,
                                                     AccountPName = x.AccountPName,
                                                     ItemCategoryId = x.ItemCategoryId,
                                                     ItemCategoryName = x.ItemCategory.ItemCategoryName,
                                                     AddedBy = x.AddedBy,
                                                     DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                     IsActive = x.IsActive
                                                 });

            return await category.ToListAsync();

        }

        public async Task<bool> AddNewAccountTitles(AccountTitle category)
        {
            await _context.AccountTitles.AddAsync(category);
            return true;
        }


        public async Task<bool> UpdateAccountTitles(AccountTitle category)
        {
            var update = await _context.AccountTitles.Where(x => x.Id == category.Id)
                                                     .FirstOrDefaultAsync();

            if (update == null)
                return false;

            update.AccountPName = category.AccountPName;
            update.ItemCategoryId = category.ItemCategoryId;

            return true;

        }


        public async Task<bool> ActivateAccountTitles(AccountTitle category)
        {
            var update = await _context.AccountTitles.Where(x => x.Id == category.Id)
                                                     //.Where(x => x.ItemCategoryId == category.ItemCategoryId)
                                                     .FirstOrDefaultAsync();



            if (update == null)
                return false;


            update.IsActive = category.IsActive = true;

            return true;
        }


        public async Task<bool> InActiveAccountTitles(AccountTitle category)
        {

            var update = await _context.AccountTitles.Where(x => x.Id == category.Id)
                                                  .FirstOrDefaultAsync();

            if (update == null)
                return false;

            update.IsActive = false;

            return true;
        }


        public async Task<PagedList<AccountTitlesDto>> GetAllAccountTitlesPagination(bool status, UserParams userParams)
        {
            var categories = _context.AccountTitles.Where(x => x.IsActive == status)
                                                   .OrderByDescending(x => x.DateAdded)
                                                    .Select(x => new AccountTitlesDto
                                                    {
                                                        Id = x.Id,
                                                        AccountPName = x.AccountPName,
                                                        ItemCategoryId = x.ItemCategoryId,
                                                        ItemCategoryName = x.ItemCategory.ItemCategoryName,
                                                        AddedBy = x.AddedBy,
                                                        DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                        IsActive = x.IsActive
                                                    });

            return await PagedList<AccountTitlesDto>.CreateAsync(categories, userParams.PageNumber, userParams.PageSize);


        }


        public async Task<PagedList<AccountTitlesDto>> GetAllAccountTitlesPaginationOrig(UserParams userParams, bool status, string search){
            var categories = _context.AccountTitles.Where(x => x.IsActive == status)
                                                    .OrderByDescending(x => x.DateAdded)
                                                     .Select(x => new AccountTitlesDto
                                                     {
                                                         Id = x.Id,
                                                         AccountPName = x.AccountPName,
                                                         ItemCategoryId = x.ItemCategoryId,
                                                         ItemCategoryName = x.ItemCategory.ItemCategoryName,
                                                         AddedBy = x.AddedBy,
                                                         DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                         IsActive = x.IsActive

                                                     }).Where(x => x.AccountPName.ToLower().Contains(search.Trim().ToLower())
                                                      || x.ItemCategoryName.ToLower().Contains(search.Trim().ToLower()));

            return await PagedList<AccountTitlesDto>.CreateAsync(categories, userParams.PageNumber, userParams.PageSize);

        }


        //-----------VALIDATION----------

        public async Task<bool> ValidateItemCategory(int ItemCateg)
        {
            var valid = await _context.ItemCategories.FindAsync(ItemCateg);

            if (valid == null)

                return false;
            return true;
        }



        public async Task<bool> ValidateItemCategoryId(int id)
        {
            var validateExisting = await _context.ItemCategories.FindAsync(id);

            if (validateExisting == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateUOMId(int id)
        {
            var validateExisting = await _context.Uoms.FindAsync(id);

            if (validateExisting == null)
                return false;

            return true;
        }

        public async Task<bool> ItemCodeExist(string itemcode)
        {
            return await _context.Materials.AnyAsync(x => x.ItemCode == itemcode);
        }

        public async Task<bool> ItemCategoryExist(string category)
        {
            return await _context.ItemCategories.AnyAsync(x => x.ItemCategoryName == category);
        }

        public async Task<bool> ValidateDescritionAndUom(Material materials)
        {
            var valid = await _context.Materials.Where(x => x.ItemDescription == materials.ItemDescription)
                                                .FirstOrDefaultAsync();

            if (valid == null)
            {
                return false;
            }
            return true;
        }



        public async Task<bool> ValidateAccountInUse(int subcateg)
        {
            return await _context.Materials.AnyAsync(x => x.AccountTitleId == subcateg && x.IsActive == true);

        }


        public async Task<bool> DuplicateAccountTitleAndItemCategories(AccountTitle category)
        {
            var validate = await _context.AccountTitles.Where(x => x.AccountPName == category.AccountPName)
                                                        .Where(x => x.ItemCategoryId == category.ItemCategoryId)
                                                        .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;

        }

        public async Task<bool> ValidateItemCategInUse(int ItemCateg)
        {
            return await _context.AccountTitles.AnyAsync(x => x.ItemCategoryId == ItemCateg && x.IsActive == true);
        }

        public async Task<bool> ExistItemCateg(string itemcateg)
        {
            return await _context.ItemCategories.AnyAsync(x => x.ItemCategoryName == itemcateg && x.IsActive == true);
        }


        public async Task<bool> ExistingMaterialAndItemCode(Material material)
        {
            var validate = await _context.Materials.Where(x => x.ItemDescription == material.ItemDescription)
                                                   .Where(x => x.ItemCode == material.ItemCode)
                                                   .FirstOrDefaultAsync();
            if (validate == null)
                return false;


            return true;
        }

        public async Task<bool> ValidateMaterialSubCategoryInActive(ItemCategory Itemcateg)
        {
            var validate = await _context.ItemCategories.Where(x => x.Id == Itemcateg.Id && x.IsActive == false)
                                                        .FirstOrDefaultAsync();


            if (validate == null)
                return false;

            return true;
        }



        public async Task<IReadOnlyList<DtoItemcategDropdown>> GetAllListofItemMaterial(string category)
        {
            var items = _context.AccountTitles.Where(x => x.IsActive == true)
                                       .Where(x => x.ItemCategory.ItemCategoryName == category)
                                       .Select(x => new DtoItemcategDropdown
                                       {
                                           ItemCategoryId = x.ItemCategoryId,
                                           AccountTitleId= x.Id,
                                           AccountPName = x.AccountPName,

                                       });

            return await items.ToListAsync();

        }


        public async Task<IReadOnlyList<DtoItemcategDropdown>> GetallActiveSubcategoryDropDown()
        {
            var itemcategory = _context.AccountTitles.Where(x => x.IsActive == true)
                                                     .Select(x => new DtoItemcategDropdown
                                                     {
                                                         ItemCategoryId = x.ItemCategoryId,
                                                         ItemCategoryName = x.ItemCategory.ItemCategoryName,


                                                     }).Distinct();

            return await itemcategory.ToListAsync();
        }

        public async Task<bool> ValidateMaterialExist(string materialname)
        {
            return await _context.Materials.AnyAsync(x => x.ItemDescription == materialname);
        }

        public async Task<bool> ValidateItemCategorySame(ItemCategory category)
        {
            var itemcateg = await _context.ItemCategories.Where(x => x.Id == category.Id && x.ItemCategoryName == category.ItemCategoryName)
                                                         .FirstOrDefaultAsync();

            if (itemcateg == null)
                return false;

            return true;
        }


        public async Task<bool> ValidateMaterialAndAccountAndItem(string material)
        {
            
            return await _context.Materials.AnyAsync(x => x.ItemDescription == material );
        }



        public async Task<IReadOnlyList<DtoItemcategDropdown>> GetAllAccountmaterial()
        {
            var items = _context.AccountTitles.Where(x => x.IsActive == true)

                                     .Select(x => new DtoItemcategDropdown
                                     {
                                         ItemCategoryId = x.ItemCategoryId,
                                         AccountTitleId = x.Id,
                                         AccountPName = x.AccountPName,
                                         ItemCategoryName = x.ItemCategory.ItemCategoryName

                                     });

            return await items.ToListAsync();


        }

        public async Task<IReadOnlyList<MaterialDto>> GetAllMaterial()
        {

            var materials = _context.Materials.Select(x => new MaterialDto
            {
                Id = x.Id,
                ItemCode = x.ItemCode,
                ItemCategoryId = x.AccountTitle.ItemCategoryId,
                ItemCategoryName = x.AccountTitle.ItemCategory.ItemCategoryName,
                ItemDescription = x.ItemDescription,
                //AccountTitleId = x.AccountTitleId,
                //AccountPName = x.AccountTitle.AccountPName,
                BufferLevel = x.BufferLevel,
                Uom = x.Uom.UomCode,
                UomId = x.UomId,
                DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                AddedBy = x.AddedBy,
                IsActive = x.IsActive

            });


            return await materials.ToListAsync();
        }

        public async Task<IReadOnlyList<ItemCategoryDto>> GetAllItemCategory()
        {
            var categories = _context.ItemCategories
                                       .Select(x => new ItemCategoryDto
                                       {
                                           Id = x.Id,
                                           ItemCategoryName = x.ItemCategoryName,
                                           AddedBy = x.AddedBy,
                                           DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                           IsActive = x.IsActive


                                       });

            return await categories.ToListAsync();
        }

        public async Task<bool> ValidateDuplicateImport(string itemCode, string itemdescription, int uom)
        {

            var validate = await _context.Materials.Where(x => x.ItemCode == itemCode && x.ItemDescription == itemdescription &&  x.UomId == uom )
                                             .FirstOrDefaultAsync();

            if(validate == null)
            {
                return false;
            }

            return true;
        

        }


        public async Task<bool> AddMaterialImport(Material material)
        {
        
            await _context.Materials.AddAsync(material);

            return true;
        }

        public async Task<AccountTitle> GetByNameAndItemCategoryIdAsync(string AccountPName, int itemCategoryId)
        {
            return await _context.AccountTitles.FirstOrDefaultAsync(x => x.AccountPName == AccountPName && x.ItemCategoryId == itemCategoryId);
        }

        public async Task<ItemCategory> GetByNameAsync(string itemCategoryName)
        {
            return await _context.ItemCategories.FirstOrDefaultAsync(x => x.ItemCategoryName == itemCategoryName);
        }



        public async Task<bool> ValildateItemCodeForPoSummary(string itemCode)
        {


            var validate = await _context.PoSummaries.Where(x => x.ItemCode == itemCode)
                                                     .Where(x => x.IsActive == true)
                                                     .FirstOrDefaultAsync();


            if(validate == null) 
                return false;

            return true;

        }

        public async Task<bool> ValildateItemCodeForReceiving(string itemCode)
        {
            var validate = await _context.WarehouseReceived.Where(x => x.ItemCode == itemCode)
                                                    .Where(x => x.IsActive == true)
                                                    .FirstOrDefaultAsync();


            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> ValildateItemCodeForOrdering(string itemCode)
        {
            var validate = await _context.Orders.Where(x => x.ItemCode == itemCode)
                                                    .Where(x => x.IsActive == true)
                                                    .FirstOrDefaultAsync();


            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> ValildateItemCodeForMiscIssue(string itemCode)
        {
            var validate = await _context.MiscellaneousIssueDetail.Where(x => x.ItemCode == itemCode)
                                                    .Where(x => x.IsActive == true)
                                                    .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> ValildateItemCodeForBorrowedIssue(string itemCode)
        {
            var validate = await _context.BorrowedIssueDetails.Where(x => x.ItemCode == itemCode)
                                                     .Where(x => x.IsActive == true)
                                                     .FirstOrDefaultAsync();


            if (validate == null)
                return false;

            return true;
        }

        public async Task<int> CountMatchingMaterials(int id ,string material)
        {
            return await _context.Materials.CountAsync(x => x.Id == id  && x.ItemDescription == material);

        }

        public async Task<int> CountMatchingMaterialsByItemdescription(string material)
        {
            return await _context.Materials.CountAsync(x => x.ItemDescription == material );
        }





    }
    
 
}
