using Actionstep.API.WebClient.Domain_Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Actionstep.API.WebClient.UI_Components
{
    public partial class MatterTypeSelector
    {
        [Inject] private ActionstepApi _api { get; set; }

        [Parameter] public EventCallback<int> SelectedMatterTypeChanged { get; set; }
        [Parameter] public int InitialSelectedMatterTypeId { get; set; }

        private IEnumerable<MatterTypeLookup> MatterTypeLookupList { get; set; } = new List<MatterTypeLookup>();


        protected override async Task OnInitializedAsync()
        {
            if (MatterTypeLookupList.Count() == 0)
                MatterTypeLookupList = ConversionFactory.ConvertToDomainModel(await _api.GetMatterTypesLookupAsync());

            await base.OnInitializedAsync();
        }


        public async Task OnSelectedMatterTypeChanged(ChangeEventArgs e)
        {
            await SelectedMatterTypeChanged.InvokeAsync(Convert.ToInt32(e.Value));
        }
    }
}