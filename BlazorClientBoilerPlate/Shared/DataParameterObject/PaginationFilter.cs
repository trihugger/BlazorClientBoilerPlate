using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorClientBoilerPlate.Shared.DataParameterObject
{
    public class PaginationFilter : Filter
    {
        public PaginationFilter(int pageSize = int.MaxValue, int pageNumber = 1, string[] orderBy = null)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
            OrderBy = orderBy != null ? orderBy : new string[] { "" };
        }
        public int PageSize { get; set; } = int.MaxValue;
        public int PageNumber { get; set; } = 1;
        public string[] OrderBy { get; set; } = new string[] { "" };
    }
}
