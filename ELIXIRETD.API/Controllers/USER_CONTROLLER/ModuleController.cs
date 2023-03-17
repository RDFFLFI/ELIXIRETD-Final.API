using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.USER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;

namespace ELIXIRETD.API.Controllers.USER_CONTROLLER
{
    public class ModuleController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ModuleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;   
        }


        [HttpGet]
        [Route("GetAllActiveModules")]
        public async Task<IActionResult> GetAllActiveModules()
        {
            var roles = await _unitOfWork.Modules.GetAllActiveModules();

            return Ok(roles);
        }


        [HttpGet]
        [Route("GetAllInActiveModules")]
        public async Task<IActionResult> GetAllIncActiveModules()
        {
            var roles = await _unitOfWork.Modules.GetAllInActiveModules();

            return Ok(roles);
        }


        [HttpPost]
        [Route("AddNewModule")]
        public async Task<IActionResult> CreateModule(Module module)
        {
        
                var getMainMenuId = await _unitOfWork.Modules.CheckMainMenu(module.MainMenuId);

                if (getMainMenuId == false)
                    return BadRequest("MainMenu doesn't exist");

                if (await _unitOfWork.Modules.SubMenuNameExist(module.SubMenuName))
                    return BadRequest("SubMenu already exist!");

                if (await _unitOfWork.Modules.ModuleNameExist(module.ModuleName))
                    return BadRequest("ModuleName already exist!");

                await _unitOfWork.Modules.AddNewModule(module);
                await _unitOfWork.CompleteAsync();

            return Ok(module);
        }

        [HttpPut]
        [Route("UpdateModule")]
        public async Task<IActionResult> UpdateModuleById([FromBody] Module module)
        {
            var getMainMenuId = await _unitOfWork.Modules.CheckMainMenu(module.MainMenuId);

            if (getMainMenuId == false)
                return BadRequest("MainMenu doesn't exist");

            if (await _unitOfWork.Modules.SubMenuNameExist(module.SubMenuName))
                return BadRequest("SubMenu already exist!");

            if (await _unitOfWork.Modules.ModuleNameExist(module.ModuleName))
                return BadRequest("ModuleName already exist!");

            await _unitOfWork.Modules.UpdateModule(module);
            await _unitOfWork.CompleteAsync();

            return Ok(module);
        }

        [HttpPut]
        [Route("InActiveModule")]
        public async Task<IActionResult> InActiveModule([FromBody] Module module)
        {
            
            var valid = await _unitOfWork.Modules.ValidateMenu(module.Id);

            if (valid == true)
                return BadRequest("Module was in use!");


            await _unitOfWork.Modules.InActiveModule(module);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully inactive module!");
        }

        [HttpPut]
        [Route("ActivateModule")]
        public async Task<IActionResult> ActivateModule([FromBody] Module module)
        {
    
            await _unitOfWork.Modules.ActivateModule(module);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully activate module!");
        }


        [HttpGet]
        [Route("GetAllModulesWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetAllModulesWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var module = await _unitOfWork.Modules.GetAllModulessWithPagination(status, userParams);

            Response.AddPaginationHeader(module.CurrentPage, module.PageSize, module.TotalCount, module.TotalPages, module.HasNextPage, module.HasPreviousPage);

            var moduleResult = new
            {
                module,
                module.CurrentPage,
                module.PageSize,
                module.TotalCount,
                module.TotalPages,
                module.HasNextPage,
                module.HasPreviousPage
            };

            return Ok(moduleResult);
        }

        [HttpGet]
        [Route("GetAllModulesWithPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetAllUsersWithPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllModulesWithPagination(status, userParams);

            var module = await _unitOfWork.Modules.GetModulesByStatusWithPaginationOrig(userParams, status, search);


            Response.AddPaginationHeader(module.CurrentPage, module.PageSize, module.TotalCount, module.TotalPages, module.HasNextPage, module.HasPreviousPage);

            var moduleResult = new
            {
                module,
                module.CurrentPage,
                module.PageSize,
                module.TotalCount,
                module.TotalPages,
                module.HasNextPage,
                module.HasPreviousPage
            };

            return Ok(moduleResult);
        }

        //---------------MAIN MENU-------------------


        [HttpGet]
        [Route("GetAllActiveMainMenu")]
        public async Task<IActionResult> GetAllActiveMainMenu()
        {
            var menu = await _unitOfWork.Modules.GetAllActiveMainMenu();

            return Ok(menu);
        }


        [HttpGet]
        [Route("GetAllInActiveMainMenu")]
        public async Task<IActionResult> GetAllInActiveMainMenu()
        {
            var menu = await _unitOfWork.Modules.GetAllInActiveModules();

            return Ok(menu);
        }

        [HttpPost]
        [Route("AddNewMainMenu")]
        public async Task<IActionResult> AddNewMainMenu(MainMenu menu)
        {

            if (await _unitOfWork.Modules.MenuAlreadyExist(menu.ModuleName))
                return BadRequest("Menu already exist!");

            await _unitOfWork.Modules.AddNewMainMenu(menu);
            await _unitOfWork.CompleteAsync();

            return Ok(menu);

        }


        [HttpPut]
        [Route("UpdateMenu")]
        public async Task<IActionResult> UpdateMenu([FromBody] MainMenu menu)
        {

            if (await _unitOfWork.Modules.MenuAlreadyExist(menu.ModuleName))
                return BadRequest("Menu already exist!");

            await _unitOfWork.Modules.UpdateMainMenu(menu);
            await _unitOfWork.CompleteAsync();

            return Ok(menu);
        }

        [HttpPut]
        [Route("UpdateMenuOrder")]
        public async Task<IActionResult> UpdateMenuOrder([FromBody] MainMenu menu)
        {
            await _unitOfWork.Modules.UpdateMenuOrder(menu);
            await _unitOfWork.CompleteAsync();

            return Ok(menu);


        }


        [HttpPut]
        [Route("InActiveMenu")]
        public async Task<IActionResult> InActiveMenu([FromBody] MainMenu menu)
        {
            var validmainmenu = await _unitOfWork.Modules.validatemoduleMainMenu(menu.Id);


            if (validmainmenu == true)
                return BadRequest("Main menu was in use!");

            await _unitOfWork.Modules.InActiveMainMenu(menu);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully inactive menu!");
        }


        [HttpPut]
        [Route("ActivateMainMenu")]
        public async Task<IActionResult> ActivateMenu([FromBody] MainMenu menu)
        {
       
            await _unitOfWork.Modules.ActivateMainMenu(menu);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully active menu!");
        }


        [HttpGet]
        [Route("GetAllMainMenuWithPagination/{status}")]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetAllMainMenuWithPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var module = await _unitOfWork.Modules.GetAllMainMenuWithPagination(status, userParams);

            Response.AddPaginationHeader(module.CurrentPage, module.PageSize, module.TotalCount, module.TotalPages, module.HasNextPage, module.HasPreviousPage);

            var moduleResult = new
            {
                module,
                module.CurrentPage,
                module.PageSize,
                module.TotalCount,
                module.TotalPages,
                module.HasNextPage,
                module.HasPreviousPage
            };

            return Ok(moduleResult);
        }

        [HttpGet]
        [Route("GetAllMainMenuPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetAllMainMenuPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllMainMenuWithPagination(status, userParams);

            var module = await _unitOfWork.Modules.GetMainMenuPaginationOrig(userParams, status, search);


            Response.AddPaginationHeader(module.CurrentPage, module.PageSize, module.TotalCount, module.TotalPages, module.HasNextPage, module.HasPreviousPage);

            var moduleResult = new
            {
                module,
                module.CurrentPage,
                module.PageSize,
                module.TotalCount,
                module.TotalPages,
                module.HasNextPage,
                module.HasPreviousPage
            };

            return Ok(moduleResult);
        }



    }
}
