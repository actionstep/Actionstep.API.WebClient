using Newtonsoft.Json;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Requests
{
    [JsonObject]
    public class CreateDocumentRequestDto
    {
        [JsonProperty("actiondocuments")] public Document Documents { get; set; } = new Document();


        [JsonObject]
        public class Document
        {
            [JsonProperty("name")] public string Name { get; set; }
            [JsonProperty("file")] public string FileId { get; set; }
            [JsonProperty("links")] public Links DocumentLinks { get; set; }


            [JsonObject]
            public class Links
            {
                [JsonProperty("action")] public int? MatterId { get; set; }
            }
        }
    }
}