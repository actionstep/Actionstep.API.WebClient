using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Responses
{
    public class ResthooksResponseDto
    {
        [JsonProperty("resthooks")] public List<ResthookDto> Resthooks { get; set; } = new List<ResthookDto>();

        public PageMetaDataResponseDto PageMetaDataDto { get; set; } = new PageMetaDataResponseDto();


        public class ResthookDto
        {
            [JsonProperty("id")] public int ResthookId { get; set; }
            [JsonProperty("eventName")] public string EventName { get; set; }
            [JsonProperty("targetUrl")] public string TargetUrl { get; set; }
            [JsonProperty("status")] public string Status { get; set; }
            [JsonProperty("triggeredCount")] public int TriggeredCount { get; set; }
            [JsonProperty("triggeredLastTimestamp")] public DateTime? LastTriggered { get; set; }
        }
    }
}