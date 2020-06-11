using Actionstep.API.WebClient.Paging;
using System;
using System.Collections.Generic;

namespace Actionstep.API.WebClient.View_Models
{
    public class ResthooksViewModel
    {
        private const int DEFAULT_PAGE_SIZE = 10;
        private const int DEFAULT_PAGE = 1;

        public Page<ResthookViewModel> ResthookPagedData { get; set; } = new Page<ResthookViewModel>();
        public IEnumerable<String> EventNameLookupList { get; set; } = new List<string>();
        public ResthookAddEditViewModel AddEditViewModel { get; set; } = new ResthookAddEditViewModel();

        public int PageSize { get; set; } = DEFAULT_PAGE_SIZE;
        public int PageNumber { get; set; } = DEFAULT_PAGE;

        public int ResthookBeingEdited { get; set; }
        public bool Loading { get; set; } = true;
        public bool ShowResthookCard { get; set; } = false;
        public bool EnableMultipleDelete { get; set; } = false;
        public bool Debounce { get; set; }


        public ResthooksViewModel()
        {
            EventNameLookupList = new List<string>
            {
                "ActionCreated", 
                "ActionUpdated",
                "DataCollectionRecordUpdated",
                "ParticipantCreated",
                "FileNoteCreated",
                "TaskCreated",
                "ActionParticipantAdded", 
                "ActionParticipantRemoved",
                "StepChanged",
                "ActionDocumentCreated", 
                "ActionDocumentUpdated", 
                "ActionDocumentDeleted"
            };
        }
    }
}