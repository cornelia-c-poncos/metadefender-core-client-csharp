
using Newtonsoft.Json;

namespace Opswat.Metadefender.Core.Client.Responses
{
	public class License
	{
		[JsonProperty(Required = Required.Always)]
		public string deployment;

		[JsonProperty(Required = Required.Always)]
		public string expiration;

		[JsonProperty(Required = Required.Always)]
		public int days_left;

		[JsonProperty(Required = Required.Always)]
		public string licensed_engines;

		[JsonProperty(Required = Required.Always)]
		public string licensed_to;

		[JsonProperty(Required = Required.Always)]
		public int max_agent_count;

		[JsonProperty(Required = Required.Always)]
		public bool online_activated;

		[JsonProperty(Required = Required.Always)]
		public string product_id;

		[JsonProperty(Required = Required.Always)]
		public string product_name;
	}
}
