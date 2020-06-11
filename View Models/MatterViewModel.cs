using System;

namespace Actionstep.API.WebClient.View_Models
{
    public class MatterViewModel
    {
        public int MatterId { get; set; }
        public string Name { get; set; }
        public string MatterType { get; set; }
        public int MatterTypeId { get; set; }
        public string Status { get; set; }
        public DateTime LastModified { get; set; }
        public string AssignedTo { get; set; }
    }
}