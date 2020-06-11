using Actionstep.API.WebClient.Data_Transfer_Objects.Responses;
using Actionstep.API.WebClient.Domain_Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace Actionstep.API.WebClient.Shared
{
    public partial class SignIn
    {
        [Parameter] public RenderFragment ChildContent { get; set; }

        [Inject] private AppConfiguration _appConfiguration { get; set; }
        [Inject] private NavigationManager _navigationManager { get; set; }
        [Inject] private AppState _appState { get; set; }

        private string AuthorizationToken { get; set; }


        protected override void OnInitialized()
        {
            var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("code", out var authCode))
            {
                AuthorizationToken = authCode.First();
            }

            base.OnInitialized();
        }


        protected override async Task OnInitializedAsync()
        {
            if (!String.IsNullOrEmpty(AuthorizationToken))
            {
                var accessTokenDto = await GetAccessTokenAsync(AuthorizationToken, _appConfiguration.ClientId, _appConfiguration.ClientSecret, new Uri(_appConfiguration.RedirectUrl));
                if (accessTokenDto != null)
                {
                    UpdateAppState(accessTokenDto);
                    _appState.SetSignInState(true);
                    _navigationManager.NavigateTo("/user");
                };
            }

            await base.OnInitializedAsync();
        }


        private void UpdateAppState(AccessTokenResponseDto accessTokenDto)
        {
            _appState.AccessToken = accessTokenDto.access_token;
            _appState.TokenType = accessTokenDto.token_type;
            _appState.ExpiresIn = accessTokenDto.expires_in;
            _appState.ApiEndpoint = accessTokenDto.api_endpoint;
            _appState.Orgkey = accessTokenDto.orgkey;
            _appState.RefreshToken = accessTokenDto.refresh_token;
        }


        private void SignInUser()
        {
            var qs = $"?response_type=code&scope={_appConfiguration.Scopes}&client_id={_appConfiguration.ClientId}&redirect_uri={HttpUtility.HtmlEncode(_appConfiguration.RedirectUrl)}";
            _navigationManager.NavigateTo(_appConfiguration.AuthorizeUrl + qs);
        }


        private void SignOutUser()
        {
            _appState.SetSignInState(false);
            _navigationManager.NavigateTo("/");
        }


        private async Task<AccessTokenResponseDto> GetAccessTokenAsync(string authorizationCode, string clientId, string clientSecret, Uri redirectUrl)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

            var kvCollection = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("code", authorizationCode),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("redirect_uri", HttpUtility.HtmlEncode(redirectUrl))
                };

            var body = new FormUrlEncodedContent(kvCollection);

            var response = await client.PostAsync(_appConfiguration.AccessTokenUrl, body);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AccessTokenResponseDto>(data);
            }
            return null;
        }
    }
}