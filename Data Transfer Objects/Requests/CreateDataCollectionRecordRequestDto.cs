using Newtonsoft.Json;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Requests
{
    [JsonObject]
    public class CreateDataCollectionRecordRequestDto
    {
        [JsonProperty("datacollectionrecords")] public DataCollectionRecord DataCollectionRecords { get; set; } = new DataCollectionRecord();


        [JsonObject]
        public class DataCollectionRecord
        {
            [JsonProperty("links")] public Links DataCollectionRecordLinks { get; set; }
        }


        [JsonObject]
        public class Links
        {
            [JsonProperty("action")] public int MatterId { get; set; }
            [JsonProperty("dataCollection")] public int DataCollectionId { get; set; }
        }
    }
}