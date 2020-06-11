using Newtonsoft.Json;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Requests
{
    [JsonObject]
    public class UpdateResthookRequestDto
    {
        [JsonProperty("resthooks")] public Resthook Resthooks { get; set; } = new Resthook();


        [JsonObject]
        public class Resthook
        {
            [JsonProperty("eventName")] public string EventName { get; set; }
            [JsonProperty("targetUrl")] public string TargetUrl { get; set; }
        }
    }
}