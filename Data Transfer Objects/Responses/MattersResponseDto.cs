using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Responses
{
    public class MattersResponseDto
    {
        [JsonProperty("actions")] public List<MatterDto> Matters { get; set; } = new List<MatterDto>();

        public List<LinkedMatterTypeDto> LinkedMatterTypes { get; set; } = new List<LinkedMatterTypeDto>();
        public List<LinkedParticipantDto> LinkedParticipants { get; set; } = new List<LinkedParticipantDto>();
        public PageMetaDataResponseDto PageMetaDataDto { get; set; } = new PageMetaDataResponseDto();


        public class MatterDto
        {
            [JsonProperty("id")] public int MatterId { get; set; }
            [JsonProperty("name")] public string Name { get; set; }
            [JsonProperty("status")] public string Status { get; set; }
            [JsonProperty("modifiedTimestamp")] public DateTime LastModified { get; set; }
            [JsonProperty("links")] public MatterLinksDto Links { get; set; }


            public class MatterLinksDto
            {
                [JsonProperty("actionType")] public int MatterTypeId { get; set; }
                [JsonProperty("assignedTo")] public int ParticipantId { get; set; }
            }
        }


        public class LinkedMatterTypeDto
        {
            [JsonProperty("id")] public int MatterTypeId { get; set; }
            [JsonProperty("name")] public string Name { get; set; }
        }


        public class LinkedParticipantDto
        {
            [JsonProperty("id")] public int ParticipantId { get; set; }
            [JsonProperty("displayName")] public string DisplayName { get; set; }
        }
    }
}