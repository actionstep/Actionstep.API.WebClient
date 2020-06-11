using System;

namespace Actionstep.API.WebClient.View_Models
{
    public class FilenoteViewModel
    {
        public int FilenoteId { get; set; }
        public string Content { get; set; }
        public int MatterId { get; set; }
        public string MatterName { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }

        public bool Selected { get; set; }
    }
}