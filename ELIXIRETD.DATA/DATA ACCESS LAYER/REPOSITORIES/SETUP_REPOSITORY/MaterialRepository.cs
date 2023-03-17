using ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
                                                ItemDescription = x.ItemDescription,
                                                SubCategoryId = x.SubCategoryId,
                                                SubCategoryName = x.SubCategory.SubCategoryName,
                                                ItemCategoryId = x.SubCategory.ItemCategoryId,
                                                ItemCategoryName = x.SubCategory.ItemCategory.ItemCategoryName,
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
                                                 ItemDescription = x.ItemDescription,
                                                 SubCategoryId = x.SubCategoryId,
                                                 SubCategoryName = x.SubCategory.SubCategoryName,
                                                 ItemCategoryId = x.SubCategory.ItemCategoryId,
                                                 ItemCategoryName = x.SubCategory.ItemCategory.ItemCategoryName,
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
            var exisitngMaterial = await _context.Materials.Where(x => x.Id == materials.Id)
                                                           .FirstOrDefaultAsync();

            exisitngMaterial.ItemDescription = materials.ItemDescription;
            exisitngMaterial.SubCategoryId = materials.SubCategoryId;
            exisitngMaterial.SubCategory.ItemCategoryId = materials.SubCategory.ItemCategoryId;
            exisitngMaterial.UomId = materials.UomId;
            exisitngMaterial.BufferLevel = materials.BufferLevel;

            return true;

        }

        public async Task<bool> ActivateMaterial(Material materials)
        {
            var existingMaterial = await _context.Materials.Where(x => x.Id == materials.Id)
                                                           .FirstOrDefaultAsync();

            existingMaterial.IsActive = true;

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
                                                  ItemDescription = x.ItemDescription,
                                                  SubCategoryId = x.SubCategoryId,
                                                  SubCategoryName = x.SubCategory.SubCategoryName,
                                                  ItemCategoryId = x.SubCategory.ItemCategoryId,
                                                  ItemCategoryName = x.SubCategory.ItemCategory.ItemCategoryName,
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
                                                ItemDescription = x.ItemDescription,
                                                SubCategoryId = x.SubCategoryId,
                                                SubCategoryName = x.SubCategory.SubCategoryName,
                                                ItemCategoryId = x.SubCategory.ItemCategoryId,
                                                ItemCategoryName = x.SubCategory.ItemCategory.ItemCategoryName,
                                                BufferLevel = x.BufferLevel,
                                                Uom = x.Uom.UomCode,
                                                UomId = x.UomId,
                                                DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                AddedBy = x.AddedBy,
                                                IsActive = x.IsActive
                                            }).Where(x => x.ItemCode.ToLower()
                                              .Contains(search.Trim().ToLower()));
            
            return await PagedList<MaterialDto>.CreateAsync(materials, userParams.PageNumber, userParams.PageSize);

        }



        //---------------ITEM CATEGORY---------------



        public async Task<IReadOnlyList<ItemCategoryDto>> GetAllActiveItemCategory()
        {
            var categories =  _context.ItemCategories.Where(x => x.IsActive == true)
                                        .Select(x => new ItemCategoryDto
                                        {
                                              Id = x.Id,
                                              ItemCategoryName = x.ItemCategoryName,
                                              AddedBy = x.AddedBy,
                                              DateAdded = x.DateAdded.ToString("MM/dd/yyyy")

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
                                          DateAdded = x.DateAdded.ToString("MM/dd/yyyy")

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
                                                     }).Where(x => x.ItemCategoryName.ToLower()
                                                       .Contains(search.Trim().ToLower()));

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
                                                     CategoryName = x.ItemCategory.ItemCategoryName,
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
                                                     CategoryName = x.ItemCategory.ItemCategoryName,
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
                                                   .FirstOrDefaultAsync();

            if (update == null)
                return false;

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
                                                        CategoryName = x.ItemCategory.ItemCategoryName,
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
                                                         CategoryName = x.ItemCategory.ItemCategoryName,
                                                         AddedBy = x.AddedBy,
                                                         DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                         IsActive = x.IsActive
                                                     }).Where(x => x.SubcategoryName.ToLower()
                                                       .Contains(search.Trim().ToLower()));

            return await PagedList<SubCategoryDto>.CreateAsync(categories, userParams.PageNumber, userParams.PageSize);

        }


        //-----------VALIDATION----------

        public async Task<bool> ValidateItemCategory(int ItemCateg)
        {
            var valid = await _context.ItemCategories.FindAsync(ItemCateg);

            if(valid == null)

                return false;
            return true;
        }


        public async Task<bool> ValidationSubCategory(int Subcategory)
        {
            var valid = await _context.SubCategories.FindAsync(Subcategory);

            if(valid == null)
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
                                                .Where(x => x.UomId == materials.UomId)
                                                .FirstOrDefaultAsync();

            if(valid == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> ExistingSubCateg(string subcateg)
        {
            return await _context.SubCategories.AnyAsync(x => x.SubCategoryName == subcateg);
        }

        public async Task<bool> ExistSubCategoryAndItemCateg(SubCategory category)
        {
            var exist = await _context.SubCategories.Where(x => x.SubCategoryName == category.SubCategoryName)
                                                    .Where(x => x.ItemCategoryId == category.ItemCategoryId)
                                                    .FirstOrDefaultAsync();
            if (exist == null)
                return false;
            return true;
                                                                                           
        }

        public async Task<bool> ExistingItemAndSubCateg(Material materials)
        {
            var valid = await _context.Materials.Where(x => x.ItemCode == materials.ItemCode)
                                                .Where(x => x.SubCategoryId == materials.SubCategoryId)
                                                .FirstOrDefaultAsync();

            if (valid == null)
                return false;
            return true;
        }

        public async Task<bool> ValidateSubcategAndcategor(int category)
        {
            var valid = await _context.SubCategories.Where(x => x.ItemCategoryId == category)
                                                     .Where(x => x.IsActive == true)
                                                     .FirstOrDefaultAsync();
            if(valid == null) 
                return false;
            return true;
        }

        public async Task<bool> ValidateSubCategand(int category)
        {
            var validsub = await _context.Materials.Where(x => x.SubCategoryId == category)
                                                   .Where(x => x.IsActive == true)
                                                     .FirstOrDefaultAsync();

            if (validsub == null) 
                return false;
            return true;

        }

        public async Task<IReadOnlyList<SubCategoryDto>> GetAllListofSubcategorymaterial(string category)
        {

            var sub = _context.SubCategories.Where(x => x.IsActive == true)
                                            .Where(x => x.SubCategoryName == category)
                                            .Select(x => new SubCategoryDto
                                              {
                                                  SubCategoryId = x.Id,
                                                  SubcategoryName = x.SubCategoryName,
                                                  CategoryName = x.ItemCategory.ItemCategoryName,
                                                  ItemCategoryId = x.ItemCategoryId
                                              });


            return await sub.ToListAsync();
        }

        public async Task<IReadOnlyList<SubCategoryDto>> GetallActiveSubcategoryDropDown()
        {
            var subcategory = _context.SubCategories.Where(x => x.IsActive == true)
                                                     .Select(x => new SubCategoryDto
                                                     {
                                                         SubCategoryId = x.Id,
                                                         SubcategoryName = x.SubCategoryName,
                                                         ItemCategoryId = x.ItemCategoryId,
                                                         CategoryName = x.ItemCategory.ItemCategoryName,
                                                         
                                                     }).Distinct();
                                              
                   return await subcategory.ToListAsync();
        }

    }
}
