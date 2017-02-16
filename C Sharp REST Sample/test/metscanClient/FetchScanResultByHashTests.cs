
using NUnit.Framework;
using Opswat.Metadefender.Core.Client;
using Opswat.Metadefender.Core.Client.Exceptions;
using Opswat.Metadefender.Core.Client.Responses;

namespace MetscanClient
{
	[TestFixture]
	public class FetchScanResultByHashTests : BaseHttpMockTest
	{
		[Test]
		public void Success()
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());

			string existingHash = "e981b537cff14c3fbbba923d7a71ff2e";

			CreateStub("/hash/" + existingHash, "GET", 200, GetJsonFromFile("C_Sharp_REST_Sample.test.resources.apiResponses.fetchScanResultByHash.fetchScanResultByHash_success.json"));

			FileScanResult result = metadefenderCoreClient.FetchScanResultByHash(existingHash);
			Assert.AreEqual(existingHash, result.data_id);
			Assert.AreEqual("Allowed", result.process_info.result);
			Assert.AreEqual("Clean", result.scan_results.scan_all_result_a);

			HttpServer.AssertWasCalled(x =>
			{
				return x.CustomVerb("GET", "/hash/" + existingHash);
			});
		}

		[Test]
		public void WithNotFound()
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());

			string nonExistingHash = "61dffeaa728844adbf49eb090e4ece0e";

			CreateStub("/hash/" + nonExistingHash, "GET", 200, GetJsonFromFile("C_Sharp_REST_Sample.test.resources.apiResponses.fetchScanResultByHash.fetchScanResult_notFound.json"));


			bool isException = false;
			try
			{
				metadefenderCoreClient.FetchScanResultByHash(nonExistingHash);
			}
			catch (MetadefenderClientException)
			{
				isException = true;
			}
			Assert.True(isException);

			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("GET", "/hash/" + nonExistingHash);
				}
			);
		}

		[Test]
		public void WithError()
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());

			string existingDataId = "61dffeaa728844adbf49eb090e4ece0e";

			CreateStub("/hash/" + existingDataId, "GET", 500, GetJsonFromFile("C_Sharp_REST_Sample.test.resources.apiResponses.errorJson.json"));

			bool isException = false;
			try
			{
				metadefenderCoreClient.FetchScanResultByHash(existingDataId);
			}
			catch (MetadefenderClientException)
			{
				isException = true;
			}
			Assert.True(isException);

			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("GET", "/hash/" + existingDataId);
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
				metadefenderCoreClient.FetchScanResultByHash(null);
			}
			catch (MetadefenderClientException)
			{
				isException = true;
			}
			Assert.True(isException);
		}

	}
}