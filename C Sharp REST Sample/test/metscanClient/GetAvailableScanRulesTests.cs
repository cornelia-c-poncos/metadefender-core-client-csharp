
using NUnit.Framework;
using Opswat.Metadefender.Core.Client;
using Opswat.Metadefender.Core.Client.Exceptions;
using Opswat.Metadefender.Core.Client.Responses;
using System.Collections.Generic;

namespace MetscanClient
{
	[TestFixture]
	public class GetAvailableScanRulesTests : BaseHttpMockTest
	{
		[Test]
		public void Success()
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());

			CreateStub("/file/rules", "GET", 200, GetJsonFromFile("C_Sharp_REST_Sample.test.resources.apiResponses.getAvailableScanRules.getAvailableScanRules_success.json"));

			List<ScanRule> result = metadefenderCoreClient.GetAvailableScanRules();

			Assert.AreEqual(6, result.Count);

			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("GET", "/file/rules");
				}
			);
		}

		[Test]
		public void Success_withNewUnknownFields()
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());

			CreateStub("/file/rules", "GET", 200, GetJsonFromFile("C_Sharp_REST_Sample.test.resources.apiResponses.getAvailableScanRules.getAvailableScanRules_withNewUnknownFieldsJson.json"));

			List<ScanRule> result = metadefenderCoreClient.GetAvailableScanRules();

			Assert.AreEqual(2, result.Count);

			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("GET", "/file/rules");
				}
			);
		}

		[Test]
		public void ServerError()
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());

			CreateStub("/file/rules", "GET", 500, GetJsonFromFile("C_Sharp_REST_Sample.test.resources.apiResponses.errorJson.json"));

			bool isException = false;
			try
			{
				metadefenderCoreClient.GetAvailableScanRules();
			}
			catch (MetadefenderClientException)
			{
				isException = true;
			}
			Assert.True(isException);

			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("GET", "/file/rules");
				}
			);
		}
	}
}