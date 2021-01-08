using Newtonsoft.Json;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Responses
{
    public class DataCollectionRecordsResponseDto
    {
        [JsonProperty("id")] public int DataCollectionRecordId { get; set; }

    }
}