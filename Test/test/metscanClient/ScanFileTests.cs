
using HttpMock;
using HttpMock.Verify.NUnit;
using System;
using NUnit.Framework;
using Opswat.Metadefender.Core.Client;
using Opswat.Metadefender.Core.Client.Exceptions;
using Opswat.Metadefender.Core.Client.Responses;
using System.Reflection;
using System.IO;
using NUnit.Framework.Constraints;

namespace MetscanClient
{
	[TestFixture]
	public class ScanFileTests : BaseHttpMockTest
	{
		[Test]
		public void Success()
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());

			CreateStub("/file", "POST", 200, GetJsonFromFile("MetadefenderCoreClient.test.resources.apiResponses.scanFile.scanFile_success.json"));

			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MetadefenderCoreClient.test.resources.testScanFile.txt"))
			{
				string dataId = metadefenderCoreClient.ScanFile(stream, null);
				Assert.AreEqual("61dffeaa728844adbf49eb090e4ece0e", dataId);
			}

			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("POST", "/file");
				}
			);
		}

		[Test]
		public void Success_withDefaultUserAgent()
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());
			metadefenderCoreClient.SetUserAgent("MyAgent");

			CreateStub("/file", "POST", 200, GetJsonFromFile("MetadefenderCoreClient.test.resources.apiResponses.scanFile.scanFile_success.json"));

			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MetadefenderCoreClient.test.resources.testScanFile.txt"))
			{
				string dataId = metadefenderCoreClient.ScanFile(stream, null);
				Assert.AreEqual("61dffeaa728844adbf49eb090e4ece0e", dataId);
			}

			HttpServer.AssertWasCalled(x =>
				{
					var ret = x.CustomVerb("POST", "/file");
					ret.WithHeader("user_agent", new EqualConstraint("MyAgent"));
					return ret;
				}
			);
		}

		[Test]
		public void Success_sync()
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());

			string existingDataId = "61dffeaa728844adbf49eb090e4ece0e";

			CreateStub("/file", "POST", 200, GetJsonFromFile("MetadefenderCoreClient.test.resources.apiResponses.scanFile.scanFile_success.json"));
			CreateStub("/file/" + existingDataId, "GET", 200, GetJsonFromFile("MetadefenderCoreClient.test.resources.apiResponses.fetchScanResult.fetchScanResult_success.json"));

			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MetadefenderCoreClient.test.resources.testScanFile.txt"))
			{
				FileScanResult result = metadefenderCoreClient.ScanFileSync(
					stream, new FileScanOptions().SetFileName("fileName.txt"), 50, 4000);
				Assert.AreEqual("Allowed", result.process_info.result);
				Assert.AreEqual("Clean", result.scan_results.scan_all_result_a);
				Assert.Null(result.extracted_files);
			}

			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("POST", "/file");
				}
			);
			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("GET", "/file/" + existingDataId);
				}
			);
		}

		[Test]
		public void Success_syncTimeout()
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());

			string existingDataId = "61dffeaa728844adbf49eb090e4ece0e";

			CreateStub("/file", "POST", 200, GetJsonFromFile("MetadefenderCoreClient.test.resources.apiResponses.scanFile.scanFile_success.json"));
			CreateStub("/file/" + existingDataId, "GET", 200, GetJsonFromFile("MetadefenderCoreClient.test.resources.apiResponses.fetchScanResult.fetchScanResult_inProgress.json"));

			bool isException = false;
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MetadefenderCoreClient.test.resources.testScanFile.txt"))
			{
				try
				{
					// it should be a timeout, because we return in progress response every time
					FileScanResult result = metadefenderCoreClient.ScanFileSync(stream, new FileScanOptions().SetFileName("fileName.txt"), 50, 2000);
				}
				catch (TimeoutException)
				{
					isException = true;
				}
				Assert.True(isException);
			}

			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("POST", "/file");
				}
			);
			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("GET", "/file/" + existingDataId);
				}
			);
		}

		[Test]
		public void SuccessWithFileOptions()
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());

			CreateStub("/file", "POST", 200, GetJsonFromFile("MetadefenderCoreClient.test.resources.apiResponses.scanFile.scanFile_success.json"));

			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MetadefenderCoreClient.test.resources.testScanFile.txt"))
			{
				string dataId = metadefenderCoreClient.ScanFile(
					stream,
					new FileScanOptions()
						.SetUserAgent("Java client")
						.SetFileName("file.txt")
						.SetRule("Default Rule")
				);
				Assert.AreEqual("61dffeaa728844adbf49eb090e4ece0e", dataId);
			}

			HttpServer.AssertWasCalled(x =>
				{
					var ret = x.CustomVerb("POST", "/file");
					ret.WithHeader("user_agent", new EqualConstraint("Java client"));
					return ret;
				}
			);
		}

		[Test]
		public void WithError()
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());

			CreateStub("/file", "POST", 500, GetJsonFromFile("MetadefenderCoreClient.test.resources.apiResponses.errorJson.json"));

			bool isException = false;
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MetadefenderCoreClient.test.resources.testScanFile.txt"))
			{
				try
				{
					metadefenderCoreClient.ScanFile(stream, null);
				}
				catch (MetadefenderClientException)
				{
					isException = true;
				}
			}
			Assert.True(isException);

			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("POST", "/file");
				}
			);
		}
	}
}
