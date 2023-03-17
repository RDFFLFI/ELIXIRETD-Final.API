using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.IMPORT_INTERFACE
{
    public interface IPoSummaryRepository
    {

        Task<bool> AddNewPORequest(PoSummary posummary);

        Task<bool> CheckItemCode(string rawmaterial);
        Task<bool> CheckUomCode(string uom);
        Task<bool> CheckSupplier(string supplier);
        Task<bool> ValidatePOAndItemcodeManual(int ponumber, string itemcode);
        Task<bool> ValidateQuantityOrder(decimal quantity);
             
    }
}
