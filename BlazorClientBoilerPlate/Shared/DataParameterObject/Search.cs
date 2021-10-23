using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorClientBoilerPlate.Shared.DataParameterObject
{
    public class Search
    {
        public List<string> Fields { get; set; } = new List<string>();
        public string Keyword { get; set; } = string.Empty;

        public Search(List<string> fields, string Keyword)
        {
            Fields = fields;
            Keyword = Keyword;
        }
    }
}
