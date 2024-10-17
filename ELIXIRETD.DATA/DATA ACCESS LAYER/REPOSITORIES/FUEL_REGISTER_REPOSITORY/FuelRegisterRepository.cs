using ELIXIRETD.DATA.CORE.INTERFACES.FUEL_REGISTER_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.FUEL_REGISTER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.FUEL_REGISTER_REPOSITORY
{
    public class FuelRegisterRepository : IFuelRegisterRepository
    {
        private readonly StoreContext _context;

        public FuelRegisterRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateFuelRegister(CreateFuelRegisterDto fuel)
        {
            
           

             

            return true;
        }
    }
}
