using Actionstep.API.WebClient.Domain_Models;
using Actionstep.API.WebClient.Paging;
using System.Collections.Generic;

namespace Actionstep.API.WebClient.View_Models
{
    public class FilenotesViewModel
    {
        private const int DEFAULT_PAGE_SIZE = 10;
        private const int DEFAULT_PAGE = 1;

        public Page<FilenoteViewModel> FilenotePagedData { get; set; } = new Page<FilenoteViewModel>();
        public IEnumerable<MatterLookup> MatterLookupList { get; set; }

        public int PageSize { get; set; } = DEFAULT_PAGE_SIZE;
        public int PageNumber { get; set; } = DEFAULT_PAGE;

        public int FilterByMatter { get; set; } = -1;
        public bool Loading { get; set; } = true;
        public bool ShowNewFilenoteCard { get; set; } = false;
        public bool EnableMultipleDelete { get; set; } = false;
        public bool Debounce { get; set; }

        public FilenoteAddEditViewModel AddEditViewModel { get; set; }
    }
}