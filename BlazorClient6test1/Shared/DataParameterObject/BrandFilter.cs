using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorClient6test1.Shared.DataParameterObject
{
    public class BrandFilter : PaginationFilter
    {
        public BrandFilter(Search search, PaginationFilter pagination)
        {
            this.Search = search;
            this.PageNumber = pagination.PageNumber;
            this.PageSize = pagination.PageSize;
            this.OrderBy = pagination.OrderBy;
        }

        public BrandFilter(PaginationFilter pagination)
        {
            this.PageNumber = pagination.PageNumber;
            this.PageSize = pagination.PageSize;
            this.OrderBy = pagination.OrderBy;
        }

        public BrandFilter()
        {
            this.PageNumber = 1;
            this.PageSize = int.MaxValue;
            this.OrderBy[0] = "name";
        }
    }
}
