
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Opswat.Metadefender.Core.Client.Responses
{
	public class ExtractedFiles
	{
		[JsonProperty(Required = Required.Always)]
		public List<ExtractedFile> files_in_archive;
	}
}
