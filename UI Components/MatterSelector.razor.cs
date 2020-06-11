using Actionstep.API.WebClient.Domain_Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Actionstep.API.WebClient.UI_Components
{
    public partial class MatterSelector
    {
        [Inject] private ActionstepApi _api { get; set; }

        [Parameter] public EventCallback<int> SelectedMatterChanged { get; set; }
        [Parameter] public int InitialSelectedMatterId { get; set; }

        private IEnumerable<MatterLookup> MatterLookupList { get; set; } = new List<MatterLookup>();


        protected override async Task OnInitializedAsync()
        {
            if (MatterLookupList.Count() == 0)
                MatterLookupList = ConversionFactory.ConvertToDomainModel(await _api.GetMatterLookupAsync());

            await base.OnInitializedAsync();
        }


        public async Task OnSelectedMatterChanged(ChangeEventArgs e)
        {
            await SelectedMatterChanged.InvokeAsync(Convert.ToInt32(e.Value));
        }
    }
}