
using Newtonsoft.Json;

namespace Opswat.Metadefender.Core.Client.Requests
{
	public class Login
	{
		[JsonProperty(Required = Required.Always)]
		public string user;

		[JsonProperty(Required = Required.Always)]
		public string password;
	}
}
