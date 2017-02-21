
using Newtonsoft.Json;

namespace Opswat.Metadefender.Core.Client.Responses
{
	public class ExtractedFile
	{
		[JsonProperty(Required = Required.Always)]
		public string data_id;

		[JsonProperty(Required = Required.Always)]
		public int detected_by;

		[JsonProperty(Required = Required.Always)]
		public string display_name;

		[JsonProperty(Required = Required.Always)]
		public long file_size;

		[JsonProperty(Required = Required.Always)]
		public string file_type;

		[JsonProperty(Required = Required.Always)]
		public string file_type_description;

		[JsonProperty(Required = Required.Always)]
		public int progress_percentage;

		[JsonProperty(Required = Required.Always)]
		public int scan_all_result_i;

		[JsonProperty(Required = Required.Always)]
		public int scanned_with;
	}
}
