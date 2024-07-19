using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS
{
    public class UserParams
    {
        private const int MaxPageSize = 10000;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = MaxPageSize;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }


        //public void SetDynamicMaxPageSize(int totalCount)
        //{
        //    _pageSize = totalCount;
        //}

    }
}
