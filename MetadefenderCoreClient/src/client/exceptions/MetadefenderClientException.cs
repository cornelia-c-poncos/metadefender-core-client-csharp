using System;

namespace Opswat.Metadefender.Core.Client.Exceptions
{
	public class MetadefenderClientException : Exception
	{
		public int responseCode = 0;

		public MetadefenderClientException(string msg, int responseCode)
			: base(msg)
		{
			this.responseCode = responseCode;
		}

		public MetadefenderClientException(string msg)
			: base(msg)
		{
		}

		public string GetDetailedMessage()
		{
			string mess = Message;

			if (responseCode != 0)
			{
				mess += " [code: " + responseCode + "]";
			}

			return mess;
		}
	}
}
