using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
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

        Task<bool> ValidateDuplicateImport(string itemCode, string itemdescription, int uom, int? category);

        Task<bool> ValildateItemCodeForPoSummary(string itemCode);
        Task<bool> ValildateItemCodeForReceiving(string itemCode);
        Task<bool> ValildateItemCodeForOrdering(string itemCode);
        Task<bool> ValildateItemCodeForMiscIssue(string itemCode);
        Task<bool> ValildateItemCodeForBorrowedIssue(string itemCode);






        Task<bool> AddMaterialImport(Material material);
        //Task<AccountTitle> GetByNameAndItemCategoryIdAsync(string AccountPName, int itemCategoryId);
        Task<ItemCategory> GetByNameAsync(string itemCategoryName);


        //Task<bool> ExistAccountTitle(AccountTitle category);
        Task<bool> ValidateItemCategory(int ItemCateg);
        //Task<bool> ValidationSubCategory(int Subcategory);
        Task<IReadOnlyList<ItemCategoryDto>> GetAllActiveItemCategory();
        Task<IReadOnlyList<ItemCategoryDto>> GetAllInActiveItemCategory();
        Task<bool> AddNewItemCategory(ItemCategory category);
        Task<bool> UpdateItemCategory(ItemCategory category);
        Task<bool> InActiveItemCategory(ItemCategory category);
        Task<bool> ActivateItemCategory(ItemCategory category);
        Task<PagedList<ItemCategoryDto>> GetAllItemCategoryWithPagination(bool status, UserParams userParams);
        Task<PagedList<ItemCategoryDto>> GetItemCategoryWithPaginationOrig(UserParams userParams, bool status, string search);


        //Task<bool> ExistSubCategoryId(int subCategoryId);
        //Task<bool> DuplicateAccountTitleAndItemCategories(AccountTitle category);
        Task<bool> ValidateItemCategInUse(int ItemCateg);
        Task<bool> ExistItemCateg(string itemcateg);

        Task<bool> ExistingMaterialAndItemCode(Material material);
        Task<bool> ValidateMaterialSubCategoryInActive(ItemCategory Itemcateg);



        //====================================================== Sub Category =========================================================

        //Task<IReadOnlyList<AccountTitlesDto>> GetAllActiveAccountTitles();
        //Task<IReadOnlyList<AccountTitlesDto>> GetInActiveAccountTitles();
        //Task<bool> AddNewAccountTitles(AccountTitle category);
        //Task<bool> UpdateAccountTitles(AccountTitle category);
        //Task<bool> ActivateAccountTitles(AccountTitle category);
        //Task<bool> InActiveAccountTitles(AccountTitle category);

        //Task<PagedList<AccountTitlesDto>> GetAllAccountTitlesPagination(bool status, UserParams userParams);
        //Task<PagedList<AccountTitlesDto>> GetAllAccountTitlesPaginationOrig(UserParams userParams, bool status, string search);



        //Task<IReadOnlyList<DtoItemcategDropdown>> GetAllListofItemMaterial(string category);
        Task<IReadOnlyList<DtoItemcategDropdown>> GetallActiveSubcategoryDropDown();

        Task<bool> ValidateMaterialExist(string materialname);

        Task<bool> ValidateItemCategorySame(ItemCategory category);
        //Task<bool> ValidateSubCategorySame(SubCategory category);



        Task<bool> ValidateMaterialAndAccountAndItem(string material, int? category);

        Task<int> CountMatchingMaterials(int id, string material/*, int account*/);

        Task<int> CountMatchingMaterialsByItemdescription(string material/*, int account*/);


        //Task<IReadOnlyList<DtoItemcategDropdown>> GetAllAccountmaterial();




        Task<IReadOnlyList<MaterialDto>> GetAllMaterial();

        Task<IReadOnlyList<ItemCategoryDto>> GetAllItemCategory();



        // Sync Item Category Genus // 

        Task<ItemCategory> GetbyItemCategoryId(int id);

        Task<ItemCategory> GetByItemCategory(int itemCategoryNo);

        Task UpdateAsyncCategory(ItemCategory itemCategory);


        // Sync Item Category 

        Task<Material> GetByItemCategoryId (int id);

        Task<Material> GetByMaterial(int ? material_No);

        Task UpdateAsyncMaterial(Material material);

        Task<bool> AddSyncMaterial(SyncMaterialDto material);

        Task<bool> UpdateAsyncBufferLvl(Material material);





    }
}
