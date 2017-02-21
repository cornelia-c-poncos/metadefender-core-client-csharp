
using Newtonsoft.Json;

namespace Opswat.Metadefender.Core.Client.Responses
{
	public class ScanRule
	{
		[JsonProperty(Required = Required.Always)]
		public long max_file_size;

		[JsonProperty(Required = Required.Always)]
		public string name;
	}
}
