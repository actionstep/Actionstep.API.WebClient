using Actionstep.API.WebClient.Domain_Models;
using Actionstep.API.WebClient.Paging;
using System.Collections.Generic;

namespace Actionstep.API.WebClient.View_Models
{
    public class DocumentsViewModel
    {
        private const int DEFAULT_PAGE_SIZE = 10;
        private const int DEFAULT_PAGE = 1;

        public Page<DocumentViewModel> DocumentPagedData { get; set; } = new Page<DocumentViewModel>();
        public IEnumerable<MatterLookup> MatterLookupList { get; set; }

        public int PageSize { get; set; } = DEFAULT_PAGE_SIZE;
        public int PageNumber { get; set; } = DEFAULT_PAGE;

        public int FilterByMatter { get; set; } = -1;
        public bool Loading { get; set; } = true;
        public bool ShowNewDocumentCard { get; set; } = false;
        public bool EnableMultipleDelete { get; set; } = false;
        public bool Debounce { get; set; }

        public bool IsDownloading { get; set; }
        public int DownloadingDocumentId { get; set; }

        public DocumentAddEditViewModel AddEditViewModel { get; set; }
    }
}
