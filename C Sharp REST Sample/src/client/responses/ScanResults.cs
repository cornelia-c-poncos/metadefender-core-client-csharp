
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Opswat.Metadefender.Core.Client.Responses
{
	public class ScanResults
	{
		[JsonProperty(Required = Required.Always)]
		public string data_id;

		[JsonProperty(Required = Required.Always)]
		public int progress_percentage;

		[JsonProperty(Required = Required.Always)]
		public string scan_all_result_a;

		[JsonProperty(Required = Required.Always)]
		public int scan_all_result_i;

		[JsonProperty(Required = Required.Always)]
		public Dictionary<string, EngineScanDetail> scan_details;

		[JsonProperty(Required = Required.Always)]
		public string start_time;

		[JsonProperty(Required = Required.Always)]
		public int total_avs;

		[JsonProperty(Required = Required.Always)]
		public long total_time;
	}
}
