using Actionstep.API.WebClient.Domain_Models;
using Actionstep.API.WebClient.Paging;
using System.Collections.Generic;

namespace Actionstep.API.WebClient.View_Models
{
    public class MattersViewModel
    {
        private const int DEFAULT_PAGE_SIZE = 10;
        private const int DEFAULT_PAGE = 1;

        public Page<MatterViewModel> MatterPagedData { get; set; } = new Page<MatterViewModel>();
        public IEnumerable<MatterTypeLookup> MatterTypeLookupList { get; set; }
        public List<DisplayColumn> DisplayColumns { get; set; } = new List<DisplayColumn>();
        public Dictionary<int, Dictionary<string, string>> CustomData {get; set; } = new Dictionary<int, Dictionary<string, string>>();
        public ModifyDataFieldsViewModel ModifyMatterDataFieldsViewModel { get; set; } = new ModifyDataFieldsViewModel();

        public int PageSize { get; set; } = DEFAULT_PAGE_SIZE;
        public int PageNumber { get; set; } = DEFAULT_PAGE;

        public int FilterByMatterType { get; set; } = -1;
        public int FilterByDataCollection { get; set; } = -1;
        public int SelectedDataCollection { get; set; } = -1;

        public bool Loading { get; set; } = true;
        public bool ShowCustomDataCard { get; set; } = false;
        public bool Debounce { get; set; }


        public class DisplayColumn
        {
            public string DataCollectionFieldId { get; set; }
            public string Label { get; set; }
            public int DisplayOrder { get; set; }
            public bool Visible { get; set; }
        }


        public class ModifyDataFieldsViewModel
        {
            public int SelectedDataCollection { get; set; } = -1;
            public int MatterId { get; set; }
            public int MatterTypeId { get; set; }
            public Dictionary<string, DataFieldViewModel> DataFields { get; set; } = new Dictionary<string, DataFieldViewModel>();


            public class DataFieldViewModel
            {
                public string DataFieldId { get; set; }
                public string DataCollectionRecordValueId { get; set; }
                public string Label { get; set; }
                public string DataType { get; set; }
                public string Value { get; set; }
            }
        }
    }
}