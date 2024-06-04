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
                                                ItemCategoryId = x.ItemCategoryId,
                                                ItemCategoryName = x.ItemCategory.ItemCategoryName,
                                                ItemDescription = x.ItemDescription,
                                                //AccountTitleId = x.AccountTitleId,
                                                //AccountPName = x.AccountTitle.AccountPName,
                                                LotSectionId = x.LotSectionId,
                                                SectionName  = x.LotSection.SectionName,
                                                
                                                BufferLevel = x.BufferLevel,
                                                Uom = x.Uom.UomCode,
                                                UomId = x.UomId,
                                                DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                AddedBy = x.AddedBy,
                                                IsActive = x.IsActive,
                                                ModifyBy = x.ModifyBy,
                                                ModifyDate = x.ModifyDate.ToString(),

                                                SyncDate = x.SyncDate.ToString(),
                                                SyncStatus = x.StatusSync,
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
                                                 ItemCategoryId = x.ItemCategoryId,
                                                 ItemCategoryName = x.ItemCategory.ItemCategoryName,
                                                 ItemDescription = x.ItemDescription,

                                                 LotSectionId = x.LotSectionId,
                                                 SectionName = x.LotSection.SectionName,
                                                 //AccountTitleId = x.AccountTitleId,
                                                 //AccountPName = x.AccountTitle.AccountPName,
                                                 BufferLevel = x.BufferLevel,
                                                 Uom = x.Uom.UomCode,
                                                 UomId = x.UomId,
                                                 DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                 AddedBy = x.AddedBy,
                                                 IsActive = x.IsActive,
                                                 ModifyBy = x.ModifyBy,
                                                 ModifyDate = x.ModifyDate.ToString(),

                                                 SyncDate = x.SyncDate.ToString(),
                                                 SyncStatus = x.StatusSync,

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
            existingMaterial.ItemCategoryId = materials.ItemCategoryId;
            existingMaterial.UomId = materials.UomId;
            existingMaterial.BufferLevel = materials.BufferLevel;
            existingMaterial.LotSectionId = materials.LotSectionId;

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
                                                  ItemCategoryId = x.ItemCategoryId,
                                                  ItemCategoryName = x.ItemCategory.ItemCategoryName,
                                                  ItemDescription = x.ItemDescription,
                                                  //AccountTitleId = x.AccountTitleId,
                                                  //AccountPName = x.AccountTitle.AccountPName,

                                                  LotSectionId = x.LotSectionId,
                                                  SectionName = x.LotSection.SectionName,

                                                  BufferLevel = x.BufferLevel,
                                                  Uom = x.Uom.UomCode,
                                                  UomId = x.UomId,
                                                  DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                  AddedBy = x.AddedBy,
                                                  IsActive = x.IsActive,
                                                  ModifyBy = x.ModifyBy,
                                                  ModifyDate = x.ModifyDate.ToString(),

                                                  SyncDate = x.SyncDate.ToString(),
                                                  SyncStatus = x.StatusSync,

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
                                                ItemCategoryId = x.ItemCategoryId,
                                                ItemCategoryName = x.ItemCategory.ItemCategoryName,
                                                ItemDescription = x.ItemDescription,
                                                //AccountTitleId = x.AccountTitleId,
                                                //AccountPName = x.AccountTitle.AccountPName,

                                                LotSectionId = x.LotSectionId,
                                                SectionName = x.LotSection.SectionName,

                                                BufferLevel = x.BufferLevel,
                                                Uom = x.Uom.UomCode,
                                                UomId = x.UomId,
                                                DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                AddedBy = x.AddedBy,
                                                IsActive = x.IsActive,
                                                ModifyBy = x.ModifyBy,
                                                ModifyDate = x.ModifyDate.ToString(),

                                                SyncDate = x.SyncDate.ToString(),
                                                SyncStatus = x.StatusSync,

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
                                            IsActive = x.IsActive,
                                            ModifyBy = x.ModifyBy,
                                            ModifyDate = x.ModifyDate.ToString(),

                                            SyncDate = x.SyncDate.ToString(),
                                            SyncStatus = x.StatusSync,


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
                                          IsActive = x.IsActive,
                                           ModifyBy = x.ModifyBy,
                                          ModifyDate = x.ModifyDate.ToString(),

                                          SyncDate = x.SyncDate.ToString(),
                                          SyncStatus = x.StatusSync,
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
                                                         IsActive = x.IsActive,
                                                         ModifyBy = x.ModifyBy,
                                                         ModifyDate = x.ModifyDate.ToString(),

                                                         SyncDate = x.SyncDate.ToString(),
                                                         SyncStatus = x.StatusSync,
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
                                                         IsActive = x.IsActive,
                                                         ModifyBy = x.ModifyBy,
                                                         ModifyDate = x.ModifyDate.ToString(),

                                                         SyncDate = x.SyncDate.ToString(),
                                                         SyncStatus = x.StatusSync,

                                                     }).Where(x => x.ItemCategoryName.ToLower().Contains(search.Trim().ToLower()));

            return await PagedList<ItemCategoryDto>.CreateAsync(categories, userParams.PageNumber, userParams.PageSize);
        }

        //==================================================== Sub Category ===================================================


    


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



        //public async Task<bool> ValidateAccountInUse(int subcateg)
        //{
        //    return await _context.Materials.AnyAsync(x => x.AccountTitleId == subcateg && x.IsActive == true);

        //}

        public async Task<bool> ValidateItemCategInUse(int ItemCateg)
        {
            return await _context.Materials.AnyAsync(x => x.ItemCategoryId == ItemCateg && x.IsActive == true);
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



        public async Task<IReadOnlyList<DtoItemcategDropdown>> GetallActiveSubcategoryDropDown()
        {
            var itemcategory = _context.ItemCategories.Where(x => x.IsActive == true)
                                                     .Select(x => new DtoItemcategDropdown
                                                     {
                                                         ItemCategoryId = x.Id,
                                                         ItemCategoryName = x.ItemCategoryName,


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


        public async Task<bool> ValidateMaterialAndAccountAndItem(string material, int ? category)
        {
            
            return await _context.Materials.AnyAsync(x => x.ItemDescription == material  && x.ItemCategoryId == category);
        }



        public async Task<IReadOnlyList<MaterialDto>> GetAllMaterial()
        {

            var materials = _context.Materials.Select(x => new MaterialDto
            {
                Id = x.Id,
                ItemCode = x.ItemCode,
                ItemCategoryId = x.ItemCategoryId,
                ItemCategoryName = x.ItemCategory.ItemCategoryName,
                ItemDescription = x.ItemDescription,
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

        public async Task<bool> ValidateDuplicateImport(string itemCode, string itemdescription, int uom, int ? category)
        {

            var validate = await _context.Materials.Where(x => x.ItemCode == itemCode && x.ItemDescription == itemdescription &&  x.UomId == uom  && x.ItemCategoryId == category)
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



        // Sync ItemCategory Genus  //

        public async Task<ItemCategory> GetbyItemCategoryId(int id)
        {
            return await _context.ItemCategories.FindAsync(id);
        }

        public async Task<ItemCategory> GetByItemCategory(int itemCategoryNo)
        {
            return await _context.ItemCategories.FirstOrDefaultAsync(x => x.ItemCategory_No == itemCategoryNo);
        }

        public async Task UpdateAsyncCategory(ItemCategory itemCategory)
        {
            _context.ItemCategories.Update(itemCategory);
            await Task.CompletedTask;
        }



        // Sync Material Genus //

        public async Task<Material> GetByItemCategoryId(int id)
        {
            return await _context.Materials.FindAsync(id);
        }

        public async Task<Material> GetByMaterial(int ? material_No)
        {
            return await _context.Materials.FirstOrDefaultAsync(x => x.Material_No == material_No);
        }

        public async Task UpdateAsyncMaterial(Material material)
        {
            _context.Materials.Update(material);
            await Task.CompletedTask;
        }

        public async Task<bool> AddSyncMaterial(SyncMaterialDto material)
        {
            var materials = new Material
            {
                Material_No = material.Material_No,
                ItemCode = material.ItemCode,
                ItemDescription = material.ItemDescription,
                UomId = material.UomId,
                ItemCategoryId = material.ItemCategoryId,
                SyncDate = DateTime.Now,
                StatusSync = "New Added",
                BufferLevel = 0,
                AddedBy = material.AddedBy
            };

            await _context.AddAsync(materials);

            return true;
        }

        public async Task<bool> UpdateAsyncBufferLvl(Material material)
        {


            var update = await _context.Materials.Where(x => x.Id == material.Id)
                                                 .FirstOrDefaultAsync();

            if (update == null)
            {
                return false;
            }

            var lotNamesExist = await _context.LotSections
                .FirstOrDefaultAsync(x => x.Id == material.LotSectionId);

            update.BufferLevel = material.BufferLevel;
            update.LotSectionId = material.LotSectionId;    

            return true;
        }
    }
    
 
}
