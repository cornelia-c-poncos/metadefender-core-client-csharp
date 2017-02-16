using System.IO;
using System.Collections.Generic;
using System.Net;
using Opswat.Metadefender.Core.Client.Exceptions;
using Opswat.Metadefender.Core.Client.Responses;
using System;

namespace Opswat.Metadefender.Core.Client
{
	public class HttpConnector
	{
		public class HttpResponse
		{
			public string response;
			public int responseCode;

			public HttpResponse(string response, int responseCode)
			{
				this.response = response;
				this.responseCode = responseCode;
			}
		}

		public HttpResponse SendRequest(string url, string method, Stream inputStream, Dictionary<string, string> headers)
		{
			try
			{
				HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
				request.Method = method;

				if (headers != null)
				{
					foreach (KeyValuePair<string, string> pair in headers)
					{
						request.Headers.Add(pair.Key, pair.Value);
					}
				}
				request.Accept = "application/json";

				if (inputStream != null)
				{
					using (Stream dataStream = request.GetRequestStream())
					{
						inputStream.CopyTo(dataStream);
						dataStream.Flush();
					}
				}

				HttpWebResponse response = (HttpWebResponse) request.GetResponse();

				StreamReader streamReader = new StreamReader(response.GetResponseStream());
				return new HttpResponse(streamReader.ReadToEnd(), (int) response.StatusCode);
			}
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
				{
					HttpWebResponse response = ex.Response as HttpWebResponse;
					StreamReader streamReader = new StreamReader(ex.Response.GetResponseStream());
					return new HttpResponse(streamReader.ReadToEnd(), (int) response.StatusCode);
				}
				else
				{
					throw new MetadefenderClientException("Cannot connect to: " + url + " " + ex.Message);
				}
			}
			catch (UriFormatException ex)
			{
				throw new MetadefenderClientException("Cannot connect to: " + url + " " + ex.Message);
			}
		}

		public HttpConnector.HttpResponse SendRequest(string url, string method)
		{
			return SendRequest(url, method, null, new Dictionary<string, string>());
		}

		public HttpConnector.HttpResponse SendRequest(string url, string method, byte[] body)
		{
			return SendRequest(url, method, new MemoryStream(body), new Dictionary<string, string>());
		}
	}
}
