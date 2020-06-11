using System;

namespace Actionstep.API.WebClient.View_Models
{
    public class ResthookViewModel
    {
        public int ResthookId { get; set; }
        public string EventName { get; set; }
        public string TargetUrl { get; set; }
        public string Status { get; set; }
        public int TriggeredCount { get; set; }
        public DateTime? LastTriggered { get; set; }

        public bool Selected { get; set; }
    }
}