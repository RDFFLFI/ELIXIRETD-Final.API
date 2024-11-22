using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
//using System.Data.Entity;

namespace ELIXIRETD.API.Controllers.SETUP_CONTROLLER
{

    public class MaterialController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StoreContext _context;

        public MaterialController(IUnitOfWork unitOfWork, StoreContext context)
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
                //List<Material> AccountTitleNotExist = new List<Material>();
                List<Material> UomNotExist = new List<Material>();
                List<Material> ItemCodeNull = new List<Material>();
                List<Material> ItemDescriptionNull = new List<Material>();
                List<Material> ItemCodeAlreadyExist = new List<Material>();
                List<Material> ItemDescriptionAlreadyExist = new List<Material>();
                var allList = new List<Material>(); 

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
                    items.ItemCategoryId = itemCategory.Id;

                    if (materials.Count(x => x.ItemCode == items.ItemCode && x.ItemDescription == items.ItemDescription && x.UomId == items.UomId && x.ItemCategoryId == items.ItemCategoryId) > 1)
                    {
                        DuplicateList.Add(items);
                        continue;

                    }

                    var validateDuplicate = await _unitOfWork.Materials.ValidateDuplicateImport(items.ItemCode, items.ItemDescription, items.UomId, items.ItemCategoryId);

                    if (await _unitOfWork.Materials.ItemCodeExist(items.ItemCode))
                    {
                        ItemCodeAlreadyExist.Add(items);
                        continue;
                    }
                    if (await _unitOfWork.Materials.ValidateMaterialAndAccountAndItem(items.ItemDescription, items.ItemCategoryId))
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



                var allListSelect = allList.Select(x => x.Material_No);
                var allDelete = await _context.Materials.ToListAsync();
                var allMaterialList = allDelete.Where(x => !allListSelect.Contains(x.Material_No)).ToList();

                if(allMaterialList.Count() > 0)
                {
                    foreach (var item in allMaterialList)
                    {
                        var itemInActive = await _context.Materials.FirstOrDefaultAsync(x => x.Material_No == item.Material_No);

                        itemInActive.IsActive = false;
                    }
                }

                var resultList = new
                {
                    AvailableImport,
                    DuplicateList,
                    ItemcategoryNotExist,
                    UomNotExist,
                    ItemCodeNull,
                    ItemDescriptionNull,
                    ItemCodeAlreadyExist,
                    ItemDescriptionAlreadyExist


                };

                if (DuplicateList.Count == 0 && ItemcategoryNotExist.Count == 0 && UomNotExist.Count == 0 && ItemCodeNull.Count == 0 && ItemDescriptionNull.Count == 0
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


            if (await _unitOfWork.Materials.ValidateMaterialAndAccountAndItem(material.ItemDescription, material.ItemCategoryId))
                return BadRequest("Item description and item category already exist. Please try something else!");

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
        public async Task<IActionResult> UpdateRawMaterials([FromBody] Material material)
        {

            if (await _unitOfWork.Materials.ValidateMaterialAndAccountAndItem(material.ItemDescription, material.ItemCategoryId))
                return BadRequest("Item description and item category already exist. Please try something else!");

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

            var itemcodes = await _context.Materials.Where(x => x.Id == materials.Id)
                                                      .Where(x => x.IsActive == true)
                                                      .FirstOrDefaultAsync();

            if (itemcodes == null)
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



        [HttpGet]
        [Route("GetAllAccountmaterial")]
        public async Task<IActionResult> GetAllAccountmaterial()
        {
            var categ = await _unitOfWork.Materials.GetallActiveSubcategoryDropDown();

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



        // Sync ItemCategory Genus //


        [HttpPut]
        [Route("SyncItemCategory")]
        public async Task<IActionResult> SyncItemCategory([FromBody] ItemCategory[] category)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult("Something went wrong!") { StatusCode = 500 };
            }

            List<ItemCategory> duplicateList = new List<ItemCategory>();
            List<ItemCategory> availableImport = new List<ItemCategory>();
            List<ItemCategory> availableUpdate = new List<ItemCategory>();
            List<ItemCategory> itemCategoryEmpty = new List<ItemCategory>();

            foreach (ItemCategory items in category)
            {
                if(category.Count(x => x.ItemCategoryName == items.ItemCategoryName) > 1)
                {
                    duplicateList.Add(items);
                }
                if (items.ItemCategoryName == string.Empty || items.ItemCategoryName == null)
                {
                    itemCategoryEmpty.Add(items);
                    continue;

                }

                else
                {
                    var existingItemCategory = await _unitOfWork.Materials.GetByItemCategory(items.ItemCategory_No);

                    if (existingItemCategory != null )
                    {
                        bool hasChanged = false;

                        if(existingItemCategory.ItemCategoryName != items.ItemCategoryName)
                        {

                            existingItemCategory.ItemCategoryName = items.ItemCategoryName;
                            hasChanged = true;
                        }

                        if(hasChanged)
                        {
                            existingItemCategory.IsActive = items.IsActive;
                            existingItemCategory.ModifyBy = User.Identity.Name;
                            existingItemCategory.ModifyDate = DateTime.Now;
                            existingItemCategory.StatusSync = "New update";
                            existingItemCategory.SyncDate = DateTime.Now;

                            availableUpdate.Add(existingItemCategory);
                            await _unitOfWork.Materials.UpdateAsyncCategory(existingItemCategory);
                        }

                        if(!hasChanged)
                        {
                            existingItemCategory.SyncDate = DateTime.Now;
                            existingItemCategory.StatusSync = "No new update";
                          
                        }

                        
                    }
                    else
                    {
                        items.SyncDate = DateTime.Now;
                        items.DateAdded = DateTime.Now;
                        items.StatusSync = "New Added";
                        availableImport.Add(items);
                        await _unitOfWork.Materials.AddNewItemCategory(items);

                    }

                }
            }

            var resultlist = new
            {
                AvailableImport = availableImport,
                AvailableUpdate = availableUpdate,
                 ItemCategoryEmpty = itemCategoryEmpty,
                DuplicateList = duplicateList,
            };

            if (duplicateList.Count == 0 &&  itemCategoryEmpty.Count == 0)
            {
                await _unitOfWork.CompleteAsync();
                return Ok("Successfully updated and added!");
            }
            else
            {

                return BadRequest(resultlist);
            }

        }



        [HttpPut]
        [Route("SyncMaterial")]

        public async Task<IActionResult> SyncMaterial([FromBody] SyncMaterialDto[] material)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult("Something went wrong!") { StatusCode = 500 };
            }


            var allList = new List<SyncMaterialDto>();
            List<SyncMaterialDto> duplicateList = new List<SyncMaterialDto>();
            List<SyncMaterialDto> availableImport = new List<SyncMaterialDto>();
            List<SyncMaterialDto> availableUpdate = new List<SyncMaterialDto>();
            List<SyncMaterialDto> itemCategoryNotExist = new List<SyncMaterialDto>();
            List<SyncMaterialDto> uomNotExist = new List<SyncMaterialDto>();
            List<SyncMaterialDto> itemCodeEmpty = new List<SyncMaterialDto>();
            List<SyncMaterialDto> itemDescriptionEmpty = new List<SyncMaterialDto>();


            foreach (SyncMaterialDto item in material)
            {

                Uom uom =  await _unitOfWork.Uoms.GetByCodeAsync(item.UomCode);
                if(uom == null )
                {
                    uomNotExist.Add(item);
                    continue;
                }
                item.UomId = uom.Id;

                ItemCategory itemCategory = await _unitOfWork.Materials.GetByNameAsync(item.ItemCategoryName);
                if (itemCategory == null)
                {
                    itemCategoryNotExist.Add(item);
                    continue;
                }
                item.ItemCategoryId = itemCategory.Id;

                if (item.ItemCode == string.Empty || item.ItemCode == null)
                {
                    itemCodeEmpty.Add(item);
                    continue;

                }

                if (item.ItemDescription == string.Empty || item.ItemDescription == null)
                {
                    itemDescriptionEmpty.Add(item);
                    continue;

                }


                if (material.Count( x => x.ItemCode == item.ItemCode && x.ItemDescription == item.ItemDescription) < 1 )
                {
                    duplicateList.Add(item);
                }
                else
                {
                    var existingMaterials = await _unitOfWork.Materials.GetByMaterial(item.Material_No);

                    if(existingMaterials != null)
                    {
                        bool hasChanged  = false;
                        
                        if(existingMaterials.ItemCategoryId != item.ItemCategoryId)
                        {
                            existingMaterials.ItemCategoryId = item.ItemCategoryId;
                            hasChanged = true;
                        }

                        if(existingMaterials.UomId != item.UomId)
                        {
                            existingMaterials.UomId = item.UomId;
                            hasChanged = true;
                        }

                        if(existingMaterials.ItemCode != item.ItemCode)
                        {
                            existingMaterials.ItemCode = item.ItemCode;
                            hasChanged = true;
                        }
                        if(existingMaterials.ItemDescription != item.ItemDescription)
                        {
                            existingMaterials.ItemDescription = item.ItemDescription;
                            hasChanged = true;
                        }
                        if(existingMaterials.IsActive != item.IsActive)
                        {
                            existingMaterials.IsActive = !existingMaterials.IsActive;
                            hasChanged = true;
                        }

                        if(hasChanged)
                        {
                            existingMaterials.SyncDate = DateTime.Now;
                            existingMaterials.ModifyBy = User.Identity.Name;
                            existingMaterials.ModifyDate = DateTime.Now;
                            existingMaterials.StatusSync = "New update";
                            availableUpdate.Add(item);

                            await _unitOfWork.Materials.UpdateAsyncMaterial(existingMaterials);

                            var updateItemReceiver = await _context.WarehouseReceived
                                .Where(x => x.ItemCode == existingMaterials.ItemCode).ToListAsync();

                            foreach(var receiver in updateItemReceiver)
                            {
                                receiver.ItemCode = existingMaterials.ItemCode;
                                receiver.ItemDescription = existingMaterials.ItemDescription;   
                                receiver.Uom = existingMaterials.UomCode;

                            }

                            var updateItemOrder = await _context.Orders
                                .Where(x => x.ItemCode == existingMaterials.ItemCode).ToListAsync();

                            foreach(var order in updateItemOrder)
                            {
                                order.ItemCode = existingMaterials.ItemCode;
                                order.ItemdDescription = existingMaterials.ItemDescription;
                                order.Uom = existingMaterials.UomCode;
                            }

                            var updateItemMoveOrder = await _context.MoveOrders
                                .Where(x => x.ItemCode == existingMaterials.ItemCode).ToListAsync();

                            foreach (var moverOrder in updateItemMoveOrder)
                            {
                                moverOrder.ItemCode = existingMaterials.ItemCode;
                                moverOrder.ItemDescription = existingMaterials.ItemDescription;
                                moverOrder.Uom = existingMaterials.UomCode;
                            }

                            var updateItemMiscIssue = await _context.MiscellaneousIssueDetail
                                .Where(x => x.ItemCode == existingMaterials.ItemCode).ToListAsync();

                            foreach (var issue in updateItemMiscIssue)
                            {
                                issue.ItemCode = existingMaterials.ItemCode;
                                issue.ItemDescription = existingMaterials.ItemDescription;
                                issue.Uom = existingMaterials.UomCode;
                            }

                            var updateItemBorrowedDetails = await _context.BorrowedIssueDetails
                                .Where(x => x.ItemCode == existingMaterials.ItemCode).ToListAsync();

                            foreach( var borrow in updateItemBorrowedDetails)
                            {
                                borrow.ItemCode = existingMaterials.ItemCode;
                                borrow.ItemDescription = existingMaterials.ItemDescription;
                                borrow.Uom = existingMaterials.UomCode; 
                            }

                            var updateItemBorrowedConsume = await _context.BorrowedConsumes
                                .Where(x => x.ItemCode == existingMaterials.ItemCode).ToListAsync();

                            foreach(var consume  in updateItemBorrowedConsume)
                            {
                                consume.ItemCode = existingMaterials.ItemCode;
                                consume.ItemDescription = existingMaterials.ItemDescription;
                                consume.Uom = existingMaterials.UomCode;
                            }

                        }

                        if (!hasChanged)
                        {
                            existingMaterials.SyncDate = DateTime.Now;
                            existingMaterials.StatusSync = "No new update";

                        }

                    }
                    else
                    {

                        item.StatusSync = "New Added";
                        availableImport.Add(item);
                        await _unitOfWork.Materials.AddSyncMaterial(item);


                    }


                }

                allList.Add(item);
            }

            string diesel = "DIESEL";

            var allListSelect = allList.Select(x => x.Material_No);
            var allDelete = await _context.Materials.ToListAsync();
            var allMaterialList = allDelete.Where(x => !allListSelect.Contains(x.Material_No) && x.ItemDescription != diesel).ToList();

            if (allMaterialList.Count() > 0)
            {
                foreach (var item in allMaterialList)
                {
                    var itemInActive = await _context.Materials.FirstOrDefaultAsync(x => x.Material_No == item.Material_No);

                    itemInActive.IsActive = false;
                }
            }

            var resultlist = new
            {
                AvailableImport = availableImport,
                AvailableUpdate = availableUpdate,
                DuplicateList = duplicateList,
                UomNotExist = uomNotExist,
                ItemCategoryNotExist = itemCategoryNotExist,
                ItemCodeEmpty = itemCodeEmpty,
                ItemDescriptionEmpty = itemDescriptionEmpty,
                

            };

            if (duplicateList.Count == 0 && uomNotExist.Count == 0 && itemCategoryNotExist.Count == 0 && itemCodeEmpty.Count == 0 && itemDescriptionEmpty.Count == 0)
            {
                await _unitOfWork.CompleteAsync();
                return Ok("Successfully updated and added!");
            }
            else
            {
                return BadRequest(resultlist);
            }

           
        }


        [HttpPut]
        [Route("UpdateAsyncBufferLvl")]
        public async Task<IActionResult> UpdateAsyncBufferLvl(Material material)
        {

            var update = await _unitOfWork.Materials.UpdateAsyncBufferLvl(material);

            if(update == false)
            {
                return BadRequest("Material not found");
            }


             await _unitOfWork.Materials.UpdateAsyncBufferLvl(material);
             await _unitOfWork.CompleteAsync();

            return Ok(material);
        }





    }
}
