
using Newtonsoft.Json;

namespace Opswat.Metadefender.Core.Client.Responses
{
	public class ProcessInfo
	{
		[JsonProperty(Required = Required.Always)]
		public string blocked_reason;

		[JsonProperty(Required = Required.Always)]
		public bool file_type_skipped_scan;

		[JsonProperty(Required = Required.Always)]
		public string profile;

		[JsonProperty(Required = Required.Always)]
		public int progress_percentage;

		[JsonProperty(Required = Required.Always)]
		public string result;

		[JsonProperty(Required = Required.Always)]
		public string user_agent;

		[JsonProperty(Required = Required.Default)]
		public PostProcessing post_processing;
	}
}
