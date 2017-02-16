
using Newtonsoft.Json;

namespace Opswat.Metadefender.Core.Client.Responses
{
	public class FileInfo
	{
		[JsonProperty(Required = Required.Always)]
		public string display_name;

		[JsonProperty(Required = Required.Always)]
		public long file_size;

		public string file_type;

		[JsonProperty(Required = Required.Always)]
		public string file_type_description;

		[JsonProperty(Required = Required.Always)]
		public string md5;

		[JsonProperty(Required = Required.Always)]
		public string sha1;

		[JsonProperty(Required = Required.Always)]
		public string sha256;

		[JsonProperty(Required = Required.Always)]
		public string upload_timestamp;
	}
}
