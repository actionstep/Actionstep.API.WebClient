using Actionstep.API.WebClient.Domain_Models;
using Actionstep.API.WebClient.Paging;
using System.Collections.Generic;

namespace Actionstep.API.WebClient.View_Models
{
    public class DataCollectionsAndFieldsViewModel
    {
        private const int DEFAULT_PAGE_SIZE = 10;
        private const int DEFAULT_PAGE = 1;

        public Page<DataCollectionViewModel> DataCollectionPagedData { get; set; } = new Page<DataCollectionViewModel>();
        public Page<DataFieldViewModel> DataFieldPagedData { get; set; } = new Page<DataFieldViewModel>();
        public IEnumerable<MatterTypeLookup> MatterTypeLookupList { get; set; } = new List<MatterTypeLookup>();
        public IEnumerable<DataCollectionLookup> DataCollectionLookupList { get; set; } = new List<DataCollectionLookup>();
        public IEnumerable<DataTypeLookup> DataTypeLookupList { get; set; } = new List<DataTypeLookup>();

        public int DataCollectionPageSize { get; set; } = DEFAULT_PAGE_SIZE;
        public int DataCollectionPageNumber { get; set; } = DEFAULT_PAGE;

        public int DataFieldPageSize { get; set; } = DEFAULT_PAGE_SIZE;
        public int DataFieldPageNumber { get; set; } = DEFAULT_PAGE;

        public int FilterByMatterType { get; set; } = -1;
        public int SelectedDataCollection { get; set; } = -1;
        public bool DataCollectionLoading { get; set; }
        public bool DataFieldLoading { get; set; }
        public bool Debounce { get; set; }

        public bool ShowCreateDataCollectionCard { get; set; }
        public bool ShowCreateDataFieldCard { get; set; }
        public string ModalTitle { get; set; }
        public string ModalContent { get; set; }

        public DataCollectionAddEditViewModel DataCollectionAddEditViewModel { get; set; } = new DataCollectionAddEditViewModel();
        public DataFieldAddEditViewModel DataFieldAddEditViewModel { get; set; } = new DataFieldAddEditViewModel();
    }
}