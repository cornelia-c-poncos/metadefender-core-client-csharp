using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Opswat.Metadefender.Core.Client
{
	public class FileScanOptions
	{
		private Dictionary<string, string> options = new Dictionary<string, string>();

		public Dictionary<string, string> GetOptions()
		{
			return options;
		}


		public FileScanOptions SetFileName(string fileName)
		{
			options["filename"] = UrlEncodeStr(fileName);
			return this;
		}

		public FileScanOptions SetFilePath(string filePath)
		{
			options["filepath"] = filePath;
			return this;
		}

		public FileScanOptions SetUserAgent(string userAgent)
		{
			options["user_agent"] = userAgent;
			return this;
		}

		public string GetUserAgent()
		{
			return options.Get("user_agent");
		}

		public FileScanOptions SetRule(string rule)
		{
			options["rule"] = UrlEncodeStr(rule);
			return this;
		}

		public FileScanOptions SetArchivepwd(string archivepwd)
		{
			options["archivepwd"] = archivepwd;
			return this;
		}

		private string UrlEncodeStr(string str)
		{
			string s = HttpUtility.UrlEncode(str, Encoding.UTF8);
			if (s == null)
			{
				throw new Exception("Could not encode URL");
			}

			// we have to use %20 for space encoding, instead of + sign
			return s.Replace("+", "%20");
		}

	}
}
