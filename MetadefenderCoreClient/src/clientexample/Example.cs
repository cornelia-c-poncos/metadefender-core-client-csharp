using Opswat.Metadefender.Core.Client;
using Opswat.Metadefender.Core.Client.Exceptions;
using Opswat.Metadefender.Core.Client.Responses;
using System;
using System.Collections.Generic;
using System.IO;

namespace Opswat.Metadefender.Core.ClientExample
{
	public class Example
	{
		public static void Main(string[] args)
		{
			Dictionary<string, string> arguments = ProcessArguments(args);

			string apiUrl = arguments.Get("-h");
			string apiUser = arguments.Get("-u");
			string apiUserPass = arguments.Get("-p");
			string action = arguments.Get("-a");
			string file = arguments.Get("-f");
			string hash = arguments.Get("-m");

			if (file != null && file.Length != 0)
			{
				if ("scan".Equals(action))
				{
					ScanFile(apiUrl, file);
				}
				else if ("scan_sync".Equals(action))
				{
					ScanFileSync(apiUrl, file);
				}
			}

			if (hash != null && hash.Length != 0)
			{
				FetchScanResultByHash(apiUrl, hash);
			}

			if ("info".Equals(action))
			{
				ShowApiInfo(apiUrl, apiUser, apiUserPass);
			}
		}


		private static void ScanFileSync(string apiUrl, string file)
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(apiUrl);

			try
			{
				Stream inputStream = File.Open(file, FileMode.Open);
				FileScanResult result = metadefenderCoreClient.ScanFileSync(inputStream,
					new FileScanOptions().SetFileName(GetFileNameFromPath(file)), 200, 5000);
				Console.WriteLine("File scan finished with result: " + result.process_info.result);
			}
			catch (MetadefenderClientException e)
			{
				Console.WriteLine("Error during file scan: " + e.GetDetailedMessage());
			}
			catch (FileNotFoundException e)
			{
				Console.WriteLine("File not found: " + file + " Exception: " + e.Message);
			}
		}

		private static void ScanFile(string apiUrl, string file)
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(apiUrl);

			// This is optional: using custom HttpConnector
			metadefenderCoreClient.SetHttpConnector(new CustomHttpConnector());

			try
			{
				Stream inputStream = File.Open(file, FileMode.Open);
				string dataId = metadefenderCoreClient.ScanFile(inputStream,
					new FileScanOptions().SetFileName(GetFileNameFromPath(file)));
				Console.WriteLine("File scan started. The data id is: " + dataId);
			}
			catch (MetadefenderClientException e)
			{
				Console.WriteLine("Error during file scan: " + e.GetDetailedMessage());
			}
			catch (FileNotFoundException e)
			{
				Console.WriteLine("File not found: " + file + " Exception: " + e.Message);
			}
		}

		private static void FetchScanResultByHash(string apiUrl, string hash)
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(apiUrl);

			try
			{
				FileScanResult result = metadefenderCoreClient.FetchScanResultByHash(hash);
				Console.WriteLine("Fetch result by file hash: " + result.process_info.result);
			}
			catch (MetadefenderClientException e)
			{
				Console.WriteLine("Error during fetch scan by hash: " + e.GetDetailedMessage());
			}
		}

		private static void ShowApiInfo(string apiUrl, string apiUser, string apiUserPass)
		{
			MetadefenderCoreClient metadefenderCoreClient;

			try
			{
				metadefenderCoreClient = new MetadefenderCoreClient(apiUrl, apiUser, apiUserPass);
				metadefenderCoreClient.SetHttpConnector(new CustomHttpConnector());

				Console.WriteLine("Metadefender client created. Session id is: " + metadefenderCoreClient.GetSessionId());
			}
			catch (MetadefenderClientException e)
			{
				Console.WriteLine("Cannot login to this API. Error message: " + e.GetDetailedMessage());
				return;
			}

			try
			{
				License license = metadefenderCoreClient.GetCurrentLicenseInformation();
				Console.WriteLine("Licensed to: " + license.licensed_to);
			}
			catch (MetadefenderClientException e)
			{
				Console.WriteLine("Cannot get license details: " + e.GetDetailedMessage());
			}

			try
			{
				List<EngineVersion> result = metadefenderCoreClient.GetEngineVersions();
				Console.WriteLine("Fetched engine/database versions: " + result.Count);
			}
			catch (MetadefenderClientException e)
			{
				Console.WriteLine("Cannot get engine/database   versions: " + e.GetDetailedMessage());
			}

			try
			{
				ApiVersion apiVersion = metadefenderCoreClient.GetVersion();
				Console.WriteLine("Api endpoint apiVersion: " + apiVersion.version);
			}
			catch (MetadefenderClientException e)
			{
				Console.WriteLine("Cannot get api endpoint version: " + e.GetDetailedMessage());
			}

			try
			{
				List<ScanRule> scanRules = metadefenderCoreClient.GetAvailableScanRules();
				Console.WriteLine("Available scan rules: " + scanRules.Count);
			}
			catch (MetadefenderClientException e)
			{
				Console.WriteLine("Cannot get available scan rules: " + e.GetDetailedMessage());
			}

			try
			{
				metadefenderCoreClient.Logout();
				Console.WriteLine("Client successfully logged out.");
			}
			catch (MetadefenderClientException e)
			{
				Console.WriteLine("Cannot log out: " + e.GetDetailedMessage());
			}
		}

		////// Util methods
		public static void PrintUsage(string message)
		{
			if (message != null)
			{
				Console.WriteLine(message);
			}

			Console.WriteLine("\n\n\nExample usages: \n\n" +
							  "  Example -h http://localhost:8008 -a scan -f fileToScan\n" +
							  "  Example -h http://localhost:8008 -u yourUser -p yourPass -a info\n\n\n" +
							  "\t  -h   host                    Required\n" +
							  "\t  -u   username                Required if action is 'info'\n" +
							  "\t  -p   password                Required if action is 'info'\n" +
							  "\t  -a   action to do            Required (scan|scan_sync|info|hash)\n" +
							  "\t  -f   path to file to scan    Required if action is (scan|scan_sync)\n" +
							  "\t  -m   hash (md5|sha1|sha256)  Required if action is (hash)" +
							  "\n\n\n");
		}

		/**
		 * Processing, and validating command line arguments
		 * @param args command line arguments
		 * @return processed parameters
		 */
		private static Dictionary<string, string> ProcessArguments(string[] args)
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();

			List<string> switches = new List<string>() {"-h", "-u", "-p", "-a", "-f", "-m"};

			foreach (string switchStr in switches)
			{
				int index = GetSwitchIndex(args, switchStr);

				if (index >= 0)
				{
					if (args.Length >= index + 1)
					{
						parameters[switchStr] = args[index + 1];
					}
					else
					{
						PrintUsage("Missing value for switch: " + switchStr);
						Environment.Exit(1);
					}
				}
			}

			if (!parameters.ContainsKey("-h"))
			{
				PrintUsage("-h is required");
				Environment.Exit(1);
			}
			if (!parameters.ContainsKey("-a"))
			{
				PrintUsage("-a is required");
				Environment.Exit(1);
			}

			string action = parameters["-a"];

			List<string> allowedActions = new List<string>() {"scan", "scan_sync", "info", "hash"};

			if (!allowedActions.Contains(action))
			{
				PrintUsage("Invalid action: " + action);
				Environment.Exit(1);
			}

			if ("info".Equals(action))
			{
				if (!parameters.ContainsKey("-u"))
				{
					PrintUsage("-u is required");
					Environment.Exit(1);
				}
				if (!parameters.ContainsKey("-p"))
				{
					PrintUsage("-p is required");
					Environment.Exit(1);
				}
			}
			if ("scan".Equals(action) || "scan_sync".Equals(action))
			{
				if (!parameters.ContainsKey("-f"))
				{
					PrintUsage("-f is required");
					Environment.Exit(1);
				}
			}

			if ("hash".Equals(action))
			{
				if (!parameters.ContainsKey("-m"))
				{
					PrintUsage("-m is required");
					Environment.Exit(1);
				}
			}

			return parameters;
		}

		private static int GetSwitchIndex(string[] args, string switchStr)
		{
			for (int i = 0; i < args.Length; i++)
			{
				if (switchStr.Equals(args[i]))
				{
					return i;
				}
			}
			return -1;
		}

		private static string GetFileNameFromPath(string file)
		{
			string[] parts;
			if (file.Contains("/"))
			{
				parts = file.Split('/');
			}
			else if (file.Contains("\\"))
			{
				parts = file.Split('\\');
			}
			else
			{
				return file;
			}

			if (parts.Length > 1)
			{
				return parts[parts.Length - 1];
			}

			return file;
		}
	}
}
