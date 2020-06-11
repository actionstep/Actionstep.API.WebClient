using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Responses
{
    public class FilenotesResponseDto
    {
        [JsonProperty("filenotes")] public List<FilenoteDto> Filenotes { get; set; } = new List<FilenoteDto>();

        public List<LinkedActionDto> LinkedActions { get; set; } = new List<LinkedActionDto>();
        public PageMetaDataResponseDto PageMetaDataDto { get; set; } = new PageMetaDataResponseDto();


        public class FilenoteDto
        {
            [JsonProperty("id")] public int FilenoteId { get; set; }
            [JsonProperty("text")] public string Content { get; set; }
            [JsonProperty("enteredTimestamp")] public DateTime Created { get; set; }
            [JsonProperty("enteredBy")] public string CreatedBy { get; set; }
            [JsonProperty("links")] public FilenoteLinksDto Links { get; set; }


            public class FilenoteLinksDto
            {
                [JsonProperty("action")] public int? MatterId { get; set; }
            }
        }


        public class LinkedActionDto
        {
            [JsonProperty("id")] public int ActionId { get; set; }
            [JsonProperty("name")] public string Name { get; set; }
        }
    }
}