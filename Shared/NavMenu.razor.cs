using Actionstep.API.WebClient.Domain_Models;
using Microsoft.AspNetCore.Components;

namespace Actionstep.API.WebClient.Shared
{
    public partial class NavMenu
    {
        [Inject] private AppState _appState { get; set; }

        private bool collapseNavMenu = true;
        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;
        private string IsSignedIn = "disabled nav-link";


        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }


        protected override void OnInitialized()
        {
            IsSignedIn = _appState.IsSignedIn ? "nav-link" : "disabled nav-link";

            _appState.OnChange += OnNotify;

            base.OnInitialized();
        }


        public void Dispose()
        {
            _appState.OnChange -= OnNotify;
        }


        private async void OnNotify()
        {
            IsSignedIn = _appState.IsSignedIn ? "nav-link" : "disabled nav-link";

            await InvokeAsync(() => StateHasChanged());
        }
    }
}