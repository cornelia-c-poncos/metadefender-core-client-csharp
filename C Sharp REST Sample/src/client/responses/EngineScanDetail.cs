
using Newtonsoft.Json;

namespace Opswat.Metadefender.Core.Client.Responses
{
	public class EngineScanDetail
	{
		[JsonProperty(Required = Required.Always)]
		public string def_time;

		public string location;

		[JsonProperty(Required = Required.Always)]
		public int scan_result_i;

		[JsonProperty(Required = Required.Always)]
		public long scan_time;

		[JsonProperty(Required = Required.Always)]
		public string threat_found;
	}
}
