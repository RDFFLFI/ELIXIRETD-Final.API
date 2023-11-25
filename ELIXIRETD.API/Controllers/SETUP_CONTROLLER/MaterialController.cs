﻿using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIRETD.DATA.SERVICES;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
//using System.Data.Entity;
using System.Data.OleDb;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ELIXIRETD.API.Controllers.SETUP_CONTROLLER
{

    public class MaterialController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StoreContext _context;

        public MaterialController(IUnitOfWork unitOfWork , StoreContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [HttpPost]
        [Route("AddNewImportMaterials")]
        public async Task<IActionResult> AddNewImportMaterials([FromBody] Material[] materials)
        {

            if (ModelState.IsValid != true) return new JsonResult("Something went wrong!") { StatusCode = 500 };
            {

                List<Material> DuplicateList = new List<Material>();
                List<Material> AvailableImport = new List<Material>();
                List<Material> ItemcategoryNotExist = new List<Material>();
                List<Material> AccountTitleNotExist = new List<Material>();
                List<Material> UomNotExist = new List<Material>();
                List<Material> ItemCodeNull = new List<Material>();
                List<Material> ItemDescriptionNull = new List<Material>();
                List<Material> ItemCodeAlreadyExist = new List<Material>();
                List<Material> ItemDescriptionAlreadyExist = new List<Material>();

                foreach (Material items in materials)
                {

                    Uom uom = await _unitOfWork.Uoms.GetByCodeAsync(items.UomCode);
                    if (uom == null)
                    {
                        UomNotExist.Add(items);
                        continue;
                    }
                    items.UomId = uom.Id;

                    ItemCategory itemCategory = await _unitOfWork.Materials.GetByNameAsync(items.ItemCategoryName);
                    if (itemCategory == null)
                    {
                        ItemcategoryNotExist.Add(items);
                        continue;
                    }
                   
                    

                    AccountTitle account = await _unitOfWork.Materials.GetByNameAndItemCategoryIdAsync(items.AccountPName, itemCategory.Id);
                    if(account == null)
                    {
                        AccountTitleNotExist.Add(items);
                        continue;
                    }
                    else
                    {
                        items.AccountTitleId = account.Id;
                    }
                  


                    if (materials.Count(x => x.ItemCode == items.ItemCode && x.ItemDescription == items.ItemDescription && x.UomId == items.UomId && x.AccountTitleId == items.AccountTitleId) > 1)
                    {
                        DuplicateList.Add(items);
                        continue;

                    }

                    var validateDuplicate = await _unitOfWork.Materials.ValidateDuplicateImport(items.ItemCode, items.ItemDescription, items.UomId/*, items.AccountTitleId*/);
                    
                    if(await _unitOfWork.Materials.ItemCodeExist(items.ItemCode))
                    {
                        ItemCodeAlreadyExist.Add(items);
                        continue;
                    }
                    if (await _unitOfWork.Materials.ValidateMaterialAndAccountAndItem(items.ItemDescription/*, items.AccountTitleId*/))
                    {
                        ItemDescriptionAlreadyExist.Add(items);
                        continue;
                    }



                        //var Itemcodenull = await _unitOfWork.Materials.AddMaterialImport(items);
                        if (items.ItemCode == string.Empty || items.ItemCode == null)
                      {
                        ItemCodeNull.Add(items);
                        continue;

                      }

                    if (items.ItemDescription == string.Empty || items.ItemDescription == null)
                    {
                        ItemDescriptionNull.Add(items);
                        continue;

                    }


                      if (validateDuplicate == true)
                      {
                        DuplicateList.Add(items);
                      }
                   
                      else
                      {
                        AvailableImport.Add(items);
                        await _unitOfWork.Materials.AddMaterialImport(items);
                      }

                }

                var resultList = new
                {
                    AvailableImport,
                    DuplicateList,
                    ItemcategoryNotExist,
                    AccountTitleNotExist,
                    UomNotExist,
                    ItemCodeNull,
                    ItemDescriptionNull,
                    ItemCodeAlreadyExist,
                    ItemDescriptionAlreadyExist
                    
                 
                };

                if (DuplicateList.Count == 0 && AccountTitleNotExist.Count == 0 && ItemcategoryNotExist.Count == 0 && UomNotExist.Count == 0 && ItemCodeNull.Count == 0 && ItemDescriptionNull.Count == 0
                    && ItemCodeAlreadyExist.Count == 0 && ItemDescriptionAlreadyExist.Count == 0)
                {
                    await _unitOfWork.CompleteAsync();
                    return Ok("Successfully Add!");
                }
                else
                {
                    return BadRequest(resultList);
                }

            }
        }



        [HttpGet]
        [Route("GetAllActiveMaterials")]
        public async Task<IActionResult> GetAllActiveMaterials()
        {
            var materials = await _unitOfWork.Materials.GetAllActiveMaterials();
            return Ok(materials);
        }


        [HttpGet]
        [Route("GetAllInActiveMaterials")]
        public async Task<IActionResult> GetAllInActiveMaterials()
        {



            var materials = await _unitOfWork.Materials.GetAllInActiveMaterials();

            return Ok(materials);
        }


        [HttpPost]
        [Route("AddNewMaterial")]
        public async Task<IActionResult> AddNewMaterial(Material material)
        {
   
                var uomId = await _unitOfWork.Materials.ValidateUOMId(material.UomId);
              
            var existingMaterialsAndItemCode = await _unitOfWork.Materials.ExistingMaterialAndItemCode(material);

            if (existingMaterialsAndItemCode == true)
                return BadRequest("Item code and item description already exist, Please try something else!");


            //if (await _unitOfWork.Materials.ValidateMaterialAndAccountAndItem(material.ItemDescription))
            //    return BadRequest("Item description, item category and AccountTitle(Per Item) already exist. Please try something else!");

                if (uomId == false)
                    return BadRequest("UOM doesn't exist");

                if (await _unitOfWork.Materials.ItemCodeExist(material.ItemCode))
                    return BadRequest("Item Code already exist, Please try something else!");

                await _unitOfWork.Materials.AddMaterial(material);
                await _unitOfWork.CompleteAsync();

            return Ok(material);

        }

        [HttpPut]
        [Route("UpdateMaterials")]
        public async Task<IActionResult> UpdateRawMaterials( [FromBody] Material material)
        {

            var existingMaterialsAndItemCode = await _unitOfWork.Materials.ExistingMaterialAndItemCode(material);

            if (existingMaterialsAndItemCode == true)

                return BadRequest("Item code and item description already exist, Please try something else!");

            await _unitOfWork.Materials.UpdateMaterial(material);

          

            await _unitOfWork.CompleteAsync();
            return Ok(material);
        }


        [HttpPut]
        [Route("InActiveMaterial")]
        public async Task<IActionResult> InActiveMaterial(Material materials)
        {

            var itemcodes =  await _context.Materials.Where(x => x.Id  == materials.Id)
                                                      .Where(x => x.IsActive  == true)
                                                      .FirstOrDefaultAsync();

            if(itemcodes == null)
            {
                return BadRequest("Item not exist!");
            }
            
            var items = itemcodes.ItemCode;

            await _unitOfWork.Materials.InActiveMaterial(materials);
            await _unitOfWork.CompleteAsync();

            return Ok("Successfully inactive materials!");

        }

        [HttpPut]
        [Route("ActivateMaterial")]
        public async Task<IActionResult> ActivateRawMaterial([FromBody] Material rawmaterial)
        {
           

            await _unitOfWork.Materials.ActivateMaterial(rawmaterial);
            await _unitOfWork.CompleteAsync();

            return Ok("Successfully activated materials!");

        }

        [HttpGet]
        [Route("GetAllMaterialWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<MaterialDto>>> GetAllMaterialWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var materials = await _unitOfWork.Materials.GetAllMaterialWithPagination(status, userParams);

            Response.AddPaginationHeader(materials.CurrentPage, materials.PageSize, materials.TotalCount, materials.TotalPages, materials.HasNextPage, materials.HasPreviousPage);

            var materialResult = new
            {

                materials,
                materials.CurrentPage,
                materials.PageSize,
                materials.TotalCount,
                materials.TotalPages,
                materials.HasNextPage,
                materials.HasPreviousPage
            };

            return Ok(materialResult);
        }

        [HttpGet]
        [Route("GetAllMaterialWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<MaterialDto>>> GetAllMaterialWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {
            if (search == null)

                return await GetAllMaterialWithPagination(status, userParams);

            var materials = await _unitOfWork.Materials.GetMaterialWithPaginationOrig(userParams, status, search);

            Response.AddPaginationHeader(materials.CurrentPage, materials.PageSize, materials.TotalCount, materials.TotalPages, materials.HasNextPage, materials.HasPreviousPage);

            var materialResult = new
            {
                materials,
                materials.CurrentPage,
                materials.PageSize,
                materials.TotalCount,
                materials.TotalPages,
                materials.HasNextPage,
                materials.HasPreviousPage
            };

            return Ok(materialResult);
        }


        //----------------ITEMCATEGORY-----------------



        [HttpGet]
        [Route("GetAllActiveItemCategory")]
        public async Task<IActionResult> GetAllActiveItemCategory()
        {
            var category = await _unitOfWork.Materials.GetAllActiveItemCategory();

            return Ok(category);
        }


        [HttpGet]
        [Route("GetAllInActiveItemCategory")]
        public async Task<IActionResult> GetAllInActiveItemCategory()
        {
            var materials = await _unitOfWork.Materials.GetAllInActiveItemCategory();

            return Ok(materials);
        }

        [HttpPost]
        [Route("AddNewItemCategories")]
        public async Task<IActionResult> CreateNewIteCategories(ItemCategory category)
        {

            if (await _unitOfWork.Materials.ExistItemCateg(category.ItemCategoryName))
                return BadRequest("Item category already exist, Please try something else!");

                await _unitOfWork.Materials.AddNewItemCategory(category);
                await _unitOfWork.CompleteAsync();

            return Ok(category);         
        }


        [HttpPut]
        [Route("UpdateItemCategories")]
        public async Task<IActionResult> UpdateItemCategories([FromBody] ItemCategory category)
        {

            //var validate = await _unitOfWork.Materials.ValidateItemCategorySame(category);
            //if (validate == true)
            //    return BadRequest("The item category cannot be changed because you entered the same item category!");

            if (await _unitOfWork.Materials.ValidateItemCategInUse(category.Id))
                return BadRequest("Item category was in use!");

            if (await _unitOfWork.Materials.ExistItemCateg(category.ItemCategoryName))
                return BadRequest("Item category already exist, Please try something else!");
                    
            await _unitOfWork.Materials.UpdateItemCategory(category);
            await _unitOfWork.CompleteAsync();

            return Ok(category);
        }


        [HttpPut]
        [Route("InActiveItemCategory")]
        public async Task<IActionResult> InActiveItemCategory([FromBody] ItemCategory category)
        {

            if (await _unitOfWork.Materials.ValidateItemCategInUse(category.Id))
                return BadRequest("Item category was in use!");

            await _unitOfWork.Materials.InActiveItemCategory(category);
            await _unitOfWork.CompleteAsync();

            return Ok("Successfully inactive item category!");

        }

        [HttpPut]
        [Route("ActivateItemCategory")]
        public async Task<IActionResult> ActivateItemCategory([FromBody] ItemCategory category)
        {

            await _unitOfWork.Materials.ActivateItemCategory(category);
            await _unitOfWork.CompleteAsync();

            return Ok("Successfully activated item category!");

        }

        [HttpGet]
        [Route("GetAllItemCategoryWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<UomDto>>> GetAllItemCategoryWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var category = await _unitOfWork.Materials.GetAllItemCategoryWithPagination(status, userParams);

            Response.AddPaginationHeader(category.CurrentPage, category.PageSize, category.TotalCount, category.TotalPages, category.HasNextPage, category.HasPreviousPage);

            var categoryResult = new
            {
                category,
                category.CurrentPage,
                category.PageSize,
                category.TotalCount,
                category.TotalPages,
                category.HasNextPage,
                category.HasPreviousPage
            };

            return Ok(categoryResult);
        }


        [HttpGet]
        [Route("GetAllItemCategoryWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<UomDto>>> GetAllItemCategoryWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {
            if (search == null)

                return await GetAllItemCategoryWithPagination(status, userParams);

            var category = await _unitOfWork.Materials.GetItemCategoryWithPaginationOrig(userParams, status, search);

            Response.AddPaginationHeader(category.CurrentPage, category.PageSize, category.TotalCount, category.TotalPages, category.HasNextPage, category.HasPreviousPage);

            var categoryResult = new
            {
                category,
                category.CurrentPage,
                category.PageSize,
                category.TotalCount,
                category.TotalPages,
                category.HasNextPage,
                category.HasPreviousPage
            };

            return Ok(categoryResult);
        }

        // ============================================== AccountTitle Category =================================================================

        [HttpGet]
        [Route("GetAllActiveAccountTitles")]
        public async Task<IActionResult> GetAllActiveAccountTitles()
        {
            var category = await _unitOfWork.Materials.GetAllActiveAccountTitles();

            return Ok(category);
        }

        [HttpGet]
        [Route("GetInActiveAccountTitles")]
        public async Task<IActionResult> GetInActiveAccountTitles()
        {
            var category = await _unitOfWork.Materials.GetInActiveAccountTitles();

            return Ok(category);
        }

        [HttpPost]
        [Route("AddNewAccountTitles")]
        public async Task<IActionResult> AddNewAccountTitles(AccountTitle category)
        {
           
            var existingSubCategAndItemCateg = await _unitOfWork.Materials.DuplicateAccountTitleAndItemCategories(category);

                if (existingSubCategAndItemCateg == true)
                return BadRequest("AccountTitles(Per Item) and item category already exist, Please try something else!");

            await _unitOfWork.Materials.AddNewAccountTitles(category);
            await _unitOfWork.CompleteAsync();
            return Ok(category);
        }

        [HttpPut]
        [Route("UpdateAccountTitles")]
        public async Task<IActionResult> UpdateAccountTitles(AccountTitle category)
        {

            if (await _unitOfWork.Materials.ValidateAccountInUse(category.Id))
                return BadRequest("AccountTitles(Per Item) is in use!");

            var existingSubCategAndItemCateg = await _unitOfWork.Materials.DuplicateAccountTitleAndItemCategories(category);

            if (existingSubCategAndItemCateg == true)
                return BadRequest("AccountTitles(Per Item) and item category already exist, Please try something else!");

            await _unitOfWork.Materials.UpdateAccountTitles(category);
            await _unitOfWork.CompleteAsync();
            return Ok(category);

        }

        [HttpPut]
        [Route("ActivateAccountTitles")]
        public async Task<IActionResult> ActivateAccountTitles(AccountTitle category)
        {

            var valid = await _unitOfWork.Materials.ActivateAccountTitles(category);

            if (valid == false)
                return BadRequest("No Item category existing, Please try another input!");

            await _unitOfWork.Materials.ActivateAccountTitles(category);
            await _unitOfWork.CompleteAsync();
            return Ok(category);

        }
       
        [HttpPut]
        [Route("InActiveAccountTitles")]
        public async Task<IActionResult> InActiveAccountTitles(AccountTitle category)
        {
            var valid = await _unitOfWork.Materials.InActiveAccountTitles(category);
            if (valid == false)
                return BadRequest("No Item category existing! Please try another ");

            if (await _unitOfWork.Materials.ValidateAccountInUse(category.Id))
                return BadRequest("(AccountTitles(Per Item) is in use!");

            await _unitOfWork.Materials.InActiveAccountTitles(category);
            await _unitOfWork.CompleteAsync();
            return Ok(category);

        }

        [HttpGet]
        [Route("GetAllAccountTitlesPagination/{status}")]
        public async Task<ActionResult<IEnumerable<AccountTitlesDto>>> GetAllAccountTitlesPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var category = await _unitOfWork.Materials.GetAllAccountTitlesPagination(status, userParams);

            Response.AddPaginationHeader(category.CurrentPage, category.PageSize, category.TotalCount, category.TotalPages, category.HasNextPage, category.HasPreviousPage);

            var categoryResult = new
            {
                category,
                category.CurrentPage,
                category.PageSize,
                category.TotalCount,
                category.TotalPages,
                category.HasNextPage,
                category.HasPreviousPage
            };

            return Ok(categoryResult);
        }


        [HttpGet]
        [Route("GetAllAccountTitlesPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<AccountTitlesDto>>> GetAllSubCategoryPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {
            if (search == null)

                return await GetAllAccountTitlesPagination(status, userParams);

            var category = await _unitOfWork.Materials.GetAllAccountTitlesPaginationOrig(userParams, status, search);

            Response.AddPaginationHeader(category.CurrentPage, category.PageSize, category.TotalCount, category.TotalPages, category.HasNextPage, category.HasPreviousPage);

            var categoryResult = new
            {
                category,
                category.CurrentPage,
                category.PageSize,
                category.TotalCount,
                category.TotalPages,
                category.HasNextPage,
                category.HasPreviousPage
            };

            return Ok(categoryResult);
        }


        [HttpGet]
        [Route("GetAllItemcategoriesmaterial")]
        public async Task<IActionResult> GetAllsubcategories(string category)
        {
            var categ = await _unitOfWork.Materials.GetAllListofItemMaterial(category);

            return Ok(categ);

        }

        [HttpGet]
        [Route("GetAllAccountmaterial")]
        public async Task<IActionResult> GetAllAccountmaterial()
        {
            var categ = await _unitOfWork.Materials.GetallActiveSubcategoryDropDown();

            return Ok(categ);
        }


        [HttpGet]
        [Route("GetAllActiveItemCategories")]
        public async Task<IActionResult> GetAllActiveItemCategories()
        {
            var categ = await _unitOfWork.Materials.GetAllAccountmaterial();

            return Ok(categ);

        }


        [HttpGet]
        [Route("GetAllMaterial")]
        public async Task<IActionResult> GetAllMaterial()
        {
            var categ = await _unitOfWork.Materials.GetAllMaterial();

            return Ok(categ);
        }


        [HttpGet]
        [Route("GetAllItemCategory")]
        public async Task<IActionResult> GetAllItemCategory()
        {
            var categ = await _unitOfWork.Materials.GetAllItemCategory();

            return Ok(categ);
        }



    }
}
