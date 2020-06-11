using Actionstep.API.WebClient.Domain_Models;
using Actionstep.API.WebClient.Paging;
using Actionstep.API.WebClient.View_Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Actionstep.API.WebClient.Pages
{
    public partial class DataCollectionsAndFields
    {
        [Inject] private ActionstepApi _api { get; set; }

        private DataCollectionsAndFieldsViewModel ViewModel { get; set; }
        private EditContext dataCollectionEditContext;
        private EditContext dataFieldEditContext;


        protected override void OnInitialized()
        {
            ViewModel = new DataCollectionsAndFieldsViewModel();

            base.OnInitialized();
        }


        protected override async Task OnInitializedAsync()
        {
            if (ViewModel.MatterTypeLookupList.Count() == 0)
                ViewModel.MatterTypeLookupList = ConversionFactory.ConvertToDomainModel(await _api.GetMatterTypesLookupAsync());

            if (ViewModel.DataCollectionLookupList.Count() == 0)
                ViewModel.DataCollectionLookupList = ConversionFactory.ConvertToDomainModelForLookup(await _api.GetDataCollectionsAsync());

            if (ViewModel.DataTypeLookupList.Count() == 0)
                ViewModel.DataTypeLookupList = BuildDataTypeLookupList();

            await DataCollectionPageNumberChanged(1);

            await base.OnInitializedAsync();
        }


        private async Task DataCollectionPageNumberChanged(int changedPageNumber)
        {
            ViewModel.DataCollectionLoading = true;
            ViewModel.DataCollectionPageNumber = changedPageNumber;

            var dataCollectionsDto = await _api.GetDataCollectionsAsync(ViewModel.DataCollectionPageNumber, ViewModel.DataCollectionPageSize, ViewModel.FilterByMatterType);

            ViewModel.DataCollectionPagedData = new Page<DataCollectionViewModel>(dataCollectionsDto.PageMetaDataDto.PageNumber, dataCollectionsDto.PageMetaDataDto.PageSize,
                                                                                  dataCollectionsDto.PageMetaDataDto.PageCount, dataCollectionsDto.PageMetaDataDto.RecordCount,
                                                                                  ConversionFactory.ConvertToDomainModel(dataCollectionsDto));
            ViewModel.DataCollectionLoading = false;
        }


        private async Task DataCollectionPageSizeChanged(int changedPageSize)
        {
            ViewModel.DataCollectionPageSize = changedPageSize;
            await DataCollectionPageNumberChanged(1);
        }


        public async Task OnSelectedMatterTypeFilterChanged(int matterTypeId)
        {
            ViewModel.FilterByMatterType = matterTypeId;
            await DataCollectionPageNumberChanged(1);

            ViewModel.SelectedDataCollection = -1;
            await OnDataCollectionSelected(-1);
        }


        public async Task OnDataCollectionSelected(int dataCollectionId)
        {
            ViewModel.SelectedDataCollection = dataCollectionId;
            await DataFieldPageNumberChanged(1);
        }


        public void AddCustomDataCollection()
        {
            ViewModel.DataCollectionAddEditViewModel = new DataCollectionAddEditViewModel();
            dataCollectionEditContext = new EditContext(ViewModel.DataCollectionAddEditViewModel);

            ViewModel.ShowCreateDataCollectionCard = !ViewModel.ShowCreateDataCollectionCard;
        }


        public async Task EditDataCollection(int dataCollectionId)
        {
            var dataCollection = await _api.GetDataCollectionAsync(dataCollectionId);
            if (dataCollection != null)
            {
                ViewModel.DataCollectionAddEditViewModel = new DataCollectionAddEditViewModel()
                {
                    DataCollectionId = dataCollection.DataCollectionId,
                    MatterTypeId = dataCollection.Links.MatterTypeId.ToString(),
                    Name = dataCollection.Name,
                    Label = dataCollection.Label,
                    Description = dataCollection.Description
                };
            }

            dataCollectionEditContext = new EditContext(ViewModel.DataCollectionAddEditViewModel);
            ViewModel.ShowCreateDataCollectionCard = !ViewModel.ShowCreateDataCollectionCard;
        }


        public async Task SaveDataCollection()
        {
            ViewModel.Debounce = true;

            if (dataCollectionEditContext.Validate())
            {
                bool success;
                if (ViewModel.DataCollectionAddEditViewModel.DataCollectionId == 0)
                {
                    success = await _api.CreateDataCollectionAsync(Convert.ToInt32(ViewModel.DataCollectionAddEditViewModel.MatterTypeId), ViewModel.DataCollectionAddEditViewModel.Name,
                                                                    ViewModel.DataCollectionAddEditViewModel.Description, ViewModel.DataCollectionAddEditViewModel.Label);
                }
                else
                {
                    success = await _api.UpdateDataCollectionAsync(ViewModel.DataCollectionAddEditViewModel.DataCollectionId, Convert.ToInt32(ViewModel.DataCollectionAddEditViewModel.MatterTypeId), 
                                    ViewModel.DataCollectionAddEditViewModel.Name, ViewModel.DataCollectionAddEditViewModel.Description, ViewModel.DataCollectionAddEditViewModel.Label);
                }

                if (success)
                {
                    ViewModel.ShowCreateDataCollectionCard = !ViewModel.ShowCreateDataCollectionCard;

                    ViewModel.SelectedDataCollection = -1;
                    await OnDataCollectionSelected(-1);
                    await DataCollectionPageNumberChanged(1);
                    await RefreshDataCollectionsLookup();
                }
            }

            ViewModel.Debounce = false;
        }


        public void CancelNewDataCollection()
        {
            ViewModel.ShowCreateDataCollectionCard = !ViewModel.ShowCreateDataCollectionCard;
        }


        public async Task DeleteDataCollection(int dataCollectionId)
        {
            if (await _api.DeleteDataCollectionAsync(dataCollectionId))
            {
                ViewModel.SelectedDataCollection = -1;
                await OnDataCollectionSelected(-1);
                await DataCollectionPageNumberChanged(1);
            }
            else
            {
                ViewModel.ModalTitle = "Cannot Delete Data Collection";
                ViewModel.ModalContent = "You cannot delete a data collection that contains one or more data fields that have been assigned values by one or more existing matters.";
            }
        }


        private void CancelModalDialog()
        {
            ViewModel.ModalTitle = string.Empty;
            ViewModel.ModalContent = string.Empty;
        }
        

        private async Task DataFieldPageNumberChanged(int changedPageNumber)
        {
            ViewModel.DataFieldLoading = true;
            ViewModel.DataFieldPageNumber = changedPageNumber;

            var dataFieldDto = await _api.GetDataCollectionFieldsAsync(ViewModel.DataFieldPageNumber, ViewModel.DataFieldPageSize, ViewModel.SelectedDataCollection);

            ViewModel.DataFieldPagedData = new Page<DataFieldViewModel>(dataFieldDto.PageMetaDataDto.PageNumber, dataFieldDto.PageMetaDataDto.PageSize,
                                                                        dataFieldDto.PageMetaDataDto.PageCount, dataFieldDto.PageMetaDataDto.RecordCount,
                                                                        ConversionFactory.ConvertToDomainModel(dataFieldDto));
            ViewModel.DataFieldLoading = false;
        }


        private async Task DataFieldPageSizeChanged(int changedPageSize)
        {
            ViewModel.DataFieldPageSize = changedPageSize;
            await DataFieldPageNumberChanged(1);
        }


        public void AddCustomDataField()
        {
            ViewModel.DataFieldAddEditViewModel = new DataFieldAddEditViewModel();
            dataFieldEditContext = new EditContext(ViewModel.DataFieldAddEditViewModel);

            ViewModel.ShowCreateDataFieldCard = !ViewModel.ShowCreateDataFieldCard;
        }


        public async Task EditDataField(string dataCollectionFieldId)
        {
            var dataField = await _api.GetDataFieldAsync(dataCollectionFieldId);
            if (dataField != null)
            {
                ViewModel.DataFieldAddEditViewModel = new DataFieldAddEditViewModel()
                {
                    DataCollectionId = dataField.Links.DataCollectionId.ToString(),
                    DataFieldId = dataField.DataCollectionFieldId,
                    Name = dataField.Name,
                    Label = dataField.Label,
                    Description = dataField.Description,
                    DataType = ViewModel.DataTypeLookupList.Single(x => x.InternalIdentifier == dataField.DataType).DisplayName
                };
            }

            dataFieldEditContext = new EditContext(ViewModel.DataFieldAddEditViewModel);
            ViewModel.ShowCreateDataFieldCard = !ViewModel.ShowCreateDataFieldCard;
        }


        public async Task SaveDataField()
        {
            ViewModel.Debounce = true;

            if (dataFieldEditContext.Validate())
            {
                bool success;
                if (String.IsNullOrEmpty(ViewModel.DataFieldAddEditViewModel.DataFieldId))
                {
                    success = await _api.CreateDataFieldAsync(Convert.ToInt32(ViewModel.DataFieldAddEditViewModel.DataCollectionId), ViewModel.DataFieldAddEditViewModel.Name,
                                            ViewModel.DataFieldAddEditViewModel.Description, ViewModel.DataFieldAddEditViewModel.Label, ViewModel.DataFieldAddEditViewModel.DataType);
                }
                else
                {
                    success = await _api.UpdateDataFieldAsync(ViewModel.DataFieldAddEditViewModel.DataFieldId, ViewModel.DataFieldAddEditViewModel.Name, 
                                                              ViewModel.DataFieldAddEditViewModel.Description, ViewModel.DataFieldAddEditViewModel.Label);
                }

                if (success)
                {
                    ViewModel.ShowCreateDataFieldCard = !ViewModel.ShowCreateDataFieldCard;

                    await OnDataCollectionSelected(ViewModel.SelectedDataCollection);
                }
            }

            ViewModel.Debounce = false;
        }


        public void CancelNewDataField()
        {
            ViewModel.ShowCreateDataFieldCard = !ViewModel.ShowCreateDataFieldCard;
        }


        public async Task DeleteDataField(string dataCollectionFieldId)
        {
            if (await _api.DeleteDataFieldAsync(dataCollectionFieldId))
            {
                await DataFieldPageNumberChanged(1);
            }
            else
            {
                ViewModel.ModalTitle = "Cannot Delete Data Field";
                ViewModel.ModalContent = "You cannot delete a data field that contains a value associated with an existing matter.";
            }
        }


        private IEnumerable<DataTypeLookup> BuildDataTypeLookupList()
        {
            return new List<DataTypeLookup>()
            {
                new DataTypeLookup() { InternalIdentifier = "Str255", DisplayName = "String" },
                new DataTypeLookup() { InternalIdentifier = "DateNM", DisplayName = "Date" },
                new DataTypeLookup() { InternalIdentifier = "Money", DisplayName = "Money" }
            };
        }


        private async Task RefreshDataCollectionsLookup()
        {
            ViewModel.DataCollectionLookupList = ConversionFactory.ConvertToDomainModelForLookup(await _api.GetDataCollectionsAsync());
        }
    }
}