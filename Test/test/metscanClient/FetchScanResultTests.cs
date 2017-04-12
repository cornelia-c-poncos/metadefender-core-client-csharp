
using HttpMock.Verify.NUnit;
using NUnit.Framework;
using Opswat.Metadefender.Core.Client;
using Opswat.Metadefender.Core.Client.Exceptions;
using Opswat.Metadefender.Core.Client.Responses;

namespace MetscanClient
{
	[TestFixture]
	public class FetchScanResultTests : BaseHttpMockTest
	{
		[Test]
		public void Success()
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());

			string existingDataId = "59f92cb3e3194c6381d3f8819a0d47ed";

			CreateStub("/file/" + existingDataId, "GET", 200, GetJsonFromFile("MetadefenderCoreClient.test.resources.apiResponses.fetchScanResult.fetchScanResult_success.json"));

			FileScanResult result = metadefenderCoreClient.FetchScanResult(existingDataId);
			Assert.AreEqual(existingDataId, result.data_id);
			Assert.AreEqual("Allowed", result.process_info.result);
			Assert.AreEqual("Clean", result.scan_results.scan_all_result_a);
			Assert.Null(result.extracted_files);

			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("GET", "/file/" + existingDataId);
				}
			);
		}

		[Test]
		public void Success_withArchive()
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());

			string existingDataId = "fafb3a12b0d141909b3a3ba6b26e42c9";

			CreateStub("/file/" + existingDataId, "GET", 200, GetJsonFromFile("MetadefenderCoreClient.test.resources.apiResponses.fetchScanResult.fetchScanResult_success_withArchive.json"));


			FileScanResult result = metadefenderCoreClient.FetchScanResult(existingDataId);
			Assert.AreEqual(existingDataId, result.data_id);
			Assert.AreEqual("Allowed", result.process_info.result);
			Assert.AreEqual("Clean", result.scan_results.scan_all_result_a);
			Assert.NotNull(result.extracted_files);
			Assert.AreEqual(2L, result.extracted_files.files_in_archive.Count);

			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("GET", "/file/" + existingDataId);
				}
			);
		}

		[Test]
		public void WithNotFound()
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());

			string nonExistingId = "61dffeaa728844adbf49eb090e4ece0e";

			CreateStub("/file/" + nonExistingId, "GET", 200, GetJsonFromFile("MetadefenderCoreClient.test.resources.apiResponses.fetchScanResult.fetchScanResult_notFound.json"));

			bool isException = false;
			try
			{
				metadefenderCoreClient.FetchScanResult(nonExistingId);
			}
			catch (MetadefenderClientException)
			{
				isException = true;
			}
			Assert.True(isException);

			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("GET", "/file/" + nonExistingId);
				}
			);
		}

		[Test]
		public void WithError()
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());

			string existingDataId = "61dffeaa728844adbf49eb090e4ece0e";

			CreateStub("/file/" + existingDataId, "GET", 500, GetJsonFromFile("MetadefenderCoreClient.test.resources.apiResponses.errorJson.json"));

			bool isException = false;
			try
			{
				metadefenderCoreClient.FetchScanResult(existingDataId);
			}
			catch (MetadefenderClientException)
			{
				isException = true;
			}
			Assert.True(isException);

			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("GET", "/file/" + existingDataId);
				}
			);
		}

		[Test]
		public void WithoutDataId()
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());

			bool isException = false;
			try
			{
				metadefenderCoreClient.FetchScanResult(null);
			}
			catch (MetadefenderClientException)
			{
				isException = true;
			}
			Assert.True(isException);
		}
	}
}