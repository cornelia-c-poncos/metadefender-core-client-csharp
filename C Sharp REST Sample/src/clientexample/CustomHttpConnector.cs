using Opswat.Metadefender.Core.Client;
using System.Collections.Generic;
using System.IO;

namespace Opswat.Metadefender.Core.ClientExample
{
	public class CustomHttpConnector : HttpConnector
	{
		public HttpResponse SendRequest(string url, string method, Stream inputStream, Dictionary<string, string> headers)
		{
			return base.SendRequest(url, method, inputStream, headers);
		}
	}
}