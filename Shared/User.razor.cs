using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Actionstep.API.WebClient.Shared
{
    public partial class User
    {
        [Inject] private NavigationManager _navigationManager { get; set; }
        [Inject] private ActionstepApi _api { get; set; }


        protected override async Task OnInitializedAsync()
        {
            var userId = await _api.GetSignedInUserId();
            if (userId > 0)
            {
                _navigationManager.NavigateTo("/");
            }

            await base.OnInitializedAsync();
        }
    }
}