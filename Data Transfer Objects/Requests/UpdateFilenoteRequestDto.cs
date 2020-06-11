using Newtonsoft.Json;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Requests
{
    [JsonObject]
    public class UpdateFilenoteRequestDto
    {
        [JsonProperty("filenotes")] public Filenote Filenotes { get; set; } = new Filenote();

        [JsonObject]
        public class Filenote
        {
            [JsonProperty("text")] public string Content { get; set; }
        }
    }
}