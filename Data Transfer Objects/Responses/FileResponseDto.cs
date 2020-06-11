using Newtonsoft.Json;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Responses
{
    public class FileResponseDto
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
    }
}