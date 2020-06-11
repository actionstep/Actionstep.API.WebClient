using Newtonsoft.Json;
using System.Collections.Generic;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Responses
{
    public class DataCollectionRecordValuesResponseDto
    {
        [JsonProperty("datacollectionrecordvalues")] public List<DataCollectionRecordValueDto> DataCollectionRecordValues { get; set; } = new List<DataCollectionRecordValueDto>();

        public List<LinkedDataCollectionFieldDto> LinkedDataCollectionFields { get; set; } = new List<LinkedDataCollectionFieldDto>();
        public PageMetaDataResponseDto PageMetaDataDto { get; set; } = new PageMetaDataResponseDto();


        public class DataCollectionRecordValueDto
        {
            [JsonProperty("id")] public string DataCollectionRecordValueId { get; set; }
            [JsonProperty("stringValue")] public string Value { get; set; }
            [JsonProperty("links")] public DataCollectionRecordValueLinksDto Links { get; set; }


            public class DataCollectionRecordValueLinksDto
            {
                [JsonProperty("action")] public int MatterId { get; set; }
                [JsonProperty("dataCollectionField")] public string DataCollectionFieldId { get; set; }
            }
        }


        public class LinkedDataCollectionFieldDto
        {
            [JsonProperty("id")] public string DataCollectionFieldId { get; set; }
            [JsonProperty("name")] public string Name { get; set; }
            [JsonProperty("label")] public string Label { get; set; }
            [JsonProperty("formOrder")] public int? FormOrder { get; set; }
            [JsonProperty("description")] public string Description { get; set; }
        }
    }
}