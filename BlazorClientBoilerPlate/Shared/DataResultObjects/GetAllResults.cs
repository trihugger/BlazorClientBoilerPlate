using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlazorClientBoilerPlate.Shared.DataResultObjects
{
    public class GetAllResults<T>
    {
        [JsonPropertyName("data")]
        public List<T> Data { get; set; } = new List<T>();
        [JsonPropertyName("currentpage")]
        public int CurrentPage { get; set; } = 0;
        [JsonPropertyName("totalpages")]
        public int TotalPages { get; set; } = 0;
        [JsonPropertyName("totalcount")]
        public int TotalCount { get; set; } = 0;
        [JsonPropertyName("pagesize")]
        public long PageSize { get; set; } = 0;
        [JsonPropertyName("haspreviouspage")]
        public bool HasPreviousPage { get; set; }
        [JsonPropertyName("hasnextpage")]
        public bool HasNextPage { get; set; }
        [JsonPropertyName("messages")]
        public string[] Messages { get; set; } = null;
        [JsonPropertyName("succeeded")]
        public bool Succeeded { get; set; }
    }
}
