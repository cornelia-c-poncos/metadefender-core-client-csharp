
using Newtonsoft.Json;

namespace Opswat.Metadefender.Core.Client.Responses
{
	public class ScanRequest
	{
		[JsonProperty(Required = Required.Always)]
		public string data_id;
	}
}
