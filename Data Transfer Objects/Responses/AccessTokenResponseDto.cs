namespace Actionstep.API.WebClient.Data_Transfer_Objects.Responses
{
    public class AccessTokenResponseDto
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string api_endpoint { get; set; }
        public string orgkey { get; set; }
        public string refresh_token { get; set; }
    }
}