using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS
{
    public class PaginationHeader
    {

        public PaginationHeader(int currentPage, int itemsPerPage, int totalItems, int totalPages, bool hasPrevious, bool hasNext)
        {
            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
            TotalItems = totalItems;
            TotalPages = totalPages;
            HasPrevious = hasPrevious;
            HasNext = hasNext;
        }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }


    }
}
