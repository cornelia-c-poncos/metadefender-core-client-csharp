
using NUnit.Framework;
using Opswat.Metadefender.Core.Client;
using Opswat.Metadefender.Core.Client.Exceptions;
using Opswat.Metadefender.Core.Client.Responses;
using System.Collections.Generic;
using HttpMock;
using HttpMock.Verify.NUnit;

namespace MetscanClient
{
	[TestFixture]
	public class GetEngineVersionsTests : BaseHttpMockTest
	{
		[Test]
		public void Success()
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());

			CreateStub("/stat/engines", "GET", 200, GetJsonFromFile("MetadefenderCoreClient.test.resources.apiResponses.getEngineVersions.getEngineVersions_success.json"));

			List<EngineVersion> result = metadefenderCoreClient.GetEngineVersions();

			Assert.AreEqual(47, result.Count);

			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("GET", "/stat/engines");
				}
			);
		}

		[Test]
		public void ServerError()
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());

			CreateStub("/stat/engines", "GET", 200, GetJsonFromFile("MetadefenderCoreClient.test.resources.apiResponses.errorJson.json"));

			bool isException = false;
			try
			{
				metadefenderCoreClient.GetEngineVersions();
			}
			catch (MetadefenderClientException)
			{
				isException = true;
			}

			Assert.True(isException);

			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("GET", "/stat/engines");
				}
			);
		}
	}
}
