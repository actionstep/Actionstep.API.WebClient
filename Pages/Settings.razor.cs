using Actionstep.API.WebClient.Domain_Models;
using Actionstep.API.WebClient.View_Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Actionstep.API.WebClient.Pages
{
    public partial class Settings
    {
        [Inject] private AppConfiguration _appConfiguration { get; set; }
        [Inject] private NavigationManager _navigationManager { get; set; }

        private SettingsViewModel Model { get; set; }
        private EditContext settingsEditContext;
        private bool Debounce { get; set; } = false;


        protected override void OnInitialized()
        {
            Model = new SettingsViewModel
            {
                AuthorizeUrl = _appConfiguration.AuthorizeUrl,
                ClientId = _appConfiguration.ClientId,
                ClientSecret = _appConfiguration.ClientSecret,
                RedirectUrl = _appConfiguration.RedirectUrl,
                Scopes = _appConfiguration.Scopes,
                AccessTokenUrl = _appConfiguration.AccessTokenUrl
            };

            settingsEditContext = new EditContext(Model);

            base.OnInitialized();
        }


        public void UpdateSettings()
        {
            Debounce = true;

            if (settingsEditContext.Validate())
            {
                _appConfiguration.AccessTokenUrl = Model.AccessTokenUrl;
                _appConfiguration.AuthorizeUrl = Model.AuthorizeUrl;
                _appConfiguration.ClientId = Model.ClientId;
                _appConfiguration.ClientSecret = Model.ClientSecret;
                _appConfiguration.RedirectUrl = Model.RedirectUrl;
                _appConfiguration.Scopes = Model.Scopes;

                _navigationManager.NavigateTo("/");
            }

            Debounce = false;
        }


        public void ClearSettings()
        {
            Model.ClientId = string.Empty;
            Model.ClientSecret = string.Empty;
            Model.Scopes = string.Empty;
            Model.AuthorizeUrl = string.Empty;
            Model.AccessTokenUrl = string.Empty;
            Model.RedirectUrl = string.Empty;
        }


        public void CancelChanges()
        {
            _navigationManager.NavigateTo("/");
        }
    }
}