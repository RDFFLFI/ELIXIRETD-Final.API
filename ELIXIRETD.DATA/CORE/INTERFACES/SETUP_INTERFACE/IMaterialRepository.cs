using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE
{
    public interface IMaterialRepository
    {
        Task<IReadOnlyList<MaterialDto>> GetAllActiveMaterials();
        Task<IReadOnlyList<MaterialDto>> GetAllInActiveMaterials();
        Task<bool> AddMaterial(Material materials);
        Task<bool> UpdateMaterial(Material materials);
        Task<bool> InActiveMaterial(Material materials);
        Task<bool> ActivateMaterial(Material materials);
        Task<PagedList<MaterialDto>> GetAllMaterialWithPagination(bool status, UserParams userParams);
        Task<PagedList<MaterialDto>> GetMaterialWithPaginationOrig(UserParams userParams, bool status, string search);

        Task<bool> ValidateItemCategoryId(int id);
        Task<bool> ValidateUOMId(int id);
        Task<bool> ItemCodeExist(string itemcode);
        Task<bool> ItemCategoryExist(string category);
        Task<bool> ValidateDescritionAndUom(Material materials);
        Task<bool> ExistingSubCateg(string subcateg);
        Task<bool> ExistingItemAndSubCateg(Material materials);
        Task<bool> ValidateSubcategAndcategor(int category);
        Task<bool> ValidateSubCategand(int category);



        Task<bool> ExistSubCategoryAndItemCateg(SubCategory category);
        Task<bool> ValidateItemCategory(int ItemCateg);
        Task<bool> ValidationSubCategory(int Subcategory);
        Task<IReadOnlyList<ItemCategoryDto>> GetAllActiveItemCategory();
        Task<IReadOnlyList<ItemCategoryDto>> GetAllInActiveItemCategory();
        Task<bool> AddNewItemCategory(ItemCategory category);
        Task<bool> UpdateItemCategory(ItemCategory category);
        Task<bool> InActiveItemCategory(ItemCategory category);
        Task<bool> ActivateItemCategory(ItemCategory category);
        Task<PagedList<ItemCategoryDto>> GetAllItemCategoryWithPagination(bool status, UserParams userParams);
        Task<PagedList<ItemCategoryDto>> GetItemCategoryWithPaginationOrig(UserParams userParams, bool status, string search);
       



       //====================================================== Sub Category =========================================================

        Task<IReadOnlyList<SubCategoryDto>> GetAllActiveSubCategory();
        Task<IReadOnlyList<SubCategoryDto>> GetInActiveSubCategory();
        Task<bool> AddNewSubCategory(SubCategory category);
        Task<bool> UpdateSubCategory(SubCategory category);
        Task<bool> ActivateSubCategory(SubCategory category);
        Task<bool> InActiveSubCategory(SubCategory category);
        Task<PagedList<SubCategoryDto>> GetAllSubCategoryPagination(bool status, UserParams userParams);
        Task<PagedList<SubCategoryDto>> GetSubCategoryPaginationOrig(UserParams userParams, bool status, string search);
        


        Task<IReadOnlyList<SubCategoryDto>> GetAllListofSubcategorymaterial(string category);

        Task<IReadOnlyList<SubCategoryDto>> GetallActiveSubcategoryDropDown();















    }
}
