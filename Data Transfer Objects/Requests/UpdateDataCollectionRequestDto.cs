using Newtonsoft.Json;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Requests
{
    [JsonObject]
	public class UpdateDataCollectionRequestDto
	{
		[JsonProperty("datacollections")] public DataCollection DataCollections { get; set; } = new DataCollection();

		[JsonObject]
		public class DataCollection
		{
			[JsonProperty("name")] public string Name { get; set; }
			[JsonProperty("label")] public string Label { get; set; }
			[JsonProperty("description")] public string Description { get; set; }
			[JsonProperty("links")] public Links DataCollectionLinks { get; set; }


			[JsonObject]
			public class Links
			{
				[JsonProperty("actionType")] public int MatterTypeId { get; set; }
			}
		}
	}
}