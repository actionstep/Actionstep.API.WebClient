using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Responses
{
    public class DocumentsResponseDto
    {
        [JsonProperty("actiondocuments")] public List<DocumentDto> Documents { get; set; } = new List<DocumentDto>();

        public List<LinkedActionDto> LinkedActions { get; set; } = new List<LinkedActionDto>();
        public List<LinkedParticipantDto> LinkedParticipants { get; set; } = new List<LinkedParticipantDto>();
        public PageMetaDataResponseDto PageMetaDataDto { get; set; } = new PageMetaDataResponseDto();


        public class DocumentDto
        {
            [JsonProperty("id")] public int DocumentId { get; set; }
            [JsonProperty("name")] public string Name { get; set; }
            [JsonProperty("fileSize")] public int FileSize { get; set; }
            [JsonProperty("extension")] public string Extension { get; set; }
            [JsonProperty("fileName")] public string FileName { get; set; }
            [JsonProperty("file")] public string InternalFileIdentifier { get; set; }
            [JsonProperty("modifiedTimestamp")] public DateTime Created { get; set; }
            [JsonProperty("links")] public DocumentLinksDto Links { get; set; }


            public class DocumentLinksDto
            {
                [JsonProperty("action")] public int? MatterId { get; set; }
                [JsonProperty("createdBy")] public int? ParticipantId { get; set; }
            }
        }


        public class LinkedActionDto
        {
            [JsonProperty("id")] public int ActionId { get; set; }
            [JsonProperty("name")] public string Name { get; set; }
        }


        public class LinkedParticipantDto
        {
            [JsonProperty("id")] public int ParticipantId { get; set; }
            [JsonProperty("displayName")] public string DisplayName { get; set; }
        }
    }
}