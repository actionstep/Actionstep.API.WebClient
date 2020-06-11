using System;

namespace Actionstep.API.WebClient.View_Models
{
    public class DocumentViewModel
    {
        public int DocumentId { get; set; }
        public string Name { get; set; }
        public int FileSize { get; set; }
        public string Extension { get; set; }
        public string FileName { get; set; }
        public string InternalFileIdentifier { get; set; }
        public int MatterId { get; set; }
        public string MatterName { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }

        public bool Selected { get; set; }
    }
}