using Actionstep.API.WebClient.Domain_Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Actionstep.API.WebClient.UI_Components
{
    public partial class DataCollectionSelector
    {
        [Inject] private ActionstepApi _api { get; set; }

        [Parameter] public int FilterByMatterType { get; set; }
        [Parameter] public EventCallback<int> SelectedDataCollectionChanged { get; set; }
        [Parameter] public int InitialSelection 
        {
            get { return selectedDataCollection; }
            set 
            { 
                if (selectedDataCollection != value)
                {
                    if (SelectedDataCollectionChanged.HasDelegate)
                    {
                        SelectedDataCollectionChanged.InvokeAsync(value);
                    }
                    selectedDataCollection = value;
                }
            }
        }

        private int selectedDataCollection = -1;
        private IEnumerable<DataCollectionLookup> DataCollectionLookupList { get; set; } = new List<DataCollectionLookup>();
        

        protected override async Task OnInitializedAsync()
        {
            if (DataCollectionLookupList.Count() == 0)
                DataCollectionLookupList = ConversionFactory.ConvertToDomainModelForLookup(await _api.GetDataCollectionsAsync());

            await base.OnInitializedAsync();
        }
    }
}