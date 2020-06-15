using System;

namespace Actionstep.API.WebClient.Domain_Models
{
    public class FilenoteResthookResponseData
    {
        public string ResthookType { get; set; }
        public int FilenoteId { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public string Content { get; set; }
    }
}