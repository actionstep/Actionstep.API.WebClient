using Newtonsoft.Json;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Responses
{
    public class CurrentUserResponseDto
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("firstName")] public string Firstname { get; set; }
        [JsonProperty("lastName")] public string Lastname { get; set; }
        [JsonProperty("orgkey")] public string OrgKey { get; set; }
    }
}