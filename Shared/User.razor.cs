using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Actionstep.API.WebClient.Shared
{
    public partial class User
    {
        [Inject] private NavigationManager _navigationManager { get; set; }
        [Inject] private ActionstepApi _api { get; set; }

        private int userId { get; set; }


        protected override void OnInitialized()
        {
            userId = Task.Run(async () => await _api.GetSignedInUserId()).Result;

            base.OnInitialized();
        }


        protected override async Task OnInitializedAsync()
        {
            if (userId > 0)
            {
                _navigationManager.NavigateTo("/");
            }

            await base.OnInitializedAsync();
        }
    }
}