
using Newtonsoft.Json;

namespace Opswat.Metadefender.Core.Client.Responses
{
	public class FileScanResult
	{
		[JsonProperty(Required = Required.Always)]
		public string data_id;

		[JsonProperty(Required = Required.Always)]
		public FileInfo file_info;

		[JsonProperty(Required = Required.Always)]
		public ProcessInfo process_info;

		[JsonProperty(Required = Required.Always)]
		public ScanResults scan_results;

		/**
		 * Can be null if the scanned file is not an archive file.
		 */
		public ExtractedFiles extracted_files;

		public bool IsScanFinished()
		{
			return process_info.progress_percentage == 100;
		}
	}
}
