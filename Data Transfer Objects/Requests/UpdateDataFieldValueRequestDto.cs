using Newtonsoft.Json;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Requests
{
    [JsonObject]
	public class UpdateDataFieldValueRequestDto
	{
		[JsonProperty("datacollectionrecordvalues")] public DataFieldValue DataFields { get; set; } = new DataFieldValue();

		[JsonObject]
		public class DataFieldValue
		{
			[JsonProperty("stringValue")] public string DataValue { get; set; }
		}
	}
}