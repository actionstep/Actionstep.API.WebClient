using Newtonsoft.Json;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Requests
{
    [JsonObject]
    public class CreateFilenoteRequestDto
    {
        [JsonProperty("filenotes")] public Filenote Filenotes { get; set; } = new Filenote();


        [JsonObject]
        public class Filenote
        {
            [JsonProperty("text")] public string Content { get; set; }
            [JsonProperty("links")] public Links FilenoteLinks { get; set; }


            [JsonObject]
            public class Links
            {
                [JsonProperty("action")] public int? MatterId { get; set; }
                [JsonProperty("document")] public int? DocumentId { get; set; }
                [JsonProperty("participant")] public int? ContactId { get; set; }
            }
        }
    }
}