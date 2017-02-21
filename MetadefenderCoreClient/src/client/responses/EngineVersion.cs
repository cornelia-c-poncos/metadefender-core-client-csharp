
using Newtonsoft.Json;

namespace Opswat.Metadefender.Core.Client.Responses
{
	public class EngineVersion
	{
		[JsonProperty(Required = Required.Always)]
		public bool active;

		[JsonProperty(Required = Required.Always)]
		public string def_time;

		[JsonProperty(Required = Required.Always)]
		public int download_progress;

		[JsonProperty(Required = Required.Always)]
		public string download_time;

		[JsonProperty(Required = Required.Always)]
		public string eng_id;

		[JsonProperty(Required = Required.Always)]
		public string eng_name;

		[JsonProperty(Required = Required.Always)]
		public string eng_type;

		[JsonProperty(Required = Required.Always)]
		public string eng_ver;

		[JsonProperty(Required = Required.Always)]
		public string engine_type;

		[JsonProperty(Required = Required.Always)]
		public string state;

		[JsonProperty(Required = Required.Always)]
		public string type;
	}
}
