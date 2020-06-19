using Actionstep.API.WebClient.Paging;
using Actionstep.API.WebClient.View_Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Actionstep.API.WebClient.Pages
{
    public partial class Matters
    {
        [Inject] private ActionstepApi _api { get; set; }

        private EditContext dataFieldValuesEditContext;
        private MattersViewModel ViewModel { get; set; }


        protected override void OnInitialized()
        {
            ViewModel = new MattersViewModel();

            base.OnInitialized();
        }


        protected override async Task OnInitializedAsync()
        {
            await PageNumberChanged(1);
            await base.OnInitializedAsync();
        }


        private async Task PageNumberChanged(int changedPageNumber)
        {
            ViewModel.Loading = true;
            ViewModel.PageNumber = changedPageNumber;

            var mattersDto = await _api.GetMattersAsync(ViewModel.PageNumber, ViewModel.PageSize, ViewModel.FilterByMatterType);

            ViewModel.MatterPagedData = new Page<MatterViewModel>(mattersDto.PageMetaDataDto.PageNumber, mattersDto.PageMetaDataDto.PageSize,
                                                                  mattersDto.PageMetaDataDto.PageCount, mattersDto.PageMetaDataDto.RecordCount,
                                                                  ConversionFactory.ConvertToDomainModel(mattersDto));

            if (ViewModel.MatterPagedData.TotalRowCount > 0 && ViewModel.FilterByMatterType > 0)
            {
                var matterList = ViewModel.MatterPagedData.DataCollection.Select(x => x.MatterId).ToList();
                var customDataRecords = await _api.GetDataCollectionRecordValuesAsync(matterList, ViewModel.FilterByMatterType);

                ViewModel.CustomData.Clear();

                foreach (var dataCollectionRecordValue in customDataRecords.DataCollectionRecordValues)
                {
                    if (ViewModel.CustomData.ContainsKey(dataCollectionRecordValue.Links.MatterId))
                    {
                        ViewModel.CustomData[dataCollectionRecordValue.Links.MatterId].Add(dataCollectionRecordValue.Links.DataCollectionFieldId, dataCollectionRecordValue.Value);
                    }
                    else
                    {
                        ViewModel.CustomData.Add(dataCollectionRecordValue.Links.MatterId, 
                            new Dictionary<string, string> { { dataCollectionRecordValue.Links.DataCollectionFieldId, dataCollectionRecordValue.Value } });
                    }
                }
            }

            ViewModel.Loading = false;
        }


        private async Task PageSizeChanged(int changedPageSize)
        {
            ViewModel.PageSize = changedPageSize;
            await PageNumberChanged(1);
        }


        public async Task OnSelectedMatterTypeFilterChanged(int matterTypeId)
        {
            ViewModel.FilterByMatterType = matterTypeId;

            ViewModel.SelectedDataCollection = -1; 
            ViewModel.FilterByDataCollection = -1;
            ViewModel.DisplayColumns.Clear();

            await PageNumberChanged(1);
        }


        public async Task OnSelectedDataCollectionChanged(int dataCollectionId)
        {
            if (dataCollectionId != -1)
            {
                ViewModel.SelectedDataCollection = dataCollectionId; 
                ViewModel.FilterByDataCollection = dataCollectionId;

                await BuildDisplayColumnList(dataCollectionId);
            }
        }


        private async Task BuildDisplayColumnList(int dataCollectionId)
        {
            ViewModel.DisplayColumns.Clear();

            var dataFields = await _api.GetDataCollectionFieldsAsync(dataCollectionId);
            foreach (var dataField in dataFields.DataCollectionFields)
            {
                ViewModel.DisplayColumns.Add(new MattersViewModel.DisplayColumn()
                {
                    DataCollectionFieldId = dataField.DataCollectionFieldId,
                    Label = dataField.Label,
                    DisplayOrder = dataField.FormOrder.GetValueOrDefault(0),
                    Visible = false
                }); ;
            }
        }


        public void OnSelectedDataFieldChanged(string dataCollectionFieldId)
        {
            var displayColumn = ViewModel.DisplayColumns.Single(x => x.DataCollectionFieldId == dataCollectionFieldId);
            displayColumn.Visible = !displayColumn.Visible;
        }


        public void EditCustomData(int matterId, int matterTypeId)
        {
            ViewModel.ModifyMatterDataFieldsViewModel = new MattersViewModel.ModifyDataFieldsViewModel
            {
                MatterId = matterId,
                MatterTypeId = matterTypeId
            };

            dataFieldValuesEditContext = new EditContext(ViewModel.ModifyMatterDataFieldsViewModel);
            ViewModel.ShowCustomDataCard = !ViewModel.ShowCustomDataCard;
        }


        public async Task OnEditMatterDataSelectedDataCollectionChanged(int dataCollectionId)
        {
            ViewModel.ModifyMatterDataFieldsViewModel.DataFields.Clear();

            var dataFields = await _api.GetDataCollectionFieldsAsync(dataCollectionId);

            foreach (var dataField in dataFields.DataCollectionFields)
            {
                var field = new MattersViewModel.ModifyDataFieldsViewModel.DataFieldViewModel()
                {
                    DataFieldId = dataField.DataCollectionFieldId,
                    Label = dataField.Label,
                    DataType = dataField.DataType
                };

                ViewModel.ModifyMatterDataFieldsViewModel.DataFields.Add(dataField.DataCollectionFieldId, field);
            }

            // Retrieve any existing values for the fields in the selected data collection.
            var currentFieldData = await _api.GetDataCollectionRecordValuesAsync(new List<int>() { ViewModel.ModifyMatterDataFieldsViewModel.MatterId }, ViewModel.ModifyMatterDataFieldsViewModel.MatterTypeId);
            foreach (var fieldValue in currentFieldData.DataCollectionRecordValues.Where(x => x.Links.MatterId == ViewModel.ModifyMatterDataFieldsViewModel.MatterId))
            {
                if (ViewModel.ModifyMatterDataFieldsViewModel.DataFields.ContainsKey(fieldValue.Links.DataCollectionFieldId))
                {
                    ViewModel.ModifyMatterDataFieldsViewModel.DataFields[fieldValue.Links.DataCollectionFieldId].Value = fieldValue.Value;
                    ViewModel.ModifyMatterDataFieldsViewModel.DataFields[fieldValue.Links.DataCollectionFieldId].DataCollectionRecordValueId = fieldValue.DataCollectionRecordValueId;
                }
            }
        }


        public async Task SaveDataFieldValues()
        {
            foreach (var dataField in ViewModel.ModifyMatterDataFieldsViewModel.DataFields)
            {
                await _api.UpdateDataFieldValue(dataField.Value.DataCollectionRecordValueId, dataField.Value.Value);
            }

            ViewModel.ModifyMatterDataFieldsViewModel.DataFields.Clear();
            ViewModel.ShowCustomDataCard = !ViewModel.ShowCustomDataCard;

            await PageNumberChanged(1);
        }


        public void CancelDataFieldValues()
        {
            ViewModel.ShowCustomDataCard = !ViewModel.ShowCustomDataCard;
        }
    }
}