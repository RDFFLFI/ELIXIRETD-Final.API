using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.FUEL_REGISTER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO.FuelDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.FUEL_REGISTER_INTERFACE
{
    public interface IFuelRegisterRepository
    {
        Task<bool> CreateFuelRegister(CreateFuelRegisterDto fuel);

    }
}
