using Newtonsoft.Json;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Requests
{
    [JsonObject]
	public class CreateDataFieldRequestDto
	{
		[JsonProperty("datacollectionfields")] public DataField DataFields { get; set; } = new DataField();

		[JsonObject]
		public class DataField
		{
			[JsonProperty("name")] public string Name { get; set; }
			[JsonProperty("label")] public string Label { get; set; }
			[JsonProperty("description")] public string Description { get; set; }
			[JsonProperty("dataType")] public string DataType { get; set; }
			[JsonProperty("links")] public Links DataFieldLinks { get; set; }


			[JsonObject]
			public class Links
			{
				[JsonProperty("dataCollection")] public int DataCollectionId { get; set; }
			}
		}
	}
}