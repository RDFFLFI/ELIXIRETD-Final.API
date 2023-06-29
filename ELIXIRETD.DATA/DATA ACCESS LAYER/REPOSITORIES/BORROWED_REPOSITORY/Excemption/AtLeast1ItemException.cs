using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.BORROWED_REPOSITORY.Excemption
{
    public class AtLeast1ItemException : Exception
    {
        public AtLeast1ItemException() : base("At least one Item must be tagged") { }
    }
}
