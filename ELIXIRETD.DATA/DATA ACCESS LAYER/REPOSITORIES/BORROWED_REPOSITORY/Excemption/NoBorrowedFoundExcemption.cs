using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.BORROWED_REPOSITORY.Excemption
{
    public class NoBorrowedFoundExcemption : Exception
    {

        public NoBorrowedFoundExcemption() : base("No Id Found") { }
    }
}
