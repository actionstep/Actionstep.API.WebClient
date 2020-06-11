using Newtonsoft.Json;
using System.Collections.Generic;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Responses
{
    public class DataCollectionFieldsResponseDto
    {
        [JsonProperty("datacollectionfields")] public List<DataCollectionFieldDto> DataCollectionFields { get; set; } = new List<DataCollectionFieldDto>();
        public PageMetaDataResponseDto PageMetaDataDto { get; set; } = new PageMetaDataResponseDto();


        public class DataCollectionFieldDto
        {
            [JsonProperty("id")] public string DataCollectionFieldId { get; set; }
            [JsonProperty("name")] public string Name { get; set; }
            [JsonProperty("dataType")] public string DataType { get; set; }
            [JsonProperty("label")] public string Label { get; set; }
            [JsonProperty("formOrder")] public int? FormOrder { get; set; }
            [JsonProperty("required")] public string Required { get; set; }
            [JsonProperty("description")] public string Description { get; set; }
            [JsonProperty("links")] public DataCollectionFieldLinksDto Links { get; set; }


            public class DataCollectionFieldLinksDto
            {
                [JsonProperty("dataCollection")] public int DataCollectionId { get; set; }
            }
        }
    }
}