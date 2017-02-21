
using Newtonsoft.Json;

namespace Opswat.Metadefender.Core.Client.Responses
{
	public class Login
	{
		[JsonProperty(Required = Required.Always)]
		public string session_id;
	}
}
