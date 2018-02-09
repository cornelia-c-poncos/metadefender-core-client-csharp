
using Newtonsoft.Json;

namespace Opswat.Metadefender.Core.Client.Responses
{
	public class PostProcessing
	{
		[JsonProperty(Required = Required.Always)]
		public string actions_failed;

		[JsonProperty(Required = Required.Always)]
		public string actions_ran;

		[JsonProperty(Required = Required.Always)]
		public string converted_destination;

		[JsonProperty(Required = Required.Always)]
		public string converted_to;

		[JsonProperty(Required = Required.Always)]
		public string copy_move_destination;

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
