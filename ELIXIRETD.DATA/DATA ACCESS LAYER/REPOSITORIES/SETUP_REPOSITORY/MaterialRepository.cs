using ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.EntityFrameworkCore;
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
                                            .Select(x => new MaterialDto
                                            {
                                                Id = x.Id,
                                                ItemCode = x.ItemCode,
                                                ItemCategoryId = x.SubCategory.ItemCategoryId,
                                                ItemCategoryName = x.SubCategory.ItemCategory.ItemCategoryName,
                                                ItemDescription = x.ItemDescription,
                                                SubCategoryId = x.SubCategoryId,
                                                SubCategoryName = x.SubCategory.SubCategoryName,
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
                                                 ItemCategoryId = x.SubCategory.ItemCategoryId,
                                                 ItemCategoryName = x.SubCategory.ItemCategory.ItemCategoryName,
                                                 ItemDescription = x.ItemDescription,
                                                 SubCategoryId = x.SubCategoryId,
                                                 SubCategoryName = x.SubCategory.SubCategoryName,
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

            // Check if the updated Uom already exists in the database
            //var materialExists = await ValidateMaterialExist(materials.ItemDescription) && existingMaterial.ItemDescription != materials.ItemDescription;

            //if (materialExists)
            //    return false;

            existingMaterial.ItemDescription = materials.ItemDescription;
            existingMaterial.SubCategoryId = materials.SubCategoryId;
            existingMaterial.UomId = materials.UomId;
            existingMaterial.BufferLevel = materials.BufferLevel;

            return true;
        }

        public async Task<bool> ActivateMaterial(Material materials)
        {
            //var existingMaterial = await _context.Materials.Where(x => x.SubCategoryId == materials.Id)
            //                                               .FirstOrDefaultAsync();

            //var existingSubcategory = await _context.SubCategories.Where(x => x.Id == materials.Id)
            //                                                      .FirstOrDefaultAsync();

            //var existingUom = await _context.Uoms.Where(x => x.)

            //existingSubcategory.IsActive = true;
            //existingMaterial.IsActive = true;

            //return true;

            var material = await _context.Materials.Where(x => x.Id == materials.Id)
                                          //.Where(x => x.SubCategoryId == materials.SubCategoryId)
                                          //.Where(x => x.UomId == materials.UomId)
                                          .FirstOrDefaultAsync();

            //if (material == null)
            //{
            //    return false;
            //}
            material.IsActive = true;

            //var subcategory = await _context.SubCategories.Where(x => x.Id == material.SubCategoryId)
            //                                               .FirstOrDefaultAsync();

            //subcategory.IsActive = true;

            //var uom = await _context.Uoms.Where(x => x.Id == material.UomId)
            //                                              .FirstOrDefaultAsync();

            //uom.IsActive = true;

            return true;


        }

        public async Task<bool> InActiveMaterial(Material materials)
        {
            var existingMaterial = await _context.Materials.Where(x => x.Id == materials.Id)
                                                           .FirstOrDefaultAsync();

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
                                                  ItemCategoryId = x.SubCategory.ItemCategoryId,
                                                  ItemCategoryName = x.SubCategory.ItemCategory.ItemCategoryName,
                                                  ItemDescription = x.ItemDescription,
                                                  SubCategoryId = x.SubCategoryId,
                                                  SubCategoryName = x.SubCategory.SubCategoryName,
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
                                                ItemCategoryId = x.SubCategory.ItemCategoryId,
                                                ItemCategoryName = x.SubCategory.ItemCategory.ItemCategoryName,
                                                ItemDescription = x.ItemDescription,
                                                SubCategoryId = x.SubCategoryId,
                                                SubCategoryName = x.SubCategory.SubCategoryName,
                                                BufferLevel = x.BufferLevel,
                                                Uom = x.Uom.UomCode,
                                                UomId = x.UomId,
                                                DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                AddedBy = x.AddedBy,
                                                IsActive = x.IsActive

                                            }).Where(x => x.ItemCode.ToLower().Contains(search.Trim().ToLower())
                                             || x.ItemDescription.ToLower().Contains(search.Trim().ToLower())
                                             || x.ItemCategoryName.ToLower().Contains(search.Trim().ToLower())
                                             || x.SubCategoryName.ToLower().Contains(search.Trim().ToLower())
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


        public async Task<IReadOnlyList<SubCategoryDto>> GetAllActiveSubCategory()
        {
            var category = _context.SubCategories.Where(x => x.IsActive == true)
                                                 .Select(x => new SubCategoryDto
                                                 {
                                                     Id = x.Id,
                                                     SubcategoryName = x.SubCategoryName,
                                                     ItemCategoryId = x.ItemCategoryId,
                                                     ItemCategoryName = x.ItemCategory.ItemCategoryName,
                                                     AddedBy = x.AddedBy,
                                                     DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                     IsActive = x.IsActive

                                                 });

            return await category.ToListAsync();
        }

        public async Task<IReadOnlyList<SubCategoryDto>> GetInActiveSubCategory()
        {
            var category = _context.SubCategories.Where(x => x.IsActive == false)
                                                 .Select(x => new SubCategoryDto
                                                 {
                                                     Id = x.Id,
                                                     SubcategoryName = x.SubCategoryName,
                                                     ItemCategoryId = x.ItemCategoryId,
                                                     ItemCategoryName = x.ItemCategory.ItemCategoryName,
                                                     AddedBy = x.AddedBy,
                                                     DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                     IsActive = x.IsActive
                                                 });

            return await category.ToListAsync();

        }

        public async Task<bool> AddNewSubCategory(SubCategory category)
        {
            await _context.SubCategories.AddAsync(category);
            return true;
        }


        public async Task<bool> UpdateSubCategory(SubCategory category)
        {
            var update = await _context.SubCategories.Where(x => x.Id == category.Id)
                                                     .FirstOrDefaultAsync();

            if (update == null)
                return false;

            update.SubCategoryName = category.SubCategoryName;
            update.ItemCategoryId = category.ItemCategoryId;

            return true;

        }


        public async Task<bool> ActivateSubCategory(SubCategory category)
        {
            var update = await _context.SubCategories.Where(x => x.Id == category.Id)
                                                     //.Where(x => x.ItemCategoryId == category.ItemCategoryId)
                                                     .FirstOrDefaultAsync();


            //var updateItemCateg = await _context.ItemCategories.Where(x => x.Id == category.ItemCategoryId)
            //                                                   .FirstOrDefaultAsync();

            if (update == null)
                return false;

            //updateItemCateg.IsActive = category.IsActive = true;

            update.IsActive = category.IsActive = true;

            return true;
        }


        public async Task<bool> InActiveSubCategory(SubCategory category)
        {

            var update = await _context.SubCategories.Where(x => x.Id == category.Id)
                                                  .FirstOrDefaultAsync();

            if (update == null)
                return false;

            update.IsActive = false;

            return true;
        }


        public async Task<PagedList<SubCategoryDto>> GetAllSubCategoryPagination(bool status, UserParams userParams)
        {
            var categories = _context.SubCategories.Where(x => x.IsActive == status)
                                                   .OrderByDescending(x => x.DateAdded)
                                                    .Select(x => new SubCategoryDto
                                                    {
                                                        Id = x.Id,
                                                        SubcategoryName = x.SubCategoryName,
                                                        ItemCategoryId = x.ItemCategoryId,
                                                        ItemCategoryName = x.ItemCategory.ItemCategoryName,
                                                        AddedBy = x.AddedBy,
                                                        DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                        IsActive = x.IsActive
                                                    });

            return await PagedList<SubCategoryDto>.CreateAsync(categories, userParams.PageNumber, userParams.PageSize);


        }




        public async Task<PagedList<SubCategoryDto>> GetSubCategoryPaginationOrig(UserParams userParams, bool status, string search)
        {
            var categories = _context.SubCategories.Where(x => x.IsActive == status)
                                                    .OrderByDescending(x => x.DateAdded)
                                                     .Select(x => new SubCategoryDto
                                                     {
                                                         Id = x.Id,
                                                         SubcategoryName = x.SubCategoryName,
                                                         ItemCategoryId = x.ItemCategoryId,
                                                         ItemCategoryName = x.ItemCategory.ItemCategoryName,
                                                         AddedBy = x.AddedBy,
                                                         DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                         IsActive = x.IsActive

                                                     }).Where(x => x.SubcategoryName.ToLower().Contains(search.Trim().ToLower())
                                                      || x.ItemCategoryName.ToLower().Contains(search.Trim().ToLower()));

            return await PagedList<SubCategoryDto>.CreateAsync(categories, userParams.PageNumber, userParams.PageSize);

        }



        //-----------VALIDATION----------

        public async Task<bool> ValidateItemCategory(int ItemCateg)
        {
            var valid = await _context.ItemCategories.FindAsync(ItemCateg);

            if (valid == null)

                return false;
            return true;
        }


        public async Task<bool> ValidationSubCategory(int Subcategory)
        {
            var valid = await _context.SubCategories.FindAsync(Subcategory);

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
                                                //.Where(x => x.ItemCategoryId == materials.ItemCategoryId)
                                                .FirstOrDefaultAsync();

            if (valid == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> ExistingSubCateg(string subcateg)
        {
            return await _context.SubCategories.AnyAsync(x => x.SubCategoryName == subcateg);
        }

        public async Task<bool> ExistSubCategory(SubCategory category)
        {
            var exist = await _context.SubCategories.Where(x => x.SubCategoryName == category.SubCategoryName)
                                                    .FirstOrDefaultAsync();
            if (exist == null)
                return false;
            return true;

        }


        public async Task<bool> ValidateSubcategInUse(int subcateg)
        {
            return await _context.Materials.AnyAsync(x => x.SubCategoryId == subcateg && x.IsActive == true);

        }

        public async Task<bool> ExistSubCategoryId(int subCategoryId)
        {
            var validate = await _context.SubCategories.FindAsync(subCategoryId);

            if (validate == null)
                return false;

            return true;

        }

        public async Task<bool> DuplicateSubCategoryAndItemCategories(SubCategory category)
        {
            var validate = await _context.SubCategories.Where(x => x.SubCategoryName == category.SubCategoryName)
                                                        .Where(x => x.ItemCategoryId == category.ItemCategoryId)
                                                        .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;

        }

        public async Task<bool> ValidateItemCategInUse(int ItemCateg)
        {
            return await _context.SubCategories.AnyAsync(x => x.ItemCategoryId == ItemCateg && x.IsActive == true);
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
            var items = _context.SubCategories.Where(x => x.IsActive == true)
                                       .Where(x => x.ItemCategory.ItemCategoryName == category)
                                       .Select(x => new DtoItemcategDropdown
                                       {
                                           ItemCategoryId = x.ItemCategoryId,
                                           SubcategoryId = x.Id,
                                           SubcategoryName = x.SubCategoryName,

                                       });

            return await items.ToListAsync();

        }


        public async Task<IReadOnlyList<DtoItemcategDropdown>> GetallActiveSubcategoryDropDown()
        {
            var itemcategory = _context.SubCategories.Where(x => x.IsActive == true)
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

        public async Task<bool> ValidateSubCategorySame(SubCategory category)
        {
            var subcategory = await _context.SubCategories.Where(x => x.Id == category.Id && x.SubCategoryName == category.SubCategoryName
                                                           && x.ItemCategoryId == category.ItemCategoryId)
                                                          .FirstOrDefaultAsync();

            if (subcategory == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateMaterialAndSubAndItem(string material, int Subcateg)
        {
            return await _context.Materials.AnyAsync(x => x.ItemDescription == material && x.SubCategoryId == Subcateg);
        }



        public async Task<IReadOnlyList<DtoItemcategDropdown>> GetAllSubCategmaterial()
        {
            var items = _context.SubCategories.Where(x => x.IsActive == true)

                                     .Select(x => new DtoItemcategDropdown
                                     {
                                         ItemCategoryId = x.ItemCategoryId,
                                         SubcategoryId = x.Id,
                                         SubcategoryName = x.SubCategoryName,
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
                ItemCategoryId = x.SubCategory.ItemCategoryId,
                ItemCategoryName = x.SubCategory.ItemCategory.ItemCategoryName,
                ItemDescription = x.ItemDescription,
                SubCategoryId = x.SubCategoryId,
                SubCategoryName = x.SubCategory.SubCategoryName,
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
    }
 
}
