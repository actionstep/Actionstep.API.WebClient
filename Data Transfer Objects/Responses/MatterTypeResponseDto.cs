using Newtonsoft.Json;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Responses
{
    public class MatterTypeResponseDto
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
    }
}