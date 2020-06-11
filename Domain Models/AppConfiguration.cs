namespace Actionstep.API.WebClient.Domain_Models
{
    public class AppConfiguration
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scopes { get; set; }
        public string AuthorizeUrl { get; set; }
        public string AccessTokenUrl { get; set; }
        public string RedirectUrl { get; set; }
    }
}