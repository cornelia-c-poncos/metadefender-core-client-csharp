
using Newtonsoft.Json;

namespace Opswat.Metadefender.Core.Client.Responses
{
	public class Error
	{
		[JsonProperty(Required = Required.Always)]
		public string err;
	}
}
