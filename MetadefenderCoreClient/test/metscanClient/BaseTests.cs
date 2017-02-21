
using HttpMock;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Opswat.Metadefender.Core.Client;
using Opswat.Metadefender.Core.Client.Exceptions;
using Opswat.Metadefender.Core.Client.Responses;

namespace MetscanClient
{
	[TestFixture]
	public class BaseTests : BaseHttpMockTest
	{
		[Test]
		public void LoginTest()
		{
			CreateStubForLogin();

			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl(), "admin", "admin");
			Assert.AreEqual(TEST_SESSION_ID, metadefenderCoreClient.GetSessionId());

			HttpServer.AssertWasCalled(x => {
				var ret = x.CustomVerb("POST", "/login");
				ret.WithBody("{\"user\":\"admin\",\"password\":\"admin\"}");
				return ret;
			});
		}

		[Test]
		public void ValidateCurrentSessionTest_loggedIn()
		{
			CreateStubForLogin();

			CreateStubForVersion();

			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());
			metadefenderCoreClient.Login("admin", "admin");

			bool isLoggedIn = metadefenderCoreClient.ValidateCurrentSession();

			Assert.True(isLoggedIn);

		}

		[Test]
		public void ValidateCurrentSessionTest_notLoggedIn_1()
		{
			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());

			bool isLoggedIn = metadefenderCoreClient.ValidateCurrentSession();

			Assert.False(isLoggedIn);
		}

		[Test]
		public void ValidateCurrentSessionTest_notLoggedIn_sessionExpired()
		{
			CreateStubForLogin();

			CreateStub("/version", "GET", 403,
				GetJsonFromFile("MetadefenderCoreClient.test.resources.apiResponses.version.getVersion_accessDenied.json"));

			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl());
			metadefenderCoreClient.Login("admin", "admin");

			bool isLoggedIn = metadefenderCoreClient.ValidateCurrentSession();

			Assert.False(isLoggedIn);
		}

		[Test]
		public void GetVersionTest()
		{
			CreateStubForLogin();
			CreateStubForVersion();

			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl(), "admin", "admin");

			ApiVersion apiVersion = metadefenderCoreClient.GetVersion();

			Assert.AreEqual("4.3.0.256", apiVersion.version);
			Assert.AreEqual("MSCL", apiVersion.product_id);

			HttpServer.AssertWasCalled(x =>
				{
					var ret = x.CustomVerb("GET", "/version");
					ret.WithHeader("apikey", new EqualConstraint(TEST_SESSION_ID));
					return ret;
				}
			);
		}

		[Test]
		public void LogoutTest()
		{
			CreateStubForLogin();

			CreateStub("/logout", "POST", 200, GetJsonFromFile("MetadefenderCoreClient.test.resources.apiResponses.logout.logout_success.json"));

			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl(), "admin", "admin");

			Assert.NotNull(metadefenderCoreClient.GetSessionId());

			metadefenderCoreClient.Logout();

			Assert.Null(metadefenderCoreClient.GetSessionId());

			HttpServer.AssertWasCalled(x =>
				{
					var ret = x.CustomVerb("POST", "/logout");
					ret.WithHeader("apikey", new EqualConstraint(TEST_SESSION_ID));
					return ret;
				}
			);
		}

		[Test]
		public void MalformedUrlTest()
		{
			bool isException = false;
			try
			{
				new MetadefenderCoreClient("htt://mal formed url:" + Port, "admin", "admin");
			}
			catch (MetadefenderClientException)
			{
				isException = true;
			}
			Assert.True(isException);
		}
	}
}