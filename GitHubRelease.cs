using Newtonsoft.Json;

namespace ConsumingRestfulApiExamples
{
	public class GitHubRelease
	{
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "published_at")] 
		public string PublishedAt { get; set; }
	}
}
