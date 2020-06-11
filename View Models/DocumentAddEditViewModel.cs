using BlazorInputFile;

namespace Actionstep.API.WebClient.View_Models
{
    public class DocumentAddEditViewModel
    {
        public int DocumentId { get; set; }
        public string ActionId { get; set; }
        public string Name { get; set; }
        public IFileListEntry UploadFile { get; set; }
    }
}