using Newtonsoft.Json;
using System.Collections.Generic;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Responses
{
    public class DataCollectionsResponseDto
    {
        [JsonProperty("datacollections")] public List<DataCollectionDto> DataCollections { get; set; } = new List<DataCollectionDto>();

        public List<LinkedMatterTypeDto> LinkedMatterTypes { get; set; } = new List<LinkedMatterTypeDto>();
        public PageMetaDataResponseDto PageMetaDataDto { get; set; } = new PageMetaDataResponseDto();


        public class DataCollectionDto
        {
            [JsonProperty("id")] public int DataCollectionId { get; set; }
            [JsonProperty("name")] public string Name { get; set; }
            [JsonProperty("description")] public string Description { get; set; }
            [JsonProperty("label")] public string Label { get; set; }
            [JsonProperty("order")] public int? Order { get; set; }
            [JsonProperty("links")] public DataCollectionLinksDto Links { get; set; }


            public class DataCollectionLinksDto
            {
                [JsonProperty("actionType")] public int MatterTypeId { get; set; }
            }
        }

        public class LinkedMatterTypeDto
        {
            [JsonProperty("id")] public int MatterTypeId { get; set; }
            [JsonProperty("name")] public string Name { get; set; }
        }
    }
}