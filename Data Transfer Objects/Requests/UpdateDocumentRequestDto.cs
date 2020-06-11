using Newtonsoft.Json;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Requests
{
    [JsonObject]
    public class UpdateDocumentRequestDto
    {
        [JsonProperty("actiondocuments")] public Document Documents { get; set; } = new Document();

        [JsonObject]
        public class Document
        {
            [JsonProperty("name")] public string Name { get; set; }
            [JsonProperty("links")] public Links DocumentLinks { get; set; } = new Links();


            [JsonObject]
            public class Links
            {
                [JsonProperty("action")] public int? MatterId { get; set; }
            }
        }
    }
}