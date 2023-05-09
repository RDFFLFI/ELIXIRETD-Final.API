using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.OleDb;

namespace ELIXIRETD.API.Controllers.SETUP_CONTROLLER
{

    public class MaterialController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public MaterialController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                return BadRequest("Item code and item description already exist!");
              
           
                if (uomId == false)
                    return BadRequest("UOM doesn't exist");

                if (await _unitOfWork.Materials.ItemCodeExist(material.ItemCode))
                    return BadRequest("Item Code already exist!");

                await _unitOfWork.Materials.AddMaterial(material);
                await _unitOfWork.CompleteAsync();

            return Ok(material);

        }

        [HttpPut]
        [Route("UpdateMaterials")]
        public async Task<IActionResult> UpdateRawMaterials( [FromBody] Material material)
        {
            var updated = await _unitOfWork.Materials.UpdateMaterial(material);
            if (!updated)
                return BadRequest("The material cannot be updated because a material with the same description already exists!");

            await _unitOfWork.CompleteAsync();
            return Ok(material);
        }

        [HttpPut]
        [Route("InActiveMaterial")]
        public async Task<IActionResult> InActiveRawMaterial([FromBody] Material rawmaterial)
        {
         
            await _unitOfWork.Materials.InActiveMaterial(rawmaterial);
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
        public async Task<ActionResult<IEnumerable<UomDto>>> GetAllMaterialWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
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
        public async Task<ActionResult<IEnumerable<UomDto>>> GetAllMaterialWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
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
                return BadRequest("Item category already exist!");

                await _unitOfWork.Materials.AddNewItemCategory(category);
                await _unitOfWork.CompleteAsync();

            return Ok(category);         
        }


        [HttpPut]
        [Route("UpdateItemCategories")]
        public async Task<IActionResult> UpdateItemCategories([FromBody] ItemCategory category)
        {
         
            var valid = await _unitOfWork.Materials.UpdateItemCategory(category);

            if (await _unitOfWork.Materials.ExistItemCateg(category.ItemCategoryName))
                return BadRequest("Item category already exist!");

            if (valid == false)
                return BadRequest("No Existing Item Category");

            
          

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

        // ============================================== Sub Category =================================================================

        [HttpGet]
        [Route("GetAllActiveSubCategory")]
        public async Task<IActionResult> GetallActiveSubCategory()
        {
            var category = await _unitOfWork.Materials.GetAllActiveSubCategory();

            return Ok(category);
        }

        [HttpGet]
        [Route("GetAllInActiveSubCategory")]
        public async Task<IActionResult> GetAllInActiveSubCategory()
        {
            var category = await _unitOfWork.Materials.GetInActiveSubCategory();

            return Ok(category);
        }

        [HttpPost]
        [Route("AddNewSubCategory")]
        public async Task<IActionResult> AddnewSubCategory(SubCategory category)
        {
           
            var existingSubCategAndItemCateg = await _unitOfWork.Materials.DuplicateSubCategoryAndItemCategories(category);

                if (existingSubCategAndItemCateg == true)
                return BadRequest("Sub category and item category already exist!");

            await _unitOfWork.Materials.AddNewSubCategory(category);
            await _unitOfWork.CompleteAsync();
            return Ok(category);
        }

        [HttpPut]
        [Route("UpdateSubCategory")]
        public async Task<IActionResult> UpdateSubCategory (SubCategory category)
        {

            var existingSubCategAndItemCateg = await _unitOfWork.Materials.DuplicateSubCategoryAndItemCategories(category);

            if (existingSubCategAndItemCateg == true)
                return BadRequest("Sub category and item category already exist!");

            await _unitOfWork.Materials.UpdateSubCategory(category);
            await _unitOfWork.CompleteAsync();
            return Ok(category);

        }

        [HttpPut]
        [Route("ActiveSubCategory")]
        public async Task<IActionResult> ActiveSubcategory(SubCategory category)
        {


            var valid = await _unitOfWork.Materials.ActivateSubCategory(category);

            if (valid == false)
                return BadRequest("No Item category existing! Please try another input!");

            await _unitOfWork.Materials.ActivateSubCategory(category);
            await _unitOfWork.CompleteAsync();
            return Ok(category);


        }


        

        [HttpPut]
        [Route("InActiveSubCategory")]
        public async Task<IActionResult> InActiveSubcategory(SubCategory category)
        {
            var valid = await _unitOfWork.Materials.InActiveSubCategory(category);

            if (valid == false)
                return BadRequest("No Item category existing! Please try another ");

            if (await _unitOfWork.Materials.ValidateSubcategInUse(category.Id))
                return BadRequest("Sub category is in use!");

            await _unitOfWork.Materials.InActiveSubCategory(category);
            await _unitOfWork.CompleteAsync();
            return Ok(category);

        }

        [HttpGet]
        [Route("GetAllSubCategoryPagination/{status}")]
        public async Task<ActionResult<IEnumerable<SubCategoryDto>>> GetAllSubcategoryPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var category = await _unitOfWork.Materials.GetAllSubCategoryPagination(status, userParams);

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
        [Route("GetAllSubCategoryPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<SubCategoryDto>>> GetAllSubCategoryPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {
            if (search == null)

                return await GetAllSubcategoryPagination(status, userParams);

            var category = await _unitOfWork.Materials.GetSubCategoryPaginationOrig(userParams, status, search);

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
        [Route("GetallActiveSubcategoryDropDown")]
        public async Task<IActionResult> GetallActiveSubcategoryDropDowns()
        {
            var categ = await _unitOfWork.Materials.GetallActiveSubcategoryDropDown();

            return Ok(categ);
        }






    }
}
