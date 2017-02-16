
using Newtonsoft.Json;

namespace Opswat.Metadefender.Core.Client.Responses
{
	public class ApiVersion
	{
		[JsonProperty(Required = Required.Always)]
		public string product_id;

		[JsonProperty(Required = Required.Always)]
		public string version;
	}
}
