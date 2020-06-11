namespace Actionstep.API.WebClient.View_Models
{
    public class SettingsViewModel
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scopes { get; set; }
        public string AuthorizeUrl { get; set; }
        public string AccessTokenUrl { get; set; }
        public string RedirectUrl { get; set; }
    }
}