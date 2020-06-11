using Actionstep.API.WebClient.View_Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Actionstep.API.WebClient.UI_Components
{
    public partial class DataFieldSelector
    {
        [Inject] private ActionstepApi _api { get; set; }

        [Parameter] public int FilterByDataCollection { get; set; }
        [Parameter] public EventCallback<string> SelectedDataFieldChanged { get; set; }

        private List<DataFieldLookupViewModel> DataFieldLookupList { get; set; } = new List<DataFieldLookupViewModel>();
        private bool ShowMenu { get; set; }
        private int lastDataCollectionId = -1;


        protected override async Task OnParametersSetAsync()
        {
            if (lastDataCollectionId != FilterByDataCollection)
            {
                ShowMenu = false;
                DataFieldLookupList.Clear();
                lastDataCollectionId = FilterByDataCollection;
            }

            await RetrieveDataCollectionFields();
            await base.OnParametersSetAsync();
        }


        private async Task RetrieveDataCollectionFields()
        {
            var dataFields = await _api.GetDataCollectionFieldsAsync(FilterByDataCollection);

            foreach (var dataField in dataFields.DataCollectionFields)
            {
                if (!DataFieldLookupList.Any(x => x.DataCollectionFieldId == dataField.DataCollectionFieldId))
                {
                    DataFieldLookupList.Add(new DataFieldLookupViewModel()
                    {
                        DataCollectionFieldId = dataField.DataCollectionFieldId,
                        DataCollectionId = dataField.Links.DataCollectionId,
                        Label = dataField.Label,
                        Selected = false                        
                    });
                }
            }
        }


        public void DisplayMenu()
        {
            ShowMenu = !ShowMenu;
        }


        public async Task OnSelectedDataFieldChanged(string dataCollectionFieldId)
        {
            await SelectedDataFieldChanged.InvokeAsync(dataCollectionFieldId);
        }
    }
}