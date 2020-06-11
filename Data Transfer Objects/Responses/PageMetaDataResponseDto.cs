using Newtonsoft.Json;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Responses
{
    public class PageMetaDataResponseDto
    {
        [JsonProperty("recordCount")] public int RecordCount { get; set; }
        [JsonProperty("pageCount")] public int PageCount { get; set; }
        [JsonProperty("page")] public int PageNumber { get; set; }
        [JsonProperty("pageSize")] public int PageSize { get; set; }
        [JsonProperty("prevPage")] public string PreviousPageUrl { get; set; }
        [JsonProperty("nextPage")] public string NextPageUrl { get; set; }

        public int DataRows => PageSize + (RecordCount - (PageNumber * PageSize));        
    }
}